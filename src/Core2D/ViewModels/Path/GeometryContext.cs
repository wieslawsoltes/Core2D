// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using Core2D.Model;
using Core2D.Model.Path;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Path;

public class GeometryContext
{
    private readonly IViewModelFactory _viewModelFactory;
    private readonly PathShapeViewModel _path;
    private PathFigureViewModel? _currentFigure;

    public GeometryContext(IViewModelFactory viewModelFactory, PathShapeViewModel path)
    {
        _viewModelFactory = viewModelFactory;
        _path = path ?? throw new ArgumentNullException(nameof(path));
    }

    public void BeginFigure(PointShapeViewModel startPoint, bool isClosed = true)
    {
        _currentFigure = _viewModelFactory.CreatePathFigure(startPoint, isClosed);
        _path.Figures = _path.Figures.Add(_currentFigure);
    }

    public void SetClosedState(bool isClosed)
    {
        if (_currentFigure is not null)
        {
            _currentFigure.IsClosed = isClosed;
        }
    }

    public void LineTo(PointShapeViewModel point)
    {
        if (_currentFigure is not null)
        {
            var segment = _viewModelFactory.CreateLineSegment(point);
            _currentFigure.Segments = _currentFigure.Segments.Add(segment);
        }
    }

    public void ArcTo(PointShapeViewModel point, PathSizeViewModel size, double rotationAngle = 0.0, bool isLargeArc = false, SweepDirection sweepDirection = SweepDirection.Clockwise)
    {
        if (_currentFigure is not null)
        {
            var segment = _viewModelFactory.CreateArcSegment(
                point,
                size,
                rotationAngle,
                isLargeArc,
                sweepDirection);
            _currentFigure.Segments = _currentFigure.Segments.Add(segment);
        }
    }

    public void CubicBezierTo(PointShapeViewModel point1, PointShapeViewModel point2, PointShapeViewModel point3)
    {
        if (_currentFigure is not null)
        {
            var segment = _viewModelFactory.CreateCubicBezierSegment(
                point1,
                point2,
                point3);
            _currentFigure.Segments = _currentFigure.Segments.Add(segment);
        }
    }

    public void QuadraticBezierTo(PointShapeViewModel point1, PointShapeViewModel point2)
    {
        if (_currentFigure is not null)
        {
            var segment = _viewModelFactory.CreateQuadraticBezierSegment(
                point1,
                point2);
            _currentFigure.Segments = _currentFigure.Segments.Add(segment);
        }
    }
}
