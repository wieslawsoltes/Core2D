// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor.Bounds;
using Core2D.Math;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools
{
    /// <summary>
    /// Helper class for <see cref="Tool.QuadraticBezier"/> editor.
    /// </summary>
    public sealed class ToolQuadraticBezier : ToolBase
    {
        private ProjectEditor _editor;
        private ToolState _currentState = ToolState.None;
        private XQuadraticBezier _shape;
        private ShapeStyle _style;
        private XLine _line12;
        private XLine _line32;
        private XPoint _helperPoint1;
        private XPoint _helperPoint2;
        private XPoint _helperPoint3;

        /// <summary>
        /// Initialize new instance of <see cref="ToolQuadraticBezier"/> class.
        /// </summary>
        /// <param name="editor">The current <see cref="ProjectEditor"/> object.</param>
        public ToolQuadraticBezier(ProjectEditor editor)
            : base()
        {
            _editor = editor;
        }

        /// <summary>
        /// Try to connect <see cref="XQuadraticBezier.Point1"/> point at specified location.
        /// </summary>
        /// <param name="quadraticBezier">The quadratic bezier object.</param>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <returns>True if connected.</returns>
        public void TryToConnectPoint1(XQuadraticBezier quadraticBezier, double x, double y)
        {
            var result = ShapeHitTest.HitTest(_editor.Project.CurrentContainer.CurrentLayer, new Vector2(x, y), _editor.Project.Options.HitThreshold);
            if (result != null && result is XPoint)
            {
                quadraticBezier.Point1 = result as XPoint;
            }
        }

        /// <summary>
        /// Try to connect <see cref="XQuadraticBezier.Point2"/> point at specified location.
        /// </summary>
        /// <param name="quadraticBezier">The quadratic bezier object.</param>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <returns>True if connected.</returns>
        public void TryToConnectPoint2(XQuadraticBezier quadraticBezier, double x, double y)
        {
            var result = ShapeHitTest.HitTest(_editor.Project.CurrentContainer.CurrentLayer, new Vector2(x, y), _editor.Project.Options.HitThreshold);
            if (result != null && result is XPoint)
            {
                quadraticBezier.Point2 = result as XPoint;
            }
        }

        /// <summary>
        /// Try to connect <see cref="XQuadraticBezier.Point3"/> point at specified location.
        /// </summary>
        /// <param name="quadraticBezier">The quadratic bezier object.</param>
        /// <param name="x">The X coordinate of point.</param>
        /// <param name="y">The Y coordinate of point.</param>
        /// <returns>True if connected.</returns>
        public void TryToConnectPoint3(XQuadraticBezier quadraticBezier, double x, double y)
        {
            var result = ShapeHitTest.HitTest(_editor.Project.CurrentContainer.CurrentLayer, new Vector2(x, y), _editor.Project.Options.HitThreshold);
            if (result != null && result is XPoint)
            {
                quadraticBezier.Point3 = result as XPoint;
            }
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
                        _shape = XQuadraticBezier.Create(
                            sx, sy,
                            _editor.Project.CurrentStyleLibrary.Selected,
                            _editor.Project.Options.PointShape,
                            _editor.Project.Options.DefaultIsStroked,
                            _editor.Project.Options.DefaultIsFilled);
                        if (_editor.Project.Options.TryToConnect)
                        {
                            TryToConnectPoint1(_shape as XQuadraticBezier, sx, sy);
                        }
                        _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Add(_shape);
                        _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                        ToStateOne();
                        Move(_shape as XQuadraticBezier);
                        _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        _currentState = ToolState.One;
                        _editor.CancelAvailable = true;
                    }
                    break;
                case ToolState.One:
                    {
                        var quadraticBezier = _shape as XQuadraticBezier;
                        if (quadraticBezier != null)
                        {
                            quadraticBezier.Point2.X = sx;
                            quadraticBezier.Point2.Y = sy;
                            quadraticBezier.Point3.X = sx;
                            quadraticBezier.Point3.Y = sy;
                            if (_editor.Project.Options.TryToConnect)
                            {
                                TryToConnectPoint3(_shape as XQuadraticBezier, sx, sy);
                            }
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            ToStateTwo();
                            Move(_shape as XQuadraticBezier);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                            _currentState = ToolState.Two;
                        }
                    }
                    break;
                case ToolState.Two:
                    {
                        var quadraticBezier = _shape as XQuadraticBezier;
                        if (quadraticBezier != null)
                        {
                            quadraticBezier.Point2.X = sx;
                            quadraticBezier.Point2.Y = sy;
                            if (_editor.Project.Options.TryToConnect)
                            {
                                TryToConnectPoint2(_shape as XQuadraticBezier, sx, sy);
                            }
                            _editor.Project.CurrentContainer.WorkingLayer.Shapes = _editor.Project.CurrentContainer.WorkingLayer.Shapes.Remove(_shape);
                            Remove();
                            Finalize(_shape as XQuadraticBezier);
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
                        var quadraticBezier = _shape as XQuadraticBezier;
                        if (quadraticBezier != null)
                        {
                            if (_editor.Project.Options.TryToConnect)
                            {
                                _editor.TryToHoverShape(sx, sy);
                            }
                            quadraticBezier.Point2.X = sx;
                            quadraticBezier.Point2.Y = sy;
                            quadraticBezier.Point3.X = sx;
                            quadraticBezier.Point3.Y = sy;
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_shape as XQuadraticBezier);
                            _editor.Project.CurrentContainer.HelperLayer.Invalidate();
                        }
                    }
                    break;
                case ToolState.Two:
                    {
                        var quadraticBezier = _shape as XQuadraticBezier;
                        if (quadraticBezier != null)
                        {
                            if (_editor.Project.Options.TryToConnect)
                            {
                                _editor.TryToHoverShape(sx, sy);
                            }
                            quadraticBezier.Point2.X = sx;
                            quadraticBezier.Point2.Y = sy;
                            _editor.Project.CurrentContainer.WorkingLayer.Invalidate();
                            Move(_shape as XQuadraticBezier);
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
            _helperPoint3 = XPoint.Create(0, 0, _editor.Project.Options.PointShape);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_helperPoint3);
        }

        /// <inheritdoc/>
        public override void ToStateTwo()
        {
            base.ToStateTwo();

            _style = _editor.Project.Options.HelperStyle;
            _line12 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_line12);
            _line32 = XLine.Create(0, 0, _style, null);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_line32);
            _helperPoint2 = XPoint.Create(0, 0, _editor.Project.Options.PointShape);
            _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Add(_helperPoint2);
        }

        /// <inheritdoc/>
        public override void Move(BaseShape shape)
        {
            base.Move(shape);

            var quadraticBezier = shape as XQuadraticBezier;

            if (_line12 != null)
            {
                _line12.Start.X = quadraticBezier.Point1.X;
                _line12.Start.Y = quadraticBezier.Point1.Y;
                _line12.End.X = quadraticBezier.Point2.X;
                _line12.End.Y = quadraticBezier.Point2.Y;
            }

            if (_line32 != null)
            {
                _line32.Start.X = quadraticBezier.Point3.X;
                _line32.Start.Y = quadraticBezier.Point3.Y;
                _line32.End.X = quadraticBezier.Point2.X;
                _line32.End.Y = quadraticBezier.Point2.Y;
            }

            if (_helperPoint1 != null)
            {
                _helperPoint1.X = quadraticBezier.Point1.X;
                _helperPoint1.Y = quadraticBezier.Point1.Y;
            }

            if (_helperPoint2 != null)
            {
                _helperPoint2.X = quadraticBezier.Point2.X;
                _helperPoint2.Y = quadraticBezier.Point2.Y;
            }

            if (_helperPoint3 != null)
            {
                _helperPoint3.X = quadraticBezier.Point3.X;
                _helperPoint3.Y = quadraticBezier.Point3.Y;
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

            if (_line32 != null)
            {
                _editor.Project.CurrentContainer.HelperLayer.Shapes = _editor.Project.CurrentContainer.HelperLayer.Shapes.Remove(_line32);
                _line32 = null;
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

            _style = null;
        }
    }
}
