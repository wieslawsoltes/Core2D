using Core2D.Style;

namespace Core2D.Renderer
{
    /// <summary>
    /// Defines grid contract.
    /// </summary>
    public interface IGrid
    {
        /// <summary>
        /// Gets or sets flag indicating whether grid is enabled.
        /// </summary>
        bool IsGridEnabled { get; set; }

        /// <summary>
        /// Gets or sets flag indicating whether to draw grid border.
        /// </summary>
        bool IsBorderEnabled { get; set; }

        /// <summary>
        /// Gets or sets grid left offset.
        /// </summary>
        double GridOffsetLeft { get; set; }

        /// <summary>
        /// Gets or sets grid top offset.
        /// </summary>
        double GridOffsetTop { get; set; }

        /// <summary>
        /// Gets or sets grid right offset.
        /// </summary>
        double GridOffsetRight { get; set; }

        /// <summary>
        /// Gets or sets grid bottom offset.
        /// </summary>
        double GridOffsetBottom { get; set; }

        /// <summary>
        /// Gets or sets grid cell width.
        /// </summary>
        double GridCellWidth { get; set; }

        /// <summary>
        /// Gets or sets grid cell height.
        /// </summary>
        double GridCellHeight { get; set; }

        /// <summary>
        /// Gets or sets grid stroke color.
        /// </summary>
        BaseColor GridStrokeColor { get; set; }

        /// <summary>
        /// Gets or sets grid stroke thickness.
        /// </summary>
        double GridStrokeThickness { get; set; }

        /// <summary>
        /// Gets is dirty flag.
        /// </summary>
        bool IsDirty();
    }
}
