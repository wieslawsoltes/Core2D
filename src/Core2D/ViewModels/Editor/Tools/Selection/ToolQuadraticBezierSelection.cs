using System;
using Core2D;
using Core2D.Containers;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools.Selection
{
    public class ToolQuadraticBezierSelection
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly LayerContainer _layer;
        private readonly QuadraticBezierShape _quadraticBezier;
        private readonly ShapeStyle _style;
        private LineShape _line12;
        private LineShape _line32;
        private PointShape _helperPoint1;
        private PointShape _helperPoint2;
        private PointShape _helperPoint3;

        public ToolQuadraticBezierSelection(IServiceProvider serviceProvider, LayerContainer layer, QuadraticBezierShape shape, ShapeStyle style)
        {
            _serviceProvider = serviceProvider;
            _layer = layer;
            _quadraticBezier = shape;
            _style = style;
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
            _line12 = _serviceProvider.GetService<IFactory>().CreateLineShape(0, 0, _style);
            _line12.State |= ShapeStateFlags.Thickness;

            _line32 = _serviceProvider.GetService<IFactory>().CreateLineShape(0, 0, _style);
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
