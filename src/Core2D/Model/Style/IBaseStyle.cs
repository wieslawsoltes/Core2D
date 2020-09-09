
namespace Core2D.Style
{
    /// <summary>
    /// Define base style contract.
    /// </summary>
    public interface IBaseStyle : IObservableObject
    {
        /// <summary>
        /// Gets or sets stroke color.
        /// </summary>
        IColor? Stroke { get; set; }

        /// <summary>
        /// Gets or sets fill color.
        /// </summary>
        IColor? Fill { get; set; }

        /// <summary>
        /// Gets or sets stroke thickness.
        /// </summary>
        double Thickness { get; set; }

        /// <summary>
        /// Gets or sets line cap.
        /// </summary>
        LineCap LineCap { get; set; }

        /// <summary>
        /// Gets or sets line dashes.
        /// </summary>
        string? Dashes { get; set; }

        /// <summary>
        /// Gets or sets line dash offset.
        /// </summary>
        double DashOffset { get; set; }
    }
}
