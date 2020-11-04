using System;
using Core2D;
using Core2D.Containers;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools.Selection
{
    public class ToolCubicBezierSelection
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly LayerContainer _layer;
        private readonly CubicBezierShape _cubicBezier;
        private readonly ShapeStyle _style;
        private LineShape _line12;
        private LineShape _line43;
        private LineShape _line23;
        private PointShape _helperPoint1;
        private PointShape _helperPoint2;
        private PointShape _helperPoint3;
        private PointShape _helperPoint4;

        public ToolCubicBezierSelection(IServiceProvider serviceProvider, LayerContainer layer, CubicBezierShape shape, ShapeStyle style)
        {
            _serviceProvider = serviceProvider;
            _layer = layer;
            _cubicBezier = shape;
            _style = style;
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
            _line12 = _serviceProvider.GetService<IFactory>().CreateLineShape(0, 0, _style);
            _line12.State.Flags |= ShapeStateFlags.Thickness;

            _helperPoint2 = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0);

            _layer.Shapes = _layer.Shapes.Add(_line12);
            _layer.Shapes = _layer.Shapes.Add(_helperPoint2);
        }

        public void ToStatePoint3()
        {
            _line43 = _serviceProvider.GetService<IFactory>().CreateLineShape(0, 0, _style);
            _line43.State.Flags |= ShapeStateFlags.Thickness;

            _line23 = _serviceProvider.GetService<IFactory>().CreateLineShape(0, 0, _style);
            _line23.State.Flags |= ShapeStateFlags.Thickness;

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
