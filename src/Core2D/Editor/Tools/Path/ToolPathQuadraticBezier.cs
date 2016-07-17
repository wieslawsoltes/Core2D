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
    /// Helper class for <see cref="PathTool.QuadraticBezier"/> editor.
    /// </summary>
    internal class ToolPathQuadraticBezier : ToolBase
    {
        private ProjectEditor _editor;
        private ToolState _currentState = ToolState.None;
        private ToolPath _toolPath;
        // QuadraticBezier Tool
        private XPoint _quadraticBezierPoint1;
        private XPoint _quadraticBezierPoint2;
        private XPoint _quadraticBezierPoint3;
        // Helpers Style
        private ShapeStyle _style;
        // QuadraticBezier Helper
        private XLine _quadraticBezierLine12;
        private XLine _quadraticBezierLine32;
        private XPoint _quadraticBezierHelperPoint1;
        private XPoint _quadraticBezierHelperPoint2;
        private XPoint _quadraticBezierHelperPoint3;

        /// <summary>
        /// Initialize new instance of <see cref="ToolPathQuadraticBezier"/> class.
        /// </summary>
        /// <param name="editor">The current <see cref="ProjectEditor"/> object.</param>
        /// <param name="toolPath">The current <see cref="ToolPath"/> object.</param>
        public ToolPathQuadraticBezier(ProjectEditor editor, ToolPath toolPath)
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
                        _quadraticBezierPoint1 = _toolPath.TryToGetConnectionPoint(sx, sy) ?? XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        if (!_toolPath._isInitialized)
                        {
                            _toolPath.InitializeWorkingPath(_quadraticBezierPoint1);
                        }
                        else
                        {
                            _quadraticBezierPoint1 = _toolPath.GetLastPathPoint();
                        }

                        _quadraticBezierPoint2 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _quadraticBezierPoint3 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _toolPath._context.QuadraticBezierTo(
                            _quadraticBezierPoint2,
                            _quadraticBezierPoint3,
                            _editor.Project.Options.DefaultIsStroked,
                            _editor.Project.Options.DefaultIsSmoothJoin);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateOne();
                        Move(null);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = ToolState.One;
                        _editor.CancelAvailable = true;
                    }
                    break;
                case ToolState.One:
                    {
                        _quadraticBezierPoint3.X = sx;
                        _quadraticBezierPoint3.Y = sy;
                        if (_editor.Project.Options.TryToConnect)
                        {
                            var point2 = _toolPath.TryToGetConnectionPoint(sx, sy);
                            if (point2 != null)
                            {
                                var figure = _toolPath._geometry.Figures.LastOrDefault();
                                var quadraticBezier = figure.Segments.LastOrDefault() as XQuadraticBezierSegment;
                                quadraticBezier.Point2 = point2;
                                _quadraticBezierPoint3 = point2;
                            }
                        }
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateTwo();
                        Move(null);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = ToolState.Two;
                    }
                    break;
                case ToolState.Two:
                    {
                        _quadraticBezierPoint2.X = sx;
                        _quadraticBezierPoint2.Y = sy;
                        if (_editor.Project.Options.TryToConnect)
                        {
                            var point1 = _toolPath.TryToGetConnectionPoint(sx, sy);
                            if (point1 != null)
                            {
                                var figure = _toolPath._geometry.Figures.LastOrDefault();
                                var quadraticBezier = figure.Segments.LastOrDefault() as XQuadraticBezierSegment;
                                quadraticBezier.Point1 = point1;
                                _quadraticBezierPoint2 = point1;
                            }
                        }

                        _quadraticBezierPoint1 = _quadraticBezierPoint3;
                        _quadraticBezierPoint2 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _quadraticBezierPoint3 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _toolPath._context.QuadraticBezierTo(
                            _quadraticBezierPoint2,
                            _quadraticBezierPoint3,
                            _editor.Project.Options.DefaultIsStroked,
                            _editor.Project.Options.DefaultIsSmoothJoin);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Remove();
                        ToStateOne();
                        Move(null);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
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
                    {
                        _toolPath.RemoveLastSegment<XQuadraticBezierSegment>();

                        _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_toolPath._path);
                        Remove();
                        if (_toolPath._path.Geometry.Figures.LastOrDefault().Segments.Count > 0)
                        {
                            Finalize(null);
                            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, _toolPath._path);
                        }
                        else
                        {
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
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
                        _quadraticBezierPoint2.X = sx;
                        _quadraticBezierPoint2.Y = sy;
                        _quadraticBezierPoint3.X = sx;
                        _quadraticBezierPoint3.Y = sy;
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                    }
                    break;
                case ToolState.Two:
                    {
                        if (_editor.Project.Options.TryToConnect)
                        {
                            _editor.TryToHoverShape(sx, sy);
                        }
                        _quadraticBezierPoint2.X = sx;
                        _quadraticBezierPoint2.Y = sy;
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void ToStateOne()
        {
            base.ToStateOne();

            _style = _editor.Project.Options.HelperStyle;
            _quadraticBezierHelperPoint1 = XPoint.Create(0, 0, _editor.Project.Options.PointShape);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_quadraticBezierHelperPoint1);
            _quadraticBezierHelperPoint3 = XPoint.Create(0, 0, _editor.Project.Options.PointShape);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_quadraticBezierHelperPoint3);
        }

        /// <inheritdoc/>
        public override void ToStateTwo()
        {
            base.ToStateTwo();

            _style = _editor.Project.Options.HelperStyle;
            _quadraticBezierLine12 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_quadraticBezierLine12);
            _quadraticBezierLine32 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_quadraticBezierLine32);
            _quadraticBezierHelperPoint2 = XPoint.Create(0, 0, _editor.Project.Options.PointShape);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_quadraticBezierHelperPoint2);
        }

        /// <inheritdoc/>
        public override void Move(BaseShape shape)
        {
            base.Move(shape);

            if (_quadraticBezierLine12 != null)
            {
                _quadraticBezierLine12.Start.X = _quadraticBezierPoint1.X;
                _quadraticBezierLine12.Start.Y = _quadraticBezierPoint1.Y;
                _quadraticBezierLine12.End.X = _quadraticBezierPoint2.X;
                _quadraticBezierLine12.End.Y = _quadraticBezierPoint2.Y;
            }

            if (_quadraticBezierLine32 != null)
            {
                _quadraticBezierLine32.Start.X = _quadraticBezierPoint3.X;
                _quadraticBezierLine32.Start.Y = _quadraticBezierPoint3.Y;
                _quadraticBezierLine32.End.X = _quadraticBezierPoint2.X;
                _quadraticBezierLine32.End.Y = _quadraticBezierPoint2.Y;
            }

            if (_quadraticBezierHelperPoint1 != null)
            {
                _quadraticBezierHelperPoint1.X = _quadraticBezierPoint1.X;
                _quadraticBezierHelperPoint1.Y = _quadraticBezierPoint1.Y;
            }

            if (_quadraticBezierHelperPoint2 != null)
            {
                _quadraticBezierHelperPoint2.X = _quadraticBezierPoint2.X;
                _quadraticBezierHelperPoint2.Y = _quadraticBezierPoint2.Y;
            }

            if (_quadraticBezierHelperPoint3 != null)
            {
                _quadraticBezierHelperPoint3.X = _quadraticBezierPoint3.X;
                _quadraticBezierHelperPoint3.Y = _quadraticBezierPoint3.Y;
            }
        }

        /// <inheritdoc/>
        public override void Remove()
        {
            base.Remove();

            _currentState = ToolState.None;

            if (_quadraticBezierLine12 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_quadraticBezierLine12);
                _quadraticBezierLine12 = null;
            }

            if (_quadraticBezierLine32 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_quadraticBezierLine32);
                _quadraticBezierLine32 = null;
            }

            if (_quadraticBezierHelperPoint1 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_quadraticBezierHelperPoint1);
                _quadraticBezierHelperPoint1 = null;
            }

            if (_quadraticBezierHelperPoint2 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_quadraticBezierHelperPoint2);
                _quadraticBezierHelperPoint2 = null;
            }

            if (_quadraticBezierHelperPoint3 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_quadraticBezierHelperPoint3);
                _quadraticBezierHelperPoint3 = null;
            }

            _style = null;
        }
    }
}
