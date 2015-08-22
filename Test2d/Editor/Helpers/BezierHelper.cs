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
    public class BezierHelper : Helper
    {
        private Editor _editor;
        private State _currentState = State.None;
        private XBezier _shape;

        private ShapeStyle _style;
        private XLine _line12;
        private XLine _line43;
        private XLine _line23;
        private double _pointEllipseRadius = 3.0;
        private XEllipse _ellipsePoint1;
        private XEllipse _ellipsePoint2;
        private XEllipse _ellipsePoint3;
        private XEllipse _ellipsePoint4;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="editor"></param>
        public BezierHelper(Editor editor)
        {
            _editor = editor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bezier"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectPoint1(XBezier bezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(_editor.Project.CurrentContainer, new Vector2(x, y), _editor.Project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                bezier.Point1 = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bezier"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectPoint2(XBezier bezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(_editor.Project.CurrentContainer, new Vector2(x, y), _editor.Project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                bezier.Point2 = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bezier"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectPoint3(XBezier bezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(_editor.Project.CurrentContainer, new Vector2(x, y), _editor.Project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                bezier.Point3 = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bezier"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectPoint4(XBezier bezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(_editor.Project.CurrentContainer, new Vector2(x, y), _editor.Project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                bezier.Point4 = result as XPoint;
            }
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
                        _shape = XBezier.Create(
                            sx, sy,
                            _editor.Project.CurrentStyleLibrary.CurrentStyle,
                            _editor.Project.Options.PointShape,
                            _editor.Project.Options.DefaultIsStroked,
                            _editor.Project.Options.DefaultIsFilled);
                        if (_editor.Project.Options.TryToConnect)
                        {
                            TryToConnectPoint1(_shape as XBezier, sx, sy);
                        }
                        _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_shape);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateOne();
                        Move(_shape as XBezier);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = State.One;
                        _editor.CancelAvailable = true;
                    }
                    break;
                case State.One:
                    {
                        var bezier = _shape as XBezier;
                        if (bezier != null)
                        {
                            bezier.Point3.X = sx;
                            bezier.Point3.Y = sy;
                            bezier.Point4.X = sx;
                            bezier.Point4.Y = sy;
                            if (_editor.Project.Options.TryToConnect)
                            {
                                TryToConnectPoint4(_shape as XBezier, sx, sy);
                            }
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            ToStateTwo();
                            Move(_shape as XBezier);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                            _currentState = State.Two;
                        }
                    }
                    break;
                case State.Two:
                    {
                        var bezier = _shape as XBezier;
                        if (bezier != null)
                        {
                            bezier.Point2.X = sx;
                            bezier.Point2.Y = sy;
                            if (_editor.Project.Options.TryToConnect)
                            {
                                TryToConnectPoint2(_shape as XBezier, sx, sy);
                            }
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            ToStateThree();
                            Move(_shape as XBezier);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                            _currentState = State.Three;
                        }
                    }
                    break;
                case State.Three:
                    {
                        var bezier = _shape as XBezier;
                        if (bezier != null)
                        {
                            bezier.Point3.X = sx;
                            bezier.Point3.Y = sy;
                            if (_editor.Project.Options.TryToConnect)
                            {
                                TryToConnectPoint3(_shape as XBezier, sx, sy);
                            }
                            _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                            Remove();
                            Finalize(_shape as XBezier);
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
                        var bezier = _shape as XBezier;
                        if (bezier != null)
                        {
                            if (_editor.Project.Options.TryToConnect)
                            {
                                _editor.TryToHoverShape(sx, sy);
                            }
                            bezier.Point2.X = sx;
                            bezier.Point2.Y = sy;
                            bezier.Point3.X = sx;
                            bezier.Point3.Y = sy;
                            bezier.Point4.X = sx;
                            bezier.Point4.Y = sy;
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_shape as XBezier);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        }
                    }
                    break;
                case State.Two:
                    {
                        var bezier = _shape as XBezier;
                        if (bezier != null)
                        {
                            if (_editor.Project.Options.TryToConnect)
                            {
                                _editor.TryToHoverShape(sx, sy);
                            }
                            bezier.Point2.X = sx;
                            bezier.Point2.Y = sy;
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_shape as XBezier);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        }
                    }
                    break;
                case State.Three:
                    {
                        var bezier = _shape as XBezier;
                        if (bezier != null)
                        {
                            if (_editor.Project.Options.TryToConnect)
                            {
                                _editor.TryToHoverShape(sx, sy);
                            }
                            bezier.Point3.X = sx;
                            bezier.Point3.Y = sy;
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_shape as XBezier);
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
            _ellipsePoint1 = XEllipse.Create(0, 0, _style, null, true, true);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_ellipsePoint1);
            _ellipsePoint4 = XEllipse.Create(0, 0, _style, null, true, true);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_ellipsePoint4);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public override void ToStateTwo()
        {
            _style = _editor.Project.Options.HelperStyle;
            _line12 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_line12);
            _ellipsePoint2 = XEllipse.Create(0, 0, _style, null, true, true);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_ellipsePoint2);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ToStateThree()
        {
            _line43 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_line43);
            _line23 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_line23);
            _ellipsePoint3 = XEllipse.Create(0, 0, _style, null, true, true);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_ellipsePoint3);
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
            var bezier = shape as XBezier;
            
            if (_line12 != null)
            {
                _line12.Start.X = bezier.Point1.X;
                _line12.Start.Y = bezier.Point1.Y;
                _line12.End.X = bezier.Point2.X;
                _line12.End.Y = bezier.Point2.Y;
            }

            if (_line43 != null)
            {
                _line43.Start.X = bezier.Point4.X;
                _line43.Start.Y = bezier.Point4.Y;
                _line43.End.X = bezier.Point3.X;
                _line43.End.Y = bezier.Point3.Y;
            }
            
            if (_line23 != null)
            {
                _line23.Start.X = bezier.Point2.X;
                _line23.Start.Y = bezier.Point2.Y;
                _line23.End.X = bezier.Point3.X;
                _line23.End.Y = bezier.Point3.Y;
            }
 
            if (_ellipsePoint1 != null)
            {
                _ellipsePoint1.TopLeft.X = bezier.Point1.X - _pointEllipseRadius;
                _ellipsePoint1.TopLeft.Y = bezier.Point1.Y - _pointEllipseRadius;
                _ellipsePoint1.BottomRight.X = bezier.Point1.X + _pointEllipseRadius;
                _ellipsePoint1.BottomRight.Y = bezier.Point1.Y + _pointEllipseRadius;
            }
            
            if (_ellipsePoint2 != null)
            {
                _ellipsePoint2.TopLeft.X = bezier.Point2.X - _pointEllipseRadius;
                _ellipsePoint2.TopLeft.Y = bezier.Point2.Y - _pointEllipseRadius;
                _ellipsePoint2.BottomRight.X = bezier.Point2.X + _pointEllipseRadius;
                _ellipsePoint2.BottomRight.Y = bezier.Point2.Y + _pointEllipseRadius;
            }
            
            if (_ellipsePoint3 != null)
            {
                _ellipsePoint3.TopLeft.X = bezier.Point3.X - _pointEllipseRadius;
                _ellipsePoint3.TopLeft.Y = bezier.Point3.Y - _pointEllipseRadius;
                _ellipsePoint3.BottomRight.X = bezier.Point3.X + _pointEllipseRadius;
                _ellipsePoint3.BottomRight.Y = bezier.Point3.Y + _pointEllipseRadius;
            }
            
            if (_ellipsePoint4 != null)
            {
                _ellipsePoint4.TopLeft.X = bezier.Point4.X - _pointEllipseRadius;
                _ellipsePoint4.TopLeft.Y = bezier.Point4.Y - _pointEllipseRadius;
                _ellipsePoint4.BottomRight.X = bezier.Point4.X + _pointEllipseRadius;
                _ellipsePoint4.BottomRight.Y = bezier.Point4.Y + _pointEllipseRadius;
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
            if (_line12 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_line12);
                _line12 = null;
            }

            if (_line43 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_line43);
                _line43 = null;
            }
            
            if (_line23 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_line23);
                _line23 = null;
            }
            
            if (_ellipsePoint1 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_ellipsePoint1);
                _ellipsePoint1 = null;
            }
            
            if (_ellipsePoint2 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_ellipsePoint2);
                _ellipsePoint2 = null;
            }
   
            if (_ellipsePoint3 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_ellipsePoint3);
                _ellipsePoint3 = null;
            }
            
            if (_ellipsePoint4 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_ellipsePoint4);
                _ellipsePoint4 = null;
            }

            _style = null;
        }
    }
}
