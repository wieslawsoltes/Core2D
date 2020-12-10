using System;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor.Tools.Selection
{
    public class BezierSelectionSelection
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly LayerContainerViewModel _layer;
        private readonly CubicBezierShapeViewModel _cubicBezier;
        private readonly ShapeStyleViewModel _styleViewModel;
        private LineShapeViewModel _line12;
        private LineShapeViewModel _line43;
        private LineShapeViewModel _line23;
        private PointShapeViewModel _helperPoint1;
        private PointShapeViewModel _helperPoint2;
        private PointShapeViewModel _helperPoint3;
        private PointShapeViewModel _helperPoint4;

        public BezierSelectionSelection(IServiceProvider serviceProvider, LayerContainerViewModel layer, CubicBezierShapeViewModel shape, ShapeStyleViewModel style)
        {
            _serviceProvider = serviceProvider;
            _layer = layer;
            _cubicBezier = shape;
            _styleViewModel = style;
        }

        public void ToStatePoint4()
        {
            _helperPoint1 = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0);
            _layer.Shapes = _layer.Shapes.Add(_helperPoint1);
            _helperPoint4 = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0);
            _layer.Shapes = _layer.Shapes.Add(_helperPoint4);
        }

        public void ToStatePoint2()
        {
            _line12 = _serviceProvider.GetService<IFactory>().CreateLineShape(0, 0, _styleViewModel);
            _line12.State |= ShapeStateFlags.Thickness;

            _helperPoint2 = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0);

            _layer.Shapes = _layer.Shapes.Add(_line12);
            _layer.Shapes = _layer.Shapes.Add(_helperPoint2);
        }

        public void ToStatePoint3()
        {
            _line43 = _serviceProvider.GetService<IFactory>().CreateLineShape(0, 0, _styleViewModel);
            _line43.State |= ShapeStateFlags.Thickness;

            _line23 = _serviceProvider.GetService<IFactory>().CreateLineShape(0, 0, _styleViewModel);
            _line23.State |= ShapeStateFlags.Thickness;

            _helperPoint3 = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0);

            _layer.Shapes = _layer.Shapes.Add(_line43);
            _layer.Shapes = _layer.Shapes.Add(_line23);
            _layer.Shapes = _layer.Shapes.Add(_helperPoint3);
        }

        public void Move()
        {
            if (_line12 != null)
            {
                _line12.Start.X = _cubicBezier.Point1.X;
                _line12.Start.Y = _cubicBezier.Point1.Y;
                _line12.End.X = _cubicBezier.Point2.X;
                _line12.End.Y = _cubicBezier.Point2.Y;
            }

            if (_line43 != null)
            {
                _line43.Start.X = _cubicBezier.Point4.X;
                _line43.Start.Y = _cubicBezier.Point4.Y;
                _line43.End.X = _cubicBezier.Point3.X;
                _line43.End.Y = _cubicBezier.Point3.Y;
            }

            if (_line23 != null)
            {
                _line23.Start.X = _cubicBezier.Point2.X;
                _line23.Start.Y = _cubicBezier.Point2.Y;
                _line23.End.X = _cubicBezier.Point3.X;
                _line23.End.Y = _cubicBezier.Point3.Y;
            }

            if (_helperPoint1 != null)
            {
                _helperPoint1.X = _cubicBezier.Point1.X;
                _helperPoint1.Y = _cubicBezier.Point1.Y;
            }

            if (_helperPoint2 != null)
            {
                _helperPoint2.X = _cubicBezier.Point2.X;
                _helperPoint2.Y = _cubicBezier.Point2.Y;
            }

            if (_helperPoint3 != null)
            {
                _helperPoint3.X = _cubicBezier.Point3.X;
                _helperPoint3.Y = _cubicBezier.Point3.Y;
            }

            if (_helperPoint4 != null)
            {
                _helperPoint4.X = _cubicBezier.Point4.X;
                _helperPoint4.Y = _cubicBezier.Point4.Y;
            }

            _layer.InvalidateLayer();
        }

        public void Reset()
        {
            if (_line12 != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_line12);
                _line12 = null;
            }

            if (_line43 != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_line43);
                _line43 = null;
            }

            if (_line23 != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_line23);
                _line23 = null;
            }

            if (_helperPoint1 != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_helperPoint1);
                _helperPoint1 = null;
            }

            if (_helperPoint2 != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_helperPoint2);
                _helperPoint2 = null;
            }

            if (_helperPoint3 != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_helperPoint3);
                _helperPoint3 = null;
            }

            if (_helperPoint4 != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_helperPoint4);
                _helperPoint4 = null;
            }

            _layer.InvalidateLayer();
        }
    }
}
