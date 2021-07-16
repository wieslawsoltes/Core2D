#nullable disable
using System;
using Core2D.Model;
using Core2D.Model.Path;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Path
{
    public class GeometryContext
    {
        private readonly IViewModelFactory _viewModelFactory;
        private readonly PathGeometryViewModel _geometry;
        private PathFigureViewModel _currentFigure;

        public GeometryContext(IViewModelFactory viewModelFactory, PathGeometryViewModel geometry)
        {
            _viewModelFactory = viewModelFactory;
            _geometry = geometry ?? throw new ArgumentNullException(nameof(geometry));
        }

        public void BeginFigure(PointShapeViewModel startPoint, bool isClosed = true)
        {
            _currentFigure = _viewModelFactory.CreatePathFigure(startPoint, isClosed);
            _geometry.Figures = _geometry.Figures.Add(_currentFigure);
        }

        public void SetClosedState(bool isClosed)
        {
            _currentFigure.IsClosed = isClosed;
        }

        public void LineTo(PointShapeViewModel point)
        {
            var segment = _viewModelFactory.CreateLineSegment(
                point);
            _currentFigure.Segments = _currentFigure.Segments.Add(segment);
        }

        public void ArcTo(PointShapeViewModel point, PathSizeViewModel size, double rotationAngle = 0.0, bool isLargeArc = false, SweepDirection sweepDirection = SweepDirection.Clockwise)
        {
            var segment = _viewModelFactory.CreateArcSegment(
                point,
                size,
                rotationAngle,
                isLargeArc,
                sweepDirection);
            _currentFigure.Segments = _currentFigure.Segments.Add(segment);
        }

        public void CubicBezierTo(PointShapeViewModel point1, PointShapeViewModel point2, PointShapeViewModel point3)
        {
            var segment = _viewModelFactory.CreateCubicBezierSegment(
                point1,
                point2,
                point3);
            _currentFigure.Segments = _currentFigure.Segments.Add(segment);
        }

        public void QuadraticBezierTo(PointShapeViewModel point1, PointShapeViewModel point2)
        {
            var segment = _viewModelFactory.CreateQuadraticBezierSegment(
                point1,
                point2);
            _currentFigure.Segments = _currentFigure.Segments.Add(segment);
        }
    }
}
