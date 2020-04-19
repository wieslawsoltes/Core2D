
namespace Core2D.Renderer
{
    /// <summary>
    /// Defines shape state contract.
    /// </summary>
    public interface IShapeState : IObservableObject
    {
        /// <summary>
        /// Gets or sets shape state flags.
        /// </summary>
        ShapeStateFlags Flags { get; set; }

        /// <summary>
        /// Gets or sets <see cref="ShapeStateFlags.Default"/> flag.
        /// </summary>
        bool Default { get; set; }

        /// <summary>
        /// Gets or sets <see cref="ShapeStateFlags.Visible"/> flag.
        /// </summary>
        bool Visible { get; set; }

        /// <summary>
        /// Gets or sets <see cref="ShapeStateFlags.Printable"/> flag.
        /// </summary>
        bool Printable { get; set; }

        /// <summary>
        /// Gets or sets <see cref="ShapeStateFlags.Locked"/> flag.
        /// </summary>
        bool Locked { get; set; }

        /// <summary>
        /// Gets or sets <see cref="ShapeStateFlags.Connector"/> flag.
        /// </summary>
        bool Connector { get; set; }

        /// <summary>
        /// Gets or sets <see cref="ShapeStateFlags.None"/> flag.
        /// </summary>
        bool None { get; set; }

        /// <summary>
        /// Gets or sets <see cref="ShapeStateFlags.Standalone"/> flag.
        /// </summary>
        bool Standalone { get; set; }

        /// <summary>
        /// Gets or sets <see cref="ShapeStateFlags.Input"/> flag.
        /// </summary>
        bool Input { get; set; }

        /// <summary>
        /// Gets or sets <see cref="ShapeStateFlags.Output"/> flag.
        /// </summary>
        bool Output { get; set; }
    }
}
