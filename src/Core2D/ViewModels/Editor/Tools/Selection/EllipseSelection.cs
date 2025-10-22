// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using Core2D.Model;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor.Tools.Selection;

public class EllipseSelection
{
    private readonly IServiceProvider? _serviceProvider;
    private readonly LayerContainerViewModel _layer;
    private readonly EllipseShapeViewModel _ellipse;
    private readonly ShapeStyleViewModel _styleViewModel;
    private PointShapeViewModel? _topLeftHelperPoint;
    private PointShapeViewModel? _bottomRightHelperPoint;

    public EllipseSelection(IServiceProvider? serviceProvider, LayerContainerViewModel layer, EllipseShapeViewModel shape, ShapeStyleViewModel style)
    {
        _serviceProvider = serviceProvider;
        _layer = layer;
        _ellipse = shape;
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
        if (_topLeftHelperPoint is { } && _ellipse.TopLeft is { })
        {
            _topLeftHelperPoint.X = _ellipse.TopLeft.X;
            _topLeftHelperPoint.Y = _ellipse.TopLeft.Y;
        }

        if (_bottomRightHelperPoint is { } && _ellipse.BottomRight is { })
        {
            _bottomRightHelperPoint.X = _ellipse.BottomRight.X;
            _bottomRightHelperPoint.Y = _ellipse.BottomRight.Y;
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
