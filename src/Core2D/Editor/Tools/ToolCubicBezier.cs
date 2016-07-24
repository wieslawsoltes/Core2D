// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Tools.Selection;
using Core2D.Shape;
using Core2D.Shapes;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Helper class for <see cref="Tool.CubicBezier"/> editor.
    /// </summary>
    public class ToolCubicBezier : ToolBase
    {
        private ProjectEditor _editor;
        private ToolState _currentState = ToolState.None;
        private XCubicBezier _shape;
        private CubicBezierSelection _selection;

        /// <summary>
        /// Initialize new instance of <see cref="ToolCubicBezier"/> class.
        /// </summary>
        /// <param name="editor">The current <see cref="ProjectEditor"/> object.</param>
        public ToolCubicBezier(ProjectEditor editor)
            : base()
        {
            _editor = editor;
        }

        /// <inheritdoc/>
        public override void LeftDown(double x, double y)
        {
            base.LeftDown(x, y);

            double sx = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    {
                        var style = _editor.Project.CurrentStyleLibrary.Selected;
                        _shape = XCubicBezier.Create(
                            sx, sy,
                            _editor.Project.Options.CloneStyle ? style.Clone() : style,
                            _editor.Project.Options.PointShape,
                            _editor.Project.Options.DefaultIsStroked,
                            _editor.Project.Options.DefaultIsFilled);

                        var result = _editor.TryToGetConnectionPoint(sx, sy);
                        if (result != null)
                        {
                            _shape.Point1 = result;
                        }

                        _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_shape);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateOne();
                        Move(_shape as XCubicBezier);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = ToolState.One;
                        _editor.CancelAvailable = true;
                    }
                    break;
                case ToolState.One:
                    {
                        var cubicBezier = _shape as XCubicBezier;
                        if (cubicBezier != null)
                        {
                            cubicBezier.Point3.X = sx;
                            cubicBezier.Point3.Y = sy;
                            cubicBezier.Point4.X = sx;
                            cubicBezier.Point4.Y = sy;

                            var result = _editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _shape.Point4 = result;
                            }

                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            ToStateTwo();
                            Move(_shape as XCubicBezier);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                            _currentState = ToolState.Two;
                        }
                    }
                    break;
                case ToolState.Two:
                    {
                        var cubicBezier = _shape as XCubicBezier;
                        if (cubicBezier != null)
                        {
                            cubicBezier.Point2.X = sx;
                            cubicBezier.Point2.Y = sy;

                            var result = _editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _shape.Point2 = result;
                            }

                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            ToStateThree();
                            Move(_shape as XCubicBezier);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                            _currentState = ToolState.Three;
                        }
                    }
                    break;
                case ToolState.Three:
                    {
                        var cubicBezier = _shape as XCubicBezier;
                        if (cubicBezier != null)
                        {
                            cubicBezier.Point3.X = sx;
                            cubicBezier.Point3.Y = sy;

                            var result = _editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _shape.Point3 = result;
                            }

                            _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                            Remove();
                            Finalize(_shape as XCubicBezier);
                            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, _shape);
                            _currentState = ToolState.None;
                            _editor.CancelAvailable = false;
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void RightDown(double x, double y)
        {
            base.RightDown(x, y);

            switch (_currentState)
            {
                case ToolState.None:
                    break;
                case ToolState.One:
                case ToolState.Two:
                case ToolState.Three:
                    {
                        _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Remove();
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = ToolState.None;
                        _editor.CancelAvailable = false;
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Move(double x, double y)
        {
            base.Move(x, y);

            double sx = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    {
                        if (_editor.Project.Options.TryToConnect)
                        {
                            _editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
                case ToolState.One:
                    {
                        var cubicBezier = _shape as XCubicBezier;
                        if (cubicBezier != null)
                        {
                            if (_editor.Project.Options.TryToConnect)
                            {
                                _editor.TryToHoverShape(sx, sy);
                            }
                            cubicBezier.Point2.X = sx;
                            cubicBezier.Point2.Y = sy;
                            cubicBezier.Point3.X = sx;
                            cubicBezier.Point3.Y = sy;
                            cubicBezier.Point4.X = sx;
                            cubicBezier.Point4.Y = sy;
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_shape as XCubicBezier);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        }
                    }
                    break;
                case ToolState.Two:
                    {
                        var cubicBezier = _shape as XCubicBezier;
                        if (cubicBezier != null)
                        {
                            if (_editor.Project.Options.TryToConnect)
                            {
                                _editor.TryToHoverShape(sx, sy);
                            }
                            cubicBezier.Point2.X = sx;
                            cubicBezier.Point2.Y = sy;
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_shape as XCubicBezier);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        }
                    }
                    break;
                case ToolState.Three:
                    {
                        var cubicBezier = _shape as XCubicBezier;
                        if (cubicBezier != null)
                        {
                            if (_editor.Project.Options.TryToConnect)
                            {
                                _editor.TryToHoverShape(sx, sy);
                            }
                            cubicBezier.Point3.X = sx;
                            cubicBezier.Point3.Y = sy;
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_shape as XCubicBezier);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        }
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void ToStateOne()
        {
            base.ToStateOne();

            _selection = new CubicBezierSelection(
                _editor.Project.CurrentContainer.HelperLayer,
                _shape,
                _editor.Project.Options.HelperStyle,
                _editor.Project.Options.PointShape);

            _selection.ToStateOne();
        }

        /// <inheritdoc/>
        public override void ToStateTwo()
        {
            base.ToStateTwo();

            _selection.ToStateTwo();
        }

        /// <inheritdoc/>
        public override void ToStateThree()
        {
            base.ToStateThree();

            _selection.ToStateThree();
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
