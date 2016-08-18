// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Editor.Tools.Selection;
using Core2D.Shape;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Helper class for <see cref="Tool.Ellipse"/> editor.
    /// </summary>
    public class ToolEllipse : ToolBase
    {
        private readonly IServiceProvider _serviceProvider;
        private ToolState _currentState = ToolState.None;
        private XEllipse _ellipse;
        private EllipseSelection _selection;

        /// <inheritdoc/>
        public override string Name => "Ellipse";

        /// <summary>
        /// Initialize new instance of <see cref="ToolEllipse"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public ToolEllipse(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public override void LeftDown(double x, double y)
        {
            base.LeftDown(x, y);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            double sx = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, editor.Project.Options.SnapX) : x;
            double sy = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    {
                        var style = editor.Project.CurrentStyleLibrary.Selected;
                        _ellipse = XEllipse.Create(
                            sx, sy,
                            editor.Project.Options.CloneStyle ? style.Clone() : style,
                            editor.Project.Options.PointShape,
                            editor.Project.Options.DefaultIsStroked,
                            editor.Project.Options.DefaultIsFilled);

                        var result = editor.TryToGetConnectionPoint(sx, sy);
                        if (result != null)
                        {
                            _ellipse.TopLeft = result;
                        }

                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_ellipse);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateOne();
                        Move(_ellipse);
                        _currentState = ToolState.One;
                        editor.CancelAvailable = true;
                    }
                    break;
                case ToolState.One:
                    {
                        if (_ellipse != null)
                        {
                            _ellipse.BottomRight.X = sx;
                            _ellipse.BottomRight.Y = sy;

                            var result = editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _ellipse.BottomRight = result;
                            }

                            editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_ellipse);
                            Remove();
                            base.Finalize(_ellipse);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _ellipse);
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
                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_ellipse);
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
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
                case ToolState.One:
                    {
                        if (_ellipse != null)
                        {
                            if (editor.Project.Options.TryToConnect)
                            {
                                editor.TryToHoverShape(sx, sy);
                            }
                            _ellipse.BottomRight.X = sx;
                            _ellipse.BottomRight.Y = sy;
                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_ellipse);
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
            _selection = new EllipseSelection(
                editor.Project.CurrentContainer.HelperLayer,
                _ellipse,
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
