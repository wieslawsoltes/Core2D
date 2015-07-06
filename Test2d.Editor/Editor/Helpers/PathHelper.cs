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

        private XPoint _lineStart;
        private XPoint _lineEnd;

        private ShapeStyle _style;
        private double _pointEllipseRadius = 3.0;

        private XEllipse _lineEllipseStart;
        private XEllipse _lineEllipseEnd;

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
        private void InitializePath(XPoint start)
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
        private void LeftDownLine(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case State.None:
                    {
                        _lineStart = TryToConnect(sx, sy) ?? XPoint.Create(sx, sy, _editor.Project.Options.PointShape);
                        InitializePath(_lineStart);

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void RightDownLine(double x, double y)
        {
            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        private void MoveLine(double x, double y)
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
                            if (_editor.TryToHoverShape(sx, sy))
                            {
                                if (_lineEllipseEnd != null)
                                {
                                    _lineEllipseEnd.State &= ~ShapeState.Visible;
                                }
                            }
                            else
                            {
                                if (_lineEllipseEnd != null)
                                {
                                    _lineEllipseEnd.State |= ShapeState.Visible;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void LeftDown(double x, double y)
        {
            switch(_editor.CurrentPathTool)
            {
                case PathTool.Line:
                    {
                        LeftDownLine(x, y);
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
                        RightDownLine(x, y);
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
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public override void Move(double x, double y)
        {
            switch (_editor.CurrentPathTool)
            {
                case PathTool.Line:
                    {
                        MoveLine(x, y);
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
                        _lineEllipseStart = XEllipse.Create(0, 0, _style, null, true);
                        _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_lineEllipseStart);
                        _lineEllipseEnd = XEllipse.Create(0, 0, _style, null, true);
                        _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_lineEllipseEnd);
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
