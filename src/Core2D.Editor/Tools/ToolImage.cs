// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Editor.Input;
using Core2D.Editor.Tools.Selection;
using Core2D.Editor.Tools.Settings;
using Core2D.Shape;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Image tool.
    /// </summary>
    public class ToolImage : ToolBase
    {
        public enum State { TopLeft, BottomRight }
        private readonly IServiceProvider _serviceProvider;
        private ToolSettingsImage _settings;
        private State _currentState = State.TopLeft;
        private XImage _image;
        private ToolImageSelection _selection;

        /// <inheritdoc/>
        public override string Title => "Image";

        /// <summary>
        /// Gets or sets the tool settings.
        /// </summary>
        public ToolSettingsImage Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        /// <summary>
        /// Initialize new instance of <see cref="ToolImage"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ToolImage(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            _settings = new ToolSettingsImage();
        }

        /// <inheritdoc/>
        public override async void LeftDown(InputArgs args)
        {
            base.LeftDown(args);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            (double sx, double sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.TopLeft:
                    {
                        if (editor.ImageImporter == null)
                            return;

                        var key = await editor.ImageImporter.GetImageKeyAsync();
                        if (key == null || string.IsNullOrEmpty(key))
                            return;

                        var style = editor.Project.CurrentStyleLibrary.Selected;
                        _image = XImage.Create(
                            sx, sy,
                            editor.Project.Options.CloneStyle ? style.Clone() : style,
                            editor.Project.Options.PointShape,
                            key);

                        var result = editor.TryToGetConnectionPoint(sx, sy);
                        if (result != null)
                        {
                            _image.TopLeft = result;
                        }

                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_image);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateBottomRight();
                        Move(_image);
                        _currentState = State.BottomRight;
                        editor.CancelAvailable = true;
                    }
                    break;
                case State.BottomRight:
                    {
                        if (_image != null)
                        {
                            _image.BottomRight.X = sx;
                            _image.BottomRight.Y = sy;

                            var result = editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _image.BottomRight = result;
                            }

                            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_image);
                            Remove();
                            base.Finalize(_image);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _image);
                            _currentState = State.TopLeft;
                            editor.CancelAvailable = false;
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void RightDown(InputArgs args)
        {
            base.RightDown(args);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            switch (_currentState)
            {
                case State.TopLeft:
                    break;
                case State.BottomRight:
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_image);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Remove();
                        _currentState = State.TopLeft;
                        editor.CancelAvailable = false;
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Move(InputArgs args)
        {
            base.Move(args);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            (double sx, double sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.TopLeft:
                    if (editor.Project.Options.TryToConnect)
                    {
                        editor.TryToHoverShape(sx, sy);
                    }
                    break;
                case State.BottomRight:
                    {
                        if (_image != null)
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                editor.TryToHoverShape(sx, sy);
                            }
                            _image.BottomRight.X = sx;
                            _image.BottomRight.Y = sy;
                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_image);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// Transfer tool state to <see cref="State.BottomRight"/>.
        /// </summary>
        public void ToStateBottomRight()
        {
            var editor = _serviceProvider.GetService<ProjectEditor>();
            _selection = new ToolImageSelection(
                editor.Project.CurrentContainer.HelperLayer,
                _image,
                editor.Project.Options.HelperStyle,
                editor.Project.Options.PointShape);

            _selection.ToStateBottomRight();
        }

        /// <inheritdoc/>
        public override void Move(BaseShape shape)
        {
            base.Move(shape);

            _selection.Move();
        }

        /// <inheritdoc/>
        public override void Remove()
        {
            base.Remove();

            _selection.Remove();
            _selection = null;
        }
    }
}
