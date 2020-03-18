
namespace Core2D.Style
{
    /// <summary>
    /// Define arrow style contract.
    /// </summary>
    public interface IArrowStyle : IBaseStyle
    {
        /// <summary>
        /// Gets or sets arrow type.
        /// </summary>
        ArrowType ArrowType { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether arrow shape is stroked.
        /// </summary>
        bool IsStroked { get; set; }

        /// <summary>
        /// Gets or sets value indicating whether arrow shape is filled.
        /// </summary>
        bool IsFilled { get; set; }

        /// <summary>
        /// Gets or sets arrow X axis radius.
        /// </summary>
        double RadiusX { get; set; }

        /// <summary>
        /// Gets or sets arrow Y axis radius.
        /// </summary>
        double RadiusY { get; set; }
    }
}
