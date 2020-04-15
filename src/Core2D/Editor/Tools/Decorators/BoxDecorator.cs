using System;
using System.Collections.Generic;
using Core2D.Containers;
using Core2D.Editor.Layout;
using Core2D.Interfaces;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools.Decorators
{
    /// <summary>
    /// Box decorator.
    /// </summary>
    public class BoxDecorator : ObservableObject, IDrawable, IDecorator
    {
        private bool _isVisible;
        public IList<IBaseShape> _shapes;
        private IShapeStyle _style;
        private IMatrixObject _transform;
        private bool _isStroked;
        private bool _isFilled;
        private readonly ILayerContainer _layer;
        private readonly IFactory _factory;
        private readonly double _sizeLarge;
        private readonly double _sizeSmall;
        private readonly double _rotateDistance;
        private GroupBox _groupBox;
        private readonly IShapeStyle _handleStyle;
        private readonly IShapeStyle _boundsStyle;
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

        /// <inheritdoc/>
        public IList<IBaseShape> Shapes
        {
            get => _shapes;
            set => Update(ref _shapes, value);
        }

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
        /// <param name="state">The shape renderer state.</param>
        /// <param name="layer">The layer container.</param>
        /// <param name="factory">The factory.</param>
        public BoxDecorator(List<IBaseShape> shapes, ILayerContainer layer, IFactory factory)
        {
            _shapes = shapes;
            _layer = layer;
            _factory = factory;
            _sizeLarge = 3.75;
            _sizeSmall = 2.8125;
            _rotateDistance = -16.875;

            _groupBox = new GroupBox(_shapes);

            _handleStyle = _factory.CreateShapeStyle("Handle", 255, 0, 191, 255, 255, 255, 255, 255, 2.0);
            _boundsStyle = _factory.CreateShapeStyle("Bounds", 255, 0, 191, 255, 255, 255, 255, 255, 1.0);

            _boundsHandle = _factory.CreateRectangleShape(
                _groupBox.Bounds.Left,
                _groupBox.Bounds.Top,
                _groupBox.Bounds.Right,
                _groupBox.Bounds.Bottom,
                _boundsStyle, true, false, name: "_boundsHandle");

            _rotateLine = _factory.CreateLineShape(
                _groupBox.Bounds.CenterX,
                _groupBox.Bounds.Top,
                _groupBox.Bounds.CenterX,
                _groupBox.Bounds.Top + _rotateDistance,
                _boundsStyle, true, name: "_rotateLine");

            _rotateHandle = _factory.CreateEllipseShape(
                _groupBox.Bounds.CenterX - _sizeLarge,
                _groupBox.Bounds.Top + _rotateDistance - _sizeLarge,
                _groupBox.Bounds.CenterX + _sizeLarge,
                _groupBox.Bounds.Top + _rotateDistance + _sizeLarge,
                _handleStyle, true, true, name: "_rotateHandle");

            _topLeftHandle = _factory.CreateEllipseShape(
                _groupBox.Bounds.Left - _sizeLarge,
                _groupBox.Bounds.Top - _sizeLarge,
                _groupBox.Bounds.Left + _sizeLarge,
                _groupBox.Bounds.Top + _sizeLarge,
                _handleStyle, true, true, name: "_topLeftHandle");

            _topRightHandle = _factory.CreateEllipseShape(
                _groupBox.Bounds.Right - _sizeLarge,
                _groupBox.Bounds.Top - _sizeLarge,
                _groupBox.Bounds.Right + _sizeLarge,
                _groupBox.Bounds.Top + _sizeLarge,
                _handleStyle, true, true, name: "_topRightHandle");

            _bottomLeftHandle = _factory.CreateEllipseShape(
                _groupBox.Bounds.Left - _sizeLarge,
                _groupBox.Bounds.Bottom - _sizeLarge,
                _groupBox.Bounds.Left + _sizeLarge,
                _groupBox.Bounds.Bottom + _sizeLarge,
                _handleStyle, true, true, name: "_bottomLeftHandle");

            _bottomRightHandle = _factory.CreateEllipseShape(
                _groupBox.Bounds.Right - _sizeLarge,
                _groupBox.Bounds.Bottom - _sizeLarge,
                _groupBox.Bounds.Right + _sizeLarge,
                _groupBox.Bounds.Bottom + _sizeLarge,
                _handleStyle, true, true, name: "_bottomRightHandle");

            _topHandle = _factory.CreateRectangleShape(
                _groupBox.Bounds.CenterX - _sizeSmall,
                _groupBox.Bounds.Top - _sizeSmall,
                _groupBox.Bounds.CenterX + _sizeSmall,
                _groupBox.Bounds.Top + _sizeSmall,
                _handleStyle, true, true, name: "_topHandle");

            _bottomHandle = _factory.CreateRectangleShape(
                _groupBox.Bounds.CenterX - _sizeSmall,
                _groupBox.Bounds.Bottom - _sizeSmall,
                _groupBox.Bounds.CenterX + _sizeSmall,
                _groupBox.Bounds.Bottom + _sizeSmall,
                _handleStyle, true, true, name: "_bottomHandle");

            _leftHandle = _factory.CreateRectangleShape(
                _groupBox.Bounds.Left - _sizeSmall,
                _groupBox.Bounds.CenterY - _sizeSmall,
                _groupBox.Bounds.Left + _sizeSmall,
                _groupBox.Bounds.CenterY + _sizeSmall,
                _handleStyle, true, true, name: "_leftHandle");

            _rightHandle = _factory.CreateRectangleShape(
                _groupBox.Bounds.Right - _sizeSmall,
                _groupBox.Bounds.CenterY - _sizeSmall,
                _groupBox.Bounds.Right + _sizeSmall,
                _groupBox.Bounds.CenterY + _sizeSmall,
                _handleStyle, true, true, name: "_rightHandle");
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
                _boundsHandle.Draw(dc, renderer, dx, dy);
                _rotateLine.Draw(dc, renderer, dx, dy);
                _rotateHandle.Draw(dc, renderer, dx, dy);
                _topLeftHandle.Draw(dc, renderer, dx, dy);
                _topRightHandle.Draw(dc, renderer, dx, dy);
                _bottomLeftHandle.Draw(dc, renderer, dx, dy);
                _bottomRightHandle.Draw(dc, renderer, dx, dy);
                _topHandle.Draw(dc, renderer, dx, dy);
                _bottomHandle.Draw(dc, renderer, dx, dy);
                _leftHandle.Draw(dc, renderer, dx, dy);
                _rightHandle.Draw(dc, renderer, dx, dy);
            }

            EndTransform(dc, renderer, state);
        }

        /// <inheritdoc/>
        public virtual bool Invalidate(IShapeRenderer renderer, double dx, double dy)
        {
            return false;
        }

        /// <inheritdoc/>
        public void Update(bool rebuild)
        {
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

            _layer.Invalidate();
        }

        /// <inheritdoc/>
        public void Show()
        {
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
    }
}
