using System;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Editor.Tools.Selection
{
    public class QuadraticBezierSelection
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly LayerContainerViewModel _layer;
        private readonly QuadraticBezierShapeViewModel _quadraticBezier;
        private readonly ShapeStyleViewModel _styleViewModel;
        private LineShapeViewModel _line12;
        private LineShapeViewModel _line32;
        private PointShapeViewModel _helperPoint1;
        private PointShapeViewModel _helperPoint2;
        private PointShapeViewModel _helperPoint3;

        public QuadraticBezierSelection(IServiceProvider serviceProvider, LayerContainerViewModel layer, QuadraticBezierShapeViewModel shape, ShapeStyleViewModel style)
        {
            _serviceProvider = serviceProvider;
            _layer = layer;
            _quadraticBezier = shape;
            _styleViewModel = style;
        }

        public void ToStatePoint3()
        {
            _helperPoint1 = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0);
            _helperPoint3 = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0);

            _layer.Shapes = _layer.Shapes.Add(_helperPoint1);
            _layer.Shapes = _layer.Shapes.Add(_helperPoint3);
        }

        public void ToStatePoint2()
        {
            _line12 = _serviceProvider.GetService<IFactory>().CreateLineShape(0, 0, _styleViewModel);
            _line12.State |= ShapeStateFlags.Thickness;

            _line32 = _serviceProvider.GetService<IFactory>().CreateLineShape(0, 0, _styleViewModel);
            _line32.State |= ShapeStateFlags.Thickness;

            _helperPoint2 = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0);

            _layer.Shapes = _layer.Shapes.Add(_line12);
            _layer.Shapes = _layer.Shapes.Add(_line32);
            _layer.Shapes = _layer.Shapes.Add(_helperPoint2);
        }

        public void Move()
        {
            if (_line12 != null)
            {
                _line12.Start.X = _quadraticBezier.Point1.X;
                _line12.Start.Y = _quadraticBezier.Point1.Y;
                _line12.End.X = _quadraticBezier.Point2.X;
                _line12.End.Y = _quadraticBezier.Point2.Y;
            }

            if (_line32 != null)
            {
                _line32.Start.X = _quadraticBezier.Point3.X;
                _line32.Start.Y = _quadraticBezier.Point3.Y;
                _line32.End.X = _quadraticBezier.Point2.X;
                _line32.End.Y = _quadraticBezier.Point2.Y;
            }

            if (_helperPoint1 != null)
            {
                _helperPoint1.X = _quadraticBezier.Point1.X;
                _helperPoint1.Y = _quadraticBezier.Point1.Y;
            }

            if (_helperPoint2 != null)
            {
                _helperPoint2.X = _quadraticBezier.Point2.X;
                _helperPoint2.Y = _quadraticBezier.Point2.Y;
            }

            if (_helperPoint3 != null)
            {
                _helperPoint3.X = _quadraticBezier.Point3.X;
                _helperPoint3.Y = _quadraticBezier.Point3.Y;
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

            if (_line32 != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_line32);
                _line32 = null;
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

            _layer.InvalidateLayer();
        }
    }
}
