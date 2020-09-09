
namespace Core2D.Shapes
{
    /// <summary>
    /// Defines text shape contract.
    /// </summary>
    public interface ITextShape : IBaseShape
    {
        /// <summary>
        /// Gets or sets top-left corner point.
        /// </summary>
        IPointShape? TopLeft { get; set; }

        /// <summary>
        /// Gets or sets bottom-right corner point.
        /// </summary>
        IPointShape? BottomRight { get; set; }

        /// <summary>
        /// Gets or sets text string.
        /// </summary>
        string? Text { get; set; }
    }
}
