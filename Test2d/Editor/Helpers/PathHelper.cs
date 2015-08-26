// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class PathHelper : Helper
    {
        private Editor _editor;
        private State _currentState = State.None;
        // path
        private XPath _path;
        private XPathGeometry _geometry;
        private bool _isInitialized = false;
        private PathTool _previousPathTool;
        private PathTool _movePathTool;
        // line
        private XPoint _lineStart;
        private XPoint _lineEnd;
        // bezier
        private XPoint _bezierPoint1;
        private XPoint _bezierPoint2;
        private XPoint _bezierPoint3;
        private XPoint _bezierPoint4;
        // qbezier
        private XPoint _qbezierPoint1;
        private XPoint _qbezierPoint2;
        private XPoint _qbezierPoint3;
        // helpers
        private ShapeStyle _style;
        private double _pointEllipseRadius = 3.0;
        // line helper
        private XEllipse _lineEllipseStart;
        private XEllipse _lineEllipseEnd;
        // bezier helper
        private XLine _bezierLine12;
        private XLine _bezierLine43;
        private XLine _bezierLine23;
        private XEllipse _bezierEllipsePoint1;
        private XEllipse _bezierEllipsePoint2;
        private XEllipse _bezierEllipsePoint3;
        private XEllipse _bezierEllipsePoint4;
        // qbezier helper
        private XLine _qbezierLine12;
        private XLine _qbezierLine32;
        private XEllipse _qbezierEllipsePoint1;
        private XEllipse _qbezierEllipsePoint2;
        private XEllipse _qbezierEllipsePoint3;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="editor"></param>
        public PathHelper(Editor editor)
        {
            _editor = editor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public XPoint TryToGetConnectionPoint(double x, double y)
        {
            if (_editor.Project.Options.TryToConnect)
            {
                var result = ShapeBounds.HitTest(
                    _editor.Project.CurrentContainer, 
                    new Vector2(x, y),
                    _editor.Project.Options.HitTreshold);
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

            _geometry.BeginFigure(
                start, 
                _editor.Project.Options.DefaultIsFilled, 
                _editor.Project.Options.DefaultIsClosed);

            _path = XPath.Create(
                "Path",
                _editor.Project.CurrentStyleLibrary.CurrentStyle,
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

        private void RemoveLastBezierSegment()
        {
            var figure = _geometry.Figures.LastOrDefault();
            if (figure != null)
            {
                var segment = figure.Segments.LastOrDefault() as XBezierSegment;
                if (segment != null)
                {
                    figure.Segments.Remove(segment);
                }
            }
        }

        private void RemoveLastQBezierSegment()
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
                    else if (segment is XBezierSegment)
                    {
                        _lineStart = (segment as XBezierSegment).Point3;
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

        private void SetBezieFirstPointFromLastSegment()
        {
            var figure = _geometry.Figures.LastOrDefault();
            if (figure != null)
            {
                var segment = figure.Segments.LastOrDefault();
                if (segment != null)
                {
                    if (segment is XLineSegment)
                    {
                        _bezierPoint1 = (segment as XLineSegment).Point;
                    }
                    else if (segment is XArcSegment)
                    {
                        // TODO: Set bezier first point using last arc point.
                    }
                    else if (segment is XBezierSegment)
                    {
                        _bezierPoint1 = (segment as XBezierSegment).Point3;
                    }
                    else if (segment is XQuadraticBezierSegment)
                    {
                        _bezierPoint1 = (segment as XQuadraticBezierSegment).Point2;
                    }
                }
                else
                {
                    _bezierPoint1 = figure.StartPoint;
                }
            }
        }

        private void SetQBezieFirstPointFromLastSegment()
        {
            var figure = _geometry.Figures.LastOrDefault();
            if (figure != null)
            {
                var segment = figure.Segments.LastOrDefault();
                if (segment != null)
                {
                    if (segment is XLineSegment)
                    {
                        _qbezierPoint1 = (segment as XLineSegment).Point;
                    }
                    else if (segment is XArcSegment)
                    {
                        // TODO: Set qbezier first point using last arc point.
                    }
                    else if (segment is XBezierSegment)
                    {
                        _qbezierPoint1 = (segment as XBezierSegment).Point3;
                    }
                    else if (segment is XQuadraticBezierSegment)
                    {
                        _qbezierPoint1 = (segment as XQuadraticBezierSegment).Point2;
                    }
                }
                else
                {
                    _qbezierPoint1 = figure.StartPoint;
                }
            }
        }

        private void LineLeftDown(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case State.None:
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
                        _geometry.LineTo(
                            _lineEnd,
                            _editor.Project.Options.DefaultIsStroked,
                            _editor.Project.Options.DefaultIsSmoothJoin);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateOne();
                        Move(null);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = State.One;
                        _editor.CancelAvailable = true;
                    }
                    break;
                case State.One:
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
                        _geometry.LineTo(_lineEnd,
                            _editor.Project.Options.DefaultIsStroked,
                            _editor.Project.Options.DefaultIsSmoothJoin);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = State.One;
                    }
                    break;
            }
        }

        private void ArcLeftDown(double x, double y)
        {
            // TODO: Add Arc path helper LeftDown method implementation.
        }

        private void BezierLeftDown(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case State.None:
                    {
                        _bezierPoint1 = TryToGetConnectionPoint(sx, sy) ?? XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        if (!_isInitialized)
                        {
                            InitializeWorkingPath(_bezierPoint1);
                        }
                        else
                        {
                            SetBezieFirstPointFromLastSegment();
                        }

                        _bezierPoint2 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _bezierPoint3 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _bezierPoint4 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _geometry.BezierTo(
                            _bezierPoint2,
                            _bezierPoint3,
                            _bezierPoint4,
                            _editor.Project.Options.DefaultIsStroked,
                            _editor.Project.Options.DefaultIsSmoothJoin);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateOne();
                        Move(null);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = State.One;
                        _editor.CancelAvailable = true;
                    }
                    break;
                case State.One:
                    {
                        _bezierPoint4.X = sx;
                        _bezierPoint4.Y = sy;
                        if (_editor.Project.Options.TryToConnect)
                        {
                            var point3 = TryToGetConnectionPoint(sx, sy);
                            if (point3 != null)
                            {
                                var figure = _geometry.Figures.LastOrDefault();
                                var bezier = figure.Segments.LastOrDefault() as XBezierSegment;
                                bezier.Point3 = point3;
                                _bezierPoint4 = point3;
                            }
                        }
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateTwo();
                        Move(null);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = State.Two;
                    }
                    break;
                case State.Two:
                    {
                        _bezierPoint2.X = sx;
                        _bezierPoint2.Y = sy;
                        if (_editor.Project.Options.TryToConnect)
                        {
                            var point1 = TryToGetConnectionPoint(sx, sy);
                            if (point1 != null)
                            {
                                var figure = _geometry.Figures.LastOrDefault();
                                var bezier = figure.Segments.LastOrDefault() as XBezierSegment;
                                bezier.Point1 = point1;
                                _bezierPoint2 = point1;
                            }
                        }
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateThree();
                        Move(null);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = State.Three;
                    }
                    break;
                case State.Three:
                    {
                        _bezierPoint3.X = sx;
                        _bezierPoint3.Y = sy;
                        if (_editor.Project.Options.TryToConnect)
                        {
                            var point2 = TryToGetConnectionPoint(sx, sy);
                            if (point2 != null)
                            {
                                var figure = _geometry.Figures.LastOrDefault();
                                var bezier = figure.Segments.LastOrDefault() as XBezierSegment;
                                bezier.Point2 = point2;
                                _bezierPoint3 = point2;
                            }
                        }

                        _bezierPoint1 = _bezierPoint4;
                        _bezierPoint2 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _bezierPoint3 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _bezierPoint4 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _geometry.BezierTo(
                            _bezierPoint2,
                            _bezierPoint3,
                            _bezierPoint4,
                            _editor.Project.Options.DefaultIsStroked,
                            _editor.Project.Options.DefaultIsSmoothJoin);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Remove();
                        ToStateOne();
                        Move(null);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = State.One;
                    }
                    break;
            }
        }

        private void QBezierLeftDown(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case State.None:
                    {
                        _qbezierPoint1 = TryToGetConnectionPoint(sx, sy) ?? XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        if (!_isInitialized)
                        {
                            InitializeWorkingPath(_qbezierPoint1);
                        }
                        else
                        {
                            SetQBezieFirstPointFromLastSegment();
                        }

                        _qbezierPoint2 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _qbezierPoint3 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _geometry.QuadraticBezierTo(
                            _qbezierPoint2,
                            _qbezierPoint3,
                            _editor.Project.Options.DefaultIsStroked,
                            _editor.Project.Options.DefaultIsSmoothJoin);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateOne();
                        Move(null);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = State.One;
                        _editor.CancelAvailable = true;
                    }
                    break;
                case State.One:
                    {
                        _qbezierPoint3.X = sx;
                        _qbezierPoint3.Y = sy;
                        if (_editor.Project.Options.TryToConnect)
                        {
                            var point2 = TryToGetConnectionPoint(sx, sy);
                            if (point2 != null)
                            {
                                var figure = _geometry.Figures.LastOrDefault();
                                var qbezier = figure.Segments.LastOrDefault() as XQuadraticBezierSegment;
                                qbezier.Point2 = point2;
                                _qbezierPoint3 = point2;
                            }
                        }
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateTwo();
                        Move(null);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = State.Two;
                    }
                    break;
                case State.Two:
                    {
                        _qbezierPoint2.X = sx;
                        _qbezierPoint2.Y = sy;
                        if (_editor.Project.Options.TryToConnect)
                        {
                            var point1 = TryToGetConnectionPoint(sx, sy);
                            if (point1 != null)
                            {
                                var figure = _geometry.Figures.LastOrDefault();
                                var qbezier = figure.Segments.LastOrDefault() as XQuadraticBezierSegment;
                                qbezier.Point1 = point1;
                                _qbezierPoint2 = point1;
                            }
                        }

                        _qbezierPoint1 = _qbezierPoint3;
                        _qbezierPoint2 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _qbezierPoint3 = XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        _geometry.QuadraticBezierTo(
                            _qbezierPoint2,
                            _qbezierPoint3,
                            _editor.Project.Options.DefaultIsStroked,
                            _editor.Project.Options.DefaultIsSmoothJoin);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Remove();
                        ToStateOne();
                        Move(null);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = State.One;
                    }
                    break;
            }
        }

        private void LineRightDown(double x, double y)
        {
            switch (_currentState)
            {
                case State.None:
                    break;
                case State.One:
                    {
                        RemoveLastLineSegment();

                        _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_path);
                        Remove();
                        if (_path.Geometry.Figures.LastOrDefault().Segments.Count > 0)
                        {
                            Finalize(null);
                            _editor.AddWithHistory(_path);
                        }
                        else
                        {
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        }
                        DeInitializeWorkingPath();
                        _currentState = State.None;
                        _editor.CancelAvailable = false;
                    }
                    break;
            }
        }

        private void ArcRightDown(double x, double y)
        {
            // TODO: Add Arc path helper RightDown method implementation.
        }

        private void BezierRightDown(double x, double y)
        {
            switch (_currentState)
            {
                case State.None:
                    break;
                case State.One:
                case State.Two:
                case State.Three:
                    {
                        RemoveLastBezierSegment();

                        _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_path);
                        Remove();
                        if (_path.Geometry.Figures.LastOrDefault().Segments.Count > 0)
                        {
                            Finalize(null);
                            _editor.AddWithHistory(_path);
                        }
                        else
                        {
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        }
                        DeInitializeWorkingPath();
                        _currentState = State.None;
                        _editor.CancelAvailable = false;
                    }
                    break;
            }
        }

        private void QBezierRightDown(double x, double y)
        {
            switch (_currentState)
            {
                case State.None:
                    break;
                case State.One:
                case State.Two:
                    {
                        RemoveLastQBezierSegment();

                        _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_path);
                        Remove();
                        if (_path.Geometry.Figures.LastOrDefault().Segments.Count > 0)
                        {
                            Finalize(null);
                            _editor.AddWithHistory(_path);
                        }
                        else
                        {
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        }
                        DeInitializeWorkingPath();
                        _currentState = State.None;
                        _editor.CancelAvailable = false;
                    }
                    break;
            }
        }

        private void LineMove(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case State.None:
                    {
                        if (_editor.Project.Options.TryToConnect)
                        {
                            _editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
                case State.One:
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

        private void BezierMove(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case State.None:
                    {
                        if (_editor.Project.Options.TryToConnect)
                        {
                            _editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
                case State.One:
                    {
                        if (_editor.Project.Options.TryToConnect)
                        {
                            _editor.TryToHoverShape(sx, sy);
                        }
                        _bezierPoint2.X = sx;
                        _bezierPoint2.Y = sy;
                        _bezierPoint3.X = sx;
                        _bezierPoint3.Y = sy;
                        _bezierPoint4.X = sx;
                        _bezierPoint4.Y = sy;
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                    }
                    break;
                case State.Two:
                    {
                        if (_editor.Project.Options.TryToConnect)
                        {
                            _editor.TryToHoverShape(sx, sy);
                        }
                        _bezierPoint2.X = sx;
                        _bezierPoint2.Y = sy;
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                    }
                    break;
                case State.Three:
                    {
                        if (_editor.Project.Options.TryToConnect)
                        {
                            _editor.TryToHoverShape(sx, sy);
                        }
                        _bezierPoint3.X = sx;
                        _bezierPoint3.Y = sy;
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                    }
                    break;
            }
        }

        private void QBezierMove(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case State.None:
                    {
                        if (_editor.Project.Options.TryToConnect)
                        {
                            _editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
                case State.One:
                    {
                        if (_editor.Project.Options.TryToConnect)
                        {
                            _editor.TryToHoverShape(sx, sy);
                        }
                        _qbezierPoint2.X = sx;
                        _qbezierPoint2.Y = sy;
                        _qbezierPoint3.X = sx;
                        _qbezierPoint3.Y = sy;
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Move(null);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                    }
                    break;
                case State.Two:
                    {
                        if (_editor.Project.Options.TryToConnect)
                        {
                            _editor.TryToHoverShape(sx, sy);
                        }
                        _qbezierPoint2.X = sx;
                        _qbezierPoint2.Y = sy;
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
            _lineEllipseStart = XEllipse.Create(0, 0, _style, null, true, true);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_lineEllipseStart);
            _lineEllipseEnd = XEllipse.Create(0, 0, _style, null, true, true);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_lineEllipseEnd);
        }

        private void ToStateOneArc()
        {
            // TODO: Add Arc path helper ToStateOne method implementation.
        }

        private void ToStateOneBezier()
        {
            _style = _editor.Project.Options.HelperStyle;
            _bezierEllipsePoint1 = XEllipse.Create(0, 0, _style, null, true, true);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_bezierEllipsePoint1);
            _bezierEllipsePoint4 = XEllipse.Create(0, 0, _style, null, true, true);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_bezierEllipsePoint4);
        }

        private void ToStateOneQBezier()
        {
            _style = _editor.Project.Options.HelperStyle;
            _qbezierEllipsePoint1 = XEllipse.Create(0, 0, _style, null, true, true);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_qbezierEllipsePoint1);
            _qbezierEllipsePoint3 = XEllipse.Create(0, 0, _style, null, true, true);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_qbezierEllipsePoint3);
        }

        private void ToStateTwoArc()
        {
            // TODO: Add Arc path helper ToStateTwo method implementation.
        }

        private void ToStateTwoBezier()
        {
            _style = _editor.Project.Options.HelperStyle;
            _bezierLine12 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_bezierLine12);
            _bezierEllipsePoint2 = XEllipse.Create(0, 0, _style, null, true, true);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_bezierEllipsePoint2);
        }

        private void ToStateTwoQBezier()
        {
            _style = _editor.Project.Options.HelperStyle;
            _qbezierLine12 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_qbezierLine12);
            _qbezierLine32 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_qbezierLine32);
            _qbezierEllipsePoint2 = XEllipse.Create(0, 0, _style, null, true, true);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_qbezierEllipsePoint2);
        }

        private void ToStateThreeArc()
        {
            // TODO: Add Arc path helper ToStateThree method implementation.
        }

        private void ToStateThreeBezier()
        {
            _bezierLine43 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_bezierLine43);
            _bezierLine23 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_bezierLine23);
            _bezierEllipsePoint3 = XEllipse.Create(0, 0, _style, null, true, true);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_bezierEllipsePoint3);
        }

        private void MoveLineHelpers()
        {
            if (_lineEllipseStart != null)
            {
                _lineEllipseStart.TopLeft.X = _lineStart.X - _pointEllipseRadius;
                _lineEllipseStart.TopLeft.Y = _lineStart.Y - _pointEllipseRadius;
                _lineEllipseStart.BottomRight.X = _lineStart.X + _pointEllipseRadius;
                _lineEllipseStart.BottomRight.Y = _lineStart.Y + _pointEllipseRadius;
            }

            if (_lineEllipseEnd != null)
            {
                _lineEllipseEnd.TopLeft.X = _lineEnd.X - _pointEllipseRadius;
                _lineEllipseEnd.TopLeft.Y = _lineEnd.Y - _pointEllipseRadius;
                _lineEllipseEnd.BottomRight.X = _lineEnd.X + _pointEllipseRadius;
                _lineEllipseEnd.BottomRight.Y = _lineEnd.Y + _pointEllipseRadius;
            }
        }

        private void MoveArcHelpers()
        {
            // TODO: Add Arc path helper Move method implementation.
        }

        private void MoveBezierHelpers()
        {
            if (_bezierLine12 != null)
            {
                _bezierLine12.Start.X = _bezierPoint1.X;
                _bezierLine12.Start.Y = _bezierPoint1.Y;
                _bezierLine12.End.X = _bezierPoint2.X;
                _bezierLine12.End.Y = _bezierPoint2.Y;
            }

            if (_bezierLine43 != null)
            {
                _bezierLine43.Start.X = _bezierPoint4.X;
                _bezierLine43.Start.Y = _bezierPoint4.Y;
                _bezierLine43.End.X = _bezierPoint3.X;
                _bezierLine43.End.Y = _bezierPoint3.Y;
            }

            if (_bezierLine23 != null)
            {
                _bezierLine23.Start.X = _bezierPoint2.X;
                _bezierLine23.Start.Y = _bezierPoint2.Y;
                _bezierLine23.End.X = _bezierPoint3.X;
                _bezierLine23.End.Y = _bezierPoint3.Y;
            }

            if (_bezierEllipsePoint1 != null)
            {
                _bezierEllipsePoint1.TopLeft.X = _bezierPoint1.X - _pointEllipseRadius;
                _bezierEllipsePoint1.TopLeft.Y = _bezierPoint1.Y - _pointEllipseRadius;
                _bezierEllipsePoint1.BottomRight.X = _bezierPoint1.X + _pointEllipseRadius;
                _bezierEllipsePoint1.BottomRight.Y = _bezierPoint1.Y + _pointEllipseRadius;
            }

            if (_bezierEllipsePoint2 != null)
            {
                _bezierEllipsePoint2.TopLeft.X = _bezierPoint2.X - _pointEllipseRadius;
                _bezierEllipsePoint2.TopLeft.Y = _bezierPoint2.Y - _pointEllipseRadius;
                _bezierEllipsePoint2.BottomRight.X = _bezierPoint2.X + _pointEllipseRadius;
                _bezierEllipsePoint2.BottomRight.Y = _bezierPoint2.Y + _pointEllipseRadius;
            }

            if (_bezierEllipsePoint3 != null)
            {
                _bezierEllipsePoint3.TopLeft.X = _bezierPoint3.X - _pointEllipseRadius;
                _bezierEllipsePoint3.TopLeft.Y = _bezierPoint3.Y - _pointEllipseRadius;
                _bezierEllipsePoint3.BottomRight.X = _bezierPoint3.X + _pointEllipseRadius;
                _bezierEllipsePoint3.BottomRight.Y = _bezierPoint3.Y + _pointEllipseRadius;
            }

            if (_bezierEllipsePoint4 != null)
            {
                _bezierEllipsePoint4.TopLeft.X = _bezierPoint4.X - _pointEllipseRadius;
                _bezierEllipsePoint4.TopLeft.Y = _bezierPoint4.Y - _pointEllipseRadius;
                _bezierEllipsePoint4.BottomRight.X = _bezierPoint4.X + _pointEllipseRadius;
                _bezierEllipsePoint4.BottomRight.Y = _bezierPoint4.Y + _pointEllipseRadius;
            }
        }

        private void MoveQBezierHelpers()
        {
            if (_qbezierLine12 != null)
            {
                _qbezierLine12.Start.X = _qbezierPoint1.X;
                _qbezierLine12.Start.Y = _qbezierPoint1.Y;
                _qbezierLine12.End.X = _qbezierPoint2.X;
                _qbezierLine12.End.Y = _qbezierPoint2.Y;
            }

            if (_qbezierLine32 != null)
            {
                _qbezierLine32.Start.X = _qbezierPoint3.X;
                _qbezierLine32.Start.Y = _qbezierPoint3.Y;
                _qbezierLine32.End.X = _qbezierPoint2.X;
                _qbezierLine32.End.Y = _qbezierPoint2.Y;
            }

            if (_qbezierEllipsePoint1 != null)
            {
                _qbezierEllipsePoint1.TopLeft.X = _qbezierPoint1.X - _pointEllipseRadius;
                _qbezierEllipsePoint1.TopLeft.Y = _qbezierPoint1.Y - _pointEllipseRadius;
                _qbezierEllipsePoint1.BottomRight.X = _qbezierPoint1.X + _pointEllipseRadius;
                _qbezierEllipsePoint1.BottomRight.Y = _qbezierPoint1.Y + _pointEllipseRadius;
            }

            if (_qbezierEllipsePoint2 != null)
            {
                _qbezierEllipsePoint2.TopLeft.X = _qbezierPoint2.X - _pointEllipseRadius;
                _qbezierEllipsePoint2.TopLeft.Y = _qbezierPoint2.Y - _pointEllipseRadius;
                _qbezierEllipsePoint2.BottomRight.X = _qbezierPoint2.X + _pointEllipseRadius;
                _qbezierEllipsePoint2.BottomRight.Y = _qbezierPoint2.Y + _pointEllipseRadius;
            }

            if (_qbezierEllipsePoint3 != null)
            {
                _qbezierEllipsePoint3.TopLeft.X = _qbezierPoint3.X - _pointEllipseRadius;
                _qbezierEllipsePoint3.TopLeft.Y = _qbezierPoint3.Y - _pointEllipseRadius;
                _qbezierEllipsePoint3.BottomRight.X = _qbezierPoint3.X + _pointEllipseRadius;
                _qbezierEllipsePoint3.BottomRight.Y = _qbezierPoint3.Y + _pointEllipseRadius;
            }
        }

        private void RemoveLineHelpers()
        {
            if (_lineEllipseStart != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_lineEllipseStart);
                _lineEllipseStart = null;
            }

            if (_lineEllipseEnd != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_lineEllipseEnd);
                _lineEllipseEnd = null;
            }

            _style = null;
        }

        private void RemoveArcHelpers()
        {
            // TODO: Add Arc path helper Remove method implementation.
        }

        private void RemoveBezierHelpers()
        {
            if (_bezierLine12 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_bezierLine12);
                _bezierLine12 = null;
            }

            if (_bezierLine43 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_bezierLine43);
                _bezierLine43 = null;
            }

            if (_bezierLine23 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_bezierLine23);
                _bezierLine23 = null;
            }

            if (_bezierEllipsePoint1 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_bezierEllipsePoint1);
                _bezierEllipsePoint1 = null;
            }

            if (_bezierEllipsePoint2 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_bezierEllipsePoint2);
                _bezierEllipsePoint2 = null;
            }

            if (_bezierEllipsePoint3 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_bezierEllipsePoint3);
                _bezierEllipsePoint3 = null;
            }

            if (_bezierEllipsePoint4 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_bezierEllipsePoint4);
                _bezierEllipsePoint4 = null;
            }

            _style = null;
        }

        private void RemoveQBezierHelpers()
        {
            if (_qbezierLine12 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_qbezierLine12);
                _qbezierLine12 = null;
            }

            if (_qbezierLine32 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_qbezierLine32);
                _qbezierLine32 = null;
            }

            if (_qbezierEllipsePoint1 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_qbezierEllipsePoint1);
                _qbezierEllipsePoint1 = null;
            }

            if (_qbezierEllipsePoint2 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_qbezierEllipsePoint2);
                _qbezierEllipsePoint2 = null;
            }

            if (_qbezierEllipsePoint3 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_qbezierEllipsePoint3);
                _qbezierEllipsePoint3 = null;
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
                case PathTool.Bezier:
                    {
                        RemoveLastBezierSegment();
                        RemoveBezierHelpers();
                    }
                    break;
                case PathTool.QBezier:
                    {
                        RemoveLastQBezierSegment();
                        RemoveQBezierHelpers();
                    }
                    break;
            }

            _currentState = State.None;

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
                case PathTool.Bezier:
                    {
                        BezierLeftDown(x, y);
                    }
                    break;
                case PathTool.QBezier:
                    {
                        QBezierLeftDown(x, y);
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
            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;

            // start new figure
            var start = TryToGetConnectionPoint(sx, sy) ?? XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
            _geometry.BeginFigure(
                start,
                _editor.Project.Options.DefaultIsFilled,
                _editor.Project.Options.DefaultIsClosed);

            // switch to path tool before Move tool
            _editor.CurrentPathTool = _movePathTool;
            SwitchPathTool(x, y);
        }

        private void StartFigureMove(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case State.None:
                    {
                        if (_editor.Project.Options.TryToConnect)
                        {
                            _editor.TryToHoverShape(sx, sy);
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void LeftDown(double x, double y)
        {
            if (_isInitialized && _editor.CurrentPathTool != _previousPathTool)
            {
                SwitchPathTool(x, y);
                return;
            }

            switch(_editor.CurrentPathTool)
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
                case PathTool.Bezier:
                    {
                        BezierLeftDown(x, y);
                    }
                    break;
                case PathTool.QBezier:
                    {
                        QBezierLeftDown(x, y);
                    }
                    break;
                case PathTool.Move:
                    {
                        StartFigureLeftDown(x, y);
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void LeftUp(double x, double y)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void RightDown(double x, double y)
        {
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
                case PathTool.Bezier:
                    {
                        BezierRightDown(x, y);
                    }
                    break;
                case PathTool.QBezier:
                    {
                        QBezierRightDown(x, y);
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void RightUp(double x, double y)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void Move(double x, double y)
        {
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
                case PathTool.Bezier:
                    {
                        BezierMove(x, y);
                    }
                    break;
                case PathTool.QBezier:
                    {
                        QBezierMove(x, y);
                    }
                    break;
                case PathTool.Move:
                    {
                        StartFigureMove(x, y);
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ToStateOne()
        {
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
                case PathTool.Bezier:
                    {
                        ToStateOneBezier();
                    }
                    break;
                case PathTool.QBezier:
                    {
                        ToStateOneQBezier();
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ToStateTwo()
        {
            switch (_editor.CurrentPathTool)
            {
                case PathTool.Line:
                    break;
                case PathTool.Arc:
                    {
                        ToStateTwoArc();
                    }
                    break;
                case PathTool.Bezier:
                    {
                        ToStateTwoBezier();
                    }
                    break;
                case PathTool.QBezier:
                    {
                        ToStateTwoQBezier();
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ToStateThree()
        {
            switch (_editor.CurrentPathTool)
            {
                case PathTool.Line:
                    break;
                case PathTool.Arc:
                    {
                        ToStateThreeArc();
                    }
                    break;
                case PathTool.Bezier:
                    {
                        ToStateThreeBezier();
                    }
                    break;
                case PathTool.QBezier:
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ToStateFour()
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        public override void Move(BaseShape shape)
        {
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
                case PathTool.Bezier:
                    {
                        MoveBezierHelpers();
                    }
                    break;
                case PathTool.QBezier:
                    {
                        MoveQBezierHelpers();
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        public override void Finalize(BaseShape shape)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Remove()
        {
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
                case PathTool.Bezier:
                    {
                        RemoveBezierHelpers();
                    }
                    break;
                case PathTool.QBezier:
                    {
                        RemoveQBezierHelpers();
                    }
                    break;
            }
        }
    }
}
