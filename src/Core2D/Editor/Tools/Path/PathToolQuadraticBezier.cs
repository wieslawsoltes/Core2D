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
    /// Helper class for <see cref="PathTool.QuadraticBezier"/> editor.
    /// </summary>
    public class PathToolQuadraticBezier : ToolBase
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ToolPath _toolPath;
        private ToolState _currentState = ToolState.None;
        private XPathQuadraticBezier _quadraticBezier = new XPathQuadraticBezier();
        private QuadraticBezierSelection _selection;

        /// <inheritdoc/>
        public override string Name => "QuadraticBezier";

        /// <summary>
        /// Initialize new instance of <see cref="PathToolQuadraticBezier"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="toolPath">The current <see cref="ToolPath"/> object.</param>
        public PathToolQuadraticBezier(IServiceProvider serviceProvider, ToolPath toolPath) : base()
        {
            _serviceProvider = serviceProvider;
            _toolPath = toolPath;
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
                        _quadraticBezier.Point1 = editor.TryToGetConnectionPoint(sx, sy) ?? XPoint.Create(sx, sy, editor.Project.Options.PointShape);
                        if (!_toolPath._isInitialized)
                        {
                            _toolPath.InitializeWorkingPath(_quadraticBezier.Point1);
                        }
                        else
                        {
                            _quadraticBezier.Point1 = _toolPath.GetLastPathPoint();
                        }

                        _quadraticBezier.Point2 = XPoint.Create(sx, sy, editor.Project.Options.PointShape);
                        _quadraticBezier.Point3 = XPoint.Create(sx, sy, editor.Project.Options.PointShape);
                        _toolPath._context.QuadraticBezierTo(
                            _quadraticBezier.Point2,
                            _quadraticBezier.Point3,
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
                        _quadraticBezier.Point3.X = sx;
                        _quadraticBezier.Point3.Y = sy;
                        if (editor.Project.Options.TryToConnect)
                        {
                            var point2 = editor.TryToGetConnectionPoint(sx, sy);
                            if (point2 != null)
                            {
                                var figure = _toolPath._geometry.Figures.LastOrDefault();
                                var quadraticBezier = figure.Segments.LastOrDefault() as XQuadraticBezierSegment;
                                quadraticBezier.Point2 = point2;
                                _quadraticBezier.Point3 = point2;
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
                        _quadraticBezier.Point2.X = sx;
                        _quadraticBezier.Point2.Y = sy;
                        if (editor.Project.Options.TryToConnect)
                        {
                            var point1 = editor.TryToGetConnectionPoint(sx, sy);
                            if (point1 != null)
                            {
                                var figure = _toolPath._geometry.Figures.LastOrDefault();
                                var quadraticBezier = figure.Segments.LastOrDefault() as XQuadraticBezierSegment;
                                quadraticBezier.Point1 = point1;
                                _quadraticBezier.Point2 = point1;
                            }
                        }

                        _quadraticBezier.Point1 = _quadraticBezier.Point3;
                        _quadraticBezier.Point2 = XPoint.Create(sx, sy, editor.Project.Options.PointShape);
                        _quadraticBezier.Point3 = XPoint.Create(sx, sy, editor.Project.Options.PointShape);
                        _toolPath._context.QuadraticBezierTo(
                            _quadraticBezier.Point2,
                            _quadraticBezier.Point3,
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
            switch (_currentState)
            {
                case ToolState.None:
                    break;
                case ToolState.One:
                case ToolState.Two:
                    {
                        _toolPath.RemoveLastSegment<XQuadraticBezierSegment>();

                        editor.Project.CurrentContainer.WorkingLayer.Shapes = editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_toolPath._path);
                        Remove();
                        if (_toolPath._path.Geometry.Figures.LastOrDefault().Segments.Length > 0)
                        {
                            Finalize(null);
                            editor.Project.AddShape(editor.Project.CurrentContainer.CurrentLayer, _toolPath._path);
                        }
                        else
                        {
                            editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        }
                        _toolPath.DeInitializeWorkingPath();
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
                        _quadraticBezier.Point2.X = sx;
                        _quadraticBezier.Point2.Y = sy;
                        _quadraticBezier.Point3.X = sx;
                        _quadraticBezier.Point3.Y = sy;
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
                        _quadraticBezier.Point2.X = sx;
                        _quadraticBezier.Point2.Y = sy;
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
            _selection = new QuadraticBezierSelection(
                editor.Project.CurrentContainer.HelperLayer,
                _quadraticBezier,
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
