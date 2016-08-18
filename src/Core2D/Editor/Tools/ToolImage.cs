// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Editor.Tools.Selection;
using Core2D.Shape;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Helper class for <see cref="Tool.Image"/> editor.
    /// </summary>
    public class ToolImage : ToolBase
    {
        private readonly IServiceProvider _serviceProvider;
        private ToolState _currentState = ToolState.None;
        private XImage _image;
        private ImageSelection _selection;

        /// <inheritdoc/>
        public override string Name => "Image";

        /// <summary>
        /// Initialize new instance of <see cref="ToolImage"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ToolImage(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public override async void LeftDown(double x, double y)
        {
            base.LeftDown(x, y);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            double sx = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, editor.Project.Options.SnapX) : x;
            double sy = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    {
                        if (editor.GetImageKey == null)
                            return;

                        var key = await editor.GetImageKey();
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
                        ToStateOne();
                        Move(_image);
                        _currentState = ToolState.One;
                        editor.CancelAvailable = true;
                    }
                    break;
                case ToolState.One:
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
                            _currentState = ToolState.None;
                            editor.CancelAvailable = false;
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void RightDown(double x, double y)
        {
            base.RightDown(x, y);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            switch (_currentState)
            {
                case ToolState.None:
                    break;
                case ToolState.One:
                    {
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_image);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Remove();
                        _currentState = ToolState.None;
                        editor.CancelAvailable = false;
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Move(double x, double y)
        {
            base.Move(x, y);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            double sx = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, editor.Project.Options.SnapX) : x;
            double sy = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    if (editor.Project.Options.TryToConnect)
                    {
                        editor.TryToHoverShape(sx, sy);
                    }
                    break;
                case ToolState.One:
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

        /// <inheritdoc/>
        public override void ToStateOne()
        {
            base.ToStateOne();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            _selection = new ImageSelection(
                editor.Project.CurrentContainer.HelperLayer,
                _image,
                editor.Project.Options.HelperStyle,
                editor.Project.Options.PointShape);

            _selection.ToStateOne();
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
