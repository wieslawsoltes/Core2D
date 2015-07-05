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

        private XPath _path;
        private XPathGeometry _geometry;

        private ShapeStyle _style;
        private double _pointEllipseRadius = 3.0;

        private XPoint _lineStart;
        private XPoint _lineEnd;

        private XEllipse _ellipseStart;
        private XEllipse _ellipseEnd;

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
        public XPoint TryToConnect(double x, double y)
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="start"></param>
        private void Initialize(XPoint start)
        {
            _geometry = XPathGeometry.Create(
                new List<XPathFigure>(),
                _editor.Project.Options.DefaultFillRule,
                XPathRect.Create());

            _geometry.BeginFigure(
                start, 
                _editor.Project.Options.DefaultIsFilled, 
                _editor.Project.Options.DefaultIsClosed);

            _path = XPath.Create(
                "Path",
                _editor.Project.CurrentStyleLibrary.CurrentStyle,
                null,
                _geometry,
                ShapeTransform.Create(),
                _editor.Project.Options.DefaultIsStroked,
                _editor.Project.Options.DefaultIsFilled);

            _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_path);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void LeftDown(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch(_editor.CurrentPathTool)
            {
                case PathTool.Line:
                    {
                        switch (_currentState)
                        {
                            case State.None:
                                {
                                    _lineStart = TryToConnect(sx, sy) ?? XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                                    Initialize(_lineStart);

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
                                }
                                break;
                            case State.One:
                                {
                                    _lineEnd.X = sx;
                                    _lineEnd.Y = sy;
                                    if (_editor.Project.Options.TryToConnect)
                                    {
                                        var end = TryToConnect(sx, sy);
                                        if (end != null)
                                        {
                                            var figure = _geometry.Figures.Last();
                                            var line = figure.Segments.Last() as XLineSegment;
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
                    break;
                case PathTool.Arc:
                    {
                        // TODO:
                    }
                    break;
                case PathTool.Bezier:
                    {
                        // TODO:
                    }
                    break;
                case PathTool.QBezier:
                    {
                        // TODO:
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
            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void RightDown(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_editor.CurrentPathTool)
            {
                case PathTool.Line:
                    {
                        switch (_currentState)
                        {
                            case State.None:
                                break;
                            case State.One:
                                {
                                    var figure = _geometry.Figures.Last();
                                    var line = figure.Segments.Last() as XLineSegment;
                                    figure.Segments.Remove(line);

                                    _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_path);
                                    Remove();
                                    Finalize(null);
                                    _editor.AddWithHistory(_path);
                                    _currentState = State.None;
                                }
                                break;
                        }
                    }
                    break;
                case PathTool.Arc:
                    {
                        // TODO:
                    }
                    break;
                case PathTool.Bezier:
                    {
                        // TODO:
                    }
                    break;
                case PathTool.QBezier:
                    {
                        // TODO:
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
            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void Move(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_editor.CurrentPathTool)
            {
                case PathTool.Line:
                    {
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
                                        if (_editor.TryToHoverShape(sx, sy))
                                        {
                                            if (_ellipseEnd != null)
                                            {
                                                _ellipseEnd.State &= ~ShapeState.Visible;
                                            }
                                        }
                                        else
                                        {
                                            if (_ellipseEnd != null)
                                            {
                                                _ellipseEnd.State |= ShapeState.Visible;
                                            }
                                        }
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
                    break;
                case PathTool.Arc:
                    {
                        // TODO:
                    }
                    break;
                case PathTool.Bezier:
                    {
                        // TODO:
                    }
                    break;
                case PathTool.QBezier:
                    {
                        // TODO:
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
                        _style = _editor.Project.Options.HelperStyle;
                        _ellipseStart = XEllipse.Create(0, 0, _style, null, true);
                        _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_ellipseStart);
                        _ellipseEnd = XEllipse.Create(0, 0, _style, null, true);
                        _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_ellipseEnd);
                    }
                    break;
                case PathTool.Arc:
                    {
                        // TODO:
                    }
                    break;
                case PathTool.Bezier:
                    {
                        // TODO:
                    }
                    break;
                case PathTool.QBezier:
                    {
                        // TODO:
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
                    {
                        // TODO:
                    }
                    break;
                case PathTool.Arc:
                    {
                        // TODO:
                    }
                    break;
                case PathTool.Bezier:
                    {
                        // TODO:
                    }
                    break;
                case PathTool.QBezier:
                    {
                        // TODO:
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
                    {
                        // TODO:
                    }
                    break;
                case PathTool.Arc:
                    {
                        // TODO:
                    }
                    break;
                case PathTool.Bezier:
                    {
                        // TODO:
                    }
                    break;
                case PathTool.QBezier:
                    {
                        // TODO:
                    }
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
                        if (_ellipseStart != null)
                        {
                            _ellipseStart.TopLeft.X = _lineStart.X - _pointEllipseRadius;
                            _ellipseStart.TopLeft.Y = _lineStart.Y - _pointEllipseRadius;
                            _ellipseStart.BottomRight.X = _lineStart.X + _pointEllipseRadius;
                            _ellipseStart.BottomRight.Y = _lineStart.Y + _pointEllipseRadius;
                        }

                        if (_ellipseEnd != null)
                        {
                            _ellipseEnd.TopLeft.X = _lineEnd.X - _pointEllipseRadius;
                            _ellipseEnd.TopLeft.Y = _lineEnd.Y - _pointEllipseRadius;
                            _ellipseEnd.BottomRight.X = _lineEnd.X + _pointEllipseRadius;
                            _ellipseEnd.BottomRight.Y = _lineEnd.Y + _pointEllipseRadius;
                        }
                    }
                    break;
                case PathTool.Arc:
                    {
                        // TODO:
                    }
                    break;
                case PathTool.Bezier:
                    {
                        // TODO:
                    }
                    break;
                case PathTool.QBezier:
                    {
                        // TODO:
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
                        if (_ellipseStart != null)
                        {
                            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_ellipseStart);
                            _ellipseStart = null;
                        }

                        if (_ellipseEnd != null)
                        {
                            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_ellipseEnd);
                            _ellipseEnd = null;
                        }
                    }
                    break;
                case PathTool.Arc:
                    {
                        // TODO:
                    }
                    break;
                case PathTool.Bezier:
                    {
                        // TODO:
                    }
                    break;
                case PathTool.QBezier:
                    {
                        // TODO:
                    }
                    break;
            }
        }
    }
}
