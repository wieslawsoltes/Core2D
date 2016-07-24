// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using Core2D.Path.Segments;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools.Path
{
    /// <summary>
    /// Helper class for <see cref="PathTool.CubicBezier"/> editor.
    /// </summary>
    internal class ToolPathCubicBezier : ToolBase
    {
        private ProjectEditor _editor;
        private ToolState _currentState = ToolState.None;
        private ToolPath _toolPath;
        private XPoint _cubicBezierPoint1;
        private XPoint _cubicBezierPoint2;
        private XPoint _cubicBezierPoint3;
        private XPoint _cubicBezierPoint4;
        private ShapeStyle _style;
        private XLine _cubicBezierLine12;
        private XLine _cubicBezierLine43;
        private XLine _cubicBezierLine23;
        private XPoint _cubicBezierHelperPoint1;
        private XPoint _cubicBezierHelperPoint2;
        private XPoint _cubicBezierHelperPoint3;
        private XPoint _cubicBezierHelperPoint4;

        /// <summary>
        /// Initialize new instance of <see cref="ToolPathCubicBezier"/> class.
        /// </summary>
        /// <param name="editor">The current <see cref="ProjectEditor"/> object.</param>
        /// <param name="toolPath">The current <see cref="ToolPath"/> object.</param>
        public ToolPathCubicBezier(ProjectEditor editor, ToolPath toolPath)
            : base()
        {
            _editor = editor;
            _toolPath = toolPath;
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
                        _cubicBezierPoint1 = _editor.TryToGetConnectionPoint(sx, sy) ?? XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        if (!_toolPath._isInitialized)
                        {
                            _toolPath.InitializeWorkingPath(_cubicBezierPoint1);
                        }
                        else
                        {
                            _cubicBezierPoint1 = _toolPath.GetLastPathPoint();
                        }

                        _cubicBezierPoint2 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _cubicBezierPoint3 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _cubicBezierPoint4 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _toolPath._context.CubicBezierTo(
                            _cubicBezierPoint2,
                            _cubicBezierPoint3,
                            _cubicBezierPoint4,
                            _editor.Project.Options.DefaultIsStroked,
                            _editor.Project.Options.DefaultIsSmoothJoin);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateOne();
                        Move(null);
                        _currentState = ToolState.One;
                        _editor.CancelAvailable = true;
                    }
                    break;
                case ToolState.One:
                    {
                        _cubicBezierPoint4.X = sx;
                        _cubicBezierPoint4.Y = sy;
                        if (_editor.Project.Options.TryToConnect)
                        {
                            var point3 = _editor.TryToGetConnectionPoint(sx, sy);
                            if (point3 != null)
                            {
                                var figure = _toolPath._geometry.Figures.LastOrDefault();
                                var cubicBezier = figure.Segments.LastOrDefault() as XCubicBezierSegment;
                                cubicBezier.Point3 = point3;
                                _cubicBezierPoint4 = point3;
                            }
                        }
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateTwo();
                        Move(null);
                        _currentState = ToolState.Two;
                    }
                    break;
                case ToolState.Two:
                    {
                        _cubicBezierPoint2.X = sx;
                        _cubicBezierPoint2.Y = sy;
                        if (_editor.Project.Options.TryToConnect)
                        {
                            var point1 = _editor.TryToGetConnectionPoint(sx, sy);
                            if (point1 != null)
                            {
                                var figure = _toolPath._geometry.Figures.LastOrDefault();
                                var cubicBezier = figure.Segments.LastOrDefault() as XCubicBezierSegment;
                                cubicBezier.Point1 = point1;
                                _cubicBezierPoint2 = point1;
                            }
                        }
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateThree();
                        Move(null);
                        _currentState = ToolState.Three;
                    }
                    break;
                case ToolState.Three:
                    {
                        _cubicBezierPoint3.X = sx;
                        _cubicBezierPoint3.Y = sy;
                        if (_editor.Project.Options.TryToConnect)
                        {
                            var point2 = _editor.TryToGetConnectionPoint(sx, sy);
                            if (point2 != null)
                            {
                                var figure = _toolPath._geometry.Figures.LastOrDefault();
                                var cubicBezier = figure.Segments.LastOrDefault() as XCubicBezierSegment;
                                cubicBezier.Point2 = point2;
                                _cubicBezierPoint3 = point2;
                            }
                        }

                        _cubicBezierPoint1 = _cubicBezierPoint4;
                        _cubicBezierPoint2 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _cubicBezierPoint3 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _cubicBezierPoint4 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _toolPath._context.CubicBezierTo(
                            _cubicBezierPoint2,
                            _cubicBezierPoint3,
                            _cubicBezierPoint4,
                            _editor.Project.Options.DefaultIsStroked,
                            _editor.Project.Options.DefaultIsSmoothJoin);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Remove();
                        ToStateOne();
                        Move(null);
                        _currentState = ToolState.One;
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
                        _toolPath.RemoveLastSegment<XCubicBezierSegment>();

                        _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_toolPath._path);
                        Remove();
                        if (_toolPath._path.Geometry.Figures.LastOrDefault().Segments.Length > 0)
                        {
                            Finalize(null);
                            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, _toolPath._path);
                        }
                        else
                        {
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        }
                        _toolPath.DeInitializeWorkingPath();
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
                        if (_editor.Project.Options.TryToConnect)
                        {
                            _editor.TryToHoverShape(sx, sy);
                        }
                        _cubicBezierPoint2.X = sx;
                        _cubicBezierPoint2.Y = sy;
                        _cubicBezierPoint3.X = sx;
                        _cubicBezierPoint3.Y = sy;
                        _cubicBezierPoint4.X = sx;
                        _cubicBezierPoint4.Y = sy;
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                    }
                    break;
                case ToolState.Two:
                    {
                        if (_editor.Project.Options.TryToConnect)
                        {
                            _editor.TryToHoverShape(sx, sy);
                        }
                        _cubicBezierPoint2.X = sx;
                        _cubicBezierPoint2.Y = sy;
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                    }
                    break;
                case ToolState.Three:
                    {
                        if (_editor.Project.Options.TryToConnect)
                        {
                            _editor.TryToHoverShape(sx, sy);
                        }
                        _cubicBezierPoint3.X = sx;
                        _cubicBezierPoint3.Y = sy;
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void ToStateOne()
        {
            base.ToStateOne();

            _style = _editor.Project.Options.HelperStyle;
            _cubicBezierHelperPoint1 = XPoint.Create(0, 0, _editor.Project.Options.PointShape);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_cubicBezierHelperPoint1);
            _cubicBezierHelperPoint4 = XPoint.Create(0, 0, _editor.Project.Options.PointShape);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_cubicBezierHelperPoint4);
        }

        /// <inheritdoc/>
        public override void ToStateTwo()
        {
            base.ToStateTwo();

            _style = _editor.Project.Options.HelperStyle;
            _cubicBezierLine12 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_cubicBezierLine12);
            _cubicBezierHelperPoint2 = XPoint.Create(0, 0, _editor.Project.Options.PointShape);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_cubicBezierHelperPoint2);
        }

        /// <inheritdoc/>
        public override void ToStateThree()
        {
            base.ToStateThree();

            _cubicBezierLine43 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_cubicBezierLine43);
            _cubicBezierLine23 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_cubicBezierLine23);
            _cubicBezierHelperPoint3 = XPoint.Create(0, 0, _editor.Project.Options.PointShape);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_cubicBezierHelperPoint3);
        }

        /// <inheritdoc/>
        public override void Move(BaseShape shape)
        {
            base.Move(shape);

            if (_cubicBezierLine12 != null)
            {
                _cubicBezierLine12.Start.X = _cubicBezierPoint1.X;
                _cubicBezierLine12.Start.Y = _cubicBezierPoint1.Y;
                _cubicBezierLine12.End.X = _cubicBezierPoint2.X;
                _cubicBezierLine12.End.Y = _cubicBezierPoint2.Y;
            }

            if (_cubicBezierLine43 != null)
            {
                _cubicBezierLine43.Start.X = _cubicBezierPoint4.X;
                _cubicBezierLine43.Start.Y = _cubicBezierPoint4.Y;
                _cubicBezierLine43.End.X = _cubicBezierPoint3.X;
                _cubicBezierLine43.End.Y = _cubicBezierPoint3.Y;
            }

            if (_cubicBezierLine23 != null)
            {
                _cubicBezierLine23.Start.X = _cubicBezierPoint2.X;
                _cubicBezierLine23.Start.Y = _cubicBezierPoint2.Y;
                _cubicBezierLine23.End.X = _cubicBezierPoint3.X;
                _cubicBezierLine23.End.Y = _cubicBezierPoint3.Y;
            }

            if (_cubicBezierHelperPoint1 != null)
            {
                _cubicBezierHelperPoint1.X = _cubicBezierPoint1.X;
                _cubicBezierHelperPoint1.Y = _cubicBezierPoint1.Y;
            }

            if (_cubicBezierHelperPoint2 != null)
            {
                _cubicBezierHelperPoint2.X = _cubicBezierPoint2.X;
                _cubicBezierHelperPoint2.Y = _cubicBezierPoint2.Y;
            }

            if (_cubicBezierHelperPoint3 != null)
            {
                _cubicBezierHelperPoint3.X = _cubicBezierPoint3.X;
                _cubicBezierHelperPoint3.Y = _cubicBezierPoint3.Y;
            }

            if (_cubicBezierHelperPoint4 != null)
            {
                _cubicBezierHelperPoint4.X = _cubicBezierPoint4.X;
                _cubicBezierHelperPoint4.Y = _cubicBezierPoint4.Y;
            }
        }

        /// <inheritdoc/>
        public override void Remove()
        {
            base.Remove();

            _currentState = ToolState.None;

            if (_cubicBezierLine12 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_cubicBezierLine12);
                _cubicBezierLine12 = null;
            }

            if (_cubicBezierLine43 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_cubicBezierLine43);
                _cubicBezierLine43 = null;
            }

            if (_cubicBezierLine23 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_cubicBezierLine23);
                _cubicBezierLine23 = null;
            }

            if (_cubicBezierHelperPoint1 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_cubicBezierHelperPoint1);
                _cubicBezierHelperPoint1 = null;
            }

            if (_cubicBezierHelperPoint2 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_cubicBezierHelperPoint2);
                _cubicBezierHelperPoint2 = null;
            }

            if (_cubicBezierHelperPoint3 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_cubicBezierHelperPoint3);
                _cubicBezierHelperPoint3 = null;
            }

            if (_cubicBezierHelperPoint4 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_cubicBezierHelperPoint4);
                _cubicBezierHelperPoint4 = null;
            }

            _style = null;
        }
    }
}
