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
    /// Rectangle tool.
    /// </summary>
    public class ToolRectangle : ToolBase
    {
        public enum State { TopLeft, BottomRight }
        private readonly IServiceProvider _serviceProvider;
        private ToolSettingsRectangle _settings;
        private State _currentState = State.TopLeft;
        private RectangleShape _rectangle;
        private ToolRectangleSelection _selection;

        /// <inheritdoc/>
        public override string Title => "Rectangle";

        /// <summary>
        /// Gets or sets the tool settings.
        /// </summary>
        public ToolSettingsRectangle Settings
        {
            get => _settings;
            set => Update(ref _settings, value);
        }

        /// <summary>
        /// Initialize new instance of <see cref="ToolRectangle"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ToolRectangle(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
            _settings = new ToolSettingsRectangle();
        }

        /// <inheritdoc/>
        public override void LeftDown(InputArgs args)
        {
            base.LeftDown(args);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            (double sx, double sy) = editor.TryToSnap(args);
            switch (_currentState)
            {
                case State.TopLeft:
                    {
                        var style = editor.Project.CurrentStyleLibrary.Selected;
                        _rectangle = RectangleShape.Create(
                            sx, sy,
                            editor.Project.Options.CloneStyle ? style.Clone() : style,
                            editor.Project.Options.PointShape,
                            editor.Project.Options.DefaultIsStroked,
                            editor.Project.Options.DefaultIsFilled);

                        var result = editor.TryToGetConnectionPoint(sx, sy);
                        if (result != null)
                        {
                            _rectangle.TopLeft = result;
                        }

                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_rectangle);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateBottomRight();
                        Move(_rectangle);
                        _currentState = State.BottomRight;
                        editor.CancelAvailable = true;
                    }
                    break;
                case State.BottomRight:
                    {
                        if (_rectangle != null)
                        {
                            _rectangle.BottomRight.X = sx;
                            _rectangle.BottomRight.Y = sy;

                            var result = editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _rectangle.BottomRight = result;
                            }

                            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_rectangle);
                            Remove();
                            Finalize(_rectangle);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _rectangle);
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
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_rectangle);
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
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
                case State.BottomRight:
                    {
                        if (_rectangle != null)
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                editor.TryToHoverShape(sx, sy);
                            }
                            _rectangle.BottomRight.X = sx;
                            _rectangle.BottomRight.Y = sy;
                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_rectangle);
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
            _selection = new ToolRectangleSelection(
                editor.Project.CurrentContainer.HelperLayer,
                _rectangle,
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
