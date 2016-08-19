// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using Core2D.Editor.Tools.Path.Shapes;
using Core2D.Editor.Tools.Selection;
using Core2D.Path.Segments;
using Core2D.Shape;
using Core2D.Shapes;

namespace Core2D.Editor.Tools.Path
{
    /// <summary>
    /// Cubic bezier path tool.
    /// </summary>
    public class PathToolCubicBezier : PathToolBase
    {
        private readonly IServiceProvider _serviceProvider;
        private ToolState _currentState = ToolState.None;
        private XPathCubicBezier _cubicBezier = new XPathCubicBezier();
        private CubicBezierSelection _selection;

        /// <inheritdoc/>
        public override string Name => "CubicBezier";

        /// <summary>
        /// Initialize new instance of <see cref="PathToolCubicBezier"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public PathToolCubicBezier(IServiceProvider serviceProvider) : base()
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public override void LeftDown(double x, double y)
        {
            base.LeftDown(x, y);
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var pathTool = _serviceProvider.GetService<ToolPath>();
            double sx = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, editor.Project.Options.SnapX) : x;
            double sy = editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    {
                        _cubicBezier.Point1 = editor.TryToGetConnectionPoint(sx, sy) ?? XPoint.Create(sx, sy, editor.Project.Options.PointShape);
                        if (!pathTool.IsInitialized)
                        {
                            pathTool.InitializeWorkingPath(_cubicBezier.Point1);
                        }
                        else
                        {
                            _cubicBezier.Point1 = pathTool.GetLastPathPoint();
                        }

                        _cubicBezier.Point2 = XPoint.Create(sx, sy, editor.Project.Options.PointShape);
                        _cubicBezier.Point3 = XPoint.Create(sx, sy, editor.Project.Options.PointShape);
                        _cubicBezier.Point4 = XPoint.Create(sx, sy, editor.Project.Options.PointShape);
                        pathTool.GeometryContext.CubicBezierTo(
                            _cubicBezier.Point2,
                            _cubicBezier.Point3,
                            _cubicBezier.Point4,
                            editor.Project.Options.DefaultIsStroked,
                            editor.Project.Options.DefaultIsSmoothJoin);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateOne();
                        Move(null);
                        _currentState = ToolState.One;
                        editor.CancelAvailable = true;
                    }
                    break;
                case ToolState.One:
                    {
                        _cubicBezier.Point4.X = sx;
                        _cubicBezier.Point4.Y = sy;
                        if (editor.Project.Options.TryToConnect)
                        {
                            var point3 = editor.TryToGetConnectionPoint(sx, sy);
                            if (point3 != null)
                            {
                                var figure = pathTool.Geometry.Figures.LastOrDefault();
                                var cubicBezier = figure.Segments.LastOrDefault() as XCubicBezierSegment;
                                cubicBezier.Point3 = point3;
                                _cubicBezier.Point4 = point3;
                            }
                        }
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateTwo();
                        Move(null);
                        _currentState = ToolState.Two;
                    }
                    break;
                case ToolState.Two:
                    {
                        _cubicBezier.Point2.X = sx;
                        _cubicBezier.Point2.Y = sy;
                        if (editor.Project.Options.TryToConnect)
                        {
                            var point1 = editor.TryToGetConnectionPoint(sx, sy);
                            if (point1 != null)
                            {
                                var figure = pathTool.Geometry.Figures.LastOrDefault();
                                var cubicBezier = figure.Segments.LastOrDefault() as XCubicBezierSegment;
                                cubicBezier.Point1 = point1;
                                _cubicBezier.Point2 = point1;
                            }
                        }
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateThree();
                        Move(null);
                        _currentState = ToolState.Three;
                    }
                    break;
                case ToolState.Three:
                    {
                        _cubicBezier.Point3.X = sx;
                        _cubicBezier.Point3.Y = sy;
                        if (editor.Project.Options.TryToConnect)
                        {
                            var point2 = editor.TryToGetConnectionPoint(sx, sy);
                            if (point2 != null)
                            {
                                var figure = pathTool.Geometry.Figures.LastOrDefault();
                                var cubicBezier = figure.Segments.LastOrDefault() as XCubicBezierSegment;
                                cubicBezier.Point2 = point2;
                                _cubicBezier.Point3 = point2;
                            }
                        }

                        _cubicBezier.Point1 = _cubicBezier.Point4;
                        _cubicBezier.Point2 = XPoint.Create(sx, sy, editor.Project.Options.PointShape);
                        _cubicBezier.Point3 = XPoint.Create(sx, sy, editor.Project.Options.PointShape);
                        _cubicBezier.Point4 = XPoint.Create(sx, sy, editor.Project.Options.PointShape);
                        pathTool.GeometryContext.CubicBezierTo(
                            _cubicBezier.Point2,
                            _cubicBezier.Point3,
                            _cubicBezier.Point4,
                            editor.Project.Options.DefaultIsStroked,
                            editor.Project.Options.DefaultIsSmoothJoin);
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
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
            var editor = _serviceProvider.GetService<ProjectEditor>();
            var pathTool = _serviceProvider.GetService<ToolPath>();
            switch (_currentState)
            {
                case ToolState.None:
                    break;
                case ToolState.One:
                case ToolState.Two:
                case ToolState.Three:
                    {
                        pathTool.RemoveLastSegment<XCubicBezierSegment>();

                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(pathTool.Path);
                        Remove();
                        if (pathTool.Path.Geometry.Figures.LastOrDefault().Segments.Length > 0)
                        {
                            Finalize(null);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, pathTool.Path);
                        }
                        else
                        {
                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        }
                        pathTool.DeInitializeWorkingPath();
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
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape(sx, sy);
                        }
                        _cubicBezier.Point2.X = sx;
                        _cubicBezier.Point2.Y = sy;
                        _cubicBezier.Point3.X = sx;
                        _cubicBezier.Point3.Y = sy;
                        _cubicBezier.Point4.X = sx;
                        _cubicBezier.Point4.Y = sy;
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                    }
                    break;
                case ToolState.Two:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape(sx, sy);
                        }
                        _cubicBezier.Point2.X = sx;
                        _cubicBezier.Point2.Y = sy;
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                    }
                    break;
                case ToolState.Three:
                    {
                        if (editor.Project.Options.TryToConnect)
                        {
                            editor.TryToHoverShape(sx, sy);
                        }
                        _cubicBezier.Point3.X = sx;
                        _cubicBezier.Point3.Y = sy;
                        editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void ToStateOne()
        {
            base.ToStateOne();
            var editor = _serviceProvider.GetService<ProjectEditor>();
            _selection = new CubicBezierSelection(
                editor.Project.CurrentContainer.HelperLayer,
                _cubicBezier,
                editor.Project.Options.HelperStyle,
                editor.Project.Options.PointShape);

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
