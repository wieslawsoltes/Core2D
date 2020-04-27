using System;
using Core2D;
using Core2D.Containers;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools.Selection
{
    /// <summary>
    /// Helper class for <see cref="IQuadraticBezierShape"/> shape selection.
    /// </summary>
    public class ToolQuadraticBezierSelection
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILayerContainer _layer;
        private readonly IQuadraticBezierShape _quadraticBezier;
        private readonly IShapeStyle _style;
        private ILineShape _line12;
        private ILineShape _line32;
        private IPointShape _helperPoint1;
        private IPointShape _helperPoint2;
        private IPointShape _helperPoint3;

        /// <summary>
        /// Initialize new instance of <see cref="ToolQuadraticBezierSelection"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="layer">The selection shapes layer.</param>
        /// <param name="shape">The selected shape.</param>
        /// <param name="style">The selection shapes style.</param>
        /// <param name="point">The selection point shape.</param>
        public ToolQuadraticBezierSelection(IServiceProvider serviceProvider, ILayerContainer layer, IQuadraticBezierShape shape, IShapeStyle style)
        {
            _serviceProvider = serviceProvider;
            _layer = layer;
            _quadraticBezier = shape;
            _style = style;
        }

        /// <summary>
        /// Transfer selection state to Point3.
        /// </summary>
        public void ToStatePoint3()
        {
            _helperPoint1 = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0);
            _helperPoint3 = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0);

            _layer.Shapes = _layer.Shapes.Add(_helperPoint1);
            _layer.Shapes = _layer.Shapes.Add(_helperPoint3);
        }

        /// <summary>
        /// Transfer selection state to Point2.
        /// </summary>
        public void ToStatePoint2()
        {
            _line12 = _serviceProvider.GetService<IFactory>().CreateLineShape(0, 0, _style);
            _line12.State.Flags |= ShapeStateFlags.Thickness;

            _line32 = _serviceProvider.GetService<IFactory>().CreateLineShape(0, 0, _style);
            _line32.State.Flags |= ShapeStateFlags.Thickness;

            _helperPoint2 = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0);

            _layer.Shapes = _layer.Shapes.Add(_line12);
            _layer.Shapes = _layer.Shapes.Add(_line32);
            _layer.Shapes = _layer.Shapes.Add(_helperPoint2);
        }

        /// <summary>
        /// Move selection.
        /// </summary>
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

            _layer.Invalidate();
        }

        /// <summary>
        /// Reset selection.
        /// </summary>
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

            _layer.Invalidate();
        }
    }
}
