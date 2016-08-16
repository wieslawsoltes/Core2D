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
        private ToolState _currentState = ToolState.None;
        private XCubicBezier _cubicBezier;
        private CubicBezierSelection _selection;

        /// <inheritdoc/>
        public override void LeftDown(double x, double y)
        {
            base.LeftDown(x, y);

            double sx = Editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, Editor.Project.Options.SnapX) : x;
            double sy = Editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, Editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    {
                        var style = Editor.Project.CurrentStyleLibrary.Selected;
                        _cubicBezier = XCubicBezier.Create(
                            sx, sy,
                            Editor.Project.Options.CloneStyle ? style.Clone() : style,
                            Editor.Project.Options.PointShape,
                            Editor.Project.Options.DefaultIsStroked,
                            Editor.Project.Options.DefaultIsFilled);

                        var result = Editor.TryToGetConnectionPoint(sx, sy);
                        if (result != null)
                        {
                            _cubicBezier.Point1 = result;
                        }

                        Editor.Project.CurrentContainer.WorkingLayer.Shapes = Editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_cubicBezier);
                        Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateOne();
                        Move(_cubicBezier);
                        _currentState = ToolState.One;
                        Editor.CancelAvailable = true;
                    }
                    break;
                case ToolState.One:
                    {
                        if (_cubicBezier != null)
                        {
                            _cubicBezier.Point3.X = sx;
                            _cubicBezier.Point3.Y = sy;
                            _cubicBezier.Point4.X = sx;
                            _cubicBezier.Point4.Y = sy;

                            var result = Editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _cubicBezier.Point4 = result;
                            }

                            Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            ToStateTwo();
                            Move(_cubicBezier);
                            _currentState = ToolState.Two;
                        }
                    }
                    break;
                case ToolState.Two:
                    {
                        if (_cubicBezier != null)
                        {
                            _cubicBezier.Point2.X = sx;
                            _cubicBezier.Point2.Y = sy;

                            var result = Editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _cubicBezier.Point2 = result;
                            }

                            Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            ToStateThree();
                            Move(_cubicBezier);
                            _currentState = ToolState.Three;
                        }
                    }
                    break;
                case ToolState.Three:
                    {
                        if (_cubicBezier != null)
                        {
                            _cubicBezier.Point3.X = sx;
                            _cubicBezier.Point3.Y = sy;

                            var result = Editor.TryToGetConnectionPoint(sx, sy);
                            if (result != null)
                            {
                                _cubicBezier.Point3 = result;
                            }

                            Editor.Project.CurrentContainer.WorkingLayer.Shapes = Editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_cubicBezier);
                            Remove();
                            base.Finalize(_cubicBezier);
                            Editor.Project.AddShape(Editor.Project.CurrentContainer.CurrentLayer, _cubicBezier);
                            _currentState = ToolState.None;
                            Editor.CancelAvailable = false;
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
                        Editor.Project.CurrentContainer.WorkingLayer.Shapes = Editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_cubicBezier);
                        Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Remove();
                        _currentState = ToolState.None;
                        Editor.CancelAvailable = false;
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Move(double x, double y)
        {
            base.Move(x, y);

            double sx = Editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, Editor.Project.Options.SnapX) : x;
            double sy = Editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, Editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    {
                        if (Editor.Project.Options.TryToConnect)
                        {
                            Editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
                case ToolState.One:
                    {
                        if (_cubicBezier != null)
                        {
                            if (Editor.Project.Options.TryToConnect)
                            {
                                Editor.TryToHoverShape(sx, sy);
                            }
                            _cubicBezier.Point2.X = sx;
                            _cubicBezier.Point2.Y = sy;
                            _cubicBezier.Point3.X = sx;
                            _cubicBezier.Point3.Y = sy;
                            _cubicBezier.Point4.X = sx;
                            _cubicBezier.Point4.Y = sy;
                            Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_cubicBezier);
                        }
                    }
                    break;
                case ToolState.Two:
                    {
                        if (_cubicBezier != null)
                        {
                            if (Editor.Project.Options.TryToConnect)
                            {
                                Editor.TryToHoverShape(sx, sy);
                            }
                            _cubicBezier.Point2.X = sx;
                            _cubicBezier.Point2.Y = sy;
                            Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_cubicBezier);
                        }
                    }
                    break;
                case ToolState.Three:
                    {
                        if (_cubicBezier != null)
                        {
                            if (Editor.Project.Options.TryToConnect)
                            {
                                Editor.TryToHoverShape(sx, sy);
                            }
                            _cubicBezier.Point3.X = sx;
                            _cubicBezier.Point3.Y = sy;
                            Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_cubicBezier);
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
                Editor.Project.CurrentContainer.HelperLayer,
                _cubicBezier,
                Editor.Project.Options.HelperStyle,
                Editor.Project.Options.PointShape);

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
