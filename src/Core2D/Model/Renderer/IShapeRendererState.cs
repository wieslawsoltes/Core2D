using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Renderer
{
    /// <summary>
    /// Defines shape renderer state contract.
    /// </summary>
    public interface IShapeRendererState : IObservableObject, ISelection
    {
        /// <summary>
        /// The X coordinate of current pan position.
        /// </summary>
        double PanX { get; set; }

        /// <summary>
        /// The Y coordinate of current pan position.
        /// </summary>
        double PanY { get; set; }

        /// <summary>
        /// The X component of current zoom value.
        /// </summary>
        double ZoomX { get; set; }

        /// <summary>
        /// The Y component of current zoom value.
        /// </summary>
        double ZoomY { get; set; }

        /// <summary>
        /// Flag indicating shape state to enable its drawing.
        /// </summary>
        IShapeState DrawShapeState { get; set; }

        /// <summary>
        /// Image cache repository.
        /// </summary>
        IImageCache ImageCache { get; set; }

        /// <summary>
        /// Gets or sets style used to draw points.
        /// </summary>
        IShapeStyle PointStyle { get; set; }

        /// <summary>
        /// Gets or sets selection rectangle style.
        /// </summary>
        IShapeStyle SelectionStyle { get; set; }

        /// <summary>
        /// Gets or sets editor helper shapes style.
        /// </summary>
        IShapeStyle HelperStyle { get; set; }
    }
}
