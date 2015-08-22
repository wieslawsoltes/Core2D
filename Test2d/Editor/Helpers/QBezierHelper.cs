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
    public class QBezierHelper : Helper
    {
        private Editor _editor;
        private State _currentState = State.None;
        private XQBezier _shape;

        private ShapeStyle _style;
        private XLine _line12;
        private XLine _line32;
        private double _pointEllipseRadius = 3.0;
        private XEllipse _ellipsePoint1;
        private XEllipse _ellipsePoint2;
        private XEllipse _ellipsePoint3;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="editor"></param>
        public QBezierHelper(Editor editor)
        {
            _editor = editor;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qbezier"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectPoint1(XQBezier qbezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(_editor.Project.CurrentContainer, new Vector2(x, y), _editor.Project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                qbezier.Point1 = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qbezier"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectPoint2(XQBezier qbezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(_editor.Project.CurrentContainer, new Vector2(x, y), _editor.Project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                qbezier.Point2 = result as XPoint;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="qbezier"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void TryToConnectPoint3(XQBezier qbezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(_editor.Project.CurrentContainer, new Vector2(x, y), _editor.Project.Options.HitTreshold);
            if (result != null && result is XPoint)
            {
                qbezier.Point3 = result as XPoint;
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
                        _shape = XQBezier.Create(
                            sx, sy,
                            _editor.Project.CurrentStyleLibrary.CurrentStyle,
                            _editor.Project.Options.PointShape,
                            _editor.Project.Options.DefaultIsStroked,
                            _editor.Project.Options.DefaultIsFilled);
                        if (_editor.Project.Options.TryToConnect)
                        {
                            TryToConnectPoint1(_shape as XQBezier, sx, sy);
                        }
                        _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_shape);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateOne();
                        Move(_shape as XQBezier);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = State.One;
                        _editor.CancelAvailable = true;
                    }
                    break;
                case State.One:
                    {
                        var qbezier = _shape as XQBezier;
                        if (qbezier != null)
                        {
                            qbezier.Point2.X = sx;
                            qbezier.Point2.Y = sy;
                            qbezier.Point3.X = sx;
                            qbezier.Point3.Y = sy;
                            if (_editor.Project.Options.TryToConnect)
                            {
                                TryToConnectPoint3(_shape as XQBezier, sx, sy);
                            }
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            ToStateTwo();
                            Move(_shape as XQBezier);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                            _currentState = State.Two;
                        }
                    }
                    break;
                case State.Two:
                    {
                        var qbezier = _shape as XQBezier;
                        if (qbezier != null)
                        {
                            qbezier.Point2.X = sx;
                            qbezier.Point2.Y = sy;
                            if (_editor.Project.Options.TryToConnect)
                            {
                                TryToConnectPoint2(_shape as XQBezier, sx, sy);
                            }
                            _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                            Remove();
                            Finalize(_shape as XQBezier);
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
                        var qbezier = _shape as XQBezier;
                        if (qbezier != null)
                        {
                            if (_editor.Project.Options.TryToConnect)
                            {
                                _editor.TryToHoverShape(sx, sy);
                            }
                            qbezier.Point2.X = sx;
                            qbezier.Point2.Y = sy;
                            qbezier.Point3.X = sx;
                            qbezier.Point3.Y = sy;
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_shape as XQBezier);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        }
                    }
                    break;
                case State.Two:
                    {
                        var qbezier = _shape as XQBezier;
                        if (qbezier != null)
                        {
                            if (_editor.Project.Options.TryToConnect)
                            {
                                _editor.TryToHoverShape(sx, sy);
                            }
                            qbezier.Point2.X = sx;
                            qbezier.Point2.Y = sy;
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_shape as XQBezier);
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
            _ellipsePoint3 = XEllipse.Create(0, 0, _style, null, true, true);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_ellipsePoint3);
        }
        
        /// <summary>
        /// 
        /// </summary>
        public override void ToStateTwo()
        {
            _style = _editor.Project.Options.HelperStyle;
            _line12 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_line12);
            _line32 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_line32);
            _ellipsePoint2 = XEllipse.Create(0, 0, _style, null, true, true);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_ellipsePoint2);
        }

        /// <summary>
        /// 
        /// </summary>
        public override void ToStateThree()
        {
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
            var qbezier = shape as XQBezier;
            
            if (_line12 != null)
            {
                _line12.Start.X = qbezier.Point1.X;
                _line12.Start.Y = qbezier.Point1.Y;
                _line12.End.X = qbezier.Point2.X;
                _line12.End.Y = qbezier.Point2.Y;
            }

            if (_line32 != null)
            {
                _line32.Start.X = qbezier.Point3.X;
                _line32.Start.Y = qbezier.Point3.Y;
                _line32.End.X = qbezier.Point2.X;
                _line32.End.Y = qbezier.Point2.Y;
            }

            if (_ellipsePoint1 != null)
            {
                _ellipsePoint1.TopLeft.X = qbezier.Point1.X - _pointEllipseRadius;
                _ellipsePoint1.TopLeft.Y = qbezier.Point1.Y - _pointEllipseRadius;
                _ellipsePoint1.BottomRight.X = qbezier.Point1.X + _pointEllipseRadius;
                _ellipsePoint1.BottomRight.Y = qbezier.Point1.Y + _pointEllipseRadius;
            }
            
            if (_ellipsePoint2 != null)
            {
                _ellipsePoint2.TopLeft.X = qbezier.Point2.X - _pointEllipseRadius;
                _ellipsePoint2.TopLeft.Y = qbezier.Point2.Y - _pointEllipseRadius;
                _ellipsePoint2.BottomRight.X = qbezier.Point2.X + _pointEllipseRadius;
                _ellipsePoint2.BottomRight.Y = qbezier.Point2.Y + _pointEllipseRadius;
            }
            
            if (_ellipsePoint3 != null)
            {
                _ellipsePoint3.TopLeft.X = qbezier.Point3.X - _pointEllipseRadius;
                _ellipsePoint3.TopLeft.Y = qbezier.Point3.Y - _pointEllipseRadius;
                _ellipsePoint3.BottomRight.X = qbezier.Point3.X + _pointEllipseRadius;
                _ellipsePoint3.BottomRight.Y = qbezier.Point3.Y + _pointEllipseRadius;
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

            if (_line32 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_line32);
                _line32 = null;
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

            _style = null;
        }
    }
}
