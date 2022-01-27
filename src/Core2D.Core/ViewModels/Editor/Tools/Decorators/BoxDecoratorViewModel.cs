#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Input;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Layout;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using Core2D.Spatial;

namespace Core2D.ViewModels.Editor.Tools.Decorators;

public partial class BoxDecoratorViewModel : ViewModelBase, IDecorator
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
    [AutoNotify] private ShapeStyleViewModel? _style;
    [AutoNotify] private bool _isStroked;
    [AutoNotify] private bool _isFilled;
    [AutoNotify] private LayerContainerViewModel? _layer;
    [AutoNotify] private IList<BaseShapeViewModel>? _shapes;
    private readonly double _sizeLarge;
    private readonly double _sizeSmall;
    private readonly double _rotateDistance;
    private GroupBox _groupBox;
    private readonly ShapeStyleViewModel? _handleStyle;
    private readonly ShapeStyleViewModel? _boundsStyle;
    private readonly ShapeStyleViewModel? _selectedHandleStyle;
    private readonly ShapeStyleViewModel? _selectedBoundsStyle;
    private readonly RectangleShapeViewModel? _boundsHandle;
    private readonly LineShapeViewModel? _rotateLine;
    private readonly EllipseShapeViewModel? _rotateHandle;
    private readonly EllipseShapeViewModel? _topLeftHandle;
    private readonly EllipseShapeViewModel? _topRightHandle;
    private readonly EllipseShapeViewModel? _bottomLeftHandle;
    private readonly EllipseShapeViewModel? _bottomRightHandle;
    private readonly RectangleShapeViewModel? _topHandle;
    private readonly RectangleShapeViewModel? _bottomHandle;
    private readonly RectangleShapeViewModel? _leftHandle;
    private readonly RectangleShapeViewModel? _rightHandle;
    private readonly List<BaseShapeViewModel>? _handles;
    private BaseShapeViewModel? _currentHandle;
    private List<PointShapeViewModel>? _points;
    private Mode _mode = Mode.None;
    private double _startX;
    private double _startY;
    private double _rotateAngle = 270.0;
    private bool _previousDrawPoints = true;

    public bool IsVisible => _isVisible;

    public BoxDecoratorViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
    {
        var viewModelFactory = serviceProvider.GetService<IViewModelFactory>();

        _sizeLarge = 4.0;
        _sizeSmall = 4.0;
        _rotateDistance = -16.875;

        if (viewModelFactory is not null)
        {
            _handleStyle = viewModelFactory.CreateShapeStyle("Handle", 255, 0, 191, 255, 255, 255, 255, 255);
            _boundsStyle = viewModelFactory.CreateShapeStyle("Bounds", 255, 0, 191, 255, 255, 255, 255, 255, 1.0);
            _selectedHandleStyle = viewModelFactory.CreateShapeStyle("SelectedHandle", 255, 0, 191, 255, 255, 0, 191, 255);
            _selectedBoundsStyle = viewModelFactory.CreateShapeStyle("SelectedBounds", 255, 0, 191, 255, 255, 255, 255, 255, 1.0);
            _rotateHandle = viewModelFactory.CreateEllipseShape(0, 0, 0, 0, _handleStyle, true, true, name: "_rotateHandle");
            _topLeftHandle = viewModelFactory.CreateEllipseShape(0, 0, 0, 0, _handleStyle, true, true, name: "_topLeftHandle");
            _topRightHandle = viewModelFactory.CreateEllipseShape(0, 0, 0, 0, _handleStyle, true, true, name: "_topRightHandle");
            _bottomLeftHandle = viewModelFactory.CreateEllipseShape(0, 0, 0, 0, _handleStyle, true, true, name: "_bottomLeftHandle");
            _bottomRightHandle = viewModelFactory.CreateEllipseShape(0, 0, 0, 0, _handleStyle, true, true, name: "_bottomRightHandle");
            _topHandle = viewModelFactory.CreateRectangleShape(0, 0, 0, 0, _handleStyle, true, true, name: "_topHandle");
            _bottomHandle = viewModelFactory.CreateRectangleShape(0, 0, 0, 0, _handleStyle, true, true, name: "_bottomHandle");
            _leftHandle = viewModelFactory.CreateRectangleShape(0, 0, 0, 0, _handleStyle, true, true, name: "_leftHandle");
            _rightHandle = viewModelFactory.CreateRectangleShape(0, 0, 0, 0, _handleStyle, true, true, name: "_rightHandle");
            _boundsHandle = viewModelFactory.CreateRectangleShape(0, 0, 0, 0, _boundsStyle, true, false, name: "_boundsHandle");
            _rotateLine = viewModelFactory.CreateLineShape(0, 0, 0, 0, _boundsStyle, true, name: "_rotateLine");

            _handles = new List<BaseShapeViewModel>
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
            _rotateHandle.State |= ShapeStateFlags.Size | ShapeStateFlags.Thickness;
            _topLeftHandle.State |= ShapeStateFlags.Size | ShapeStateFlags.Thickness;
            _topRightHandle.State |= ShapeStateFlags.Size | ShapeStateFlags.Thickness;
            _bottomLeftHandle.State |= ShapeStateFlags.Size | ShapeStateFlags.Thickness;
            _bottomRightHandle.State |= ShapeStateFlags.Size | ShapeStateFlags.Thickness;
            _topHandle.State |= ShapeStateFlags.Size | ShapeStateFlags.Thickness;
            _bottomHandle.State |= ShapeStateFlags.Size | ShapeStateFlags.Thickness;
            _leftHandle.State |= ShapeStateFlags.Size | ShapeStateFlags.Thickness;
            _rightHandle.State |= ShapeStateFlags.Size | ShapeStateFlags.Thickness;
            _boundsHandle.State |= ShapeStateFlags.Thickness;
            _rotateLine.State |= ShapeStateFlags.Thickness;
        }
    }

    public override object Copy(IDictionary<object, object>? shared)
    {
        throw new NotImplementedException();
    }

    public override bool IsDirty()
    {
        var isDirty = base.IsDirty();

        if (_handles is { })
        {
            foreach (var handle in _handles)
            {
                isDirty |= handle.IsDirty();
            }
        }

        return isDirty;
    }

    public override void Invalidate()
    {
        base.Invalidate();

        if (_handles is { })
        {
            foreach (var handle in _handles)
            {
                handle.Invalidate();
            }
        }
    }

    public void DrawShape(object? dc, IShapeRenderer? renderer, ISelection? selection)
    {
        if (_isVisible)
        {
            if (_handles is { })
            {
                foreach (var handle in _handles)
                {
                    handle.DrawShape(dc, renderer, selection);
                }
            }
        }
    }

    public void DrawPoints(object? dc, IShapeRenderer? renderer, ISelection? selection)
    {
    }

    public bool Invalidate(IShapeRenderer? renderer)
    {
        return false;
    }

    public void Update(bool rebuild = true)
    {
        if (_layer is null || _shapes is null)
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

        if (_boundsHandle?.TopLeft is not null)
        {
            _boundsHandle.TopLeft.X = (double)_groupBox.Bounds.Left;
            _boundsHandle.TopLeft.Y = (double)_groupBox.Bounds.Top;
        }

        if (_boundsHandle?.BottomRight is not null)
        {
            _boundsHandle.BottomRight.X = (double)_groupBox.Bounds.Right;
            _boundsHandle.BottomRight.Y = (double)_groupBox.Bounds.Bottom;
        }

        if (_rotateLine?.Start is not null)
        {
            _rotateLine.Start.X = (double)_groupBox.Bounds.CenterX;
            _rotateLine.Start.Y = (double)_groupBox.Bounds.Top;
        }

        if (_rotateLine?.End is not null)
        {
            _rotateLine.End.X = (double)_groupBox.Bounds.CenterX;
            _rotateLine.End.Y = (double)(_groupBox.Bounds.Top + _rotateDistance);
        }

        if (_rotateHandle?.TopLeft is not null)
        {
            _rotateHandle.TopLeft.X = (double)(_groupBox.Bounds.CenterX - _sizeLarge);
            _rotateHandle.TopLeft.Y = (double)(_groupBox.Bounds.Top + _rotateDistance - _sizeLarge);
        }

        if (_rotateHandle?.BottomRight is not null)
        {
            _rotateHandle.BottomRight.X = (double)(_groupBox.Bounds.CenterX + _sizeLarge);
            _rotateHandle.BottomRight.Y = (double)(_groupBox.Bounds.Top + _rotateDistance + _sizeLarge);
        }

        if (_topLeftHandle?.TopLeft is not null)
        {
            _topLeftHandle.TopLeft.X = (double)(_groupBox.Bounds.Left - _sizeLarge);
            _topLeftHandle.TopLeft.Y = (double)(_groupBox.Bounds.Top - _sizeLarge);
        }

        if (_topLeftHandle?.BottomRight is not null)
        {
            _topLeftHandle.BottomRight.X = (double)(_groupBox.Bounds.Left + _sizeLarge);
            _topLeftHandle.BottomRight.Y = (double)(_groupBox.Bounds.Top + _sizeLarge);
        }

        if (_topRightHandle?.TopLeft is not null)
        {
            _topRightHandle.TopLeft.X = (double)(_groupBox.Bounds.Right - _sizeLarge);
            _topRightHandle.TopLeft.Y = (double)(_groupBox.Bounds.Top - _sizeLarge);
        }

        if (_topRightHandle?.BottomRight is not null)
        {
            _topRightHandle.BottomRight.X = (double)(_groupBox.Bounds.Right + _sizeLarge);
            _topRightHandle.BottomRight.Y = (double)(_groupBox.Bounds.Top + _sizeLarge);
        }

        if (_bottomLeftHandle?.TopLeft is not null)
        {
            _bottomLeftHandle.TopLeft.X = (double)(_groupBox.Bounds.Left - _sizeLarge);
            _bottomLeftHandle.TopLeft.Y = (double)(_groupBox.Bounds.Bottom - _sizeLarge);
        }

        if (_bottomLeftHandle?.BottomRight is not null)
        {
            _bottomLeftHandle.BottomRight.X = (double)(_groupBox.Bounds.Left + _sizeLarge);
            _bottomLeftHandle.BottomRight.Y = (double)(_groupBox.Bounds.Bottom + _sizeLarge);
        }

        if (_bottomRightHandle?.TopLeft is not null)
        {
            _bottomRightHandle.TopLeft.X = (double)(_groupBox.Bounds.Right - _sizeLarge);
            _bottomRightHandle.TopLeft.Y = (double)(_groupBox.Bounds.Bottom - _sizeLarge);
        }

        if (_bottomRightHandle?.BottomRight is not null)
        {
            _bottomRightHandle.BottomRight.X = (double)(_groupBox.Bounds.Right + _sizeLarge);
            _bottomRightHandle.BottomRight.Y = (double)(_groupBox.Bounds.Bottom + _sizeLarge);
        }

        if (_topHandle?.TopLeft is not null)
        {
            _topHandle.TopLeft.X = (double)(_groupBox.Bounds.CenterX - _sizeSmall);
            _topHandle.TopLeft.Y = (double)(_groupBox.Bounds.Top - _sizeSmall);
        }

        if (_topHandle?.BottomRight is not null)
        {
            _topHandle.BottomRight.X = (double)(_groupBox.Bounds.CenterX + _sizeSmall);
            _topHandle.BottomRight.Y = (double)(_groupBox.Bounds.Top + _sizeSmall);
        }

        if (_bottomHandle?.TopLeft is not null)
        {
            _bottomHandle.TopLeft.X = (double)(_groupBox.Bounds.CenterX - _sizeSmall);
            _bottomHandle.TopLeft.Y = (double)(_groupBox.Bounds.Bottom - _sizeSmall);
        }

        if (_bottomHandle?.BottomRight is not null)
        {
            _bottomHandle.BottomRight.X = (double)(_groupBox.Bounds.CenterX + _sizeSmall);
            _bottomHandle.BottomRight.Y = (double)(_groupBox.Bounds.Bottom + _sizeSmall);
        }

        if (_leftHandle?.TopLeft is not null)
        {
            _leftHandle.TopLeft.X = (double)(_groupBox.Bounds.Left - _sizeSmall);
            _leftHandle.TopLeft.Y = (double)(_groupBox.Bounds.CenterY - _sizeSmall);
        }

        if (_leftHandle?.BottomRight is not null)
        {
            _leftHandle.BottomRight.X = (double)(_groupBox.Bounds.Left + _sizeSmall);
            _leftHandle.BottomRight.Y = (double)(_groupBox.Bounds.CenterY + _sizeSmall);
        }

        if (_rightHandle?.TopLeft is not null)
        {
            _rightHandle.TopLeft.X = (double)(_groupBox.Bounds.Right - _sizeSmall);
            _rightHandle.TopLeft.Y = (double)(_groupBox.Bounds.CenterY - _sizeSmall);
        }

        if (_rightHandle?.BottomRight is not null)
        {
            _rightHandle.BottomRight.X = (double)(_groupBox.Bounds.Right + _sizeSmall);
            _rightHandle.BottomRight.Y = (double)(_groupBox.Bounds.CenterY + _sizeSmall);
        }

        if (_groupBox.Bounds.Height <= 0.0 || _groupBox.Bounds.Width <= 0.0)
        {
            if (_leftHandle != null) _leftHandle.State &= ~ShapeStateFlags.Visible;
            if (_rightHandle != null) _rightHandle.State &= ~ShapeStateFlags.Visible;
            if (_topHandle != null) _topHandle.State &= ~ShapeStateFlags.Visible;
            if (_bottomHandle != null) _bottomHandle.State &= ~ShapeStateFlags.Visible;
            if (_topRightHandle != null) _topRightHandle.State &= ~ShapeStateFlags.Visible;
            if (_bottomLeftHandle != null) _bottomLeftHandle.State &= ~ShapeStateFlags.Visible;
        }
        else
        {
            if (_leftHandle != null) _leftHandle.State |= ShapeStateFlags.Visible;
            if (_rightHandle != null) _rightHandle.State |= ShapeStateFlags.Visible;
            if (_topHandle != null) _topHandle.State |= ShapeStateFlags.Visible;
            if (_bottomHandle != null) _bottomHandle.State |= ShapeStateFlags.Visible;
            if (_topRightHandle != null) _topRightHandle.State |= ShapeStateFlags.Visible;
            if (_bottomLeftHandle != null) _bottomLeftHandle.State |= ShapeStateFlags.Visible;
        }

        _layer.RaiseInvalidateLayer();
    }

    public void Show()
    {
        if (_layer is null || _shapes is null)
        {
            return;
        }

        if (_isVisible)
        {
            return;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor?.PageState is { })
        {
            _previousDrawPoints = editor.PageState.DrawPoints;
            editor.PageState.DrawPoints = false;
        }

        _mode = Mode.None;
        if (_currentHandle is { })
        {
            _currentHandle.Style = _currentHandle == _boundsHandle ? _boundsStyle : _handleStyle;
            _currentHandle = null;
            _points = null;
            _rotateAngle = 0.0;
        }
        _isVisible = true;

        var shapesBuilder = _layer.Shapes.ToBuilder();
        if (_boundsHandle != null) shapesBuilder.Add(_boundsHandle);
        // TODO: if (_rotateLine != null) shapesBuilder.Add(_rotateLine);
        // TODO: if (_rotateHandle != null) shapesBuilder.Add(_rotateHandle);
        if (_topLeftHandle != null) shapesBuilder.Add(_topLeftHandle);
        if (_topRightHandle != null) shapesBuilder.Add(_topRightHandle);
        if (_bottomLeftHandle != null) shapesBuilder.Add(_bottomLeftHandle);
        if (_bottomRightHandle != null) shapesBuilder.Add(_bottomRightHandle);
        if (_topHandle != null) shapesBuilder.Add(_topHandle);
        if (_bottomHandle != null) shapesBuilder.Add(_bottomHandle);
        if (_leftHandle != null) shapesBuilder.Add(_leftHandle);
        if (_rightHandle != null) shapesBuilder.Add(_rightHandle);
        _layer.Shapes = shapesBuilder.ToImmutable();

        _layer.RaiseInvalidateLayer();
    }

    public void Hide()
    {
        if (_layer is null || _shapes is null)
        {
            return;
        }

        if (_isVisible)
        {
            var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            if (editor?.PageState is { })
            {
                editor.PageState.DrawPoints = _previousDrawPoints;
            }
        }

        _mode = Mode.None;
        if (_currentHandle is { })
        {
            _currentHandle.Style = _currentHandle == _boundsHandle ? _boundsStyle : _handleStyle;
            _currentHandle = null;
            _points = null;
            _rotateAngle = 0.0;
        }
        _isVisible = false;

        var shapesBuilder = _layer.Shapes.ToBuilder();
        if (_boundsHandle != null) shapesBuilder.Remove(_boundsHandle);
        // TODO: if (_rotateLine != null) shapesBuilder.Remove(_rotateLine);
        // TODO: if (_rotateHandle != null) shapesBuilder.Remove(_rotateHandle);
        if (_topLeftHandle != null) shapesBuilder.Remove(_topLeftHandle);
        if (_topRightHandle != null) shapesBuilder.Remove(_topRightHandle);
        if (_bottomLeftHandle != null) shapesBuilder.Remove(_bottomLeftHandle);
        if (_bottomRightHandle != null) shapesBuilder.Remove(_bottomRightHandle);
        if (_topHandle != null) shapesBuilder.Remove(_topHandle);
        if (_bottomHandle != null) shapesBuilder.Remove(_bottomHandle);
        if (_leftHandle != null) shapesBuilder.Remove(_leftHandle);
        if (_rightHandle != null) shapesBuilder.Remove(_rightHandle);
        _layer.Shapes = shapesBuilder.ToImmutable();
        _layer.RaiseInvalidateLayer();
    }

    public bool HitTest(InputArgs args)
    {
        if (_isVisible == false)
        {
            return false;
        }

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        var selection = ServiceProvider.GetService<ISelectionService>();
        var hitTest = ServiceProvider.GetService<IHitTest>();
        if (editor?.Project is null || selection is null || hitTest is null)
        {
            return false;
        }
            
        (double x, double y) = args;
        (double sx, double sy) = selection.TryToSnap(args);

        _mode = Mode.None;
        if (_currentHandle is { })
        {
            _currentHandle.Style = _currentHandle == _boundsHandle ? _boundsStyle : _handleStyle;
            _currentHandle = null;
            _points = null;
            _rotateAngle = 0.0;
            _layer?.RaiseInvalidateLayer();
        }

        var radius = editor.Project.Options is not null && editor.PageState is not null
            ? editor.Project.Options.HitThreshold / editor.PageState.ZoomX
            :  7.0;
        var handles = _handles?.Where(h => h.State.HasFlag(ShapeStateFlags.Visible));
        if (handles is null)
        {
            return false;
        }
        var result = hitTest.TryToGetShape(handles, new Point2(x, y), radius, editor.PageState?.ZoomX ?? 1.0);
        if (result is { })
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
                _points = null;
                _rotateAngle = 0.0;
                _layer?.RaiseInvalidateLayer();
                return true;
            }
        }
        return false;
    }

    public void Move(InputArgs args)
    {
        if (_layer is null || _shapes is null)
        {
            return;
        }

        if (_isVisible == false)
        {
            return;
        }

        var selection = ServiceProvider.GetService<ISelectionService>();
        if (selection is null)
        {
            return;
        }

        bool isProportionalResize = args.Modifier.HasFlag(ModifierFlags.Shift);

        (double sx, double sy) = selection.TryToSnap(args);
        double dx = sx - _startX;
        double dy = sy - _startY;
        _startX = sx;
        _startY = sy;

        if (_points is null)
        {
            _points = _groupBox.GetMovablePoints();
            if (_points is null)
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
                if (isProportionalResize)
                {
                    var width = _groupBox.Bounds.Width;
                    var height = _groupBox.Bounds.Height;
                    var ratioHeight = (height + dy) / height;

                    dx = (width * ratioHeight) - width;

                    _groupBox.ScaleLeft(dx / 2.0, _points);
                    _groupBox.ScaleRight(-dx / 2.0, _points);

                    _groupBox.ScaleTop(dy, _points);
                }
                else
                {
                    _groupBox.ScaleTop(dy, _points);
                }
            }
                break;
            case Mode.Bottom:
            {
                if (isProportionalResize)
                {
                    var width = _groupBox.Bounds.Width;
                    var height = _groupBox.Bounds.Height;
                    var ratioHeight = (height + dy) / height;

                    dx = (width * ratioHeight) - width;

                    _groupBox.ScaleLeft(-dx / 2.0, _points);
                    _groupBox.ScaleRight(dx / 2.0, _points);

                    _groupBox.ScaleBottom(dy, _points);
                }
                else
                {
                    _groupBox.ScaleBottom(dy, _points);
                }
            }
                break;
            case Mode.Left:
            {
                if (isProportionalResize)
                {
                    var width = _groupBox.Bounds.Width;
                    var height = _groupBox.Bounds.Height;
                    var ratioWidth = (width + dx) / width;

                    dy = (height * ratioWidth) - height;

                    _groupBox.ScaleTop(dy / 2.0, _points);
                    _groupBox.ScaleBottom(-dy / 2.0, _points);

                    _groupBox.ScaleLeft(dx, _points);
                }
                else
                {
                    _groupBox.ScaleLeft(dx, _points);
                }
            }
                break;
            case Mode.Right:
            {
                if (isProportionalResize)
                {
                    var width = _groupBox.Bounds.Width;
                    var height = _groupBox.Bounds.Height;
                    var ratioWidth = (width + dx) / width;

                    dy = (height * ratioWidth) - height;

                    _groupBox.ScaleTop(-dy / 2.0, _points);
                    _groupBox.ScaleBottom(dy / 2.0, _points);

                    _groupBox.ScaleRight(dx, _points);
                }
                else
                {
                    _groupBox.ScaleRight(dx, _points);
                }
            }
                break;
            case Mode.TopLeft:
            {
                if (isProportionalResize)
                {
                    var width = _groupBox.Bounds.Width;
                    var height = _groupBox.Bounds.Height;

                    var ratioWidth = (width + dx) / width;
                    dy = (height * ratioWidth) - height;

                    //var ratioHeight = (height + dy) / height;
                    //dx = (width * ratioHeight) - width;

                    _groupBox.ScaleTop(dy, _points);
                    _groupBox.ScaleLeft(dx, _points);
                }
                else
                {
                    _groupBox.ScaleTop(dy, _points);
                    _groupBox.ScaleLeft(dx, _points);
                }
            }
                break;
            case Mode.TopRight:
            {
                if (isProportionalResize)
                {
                    var width = _groupBox.Bounds.Width;
                    var height = _groupBox.Bounds.Height;

                    var ratioWidth = (width - dx) / width;
                    dy = (height * ratioWidth) - height;

                    //var ratioHeight = (height - dy) / height;
                    //dx = (width * ratioHeight) - width;

                    _groupBox.ScaleTop(dy, _points);
                    _groupBox.ScaleRight(dx, _points);
                }
                else
                {
                    _groupBox.ScaleTop(dy, _points);
                    _groupBox.ScaleRight(dx, _points);
                }
            }
                break;
            case Mode.BottomLeft:
            {
                if (isProportionalResize)
                {
                    var width = _groupBox.Bounds.Width;
                    var height = _groupBox.Bounds.Height;

                    var ratioWidth = (width - dx) / width;
                    dy = (height * ratioWidth) - height;

                    //var ratioHeight = (height - dy) / height;
                    //dx = (width * ratioHeight) - width;

                    _groupBox.ScaleBottom(dy, _points);
                    _groupBox.ScaleLeft(dx, _points);
                }
                else
                {
                    _groupBox.ScaleBottom(dy, _points);
                    _groupBox.ScaleLeft(dx, _points);
                }
            }
                break;
            case Mode.BottomRight:
            {
                if (isProportionalResize)
                {
                    var width = _groupBox.Bounds.Width;
                    var height = _groupBox.Bounds.Height;

                    var ratioWidth = (width + dx) / width;
                    dy = (height * ratioWidth) - height;

                    //var ratioHeight = (height + dy) / height;
                    //dx = (width * ratioHeight) - width;

                    _groupBox.ScaleBottom(dy, _points);
                    _groupBox.ScaleRight(dx, _points);
                }
                else
                {
                    _groupBox.ScaleBottom(dy, _points);
                    _groupBox.ScaleRight(dx, _points);
                }
            }
                break;
        }
    }
}
