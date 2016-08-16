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
    /// Helper class for <see cref="PathTool.QuadraticBezier"/> editor.
    /// </summary>
    internal class ToolPathQuadraticBezier : ToolBase
    {
        private ToolState _currentState = ToolState.None;
        private ToolPath _toolPath;
        private XPathQuadraticBezier _quadraticBezier = new XPathQuadraticBezier();
        private QuadraticBezierSelection _selection;

        /// <summary>
        /// Initialize new instance of <see cref="ToolPathQuadraticBezier"/> class.
        /// </summary>
        /// <param name="toolPath">The current <see cref="ToolPath"/> object.</param>
        public ToolPathQuadraticBezier(ToolPath toolPath)
            : base()
        {
            _toolPath = toolPath;
        }

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
                        _quadraticBezier.Point1 = Editor.TryToGetConnectionPoint(sx, sy) ?? XPoint.Create(sx, sy, Editor.Project.Options.PointShape);
                        if (!_toolPath._isInitialized)
                        {
                            _toolPath.InitializeWorkingPath(_quadraticBezier.Point1);
                        }
                        else
                        {
                            _quadraticBezier.Point1 = _toolPath.GetLastPathPoint();
                        }

                        _quadraticBezier.Point2 = XPoint.Create(sx, sy, Editor.Project.Options.PointShape);
                        _quadraticBezier.Point3 = XPoint.Create(sx, sy, Editor.Project.Options.PointShape);
                        _toolPath._context.QuadraticBezierTo(
                            _quadraticBezier.Point2,
                            _quadraticBezier.Point3,
                            Editor.Project.Options.DefaultIsStroked,
                            Editor.Project.Options.DefaultIsSmoothJoin);
                        Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateOne();
                        Move(null);
                        _currentState = ToolState.One;
                        Editor.CancelAvailable = true;
                    }
                    break;
                case ToolState.One:
                    {
                        _quadraticBezier.Point3.X = sx;
                        _quadraticBezier.Point3.Y = sy;
                        if (Editor.Project.Options.TryToConnect)
                        {
                            var point2 = Editor.TryToGetConnectionPoint(sx, sy);
                            if (point2 != null)
                            {
                                var figure = _toolPath._geometry.Figures.LastOrDefault();
                                var quadraticBezier = figure.Segments.LastOrDefault() as XQuadraticBezierSegment;
                                quadraticBezier.Point2 = point2;
                                _quadraticBezier.Point3 = point2;
                            }
                        }
                        Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateTwo();
                        Move(null);
                        _currentState = ToolState.Two;
                    }
                    break;
                case ToolState.Two:
                    {
                        _quadraticBezier.Point2.X = sx;
                        _quadraticBezier.Point2.Y = sy;
                        if (Editor.Project.Options.TryToConnect)
                        {
                            var point1 = Editor.TryToGetConnectionPoint(sx, sy);
                            if (point1 != null)
                            {
                                var figure = _toolPath._geometry.Figures.LastOrDefault();
                                var quadraticBezier = figure.Segments.LastOrDefault() as XQuadraticBezierSegment;
                                quadraticBezier.Point1 = point1;
                                _quadraticBezier.Point2 = point1;
                            }
                        }

                        _quadraticBezier.Point1 = _quadraticBezier.Point3;
                        _quadraticBezier.Point2 = XPoint.Create(sx, sy, Editor.Project.Options.PointShape);
                        _quadraticBezier.Point3 = XPoint.Create(sx, sy, Editor.Project.Options.PointShape);
                        _toolPath._context.QuadraticBezierTo(
                            _quadraticBezier.Point2,
                            _quadraticBezier.Point3,
                            Editor.Project.Options.DefaultIsStroked,
                            Editor.Project.Options.DefaultIsSmoothJoin);
                        Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
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
                    {
                        _toolPath.RemoveLastSegment<XQuadraticBezierSegment>();

                        Editor.Project.CurrentContainer.WorkingLayer.Shapes = Editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_toolPath._path);
                        Remove();
                        if (_toolPath._path.Geometry.Figures.LastOrDefault().Segments.Length > 0)
                        {
                            Finalize(null);
                            Editor.Project.AddShape(Editor.Project.CurrentContainer.CurrentLayer, _toolPath._path);
                        }
                        else
                        {
                            Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        }
                        _toolPath.DeInitializeWorkingPath();
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
                        if (Editor.Project.Options.TryToConnect)
                        {
                            Editor.TryToHoverShape(sx, sy);
                        }
                        _quadraticBezier.Point2.X = sx;
                        _quadraticBezier.Point2.Y = sy;
                        _quadraticBezier.Point3.X = sx;
                        _quadraticBezier.Point3.Y = sy;
                        Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                    }
                    break;
                case ToolState.Two:
                    {
                        if (Editor.Project.Options.TryToConnect)
                        {
                            Editor.TryToHoverShape(sx, sy);
                        }
                        _quadraticBezier.Point2.X = sx;
                        _quadraticBezier.Point2.Y = sy;
                        Editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void ToStateOne()
        {
            base.ToStateOne();

            _selection = new QuadraticBezierSelection(
                Editor.Project.CurrentContainer.HelperLayer,
                _quadraticBezier,
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
