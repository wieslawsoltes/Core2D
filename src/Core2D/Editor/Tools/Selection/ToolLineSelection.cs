using System;
using Core2D.Containers;
using Core2D.Interfaces;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Editor.Tools.Selection
{
    /// <summary>
    /// Helper class for <see cref="ILineShape"/> shape selection.
    /// </summary>
    public class ToolLineSelection
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILayerContainer _layer;
        private readonly ILineShape _line;
        private readonly IShapeStyle _style;
        private IPointShape _startHelperPoint;
        private IPointShape _endHelperPoint;

        /// <summary>
        /// Initialize new instance of <see cref="ToolLineSelection"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        /// <param name="layer">The selection shapes layer.</param>
        /// <param name="shape">The selected shape.</param>
        /// <param name="style">The selection shapes style.</param>
        public ToolLineSelection(IServiceProvider serviceProvider, ILayerContainer layer, ILineShape shape, IShapeStyle style)
        {
            _serviceProvider = serviceProvider;
            _layer = layer;
            _line = shape;
            _style = style;
        }

        /// <summary>
        /// Transfer selection state to End.
        /// </summary>
        public void ToStateEnd()
        {
            _startHelperPoint = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0);
            _endHelperPoint = _serviceProvider.GetService<IFactory>().CreatePointShape(0, 0);

            _layer.Shapes = _layer.Shapes.Add(_startHelperPoint);
            _layer.Shapes = _layer.Shapes.Add(_endHelperPoint);
        }

        /// <summary>
        /// Move selection.
        /// </summary>
        public void Move()
        {
            if (_startHelperPoint != null)
            {
                _startHelperPoint.X = _line.Start.X;
                _startHelperPoint.Y = _line.Start.Y;
            }

            if (_endHelperPoint != null)
            {
                _endHelperPoint.X = _line.End.X;
                _endHelperPoint.Y = _line.End.Y;
            }

            _layer.Invalidate();
        }

        /// <summary>
        /// Reset selection.
        /// </summary>
        public void Reset()
        {
            if (_startHelperPoint != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_startHelperPoint);
                _startHelperPoint = null;
            }

            if (_endHelperPoint != null)
            {
                _layer.Shapes = _layer.Shapes.Remove(_endHelperPoint);
                _endHelperPoint = null;
            }

            _layer.Invalidate();
        }
    }
}
