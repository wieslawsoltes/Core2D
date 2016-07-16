// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Linq;
using Core2D.Editor.Bounds;
using Core2D.Math;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Helper class for <see cref="Tool.Path"/> editor.
    /// </summary>
    public class ToolPath : ToolBase
    {
        private ProjectEditor _editor;
        private ToolState _currentState = ToolState.None;
        // Path Tool
        private XPath _path;
        private XPathGeometry _geometry;
        private XGeometryContext _context;
        private bool _isInitialized = false;
        private PathTool _previousPathTool;
        private PathTool _movePathTool;
        // Line Tool
        private XPoint _lineStart;
        private XPoint _lineEnd;
        // CubicBezier Tool
        private XPoint _cubicBezierPoint1;
        private XPoint _cubicBezierPoint2;
        private XPoint _cubicBezierPoint3;
        private XPoint _cubicBezierPoint4;
        // QuadraticBezier Tool
        private XPoint _quadraticBezierPoint1;
        private XPoint _quadraticBezierPoint2;
        private XPoint _quadraticBezierPoint3;
        // Helpers Style
        private ShapeStyle _style;
        // Line Helper
        private XPoint _lineStartHelperPoint;
        private XPoint _lineEndHelperPoint;
        // CubicBezier Helper
        private XLine _cubicBezierLine12;
        private XLine _cubicBezierLine43;
        private XLine _cubicBezierLine23;
        private XPoint _cubicBezierHelperPoint1;
        private XPoint _cubicBezierHelperPoint2;
        private XPoint _cubicBezierHelperPoint3;
        private XPoint _cubicBezierHelperPoint4;
        // QuadraticBezier Helper
        private XLine _quadraticBezierLine12;
        private XLine _quadraticBezierLine32;
        private XPoint _quadraticBezierHelperPoint1;
        private XPoint _quadraticBezierHelperPoint2;
        private XPoint _quadraticBezierHelperPoint3;

        /// <summary>
        /// Initialize new instance of <see cref="ToolPath"/> class.
        /// </summary>
        /// <param name="editor">The current <see cref="ProjectEditor"/> object.</param>
        public ToolPath(ProjectEditor editor)
            : base()
        {
            _editor = editor;
        }

        /// <summary>
        /// Try to get connection point at specified location.
        /// </summary>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <returns>The connected point if success.</returns>
        public XPoint TryToGetConnectionPoint(double x, double y)
        {
            if (_editor.Project.Options.TryToConnect)
            {
                var result = ShapeHitTestPoint.HitTest(
                    _editor.Project.CurrentContainer.CurrentLayer.Shapes,
                    new Vector2(x, y),
                    _editor.Project.Options.HitThreshold);
                if (result != null && result is XPoint)
                {
                    return result as XPoint;
                }
            }
            return null;
        }

        private void InitializeWorkingPath(XPoint start)
        {
            _geometry = XPathGeometry.Create(
                new List<XPathFigure>(),
                _editor.Project.Options.DefaultFillRule);

            _context = new XPathGeometryContext(_geometry);

            _context.BeginFigure(
                start,
                _editor.Project.Options.DefaultIsFilled,
                _editor.Project.Options.DefaultIsClosed);

            var style = _editor.Project.CurrentStyleLibrary.Selected;
            _path = XPath.Create(
                "Path",
                _editor.Project.Options.CloneStyle ? style.Clone() : style,
                _geometry,
                _editor.Project.Options.DefaultIsStroked,
                _editor.Project.Options.DefaultIsFilled);

            _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_path);

            _previousPathTool = _editor.CurrentPathTool;
            _isInitialized = true;
        }

        private void DeInitializeWorkingPath()
        {
            _isInitialized = false;
            _geometry = null;
            _context = null;
            _path = null;
        }

        private void RemoveLastLineSegment()
        {
            var figure = _geometry.Figures.LastOrDefault();
            if (figure != null)
            {
                var segment = figure.Segments.LastOrDefault() as XLineSegment;
                if (segment != null)
                {
                    figure.Segments.Remove(segment);
                }
            }
        }

        private void RemoveLastArcSegment()
        {
            var figure = _geometry.Figures.LastOrDefault();
            if (figure != null)
            {
                var segment = figure.Segments.LastOrDefault() as XArcSegment;
                if (segment != null)
                {
                    figure.Segments.Remove(segment);
                }
            }
        }

        private void RemoveLastCubicBezierSegment()
        {
            var figure = _geometry.Figures.LastOrDefault();
            if (figure != null)
            {
                var segment = figure.Segments.LastOrDefault() as XCubicBezierSegment;
                if (segment != null)
                {
                    figure.Segments.Remove(segment);
                }
            }
        }

        private void RemoveLastQuadraticBezierSegment()
        {
            var figure = _geometry.Figures.LastOrDefault();
            if (figure != null)
            {
                var segment = figure.Segments.LastOrDefault() as XQuadraticBezierSegment;
                if (segment != null)
                {
                    figure.Segments.Remove(segment);
                }
            }
        }

        private void SetLineStartPointFromLastSegment()
        {
            var figure = _geometry.Figures.LastOrDefault();
            if (figure != null)
            {
                var segment = figure.Segments.LastOrDefault();
                if (segment != null)
                {
                    if (segment is XLineSegment)
                    {
                        _lineStart = (segment as XLineSegment).Point;
                    }
                    else if (segment is XArcSegment)
                    {
                        // TODO: Set line start point using last arc point.
                    }
                    else if (segment is XCubicBezierSegment)
                    {
                        _lineStart = (segment as XCubicBezierSegment).Point3;
                    }
                    else if (segment is XQuadraticBezierSegment)
                    {
                        _lineStart = (segment as XQuadraticBezierSegment).Point2;
                    }
                }
                else
                {
                    _lineStart = figure.StartPoint;
                }
            }
        }

        private void SetCubicBezieFirstPointFromLastSegment()
        {
            var figure = _geometry.Figures.LastOrDefault();
            if (figure != null)
            {
                var segment = figure.Segments.LastOrDefault();
                if (segment != null)
                {
                    if (segment is XLineSegment)
                    {
                        _cubicBezierPoint1 = (segment as XLineSegment).Point;
                    }
                    else if (segment is XArcSegment)
                    {
                        // TODO: Set cubic bezier first point using last arc point.
                    }
                    else if (segment is XCubicBezierSegment)
                    {
                        _cubicBezierPoint1 = (segment as XCubicBezierSegment).Point3;
                    }
                    else if (segment is XQuadraticBezierSegment)
                    {
                        _cubicBezierPoint1 = (segment as XQuadraticBezierSegment).Point2;
                    }
                }
                else
                {
                    _cubicBezierPoint1 = figure.StartPoint;
                }
            }
        }

        private void SetQuadraticBezieFirstPointFromLastSegment()
        {
            var figure = _geometry.Figures.LastOrDefault();
            if (figure != null)
            {
                var segment = figure.Segments.LastOrDefault();
                if (segment != null)
                {
                    if (segment is XLineSegment)
                    {
                        _quadraticBezierPoint1 = (segment as XLineSegment).Point;
                    }
                    else if (segment is XArcSegment)
                    {
                        // TODO: Set quadratic bezier first point using last arc point.
                    }
                    else if (segment is XCubicBezierSegment)
                    {
                        _quadraticBezierPoint1 = (segment as XCubicBezierSegment).Point3;
                    }
                    else if (segment is XQuadraticBezierSegment)
                    {
                        _quadraticBezierPoint1 = (segment as XQuadraticBezierSegment).Point2;
                    }
                }
                else
                {
                    _quadraticBezierPoint1 = figure.StartPoint;
                }
            }
        }

        private void LineLeftDown(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    {
                        _lineStart = TryToGetConnectionPoint(sx, sy) ?? XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        if (!_isInitialized)
                        {
                            InitializeWorkingPath(_lineStart);
                        }
                        else
                        {
                            SetLineStartPointFromLastSegment();
                        }

                        _lineEnd = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _context.LineTo(
                            _lineEnd,
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
                        _lineEnd.X = sx;
                        _lineEnd.Y = sy;
                        if (_editor.Project.Options.TryToConnect)
                        {
                            var end = TryToGetConnectionPoint(sx, sy);
                            if (end != null)
                            {
                                var figure = _geometry.Figures.LastOrDefault();
                                var line = figure.Segments.LastOrDefault() as XLineSegment;
                                line.Point = end;
                            }
                        }

                        _lineStart = _lineEnd;
                        _lineEnd = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _context.LineTo(_lineEnd,
                            _editor.Project.Options.DefaultIsStroked,
                            _editor.Project.Options.DefaultIsSmoothJoin);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = ToolState.One;
                    }
                    break;
            }
        }

        private void ArcLeftDown(double x, double y)
        {
            // TODO: Add Arc path helper LeftDown method implementation.
        }

        private void CubicBezierLeftDown(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    {
                        _cubicBezierPoint1 = TryToGetConnectionPoint(sx, sy) ?? XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        if (!_isInitialized)
                        {
                            InitializeWorkingPath(_cubicBezierPoint1);
                        }
                        else
                        {
                            SetCubicBezieFirstPointFromLastSegment();
                        }

                        _cubicBezierPoint2 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _cubicBezierPoint3 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _cubicBezierPoint4 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _context.CubicBezierTo(
                            _cubicBezierPoint2,
                            _cubicBezierPoint3,
                            _cubicBezierPoint4,
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
                        _cubicBezierPoint4.X = sx;
                        _cubicBezierPoint4.Y = sy;
                        if (_editor.Project.Options.TryToConnect)
                        {
                            var point3 = TryToGetConnectionPoint(sx, sy);
                            if (point3 != null)
                            {
                                var figure = _geometry.Figures.LastOrDefault();
                                var cubicBezier = figure.Segments.LastOrDefault() as XCubicBezierSegment;
                                cubicBezier.Point3 = point3;
                                _cubicBezierPoint4 = point3;
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
                        _cubicBezierPoint2.X = sx;
                        _cubicBezierPoint2.Y = sy;
                        if (_editor.Project.Options.TryToConnect)
                        {
                            var point1 = TryToGetConnectionPoint(sx, sy);
                            if (point1 != null)
                            {
                                var figure = _geometry.Figures.LastOrDefault();
                                var cubicBezier = figure.Segments.LastOrDefault() as XCubicBezierSegment;
                                cubicBezier.Point1 = point1;
                                _cubicBezierPoint2 = point1;
                            }
                        }
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateThree();
                        Move(null);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = ToolState.Three;
                    }
                    break;
                case ToolState.Three:
                    {
                        _cubicBezierPoint3.X = sx;
                        _cubicBezierPoint3.Y = sy;
                        if (_editor.Project.Options.TryToConnect)
                        {
                            var point2 = TryToGetConnectionPoint(sx, sy);
                            if (point2 != null)
                            {
                                var figure = _geometry.Figures.LastOrDefault();
                                var cubicBezier = figure.Segments.LastOrDefault() as XCubicBezierSegment;
                                cubicBezier.Point2 = point2;
                                _cubicBezierPoint3 = point2;
                            }
                        }

                        _cubicBezierPoint1 = _cubicBezierPoint4;
                        _cubicBezierPoint2 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _cubicBezierPoint3 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _cubicBezierPoint4 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _context.CubicBezierTo(
                            _cubicBezierPoint2,
                            _cubicBezierPoint3,
                            _cubicBezierPoint4,
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

        private void QuadraticBezierLeftDown(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    {
                        _quadraticBezierPoint1 = TryToGetConnectionPoint(sx, sy) ?? XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        if (!_isInitialized)
                        {
                            InitializeWorkingPath(_quadraticBezierPoint1);
                        }
                        else
                        {
                            SetQuadraticBezieFirstPointFromLastSegment();
                        }

                        _quadraticBezierPoint2 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _quadraticBezierPoint3 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _context.QuadraticBezierTo(
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
                            var point2 = TryToGetConnectionPoint(sx, sy);
                            if (point2 != null)
                            {
                                var figure = _geometry.Figures.LastOrDefault();
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
                            var point1 = TryToGetConnectionPoint(sx, sy);
                            if (point1 != null)
                            {
                                var figure = _geometry.Figures.LastOrDefault();
                                var quadraticBezier = figure.Segments.LastOrDefault() as XQuadraticBezierSegment;
                                quadraticBezier.Point1 = point1;
                                _quadraticBezierPoint2 = point1;
                            }
                        }

                        _quadraticBezierPoint1 = _quadraticBezierPoint3;
                        _quadraticBezierPoint2 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _quadraticBezierPoint3 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _context.QuadraticBezierTo(
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

        private void LineRightDown(double x, double y)
        {
            switch (_currentState)
            {
                case ToolState.None:
                    break;
                case ToolState.One:
                    {
                        RemoveLastLineSegment();

                        _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_path);
                        Remove();
                        if (_path.Geometry.Figures.LastOrDefault().Segments.Count > 0)
                        {
                            Finalize(null);
                            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, _path);
                        }
                        else
                        {
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        }
                        DeInitializeWorkingPath();
                        _currentState = ToolState.None;
                        _editor.CancelAvailable = false;
                    }
                    break;
            }
        }

        private void ArcRightDown(double x, double y)
        {
            // TODO: Add Arc path helper RightDown method implementation.
        }

        private void CubicBezierRightDown(double x, double y)
        {
            switch (_currentState)
            {
                case ToolState.None:
                    break;
                case ToolState.One:
                case ToolState.Two:
                case ToolState.Three:
                    {
                        RemoveLastCubicBezierSegment();

                        _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_path);
                        Remove();
                        if (_path.Geometry.Figures.LastOrDefault().Segments.Count > 0)
                        {
                            Finalize(null);
                            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, _path);
                        }
                        else
                        {
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        }
                        DeInitializeWorkingPath();
                        _currentState = ToolState.None;
                        _editor.CancelAvailable = false;
                    }
                    break;
            }
        }

        private void QuadraticBezierRightDown(double x, double y)
        {
            switch (_currentState)
            {
                case ToolState.None:
                    break;
                case ToolState.One:
                case ToolState.Two:
                    {
                        RemoveLastQuadraticBezierSegment();

                        _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_path);
                        Remove();
                        if (_path.Geometry.Figures.LastOrDefault().Segments.Count > 0)
                        {
                            Finalize(null);
                            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, _path);
                        }
                        else
                        {
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        }
                        DeInitializeWorkingPath();
                        _currentState = ToolState.None;
                        _editor.CancelAvailable = false;
                    }
                    break;
            }
        }

        private void LineMove(double x, double y)
        {
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
                        _lineEnd.X = sx;
                        _lineEnd.Y = sy;
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                    }
                    break;
            }
        }

        private void ArcMove(double x, double y)
        {
            // TODO: Add Arc path helper Move method implementation.
        }

        private void CubicBezierMove(double x, double y)
        {
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
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
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
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
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
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                    }
                    break;
            }
        }

        private void QuadraticBezierMove(double x, double y)
        {
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

        private void ToStateOneLine()
        {
            _style = _editor.Project.Options.HelperStyle;
            _lineStartHelperPoint = XPoint.Create(0, 0, _editor.Project.Options.PointShape);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_lineStartHelperPoint);
            _lineEndHelperPoint = XPoint.Create(0, 0, _editor.Project.Options.PointShape);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_lineEndHelperPoint);
        }

        private void ToStateOneArc()
        {
            // TODO: Add Arc path helper ToStateOne method implementation.
        }

        private void ToStateOneCubicBezier()
        {
            _style = _editor.Project.Options.HelperStyle;
            _cubicBezierHelperPoint1 = XPoint.Create(0, 0, _editor.Project.Options.PointShape);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_cubicBezierHelperPoint1);
            _cubicBezierHelperPoint4 = XPoint.Create(0, 0, _editor.Project.Options.PointShape);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_cubicBezierHelperPoint4);
        }

        private void ToStateOneQuadraticBezier()
        {
            _style = _editor.Project.Options.HelperStyle;
            _quadraticBezierHelperPoint1 = XPoint.Create(0, 0, _editor.Project.Options.PointShape);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_quadraticBezierHelperPoint1);
            _quadraticBezierHelperPoint3 = XPoint.Create(0, 0, _editor.Project.Options.PointShape);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_quadraticBezierHelperPoint3);
        }

        private void ToStateTwoArc()
        {
            // TODO: Add Arc path helper ToStateTwo method implementation.
        }

        private void ToStateTwoCubicBezier()
        {
            _style = _editor.Project.Options.HelperStyle;
            _cubicBezierLine12 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_cubicBezierLine12);
            _cubicBezierHelperPoint2 = XPoint.Create(0, 0, _editor.Project.Options.PointShape);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_cubicBezierHelperPoint2);
        }

        private void ToStateTwoQuadraticBezier()
        {
            _style = _editor.Project.Options.HelperStyle;
            _quadraticBezierLine12 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_quadraticBezierLine12);
            _quadraticBezierLine32 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_quadraticBezierLine32);
            _quadraticBezierHelperPoint2 = XPoint.Create(0, 0, _editor.Project.Options.PointShape);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_quadraticBezierHelperPoint2);
        }

        private void ToStateThreeArc()
        {
            // TODO: Add Arc path helper ToStateThree method implementation.
        }

        private void ToStateThreeCubicBezier()
        {
            _cubicBezierLine43 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_cubicBezierLine43);
            _cubicBezierLine23 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_cubicBezierLine23);
            _cubicBezierHelperPoint3 = XPoint.Create(0, 0, _editor.Project.Options.PointShape);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_cubicBezierHelperPoint3);
        }

        private void MoveLineHelpers()
        {
            if (_lineStartHelperPoint != null)
            {
                _lineStartHelperPoint.X = _lineStart.X;
                _lineStartHelperPoint.Y = _lineStart.Y;
            }

            if (_lineEndHelperPoint != null)
            {
                _lineEndHelperPoint.X = _lineEnd.X;
                _lineEndHelperPoint.Y = _lineEnd.Y;
            }
        }

        private void MoveArcHelpers()
        {
            // TODO: Add Arc path helper Move method implementation.
        }

        private void MoveCubicBezierHelpers()
        {
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

        private void MoveQuadraticBezierHelpers()
        {
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

        private void RemoveLineHelpers()
        {
            if (_lineStartHelperPoint != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_lineStartHelperPoint);
                _lineStartHelperPoint = null;
            }

            if (_lineEndHelperPoint != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_lineEndHelperPoint);
                _lineEndHelperPoint = null;
            }

            _style = null;
        }

        private void RemoveArcHelpers()
        {
            // TODO: Add Arc path helper Remove method implementation.
        }

        private void RemoveCubicBezierHelpers()
        {
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

        private void RemoveQuadraticBezierHelpers()
        {
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

        private void SwitchPathTool(double x, double y)
        {
            switch (_previousPathTool)
            {
                case PathTool.Line:
                    {
                        RemoveLastLineSegment();
                        RemoveLineHelpers();
                    }
                    break;
                case PathTool.Arc:
                    {
                        RemoveLastArcSegment();
                        RemoveArcHelpers();
                    }
                    break;
                case PathTool.CubicBezier:
                    {
                        RemoveLastCubicBezierSegment();
                        RemoveCubicBezierHelpers();
                    }
                    break;
                case PathTool.QuadraticBezier:
                    {
                        RemoveLastQuadraticBezierSegment();
                        RemoveQuadraticBezierHelpers();
                    }
                    break;
            }

            _currentState = ToolState.None;

            switch (_editor.CurrentPathTool)
            {
                case PathTool.Line:
                    {
                        LineLeftDown(x, y);
                    }
                    break;
                case PathTool.Arc:
                    {
                        ArcLeftDown(x, y);
                    }
                    break;
                case PathTool.CubicBezier:
                    {
                        CubicBezierLeftDown(x, y);
                    }
                    break;
                case PathTool.QuadraticBezier:
                    {
                        QuadraticBezierLeftDown(x, y);
                    }
                    break;
                case PathTool.Move:
                    {
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                    }
                    break;
            }

            if (_editor.CurrentPathTool == PathTool.Move)
            {
                _movePathTool = _previousPathTool;
            }

            _previousPathTool = _editor.CurrentPathTool;
        }

        private void StartFigureLeftDown(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? ProjectEditor.Snap(y, _editor.Project.Options.SnapY) : y;

            // start new figure
            var start = TryToGetConnectionPoint(sx, sy) ?? XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
            _context.BeginFigure(
                start,
                _editor.Project.Options.DefaultIsFilled,
                _editor.Project.Options.DefaultIsClosed);

            // switch to path tool before Move tool
            _editor.CurrentPathTool = _movePathTool;
            SwitchPathTool(x, y);
        }

        private void StartFigureMove(double x, double y)
        {
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
            }
        }

        /// <inheritdoc/>
        public override void LeftDown(double x, double y)
        {
            base.LeftDown(x, y);

            if (_isInitialized && _editor.CurrentPathTool != _previousPathTool)
            {
                SwitchPathTool(x, y);
                return;
            }

            switch (_editor.CurrentPathTool)
            {
                case PathTool.Line:
                    {
                        LineLeftDown(x, y);
                    }
                    break;
                case PathTool.Arc:
                    {
                        ArcLeftDown(x, y);
                    }
                    break;
                case PathTool.CubicBezier:
                    {
                        CubicBezierLeftDown(x, y);
                    }
                    break;
                case PathTool.QuadraticBezier:
                    {
                        QuadraticBezierLeftDown(x, y);
                    }
                    break;
                case PathTool.Move:
                    {
                        StartFigureLeftDown(x, y);
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void RightDown(double x, double y)
        {
            base.RightDown(x, y);

            switch (_editor.CurrentPathTool)
            {
                case PathTool.Line:
                    {
                        LineRightDown(x, y);
                    }
                    break;
                case PathTool.Arc:
                    {
                        ArcRightDown(x, y);
                    }
                    break;
                case PathTool.CubicBezier:
                    {
                        CubicBezierRightDown(x, y);
                    }
                    break;
                case PathTool.QuadraticBezier:
                    {
                        QuadraticBezierRightDown(x, y);
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Move(double x, double y)
        {
            base.Move(x, y);

            if (_isInitialized && _editor.CurrentPathTool != _previousPathTool)
            {
                SwitchPathTool(x, y);
            }

            switch (_editor.CurrentPathTool)
            {
                case PathTool.Line:
                    {
                        LineMove(x, y);
                    }
                    break;
                case PathTool.Arc:
                    {
                        ArcMove(x, y);
                    }
                    break;
                case PathTool.CubicBezier:
                    {
                        CubicBezierMove(x, y);
                    }
                    break;
                case PathTool.QuadraticBezier:
                    {
                        QuadraticBezierMove(x, y);
                    }
                    break;
                case PathTool.Move:
                    {
                        StartFigureMove(x, y);
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void ToStateOne()
        {
            base.ToStateOne();

            switch (_editor.CurrentPathTool)
            {
                case PathTool.Line:
                    {
                        ToStateOneLine();
                    }
                    break;
                case PathTool.Arc:
                    {
                        ToStateOneArc();
                    }
                    break;
                case PathTool.CubicBezier:
                    {
                        ToStateOneCubicBezier();
                    }
                    break;
                case PathTool.QuadraticBezier:
                    {
                        ToStateOneQuadraticBezier();
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void ToStateTwo()
        {
            base.ToStateTwo();

            switch (_editor.CurrentPathTool)
            {
                case PathTool.Line:
                    break;
                case PathTool.Arc:
                    {
                        ToStateTwoArc();
                    }
                    break;
                case PathTool.CubicBezier:
                    {
                        ToStateTwoCubicBezier();
                    }
                    break;
                case PathTool.QuadraticBezier:
                    {
                        ToStateTwoQuadraticBezier();
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void ToStateThree()
        {
            base.ToStateThree();

            switch (_editor.CurrentPathTool)
            {
                case PathTool.Line:
                    break;
                case PathTool.Arc:
                    {
                        ToStateThreeArc();
                    }
                    break;
                case PathTool.CubicBezier:
                    {
                        ToStateThreeCubicBezier();
                    }
                    break;
                case PathTool.QuadraticBezier:
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Move(BaseShape shape)
        {
            base.Move(shape);

            switch (_editor.CurrentPathTool)
            {
                case PathTool.Line:
                    {
                        MoveLineHelpers();
                    }
                    break;
                case PathTool.Arc:
                    {
                        MoveArcHelpers();
                    }
                    break;
                case PathTool.CubicBezier:
                    {
                        MoveCubicBezierHelpers();
                    }
                    break;
                case PathTool.QuadraticBezier:
                    {
                        MoveQuadraticBezierHelpers();
                    }
                    break;
            }
        }

        /// <inheritdoc/>
        public override void Remove()
        {
            base.Remove();

            switch (_editor.CurrentPathTool)
            {
                case PathTool.Line:
                    {
                        RemoveLineHelpers();
                    }
                    break;
                case PathTool.Arc:
                    {
                        RemoveArcHelpers();
                    }
                    break;
                case PathTool.CubicBezier:
                    {
                        RemoveCubicBezierHelpers();
                    }
                    break;
                case PathTool.QuadraticBezier:
                    {
                        RemoveQuadraticBezierHelpers();
                    }
                    break;
            }
        }
    }
}
