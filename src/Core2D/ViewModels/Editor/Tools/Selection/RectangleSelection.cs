#nullable enable
using System;
using Core2D.Model;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor.Tools.Selection;

public class RectangleSelection
{
    private readonly IServiceProvider? _serviceProvider;
    private readonly LayerContainerViewModel _layer;
    private readonly RectangleShapeViewModel _rectangle;
    private readonly ShapeStyleViewModel _styleViewModel;
    private PointShapeViewModel? _topLeftHelperPoint;
    private PointShapeViewModel? _bottomRightHelperPoint;

    public RectangleSelection(IServiceProvider? serviceProvider, LayerContainerViewModel layer, RectangleShapeViewModel shape, ShapeStyleViewModel style)
    {
        _serviceProvider = serviceProvider;
        _layer = layer;
        _rectangle = shape;
        _styleViewModel = style;
    }

    public void ToStateBottomRight()
    {
        _topLeftHelperPoint = _serviceProvider.GetService<IViewModelFactory>()?.CreatePointShape();
        _bottomRightHelperPoint = _serviceProvider.GetService<IViewModelFactory>()?.CreatePointShape();

        if (_topLeftHelperPoint is { })
        {
            _layer.Shapes = _layer.Shapes.Add(_topLeftHelperPoint);
        }

        if (_bottomRightHelperPoint is { })
        {
            _layer.Shapes = _layer.Shapes.Add(_bottomRightHelperPoint);
        }
    }

    public void Move()
    {
        if (_topLeftHelperPoint is { } && _rectangle.TopLeft is { })
        {
            _topLeftHelperPoint.X = _rectangle.TopLeft.X;
            _topLeftHelperPoint.Y = _rectangle.TopLeft.Y;
        }

        if (_bottomRightHelperPoint is { } && _rectangle.BottomRight is { })
        {
            _bottomRightHelperPoint.X = _rectangle.BottomRight.X;
            _bottomRightHelperPoint.Y = _rectangle.BottomRight.Y;
        }

        _layer.RaiseInvalidateLayer();
    }

    public void Reset()
    {
        if (_topLeftHelperPoint is { })
        {
            _layer.Shapes = _layer.Shapes.Remove(_topLeftHelperPoint);
            _topLeftHelperPoint = null;
        }

        if (_bottomRightHelperPoint is { })
        {
            _layer.Shapes = _layer.Shapes.Remove(_bottomRightHelperPoint);
            _bottomRightHelperPoint = null;
        }

        _layer.RaiseInvalidateLayer();
    }
}