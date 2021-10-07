#nullable enable
using System;
using Core2D.Model;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor.Tools.Selection;

public class ImageSelection
{
    private readonly IServiceProvider? _serviceProvider;
    private readonly LayerContainerViewModel _layer;
    private readonly ImageShapeViewModel _image;
    private readonly ShapeStyleViewModel _styleViewModel;
    private PointShapeViewModel? _topLeftHelperPoint;
    private PointShapeViewModel? _bottomRightHelperPoint;

    public ImageSelection(IServiceProvider? serviceProvider, LayerContainerViewModel layer, ImageShapeViewModel shape, ShapeStyleViewModel style)
    {
        _serviceProvider = serviceProvider;
        _layer = layer;
        _image = shape;
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
        if (_topLeftHelperPoint is { } && _image.TopLeft is { })
        {
            _topLeftHelperPoint.X = _image.TopLeft.X;
            _topLeftHelperPoint.Y = _image.TopLeft.Y;
        }

        if (_bottomRightHelperPoint is { } && _image.BottomRight is { })
        {
            _bottomRightHelperPoint.X = _image.BottomRight.X;
            _bottomRightHelperPoint.Y = _image.BottomRight.Y;
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