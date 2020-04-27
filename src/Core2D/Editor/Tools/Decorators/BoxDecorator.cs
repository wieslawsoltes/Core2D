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
                //_rotateHandle,
                _topLeftHandle,
                _topRightHandle,
                _bottomLeftHandle,
                _bottomRightHandle,
                _topHandle,
                _bottomHandle,
                _leftHandle,
                _rightHandle,
                _boundsHandle,
                //_rotateLine
            };
            _currentHandle = null;
            _rotateHandle.State.Flags |= ShapeStateFlags.Size | ShapeStateFlags.Thickness;
            _topLeftHandle.State.Flags |= ShapeStateFlags.Size | ShapeStateFlags.Thickness;
            _topRightHandle.State.Flags |= ShapeStateFlags.Size | ShapeStateFlags.Thickness;
            _bottomLeftHandle.State.Flags |= ShapeStateFlags.Size | ShapeStateFlags.Thickness;
            _bottomRightHandle.State.Flags |= ShapeStateFlags.Size | ShapeStateFlags.Thickness;
            _topHandle.State.Flags |= ShapeStateFlags.Size | ShapeStateFlags.Thickness;
            _bottomHandle.State.Flags |= ShapeStateFlags.Size | ShapeStateFlags.Thickness;
            _leftHandle.State.Flags |= ShapeStateFlags.Size | ShapeStateFlags.Thickness;
            _rightHandle.State.Flags |= ShapeStateFlags.Size | ShapeStateFlags.Thickness;
            _boundsHandle.State.Flags |= ShapeStateFlags.Thickness;
            _rotateLine.State.Flags |= ShapeStateFlags.Thickness;
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
        public virtual void DrawShape(object dc, IShapeRenderer renderer, double dx, double dy)
        {
            if (_isVisible)
            {
                foreach (var handle in _handles)
                {
                    handle.DrawShape(dc, renderer, dx, dy);
                }
            }
        }

        /// <inheritdoc/>
        public virtual void DrawPoints(object dc, IShapeRenderer renderer, double dx, double dy)
        {

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

            if (_groupBox.Bounds.Height <= 0 || _groupBox.Bounds.Width <= 0)
            {
                _leftHandle.State.Flags &= ~ShapeStateFlags.Visible;
                _rightHandle.State.Flags &= ~ShapeStateFlags.Visible;
                _topHandle.State.Flags &= ~ShapeStateFlags.Visible;
                _bottomHandle.State.Flags &= ~ShapeStateFlags.Visible;
            }
            else
            {
                _leftHandle.State.Flags |= ShapeStateFlags.Visible;
                _rightHandle.State.Flags |= ShapeStateFlags.Visible;
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
            //shapesBuilder.Add(_rotateLine);
            //shapesBuilder.Add(_rotateHandle);
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
            //shapesBuilder.Remove(_rotateLine);
            //shapesBuilder.Remove(_rotateHandle);
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
            var result = editor.HitTest.TryToGetShape(_handles, new Point2(x, y), radius, editor.PageState.ZoomX);
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
                _points = _groupBox.GetMovablePoints();
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
                        _groupBox.Translate(dx, dy, _points);
                    }
                    break;
                case Mode.Rotate:
                    {
                        _groupBox.Rotate(sx, sy, _points, ref _rotateAngle);
                    }
                    break;
                case Mode.Top:
                    {
                        _groupBox.ScaleTop(dy, _points);
                    }
                    break;
                case Mode.Bottom:
                    {
                        _groupBox.ScaleBottom(dy, _points);
                    }
                    break;
                case Mode.Left:
                    {
                        _groupBox.ScaleLeft(dx, _points);
                    }
                    break;
                case Mode.Right:
                    {
                        _groupBox.ScaleRight(dx, _points);
                    }
                    break;
                case Mode.TopLeft:
                    {
                        _groupBox.ScaleTop(dy, _points);
                        _groupBox.ScaleLeft(dx, _points);
                    }
                    break;
                case Mode.TopRight:
                    {
                        _groupBox.ScaleTop(dy, _points);
                        _groupBox.ScaleRight(dx, _points);
                    }
                    break;
                case Mode.BottomLeft:
                    {
                        _groupBox.ScaleBottom(dy, _points);
                        _groupBox.ScaleLeft(dx, _points);
                    }
                    break;
                case Mode.BottomRight:
                    {
                        _groupBox.ScaleBottom(dy, _points);
                        _groupBox.ScaleRight(dx, _points);
                    }
                    break;
            }
        }
    }
}
