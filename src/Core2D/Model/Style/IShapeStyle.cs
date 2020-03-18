
namespace Core2D.Style
{
    /// <summary>
    /// Define shape style contract.
    /// </summary>
    public interface IShapeStyle : IBaseStyle
    {
        /// <summary>
        /// Gets or sets line style.
        /// </summary>
        ILineStyle LineStyle { get; set; }

        /// <summary>
        /// Gets or sets start arrow style.
        /// </summary>
        IArrowStyle StartArrowStyle { get; set; }

        /// <summary>
        /// Gets or sets end arrow style.
        /// </summary>
        IArrowStyle EndArrowStyle { get; set; }

        /// <summary>
        /// Gets or sets text style.
        /// </summary>
        ITextStyle TextStyle { get; set; }
    }
}
