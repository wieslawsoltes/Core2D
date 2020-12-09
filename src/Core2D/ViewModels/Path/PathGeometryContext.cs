using System;
using Core2D.Shapes;

namespace Core2D.Path
{
    public partial class GeometryContext
    {
        private readonly IFactory _factory;
        private readonly PathGeometryViewModel _geometryViewModel;
        private PathFigureViewModel _currentFigureViewModel;

        public GeometryContext(IFactory factory, PathGeometryViewModel geometryViewModel)
        {
            _factory = factory;
            _geometryViewModel = geometryViewModel ?? throw new ArgumentNullException(nameof(geometryViewModel));
        }

        public void BeginFigure(PointShapeViewModel startPoint, bool isClosed = true)
        {
            _currentFigureViewModel = _factory.CreatePathFigure(startPoint, isClosed);
            _geometryViewModel.Figures = _geometryViewModel.Figures.Add(_currentFigureViewModel);
        }

        public void SetClosedState(bool isClosed)
        {
            _currentFigureViewModel.IsClosed = isClosed;
        }

        public void LineTo(PointShapeViewModel point)
        {
            var segment = _factory.CreateLineSegment(
                point);
            _currentFigureViewModel.Segments = _currentFigureViewModel.Segments.Add(segment);
        }

        public void ArcTo(PointShapeViewModel point, PathSize size, double rotationAngle = 0.0, bool isLargeArc = false, SweepDirection sweepDirection = SweepDirection.Clockwise)
        {
            var segment = _factory.CreateArcSegment(
                point,
                size,
                rotationAngle,
                isLargeArc,
                sweepDirection);
            _currentFigureViewModel.Segments = _currentFigureViewModel.Segments.Add(segment);
        }

        public void CubicBezierTo(PointShapeViewModel point1, PointShapeViewModel point2, PointShapeViewModel point3)
        {
            var segment = _factory.CreateCubicBezierSegment(
                point1,
                point2,
                point3);
            _currentFigureViewModel.Segments = _currentFigureViewModel.Segments.Add(segment);
        }

        public void QuadraticBezierTo(PointShapeViewModel point1, PointShapeViewModel point2)
        {
            var segment = _factory.CreateQuadraticBezierSegment(
                point1,
                point2);
            _currentFigureViewModel.Segments = _currentFigureViewModel.Segments.Add(segment);
        }
    }
}
