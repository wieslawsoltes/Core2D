using System;
using System.Collections.Generic;
using Core2D;
using Core2D.Containers;
using Core2D.Input;
using Core2D.Layout;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using Spatial;

namespace Core2D.Editor.Tools.Decorators
{
    /// <summary>
    /// Box decorator.
    /// </summary>
    public class BoxDecorator : ObservableObject, IDrawable, IDecorator
    {
        private enum Mode
        {
            None,
            Move,
            Rotate,
            Top,
            Bottom,
            Left,
            Right,
            TopLeft,
            TopRight,
            BottomLeft,
            BottomRight
        }

        private bool _isVisible;
        private readonly IServiceProvider _serviceProvider;
        private IShapeStyle _style;
        private IMatrixObject _transform;
        private bool _isStroked;
        private bool _isFilled;
        private ILayerContainer _layer;
        private IList<IBaseShape> _shapes;
        private readonly IFactory _factory;
        private readonly double _sizeLarge;
        private readonly double _sizeSmall;
        private readonly double _rotateDistance;
        private GroupBox _groupBox;
        private readonly IShapeStyle _handleStyle;
        private readonly IShapeStyle _boundsStyle;
        private readonly IShapeStyle _selectedHandleStyle;
        private readonly IShapeStyle _selectedBoundsStyle;
        private readonly IRectangleShape _boundsHandle;
        private readonly ILineShape _rotateLine;
        private readonly IEllipseShape _rotateHandle;
        private readonly IEllipseShape _topLeftHandle;
        private readonly IEllipseShape _topRightHandle;
        private readonly IEllipseShape _bottomLeftHandle;
        private readonly IEllipseShape _bottomRightHandle;
        private readonly IRectangleShape _topHandle;
        private readonly IRectangleShape _bottomHandle;
        private readonly IRectangleShape _leftHandle;
        private readonly IRectangleShape _rightHandle;
        private List<IBaseShape> _handles;
        private IBaseShape _currentHandle = null;
        private List<IPointShape> _points;
        private Mode _mode = Mode.None;
        private double _startX;
        private double _startY;
        private double _historyX;
        private double _historyY;
        private double _rotateAngle = 270.0;
        private bool _previousDrawPoints = true;

        /// <inheritdoc/>
        public ILayerContainer Layer
        {
            get => _layer;
            set => Update(ref _layer, value);
        }

        /// <inheritdoc/>
        public IList<IBaseShape> Shapes
        {
            get => _shapes;
            set => Update(ref _shapes, value);
        }

        /// <inheritdoc/>
        public bool IsVisible => _isVisible;

        /// <inheritdoc/>
        public IShapeStyle Style
        {
            get => _style;
            set => Update(ref _style, value);
        }

        /// <inheritdoc/>
        public IMatrixObject Transform
        {
            get => _transform;
            set => Update(ref _transform, value);
        }

        /// <inheritdoc/>
        public bool IsStroked
        {
            get => _isStroked;
            set => Update(ref _isStroked, value);
        }

        /// <inheritdoc/>
        public bool IsFilled
        {
            get => _isFilled;
            set => Update(ref _isFilled, value);
        }

        /// <summary>
        /// Initialize new instance of <see cref="BoxDecorator"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public BoxDecorator(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            _factory = _serviceProvider.GetService<IFactory>();
            _sizeLarge = 4;
            _sizeSmall = 4;
            _rotateDistance = -16.875;
            _handleStyle = _factory.CreateShapeStyle("Handle", 255, 0, 191, 255, 255, 255, 255, 255, 2.0);
            _boundsStyle = _factory.CreateShapeStyle("Bounds", 255, 0, 191, 255, 255, 255, 255, 255, 1.0);
            _selectedHandleStyle = _factory.CreateShapeStyle("SelectedHandle", 255, 0, 191, 255, 255, 0, 191, 255, 2.0);
            _selectedBoundsStyle = _factory.CreateShapeStyle("SelectedBounds", 255, 0, 191, 255, 255, 255, 255, 255, 1.0);
            _rotateHandle = _factory.CreateEllipseShape(0, 0, 0, 0, _handleStyle, true, true, name: "_rotateHandle");
            _topLeftHandle = _factory.CreateEllipseShape(0, 0, 0, 0, _handleStyle, true, true, name: "_topLeftHandle");
            _topRightHandle = _factory.CreateEllipseShape(0, 0, 0, 0, _handleStyle, true, true, name: "_topRightHandle");
            _bottomLeftHandle = _factory.CreateEllipseShape(0, 0, 0, 0, _handleStyle, true, true, name: "_bottomLeftHandle");
            _bottomRightHandle = _factory.CreateEllipseShape(0, 0, 0, 0, _handleStyle, true, true, name: "_bottomRightHandle");
            _topHandle = _factory.CreateRectangleShape(0, 0, 0, 0, _handleStyle, true, true, name: "_topHandle");
            _bottomHandle = _factory.CreateRectangleShape(0, 0, 0, 0, _handleStyle, true, true, name: "_bottomHandle");
            _leftHandle = _factory.CreateRectangleShape(0, 0, 0, 0, _handleStyle, true, true, name: "_leftHandle");
            _rightHandle = _factory.CreateRectangleShape(0, 0, 0, 0, _handleStyle, true, true, name: "_rightHandle");
            _boundsHandle = _factory.CreateRectangleShape(0, 0, 0, 0, _boundsStyle, true, false, name: "_boundsHandle");
            _rotateLine = _factory.CreateLineShape(0, 0, 0, 0, _boundsStyle, true, name: "_rotateLine");
            _handles = new List<IBaseShape>
            {
                _rotateHandle,
                _topLeftHandle,
                _topRightHandle,
                _bottomLeftHandle,
                _bottomRightHandle,
                _topHandle,
                _bottomHandle,
                _leftHandle,
                _rightHandle,
                _boundsHandle,
                _rotateLine
            };
            _currentHandle = null;
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual object BeginTransform(object dc, IShapeRenderer renderer)
        {
            if (Transform != null)
            {
                return renderer.PushMatrix(dc, Transform);
            }
            return null;
        }

        /// <inheritdoc/>
        public virtual void EndTransform(object dc, IShapeRenderer renderer, object state)
        {
            if (Transform != null)
            {
                renderer.PopMatrix(dc, state);
            }
        }

        /// <inheritdoc/>
        public virtual void Draw(object dc, IShapeRenderer renderer, double dx, double dy)
        {
            var state = BeginTransform(dc, renderer);

            if (_isVisible)
            {
                foreach (var handle in _handles)
                {
                    handle.Draw(dc, renderer, dx, dy);
                }
            }

            EndTransform(dc, renderer, state);
        }

        /// <inheritdoc/>
        public virtual bool Invalidate(IShapeRenderer renderer, double dx, double dy)
        {
            return false;
        }

        /// <inheritdoc/>
        public void Update(bool rebuild = true)
        {
            if (_layer == null || _shapes == null)
            {
                return;
            }

            if (rebuild)
            {
                _groupBox = new GroupBox(_shapes);
            }
            else
            {
                _groupBox.Update();
            }

            _boundsHandle.TopLeft.X = _groupBox.Bounds.Left;
            _boundsHandle.TopLeft.Y = _groupBox.Bounds.Top;
            _boundsHandle.BottomRight.X = _groupBox.Bounds.Right;
            _boundsHandle.BottomRight.Y = _groupBox.Bounds.Bottom;

            _rotateLine.Start.X = _groupBox.Bounds.CenterX;
            _rotateLine.Start.Y = _groupBox.Bounds.Top;
            _rotateLine.End.X = _groupBox.Bounds.CenterX;
            _rotateLine.End.Y = _groupBox.Bounds.Top + _rotateDistance;

            _rotateHandle.TopLeft.X = _groupBox.Bounds.CenterX - _sizeLarge;
            _rotateHandle.TopLeft.Y = _groupBox.Bounds.Top + _rotateDistance - _sizeLarge;
            _rotateHandle.BottomRight.X = _groupBox.Bounds.CenterX + _sizeLarge;
            _rotateHandle.BottomRight.Y = _groupBox.Bounds.Top + _rotateDistance + _sizeLarge;

            _topLeftHandle.TopLeft.X = _groupBox.Bounds.Left - _sizeLarge;
            _topLeftHandle.TopLeft.Y = _groupBox.Bounds.Top - _sizeLarge;
            _topLeftHandle.BottomRight.X = _groupBox.Bounds.Left + _sizeLarge;
            _topLeftHandle.BottomRight.Y = _groupBox.Bounds.Top + _sizeLarge;

            _topRightHandle.TopLeft.X = _groupBox.Bounds.Right - _sizeLarge;
            _topRightHandle.TopLeft.Y = _groupBox.Bounds.Top - _sizeLarge;
            _topRightHandle.BottomRight.X = _groupBox.Bounds.Right + _sizeLarge;
            _topRightHandle.BottomRight.Y = _groupBox.Bounds.Top + _sizeLarge;

            _bottomLeftHandle.TopLeft.X = _groupBox.Bounds.Left - _sizeLarge;
            _bottomLeftHandle.TopLeft.Y = _groupBox.Bounds.Bottom - _sizeLarge;
            _bottomLeftHandle.BottomRight.X = _groupBox.Bounds.Left + _sizeLarge;
            _bottomLeftHandle.BottomRight.Y = _groupBox.Bounds.Bottom + _sizeLarge;

            _bottomRightHandle.TopLeft.X = _groupBox.Bounds.Right - _sizeLarge;
            _bottomRightHandle.TopLeft.Y = _groupBox.Bounds.Bottom - _sizeLarge;
            _bottomRightHandle.BottomRight.X = _groupBox.Bounds.Right + _sizeLarge;
            _bottomRightHandle.BottomRight.Y = _groupBox.Bounds.Bottom + _sizeLarge;

            _topHandle.TopLeft.X = _groupBox.Bounds.CenterX - _sizeSmall;
            _topHandle.TopLeft.Y = _groupBox.Bounds.Top - _sizeSmall;
            _topHandle.BottomRight.X = _groupBox.Bounds.CenterX + _sizeSmall;
            _topHandle.BottomRight.Y = _groupBox.Bounds.Top + _sizeSmall;

            _bottomHandle.TopLeft.X = _groupBox.Bounds.CenterX - _sizeSmall;
            _bottomHandle.TopLeft.Y = _groupBox.Bounds.Bottom - _sizeSmall;
            _bottomHandle.BottomRight.X = _groupBox.Bounds.CenterX + _sizeSmall;
            _bottomHandle.BottomRight.Y = _groupBox.Bounds.Bottom + _sizeSmall;

            _leftHandle.TopLeft.X = _groupBox.Bounds.Left - _sizeSmall;
            _leftHandle.TopLeft.Y = _groupBox.Bounds.CenterY - _sizeSmall;
            _leftHandle.BottomRight.X = _groupBox.Bounds.Left + _sizeSmall;
            _leftHandle.BottomRight.Y = _groupBox.Bounds.CenterY + _sizeSmall;

            _rightHandle.TopLeft.X = _groupBox.Bounds.Right - _sizeSmall;
            _rightHandle.TopLeft.Y = _groupBox.Bounds.CenterY - _sizeSmall;
            _rightHandle.BottomRight.X = _groupBox.Bounds.Right + _sizeSmall;
            _rightHandle.BottomRight.Y = _groupBox.Bounds.CenterY + _sizeSmall;

            if (_groupBox.Bounds.Height <= 0)
            {
                _leftHandle.State.Flags &= ~ShapeStateFlags.Visible;
                _rightHandle.State.Flags &= ~ShapeStateFlags.Visible;
            }
            else
            {
                _leftHandle.State.Flags |= ShapeStateFlags.Visible;
                _rightHandle.State.Flags |= ShapeStateFlags.Visible;
            }

            if (_groupBox.Bounds.Width <= 0)
            {
                _topHandle.State.Flags &= ~ShapeStateFlags.Visible;
                _bottomHandle.State.Flags &= ~ShapeStateFlags.Visible;
            }
            else
            {
                _topHandle.State.Flags |= ShapeStateFlags.Visible;
                _bottomHandle.State.Flags |= ShapeStateFlags.Visible;
            }

            _layer.Invalidate();
        }

        /// <inheritdoc/>
        public void Show()
        {
            if (_layer == null || _shapes == null)
            {
                return;
            }

            if (_isVisible == true)
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();
            _previousDrawPoints = editor.PageState.DrawPoints;
            editor.PageState.DrawPoints = false;

            _mode = Mode.None;
            if (_currentHandle != null)
            {
                _currentHandle.Style = _currentHandle == _boundsHandle ? _boundsStyle : _handleStyle;
                _currentHandle = null;
                _points = null;
                _rotateAngle = 0.0;
            }
            _isVisible = true;

            var shapesBuilder = _layer.Shapes.ToBuilder();
            shapesBuilder.Add(_boundsHandle);
            shapesBuilder.Add(_rotateLine);
            shapesBuilder.Add(_rotateHandle);
            shapesBuilder.Add(_topLeftHandle);
            shapesBuilder.Add(_topRightHandle);
            shapesBuilder.Add(_bottomLeftHandle);
            shapesBuilder.Add(_bottomRightHandle);
            shapesBuilder.Add(_topHandle);
            shapesBuilder.Add(_bottomHandle);
            shapesBuilder.Add(_leftHandle);
            shapesBuilder.Add(_rightHandle);
            _layer.Shapes = shapesBuilder.ToImmutable();

            _layer.Invalidate();
        }

        /// <inheritdoc/>
        public void Hide()
        {
            if (_layer == null || _shapes == null)
            {
                return;
            }

            if (_isVisible == true)
            {
                var editor = _serviceProvider.GetService<IProjectEditor>();
                editor.PageState.DrawPoints = _previousDrawPoints;
            }

            _mode = Mode.None;
            if (_currentHandle != null)
            {
                _currentHandle.Style = _currentHandle == _boundsHandle ? _boundsStyle : _handleStyle;
                _currentHandle = null;
                _points = null;
                _rotateAngle = 0.0;
            }
            _isVisible = false;

            var shapesBuilder = _layer.Shapes.ToBuilder();
            shapesBuilder.Remove(_boundsHandle);
            shapesBuilder.Remove(_rotateLine);
            shapesBuilder.Remove(_rotateHandle);
            shapesBuilder.Remove(_topLeftHandle);
            shapesBuilder.Remove(_topRightHandle);
            shapesBuilder.Remove(_bottomLeftHandle);
            shapesBuilder.Remove(_bottomRightHandle);
            shapesBuilder.Remove(_topHandle);
            shapesBuilder.Remove(_bottomHandle);
            shapesBuilder.Remove(_leftHandle);
            shapesBuilder.Remove(_rightHandle);
            _layer.Shapes = shapesBuilder.ToImmutable();
            _layer.Invalidate();
        }

        /// <inheritdoc/>
        public bool HitTest(InputArgs args)
        {
            if (_isVisible == false)
            {
                return false;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();
            (double x, double y) = args;
            (double sx, double sy) = editor.TryToSnap(args);

            _mode = Mode.None;
            if (_currentHandle != null)
            {
                _currentHandle.Style = _currentHandle == _boundsHandle ? _boundsStyle : _handleStyle;
                _currentHandle = null;
                _points = null;
                _rotateAngle = 0.0;
                _layer.Invalidate();
            }

            double radius = editor.Project.Options.HitThreshold / editor.PageState.ZoomX;
            var result = editor.HitTest.TryToGetShape(_handles, new Point2(x, y), radius);
            if (result != null)
            {
                if (result == _boundsHandle)
                {
                    _mode = Mode.Move;
                }
                else if (result == _rotateHandle)
                {
                    _mode = Mode.Rotate;
                }
                else if (result == _topLeftHandle)
                {
                    _mode = Mode.TopLeft;
                }
                else if (result == _topRightHandle)
                {
                    _mode = Mode.TopRight;
                }
                else if (result == _bottomLeftHandle)
                {
                    _mode = Mode.BottomLeft;
                }
                else if (result == _bottomRightHandle)
                {
                    _mode = Mode.BottomRight;
                }
                else if (result == _topHandle)
                {
                    _mode = Mode.Top;
                }
                else if (result == _bottomHandle)
                {
                    _mode = Mode.Bottom;
                }
                else if (result == _leftHandle)
                {
                    _mode = Mode.Left;
                }
                else if (result == _rightHandle)
                {
                    _mode = Mode.Right;
                }

                if (_mode != Mode.None)
                {
                    _currentHandle = result;
                    _currentHandle.Style = _currentHandle == _boundsHandle ? _selectedBoundsStyle : _selectedHandleStyle;
                    _startX = sx;
                    _startY = sy;
                    _historyX = _startX;
                    _historyY = _startY;
                    _points = null;
                    _rotateAngle = 0.0;
                    _layer.Invalidate();
                    return true;
                }
            }
            return false;
        }

        /// <inheritdoc/>
        public void Move(InputArgs args)
        {
            if (_layer == null || _shapes == null)
            {
                return;
            }

            if (_isVisible == false)
            {
                return;
            }

            var editor = _serviceProvider.GetService<IProjectEditor>();

            (double sx, double sy) = editor.TryToSnap(args);
            double dx = sx - _startX;
            double dy = sy - _startY;
            _startX = sx;
            _startY = sy;

            if (_points == null)
            {
                _points = GetMovablePoints();
                if (_points == null)
                {
                    return;
                }
            }

            switch (_mode)
            {
                case Mode.None:
                    break;
                case Mode.Move:
                    {
                        Translate(dx, dy);
                    }
                    break;
                case Mode.Rotate:
                    {
                        Rotate(sx, sy);
                    }
                    break;
                case Mode.Top:
                    {
                        ScaleTop(dy);
                    }
                    break;
                case Mode.Bottom:
                    {
                        ScaleBottom(dy);
                    }
                    break;
                case Mode.Left:
                    {
                        ScaleLeft(dx);
                    }
                    break;
                case Mode.Right:
                    {
                        ScaleRight(dx);
                    }
                    break;
                case Mode.TopLeft:
                    {
                        ScaleTop(dy);
                        ScaleLeft(dx);
                    }
                    break;
                case Mode.TopRight:
                    {
                        ScaleTop(dy);
                        ScaleRight(dx);
                    }
                    break;
                case Mode.BottomLeft:
                    {
                        ScaleBottom(dy);
                        ScaleLeft(dx);
                    }
                    break;
                case Mode.BottomRight:
                    {
                        ScaleBottom(dy);
                        ScaleRight(dx);
                    }
                    break;
            }
        }

        private bool IsPointMovable(IPointShape point, IBaseShape parent)
        {
            if (point.State.Flags.HasFlag(ShapeStateFlags.Locked))
            {
                return false;
            }

            if (point.State.Flags.HasFlag(ShapeStateFlags.Connector) && point.Owner != parent)
            {
                return false;
            }

            return true;
        }

        private List<IPointShape> GetMovablePoints()
        {
            var points = new HashSet<IPointShape>();

            for (int i = 0; i < _groupBox.Boxes.Length; i++)
            {
                foreach (var point in _groupBox.Boxes[i].Points)
                {
                    if (IsPointMovable(point, _groupBox.Boxes[i].Shape))
                    {
                        points.Add(point);
                    }
                }
            }

            return new List<IPointShape>(points);
        }

        private void TransformPoints(ref Matrix2 matrix)
        {
            if (_points == null || _points.Count == 0)
            {
                return;
            }

            for (int i = 0; i < _points.Count; i++)
            {
                var point = _points[i];
                var transformed = Matrix2.TransformPoint(matrix, new Point2(point.X, point.Y));
                point.X = transformed.X;
                point.Y = transformed.Y;
            }
        }

        private void Rotate(double sx, double sy)
        {
            var centerX = _groupBox.Bounds.CenterX;
            var centerY = _groupBox.Bounds.CenterY;
            var p0 = new Point2(centerX, centerY);
            var p1 = new Point2(sx, sy);
            var angle = p0.AngleBetween(p1) - 270.0;
            var delta = angle - _rotateAngle;
            var radians = delta * (Math.PI / 180.0);
            var matrix = Matrix2.Rotation(radians, centerX, centerY);
            TransformPoints(ref matrix);
            _rotateAngle = angle;
            _groupBox.Update();
        }

        private void Translate(double dx, double dy)
        {
            double offsetX = dx;
            double offsetY = dy;
            var matrix = Matrix2.Translate(offsetX, offsetY);
            TransformPoints(ref matrix);
            _groupBox.Update();
        }

        private void ScaleTop(double dy)
        {
            var scaleX = 1.0;
            var scaleY = (_groupBox.Bounds.Height - dy) / _groupBox.Bounds.Height;
            var centerX = _groupBox.Bounds.CenterX;
            var centerY = _groupBox.Bounds.Bottom;
            var matrix = Matrix2.ScaleAt(scaleX, scaleY, centerX, centerY);
            TransformPoints(ref matrix);
            _groupBox.Update();
        }

        private void ScaleBottom(double dy)
        {
            var scaleX = 1.0;
            var scaleY = (_groupBox.Bounds.Height + dy) / _groupBox.Bounds.Height;
            var centerX = _groupBox.Bounds.CenterX;
            var centerY = _groupBox.Bounds.Top;
            var matrix = Matrix2.ScaleAt(scaleX, scaleY, centerX, centerY);
            TransformPoints(ref matrix);
            _groupBox.Update();
        }

        private void ScaleLeft(double dx)
        {
            var scaleX = (_groupBox.Bounds.Width - dx) / _groupBox.Bounds.Width;
            var scaleY = 1.0;
            var centerX = _groupBox.Bounds.Right;
            var centerY = _groupBox.Bounds.CenterY;
            var matrix = Matrix2.ScaleAt(scaleX, scaleY, centerX, centerY);
            TransformPoints(ref matrix);
            _groupBox.Update();
        }

        private void ScaleRight(double dx)
        {
            var scaleX = (_groupBox.Bounds.Width + dx) / _groupBox.Bounds.Width;
            var scaleY = 1.0;
            var centerX = _groupBox.Bounds.Left;
            var centerY = _groupBox.Bounds.CenterY;
            var matrix = Matrix2.ScaleAt(scaleX, scaleY, centerX, centerY);
            TransformPoints(ref matrix);
            _groupBox.Update();
        }
    }
}
