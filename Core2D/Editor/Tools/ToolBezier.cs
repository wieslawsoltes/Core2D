// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

namespace Core2D
{
    /// <summary>
    /// Helper class for <see cref="Tool.Bezier"/> editor.
    /// </summary>
    public class ToolBezier : ToolBase
    {
        private Editor _editor;
        private ToolState _currentState = ToolState.None;
        private XBezier _shape;
        private ShapeStyle _style;
        private XLine _line12;
        private XLine _line43;
        private XLine _line23;
        private XPoint _helperPoint1;
        private XPoint _helperPoint2;
        private XPoint _helperPoint3;
        private XPoint _helperPoint4;

        /// <summary>
        /// Initialize new instance of <see cref="ToolBezier"/> class.
        /// </summary>
        /// <param name="editor">The current <see cref="Editor"/> object.</param>
        public ToolBezier(Editor editor)
            : base()
        {
            _editor = editor;
        }

        /// <summary>
        /// Try to connect <see cref="XBezier.Point1"/> point at specified location.
        /// </summary>
        /// <param name="bezier">The bezier object.</param>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <returns>True if connected.</returns>
        public void TryToConnectPoint1(XBezier bezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(_editor.Project.CurrentContainer, new Vector2(x, y), _editor.Project.Options.HitThreshold);
            if (result != null && result is XPoint)
            {
                bezier.Point1 = result as XPoint;
            }
        }

        /// <summary>
        /// Try to connect <see cref="XBezier.Point2"/> point at specified location.
        /// </summary>
        /// <param name="bezier">The bezier object.</param>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <returns>True if connected.</returns>
        public void TryToConnectPoint2(XBezier bezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(_editor.Project.CurrentContainer, new Vector2(x, y), _editor.Project.Options.HitThreshold);
            if (result != null && result is XPoint)
            {
                bezier.Point2 = result as XPoint;
            }
        }

        /// <summary>
        /// Try to connect <see cref="XBezier.Point3"/> point at specified location.
        /// </summary>
        /// <param name="bezier">The bezier object.</param>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <returns>True if connected.</returns>
        public void TryToConnectPoint3(XBezier bezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(_editor.Project.CurrentContainer, new Vector2(x, y), _editor.Project.Options.HitThreshold);
            if (result != null && result is XPoint)
            {
                bezier.Point3 = result as XPoint;
            }
        }

        /// <summary>
        /// Try to connect <see cref="XBezier.Point4"/> point at specified location.
        /// </summary>
        /// <param name="bezier">The bezier object.</param>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <returns>True if connected.</returns>
        public void TryToConnectPoint4(XBezier bezier, double x, double y)
        {
            var result = ShapeBounds.HitTest(_editor.Project.CurrentContainer, new Vector2(x, y), _editor.Project.Options.HitThreshold);
            if (result != null && result is XPoint)
            {
                bezier.Point4 = result as XPoint;
            }
        }

        /// <inheritdoc/>
        public override void LeftDown(double x, double y)
        {
            base.LeftDown(x, y);

            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
            switch (_currentState)
            {
                case ToolState.None:
                    {
                        _shape = XBezier.Create(
                            sx, sy,
                            _editor.Project.CurrentStyleLibrary.Selected,
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
                        _currentState = ToolState.One;
                        _editor.CancelAvailable = true;
                    }
                    break;
                case ToolState.One:
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
                            _currentState = ToolState.Two;
                        }
                    }
                    break;
                case ToolState.Two:
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
                            _currentState = ToolState.Three;
                        }
                    }
                    break;
                case ToolState.Three:
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
                            _editor.Project.AddShape(_editor.Project.CurrentContainer.CurrentLayer, _shape);
                            _currentState = ToolState.None;
                            _editor.CancelAvailable = false;
                        }
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
                case ToolState.Three:
                    {
                        _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        Remove();
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
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

            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;
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
                case ToolState.Two:
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
                case ToolState.Three:
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

        /// <inheritdoc/>
        public override void ToStateOne()
        {
            base.ToStateOne();

            _style = _editor.Project.Options.HelperStyle;
            _helperPoint1 = XPoint.Create(0, 0, _editor.Project.Options.PointShape);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_helperPoint1);
            _helperPoint4 = XPoint.Create(0, 0, _editor.Project.Options.PointShape);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_helperPoint4);
        }

        /// <inheritdoc/>
        public override void ToStateTwo()
        {
            base.ToStateTwo();

            _style = _editor.Project.Options.HelperStyle;
            _line12 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_line12);
            _helperPoint2 = XPoint.Create(0, 0, _editor.Project.Options.PointShape);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_helperPoint2);
        }

        /// <inheritdoc/>
        public override void ToStateThree()
        {
            base.ToStateThree();

            _line43 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_line43);
            _line23 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_line23);
            _helperPoint3 = XPoint.Create(0, 0, _editor.Project.Options.PointShape);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_helperPoint3);
        }

        /// <inheritdoc/>
        public override void Move(BaseShape shape)
        {
            base.Move(shape);

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

            if (_helperPoint1 != null)
            {
                _helperPoint1.X = bezier.Point1.X;
                _helperPoint1.Y = bezier.Point1.Y;
            }

            if (_helperPoint2 != null)
            {
                _helperPoint2.X = bezier.Point2.X;
                _helperPoint2.Y = bezier.Point2.Y;
            }

            if (_helperPoint3 != null)
            {
                _helperPoint3.X = bezier.Point3.X;
                _helperPoint3.Y = bezier.Point3.Y;
            }

            if (_helperPoint4 != null)
            {
                _helperPoint4.X = bezier.Point4.X;
                _helperPoint4.Y = bezier.Point4.Y;
            }
        }

        /// <inheritdoc/>
        public override void Remove()
        {
            base.Remove();

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

            if (_helperPoint1 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_helperPoint1);
                _helperPoint1 = null;
            }

            if (_helperPoint2 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_helperPoint2);
                _helperPoint2 = null;
            }

            if (_helperPoint3 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_helperPoint3);
                _helperPoint3 = null;
            }

            if (_helperPoint4 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_helperPoint4);
                _helperPoint4 = null;
            }

            _style = null;
        }
    }
}
