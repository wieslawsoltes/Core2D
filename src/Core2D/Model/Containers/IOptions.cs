using Core2D.Path;

namespace Core2D.Containers
{
    /// <summary>
    /// Defines options interface.
    /// </summary>
    public interface IOptions : IObservableObject
    {
        /// <summary>
        /// Gets or sets how grid snapping is handled. 
        /// </summary>
        bool SnapToGrid { get; set; }

        /// <summary>
        /// Gets or sets how much grid X axis is snapped.
        /// </summary>
        double SnapX { get; set; }

        /// <summary>
        /// Gets or sets how much grid Y axis is snapped.
        /// </summary>
        double SnapY { get; set; }

        /// <summary>
        /// Gets or sets hit test threshold radius.
        /// </summary>
        double HitThreshold { get; set; }

        /// <summary>
        /// Gets or sets how selected shapes are moved.
        /// </summary>
        MoveMode MoveMode { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether path/shape is stroked during creation.
        /// </summary>
        bool DefaultIsStroked { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether path/shape is filled during creation.
        /// </summary>
        bool DefaultIsFilled { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether path is closed during creation.
        /// </summary>
        bool DefaultIsClosed { get; set; }

        /// <summary>
        /// Gets or sets value indicating path fill rule during creation.
        /// </summary>
        FillRule DefaultFillRule { get; set; }

        /// <summary>
        /// Gets or sets how point connection is handled.
        /// </summary>
        bool TryToConnect { get; set; }
    }
}
