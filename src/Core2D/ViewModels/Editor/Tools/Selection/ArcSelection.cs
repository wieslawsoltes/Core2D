// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using Core2D.Spatial;
using Core2D.Spatial.Arc;

namespace Core2D.ViewModels.Editor.Tools.Selection;

public class ArcSelection
{
    private readonly IServiceProvider? _serviceProvider;
    private readonly LayerContainerViewModel _layer;
    private readonly ArcShapeViewModel _arc;
    private readonly ShapeStyleViewModel _styleViewModel;
    private LineShapeViewModel? _startLine;
    private LineShapeViewModel? _endLine;
    private EllipseShapeViewModel? _ellipse;
    private PointShapeViewModel? _p1HelperPoint;
    private PointShapeViewModel? _p2HelperPoint;
    private PointShapeViewModel? _centerHelperPoint;
    private PointShapeViewModel? _startHelperPoint;
    private PointShapeViewModel? _endHelperPoint;

    public ArcSelection(IServiceProvider? serviceProvider, LayerContainerViewModel layer, ArcShapeViewModel shapeViewModel, ShapeStyleViewModel style)
    {
        _serviceProvider = serviceProvider;
        _layer = layer;
        _arc = shapeViewModel;
        _styleViewModel = style;
    }

    public void ToStatePoint2()
    {
        _ellipse = _serviceProvider.GetService<IViewModelFactory>()?.CreateEllipseShape(0, 0, _styleViewModel);
        if (_ellipse is { })
        {
            _ellipse.State |= ShapeStateFlags.Thickness;
        }

        _p1HelperPoint = _serviceProvider.GetService<IViewModelFactory>()?.CreatePointShape();
        _p2HelperPoint = _serviceProvider.GetService<IViewModelFactory>()?.CreatePointShape();
        _centerHelperPoint = _serviceProvider.GetService<IViewModelFactory>()?.CreatePointShape();

        if (_ellipse is { })
        {
            _layer.Shapes = _layer.Shapes.Add(_ellipse);
        }

        if (_p1HelperPoint is { })
        {
            _layer.Shapes = _layer.Shapes.Add(_p1HelperPoint);
        }

        if (_p2HelperPoint is { })
        {
            _layer.Shapes = _layer.Shapes.Add(_p2HelperPoint);
        }

        if (_centerHelperPoint is { })
        {
            _layer.Shapes = _layer.Shapes.Add(_centerHelperPoint);
        }
    }

    public void ToStatePoint3()
    {
        if (_p1HelperPoint is { })
        {
            _layer.Shapes = _layer.Shapes.Remove(_p1HelperPoint);
            _p1HelperPoint = null;
        }

        if (_p2HelperPoint is { })
        {
            _layer.Shapes = _layer.Shapes.Remove(_p2HelperPoint);
            _p2HelperPoint = null;
        }

        _startLine = _serviceProvider.GetService<IViewModelFactory>()?.CreateLineShape(0, 0, _styleViewModel);
        if (_startLine is { })
        {
            _startLine.State |= ShapeStateFlags.Thickness;
        }

        _startHelperPoint = _serviceProvider.GetService<IViewModelFactory>()?.CreatePointShape();

        if (_startLine is { })
        {
            _layer.Shapes = _layer.Shapes.Add(_startLine);
        }

        if (_startHelperPoint is { })
        {
            _layer.Shapes = _layer.Shapes.Add(_startHelperPoint);
        }
    }

    public void ToStatePoint4()
    {
        if (_ellipse is { })
        {
            _layer.Shapes = _layer.Shapes.Remove(_ellipse);
            _ellipse = null;
        }

        _endLine = _serviceProvider.GetService<IViewModelFactory>()?.CreateLineShape(0, 0, _styleViewModel);
        if (_endLine is { })
        {
            _endLine.State |= ShapeStateFlags.Thickness;
        }

        _endHelperPoint = _serviceProvider.GetService<IViewModelFactory>()?.CreatePointShape();

        if (_endLine is { })
        {
            _layer.Shapes = _layer.Shapes.Add(_endLine);
        }

        if (_endHelperPoint is { })
        {
            _layer.Shapes = _layer.Shapes.Add(_endHelperPoint);
        }
    }

    public void Move()
    {
        if (_arc.Point1 is null || _arc.Point2 is null || _arc.Point3 is null || _arc.Point4 is null)
        {
            return;
        }
            
        var a = new WpfArc(
            Point2.FromXY(_arc.Point1.X, _arc.Point1.Y),
            Point2.FromXY(_arc.Point2.X, _arc.Point2.Y),
            Point2.FromXY(_arc.Point3.X, _arc.Point3.Y),
            Point2.FromXY(_arc.Point4.X, _arc.Point4.Y));

        if (_ellipse?.TopLeft is { } && _ellipse?.BottomRight is { })
        {
            _ellipse.TopLeft.X = a.P1.X;
            _ellipse.TopLeft.Y = a.P1.Y;
            _ellipse.BottomRight.X = a.P2.X;
            _ellipse.BottomRight.Y = a.P2.Y;
        }

        if (_startLine?.Start is { } && _startLine?.End is { })
        {
            _startLine.Start.X = a.Center.X;
            _startLine.Start.Y = a.Center.Y;
            _startLine.End.X = a.Start.X;
            _startLine.End.Y = a.Start.Y;
        }

        if (_endLine?.Start is { } && _endLine?.End is { })
        {
            _endLine.Start.X = a.Center.X;
            _endLine.Start.Y = a.Center.Y;
            _endLine.End.X = a.End.X;
            _endLine.End.Y = a.End.Y;
        }

        if (_p1HelperPoint is { })
        {
            _p1HelperPoint.X = a.P1.X;
            _p1HelperPoint.Y = a.P1.Y;
        }

        if (_p2HelperPoint is { })
        {
            _p2HelperPoint.X = a.P2.X;
            _p2HelperPoint.Y = a.P2.Y;
        }

        if (_centerHelperPoint is { })
        {
            _centerHelperPoint.X = a.Center.X;
            _centerHelperPoint.Y = a.Center.Y;
        }

        if (_startHelperPoint is { })
        {
            _startHelperPoint.X = a.Start.X;
            _startHelperPoint.Y = a.Start.Y;
        }

        if (_endHelperPoint is { })
        {
            _endHelperPoint.X = a.End.X;
            _endHelperPoint.Y = a.End.Y;
        }

        _layer.RaiseInvalidateLayer();
    }

    public void Reset()
    {
        if (_ellipse is { })
        {
            _layer.Shapes = _layer.Shapes.Remove(_ellipse);
            _ellipse = null;
        }

        if (_startLine is { })
        {
            _layer.Shapes = _layer.Shapes.Remove(_startLine);
            _startLine = null;
        }

        if (_endLine is { })
        {
            _layer.Shapes = _layer.Shapes.Remove(_endLine);
            _endLine = null;
        }

        if (_p1HelperPoint is { })
        {
            _layer.Shapes = _layer.Shapes.Remove(_p1HelperPoint);
            _p1HelperPoint = null;
        }

        if (_p2HelperPoint is { })
        {
            _layer.Shapes = _layer.Shapes.Remove(_p2HelperPoint);
            _p2HelperPoint = null;
        }

        if (_centerHelperPoint is { })
        {
            _layer.Shapes = _layer.Shapes.Remove(_centerHelperPoint);
            _centerHelperPoint = null;
        }

        if (_startHelperPoint is { })
        {
            _layer.Shapes = _layer.Shapes.Remove(_startHelperPoint);
            _startHelperPoint = null;
        }

        if (_endHelperPoint is { })
        {
            _layer.Shapes = _layer.Shapes.Remove(_endHelperPoint);
            _endHelperPoint = null;
        }

        _layer.RaiseInvalidateLayer();
    }
}
