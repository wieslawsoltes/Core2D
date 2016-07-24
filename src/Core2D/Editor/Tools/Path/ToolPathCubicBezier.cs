// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Linq;
using Core2D.Editor.Tools.Path.Shapes;
using Core2D.Editor.Tools.Selection;
using Core2D.Path.Segments;
using Core2D.Shape;
using Core2D.Shapes;

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
        private XPathCubicBezier _cubicBezier = new XPathCubicBezier();
        private CubicBezierSelection _selection;

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
                        _cubicBezier.Point1 = _editor.TryToGetConnectionPoint(sx, sy) ?? XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        if (!_toolPath._isInitialized)
                        {
                            _toolPath.InitializeWorkingPath(_cubicBezier.Point1);
                        }
                        else
                        {
                            _cubicBezier.Point1 = _toolPath.GetLastPathPoint();
                        }

                        _cubicBezier.Point2 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _cubicBezier.Point3 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _cubicBezier.Point4 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _toolPath._context.CubicBezierTo(
                            _cubicBezier.Point2,
                            _cubicBezier.Point3,
                            _cubicBezier.Point4,
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
                        _cubicBezier.Point4.X = sx;
                        _cubicBezier.Point4.Y = sy;
                        if (_editor.Project.Options.TryToConnect)
                        {
                            var point3 = _editor.TryToGetConnectionPoint(sx, sy);
                            if (point3 != null)
                            {
                                var figure = _toolPath._geometry.Figures.LastOrDefault();
                                var cubicBezier = figure.Segments.LastOrDefault() as XCubicBezierSegment;
                                cubicBezier.Point3 = point3;
                                _cubicBezier.Point4 = point3;
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
                        _cubicBezier.Point2.X = sx;
                        _cubicBezier.Point2.Y = sy;
                        if (_editor.Project.Options.TryToConnect)
                        {
                            var point1 = _editor.TryToGetConnectionPoint(sx, sy);
                            if (point1 != null)
                            {
                                var figure = _toolPath._geometry.Figures.LastOrDefault();
                                var cubicBezier = figure.Segments.LastOrDefault() as XCubicBezierSegment;
                                cubicBezier.Point1 = point1;
                                _cubicBezier.Point2 = point1;
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
                        _cubicBezier.Point3.X = sx;
                        _cubicBezier.Point3.Y = sy;
                        if (_editor.Project.Options.TryToConnect)
                        {
                            var point2 = _editor.TryToGetConnectionPoint(sx, sy);
                            if (point2 != null)
                            {
                                var figure = _toolPath._geometry.Figures.LastOrDefault();
                                var cubicBezier = figure.Segments.LastOrDefault() as XCubicBezierSegment;
                                cubicBezier.Point2 = point2;
                                _cubicBezier.Point3 = point2;
                            }
                        }

                        _cubicBezier.Point1 = _cubicBezier.Point4;
                        _cubicBezier.Point2 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _cubicBezier.Point3 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _cubicBezier.Point4 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _toolPath._context.CubicBezierTo(
                            _cubicBezier.Point2,
                            _cubicBezier.Point3,
                            _cubicBezier.Point4,
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
                        _cubicBezier.Point2.X = sx;
                        _cubicBezier.Point2.Y = sy;
                        _cubicBezier.Point3.X = sx;
                        _cubicBezier.Point3.Y = sy;
                        _cubicBezier.Point4.X = sx;
                        _cubicBezier.Point4.Y = sy;
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
                        _cubicBezier.Point2.X = sx;
                        _cubicBezier.Point2.Y = sy;
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
                        _cubicBezier.Point3.X = sx;
                        _cubicBezier.Point3.Y = sy;
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

            _selection = new CubicBezierSelection(
                _editor.Project.CurrentContainer.HelperLayer,
                _cubicBezier,
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

            _currentState = ToolState.None;

            _selection.Remove();
            _selection = null;
        }
    }
}
