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
    public class ArcHelper : Helper
    {
        private Editor _editor;
        private State _currentState = State.None;
        private XArc _shape;
        
        private ShapeStyle _style;
        private XLine _startLine;
        private XLine _endLine;
        private XEllipse _ellipse;
        private double _pointEllipseRadius = 3.0;
        private XEllipse _ellipseP1;
        private XEllipse _ellipseP2;
        private XEllipse _ellipseCenter;
        private XEllipse _ellipseStart;
        private XEllipse _ellipseEnd;
        private bool _connectedP3;
        private bool _connectedP4;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="editor"></param>
        public ArcHelper(Editor editor)
        {
            _editor = editor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool TryToConnectPoint1(XArc arc, double x, double y)
        {
            var result = ShapeBounds.HitTest(_editor.Project.CurrentContainer, new Vector2(x, y), _editor.Project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                arc.Point1 = result as XPoint;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool TryToConnectPoint2(XArc arc, double x, double y)
        {
            var result = ShapeBounds.HitTest(_editor.Project.CurrentContainer, new Vector2(x, y), _editor.Project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                arc.Point2 = result as XPoint;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool TryToConnectPoint3(XArc arc, double x, double y)
        {
            var result = ShapeBounds.HitTest(_editor.Project.CurrentContainer, new Vector2(x, y), _editor.Project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                arc.Point3 = result as XPoint;
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="arc"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public bool TryToConnectPoint4(XArc arc, double x, double y)
        {
            var result = ShapeBounds.HitTest(_editor.Project.CurrentContainer, new Vector2(x, y), _editor.Project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                arc.Point4 = result as XPoint;
                return true;
            }
            return false;
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
            switch (_currentState)
            {
                case State.None:
                    {
                        _connectedP3 = false;
                        _connectedP4 = false;
                        _shape = XArc.Create(
                            sx, sy,
                            _editor.Project.CurrentStyleLibrary.CurrentStyle,
                            _editor.Project.Options.PointShape,
                            _editor.Project.Options.DefaultIsStroked,
                            _editor.Project.Options.DefaultIsFilled);
                        if (_editor.Project.Options.TryToConnect)
                        {
                            TryToConnectPoint1(_shape, sx, sy);
                        }
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateOne();
                        Move(_shape);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = State.One;
                        _editor.CancelAvailable = true;
                    }
                    break;
                case State.One:
                    {
                        if (_shape != null)
                        {
                            _shape.Point2.X = sx;
                            _shape.Point2.Y = sy;
                            _shape.Point3.X = sx;
                            _shape.Point3.Y = sy;
                            if (_editor.Project.Options.TryToConnect)
                            {
                                TryToConnectPoint2(_shape, sx, sy);
                            }
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            ToStateTwo();
                            Move(_shape);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                            _currentState = State.Two;
                        }
                    }
                    break;
                case State.Two:
                    {
                        if (_shape != null)
                        {
                            _shape.Point3.X = sx;
                            _shape.Point3.Y = sy;
                            _shape.Point4.X = sx;
                            _shape.Point4.Y = sy;
                            if (_editor.Project.Options.TryToConnect)
                            {
                                _connectedP3 = TryToConnectPoint3(_shape, sx, sy);
                            }
                            _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_shape);
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            ToStateThree();
                            Move(_shape);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                            _currentState = State.Three;
                        }
                    }
                    break;
                case State.Three:
                    {
                        if (_shape != null)
                        {
                            _shape.Point4.X = sx;
                            _shape.Point4.Y = sy;
                            if (_editor.Project.Options.TryToConnect)
                            {
                                _connectedP4 = TryToConnectPoint4(_shape, sx, sy);
                            }
                            _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                            Remove();
                            Finalize(_shape);
                            _editor.AddWithHistory(_shape);
                            _currentState = State.None;
                            _editor.CancelAvailable = false;
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
            switch (_currentState)
            {
                case State.None:
                    break;
                case State.One:
                case State.Two:
                case State.Three:
                    {
                        _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Remove();
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = State.None;
                        _editor.CancelAvailable = false;
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
                        if (_shape != null)
                        {
                            if (_editor.Project.Options.TryToConnect)
                            {
                                _editor.TryToHoverShape(sx, sy);
                            }
                            _shape.Point2.X = sx;
                            _shape.Point2.Y = sy;
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_shape);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        }
                    }
                    break;
                case State.Two:
                    {
                        if (_shape != null)
                        {
                            if (_editor.Project.Options.TryToConnect)
                            {
                                _editor.TryToHoverShape(sx, sy);
                            }
                            _shape.Point3.X = sx;
                            _shape.Point3.Y = sy;
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_shape);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        }
                    }
                    break;
                case State.Three:
                    {
                        if (_shape != null)
                        {
                            if (_editor.Project.Options.TryToConnect)
                            {
                                _editor.TryToHoverShape(sx, sy);
                            }
                            _shape.Point4.X = sx;
                            _shape.Point4.Y = sy;
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_shape);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        }
                    }
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ToStateOne()
        {
            _style = _editor.Project.Options.HelperStyle;
            _ellipse = XEllipse.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_ellipse);
            _ellipseP1 = XEllipse.Create(0, 0, _style, null, true, true);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_ellipseP1);
            _ellipseP2 = XEllipse.Create(0, 0, _style, null, true, true);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_ellipseP2);
            _ellipseCenter = XEllipse.Create(0, 0, _style, null, true, true);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_ellipseCenter);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ToStateTwo()
        {
            if (_ellipseP1 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_ellipseP1);
                _ellipseP1 = null;
            }
            
            if (_ellipseP2 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_ellipseP2);
                _ellipseP2 = null;
            }
            
            _startLine = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_startLine);
            _ellipseStart = XEllipse.Create(0, 0, _style, null, true, true);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_ellipseStart);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ToStateThree()
        {
            if (_ellipse != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_ellipse);
                _ellipse = null;
            }
            
            _endLine = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_endLine);
            _ellipseEnd = XEllipse.Create(0, 0, _style, null, true, true);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_ellipseEnd);
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
            var arc = shape as XArc;
            var a = WpfArc.FromXArc(arc, 0, 0);

            if (_ellipse != null)
            {
                _ellipse.TopLeft.X = a.P1.X;
                _ellipse.TopLeft.Y = a.P1.Y;
                _ellipse.BottomRight.X = a.P2.X;
                _ellipse.BottomRight.Y = a.P2.Y;
            }

            if (_startLine != null)
            {
                _startLine.Start.X = a.Center.X;
                _startLine.Start.Y = a.Center.Y;
                _startLine.End.X = a.Start.X;
                _startLine.End.Y = a.Start.Y;
            }

            if (_endLine != null)
            {
                _endLine.Start.X = a.Center.X;
                _endLine.Start.Y = a.Center.Y;
                _endLine.End.X = a.End.X;
                _endLine.End.Y = a.End.Y;
            }
            
            if (_ellipseP1 != null)
            {
                _ellipseP1.TopLeft.X = a.P1.X - _pointEllipseRadius;
                _ellipseP1.TopLeft.Y = a.P1.Y - _pointEllipseRadius;
                _ellipseP1.BottomRight.X = a.P1.X + _pointEllipseRadius;
                _ellipseP1.BottomRight.Y = a.P1.Y + _pointEllipseRadius;
            }
            
            if (_ellipseP2 != null)
            {
                _ellipseP2.TopLeft.X = a.P2.X - _pointEllipseRadius;
                _ellipseP2.TopLeft.Y = a.P2.Y - _pointEllipseRadius;
                _ellipseP2.BottomRight.X = a.P2.X + _pointEllipseRadius;
                _ellipseP2.BottomRight.Y = a.P2.Y + _pointEllipseRadius;
            }
            
            if (_ellipseCenter != null)
            {
                _ellipseCenter.TopLeft.X = a.Center.X - _pointEllipseRadius;
                _ellipseCenter.TopLeft.Y = a.Center.Y - _pointEllipseRadius;
                _ellipseCenter.BottomRight.X = a.Center.X + _pointEllipseRadius;
                _ellipseCenter.BottomRight.Y = a.Center.Y + _pointEllipseRadius;
            }
            
            if (_ellipseStart != null)
            {
                _ellipseStart.TopLeft.X = a.Start.X - _pointEllipseRadius;
                _ellipseStart.TopLeft.Y = a.Start.Y - _pointEllipseRadius;
                _ellipseStart.BottomRight.X = a.Start.X + _pointEllipseRadius;
                _ellipseStart.BottomRight.Y = a.Start.Y + _pointEllipseRadius;
            }
            
            if (_ellipseEnd != null)
            {
                _ellipseEnd.TopLeft.X = a.End.X - _pointEllipseRadius;
                _ellipseEnd.TopLeft.Y = a.End.Y - _pointEllipseRadius;
                _ellipseEnd.BottomRight.X = a.End.X + _pointEllipseRadius;
                _ellipseEnd.BottomRight.Y = a.End.Y + _pointEllipseRadius;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shape"></param>
        public override void Finalize(BaseShape shape)
        {
            var arc = shape as XArc;
            var a = WpfArc.FromXArc(arc, 0, 0);

            if (!_connectedP3)
            {
                arc.Point3.X = a.Start.X;
                arc.Point3.Y = a.Start.Y;
            }

            if (!_connectedP4)
            {
                arc.Point4.X = a.End.X;
                arc.Point4.Y = a.End.Y;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public override void Remove()
        {
            if (_ellipse != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_ellipse);
                _ellipse = null;
            }

            if (_startLine != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_startLine);
                _startLine = null;
            }

            if (_endLine != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_endLine);
                _endLine = null;
            }

            if (_ellipseP1 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_ellipseP1);
                _ellipseP1 = null;
            }
            
            if (_ellipseP2 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_ellipseP2);
                _ellipseP2 = null;
            }
            
            if (_ellipseCenter != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_ellipseCenter);
                _ellipseCenter = null;
            }
            
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
            
            _style = null;
        }
    }
}
