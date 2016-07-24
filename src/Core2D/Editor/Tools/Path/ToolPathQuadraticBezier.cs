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
        private ProjectEditor _editor;
        private ToolState _currentState = ToolState.None;
        private ToolPath _toolPath;
        private XPathQuadraticBezier _quadraticBezier = new XPathQuadraticBezier();
        private QuadraticBezierSelection _selection;

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
                        _quadraticBezier.Point1 = _editor.TryToGetConnectionPoint(sx, sy) ?? XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        if (!_toolPath._isInitialized)
                        {
                            _toolPath.InitializeWorkingPath(_quadraticBezier.Point1);
                        }
                        else
                        {
                            _quadraticBezier.Point1 = _toolPath.GetLastPathPoint();
                        }

                        _quadraticBezier.Point2 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _quadraticBezier.Point3 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _toolPath._context.QuadraticBezierTo(
                            _quadraticBezier.Point2,
                            _quadraticBezier.Point3,
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
                        _quadraticBezier.Point3.X = sx;
                        _quadraticBezier.Point3.Y = sy;
                        if (_editor.Project.Options.TryToConnect)
                        {
                            var point2 = _editor.TryToGetConnectionPoint(sx, sy);
                            if (point2 != null)
                            {
                                var figure = _toolPath._geometry.Figures.LastOrDefault();
                                var quadraticBezier = figure.Segments.LastOrDefault() as XQuadraticBezierSegment;
                                quadraticBezier.Point2 = point2;
                                _quadraticBezier.Point3 = point2;
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
                        _quadraticBezier.Point2.X = sx;
                        _quadraticBezier.Point2.Y = sy;
                        if (_editor.Project.Options.TryToConnect)
                        {
                            var point1 = _editor.TryToGetConnectionPoint(sx, sy);
                            if (point1 != null)
                            {
                                var figure = _toolPath._geometry.Figures.LastOrDefault();
                                var quadraticBezier = figure.Segments.LastOrDefault() as XQuadraticBezierSegment;
                                quadraticBezier.Point1 = point1;
                                _quadraticBezier.Point2 = point1;
                            }
                        }

                        _quadraticBezier.Point1 = _quadraticBezier.Point3;
                        _quadraticBezier.Point2 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _quadraticBezier.Point3 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _toolPath._context.QuadraticBezierTo(
                            _quadraticBezier.Point2,
                            _quadraticBezier.Point3,
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
                    {
                        _toolPath.RemoveLastSegment<XQuadraticBezierSegment>();

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
                        _quadraticBezier.Point2.X = sx;
                        _quadraticBezier.Point2.Y = sy;
                        _quadraticBezier.Point3.X = sx;
                        _quadraticBezier.Point3.Y = sy;
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
                        _quadraticBezier.Point2.X = sx;
                        _quadraticBezier.Point2.Y = sy;
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

            _selection = new QuadraticBezierSelection(
                _editor.Project.CurrentContainer.HelperLayer,
                _quadraticBezier,
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
