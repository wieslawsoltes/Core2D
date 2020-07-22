using System;

namespace Core2D.Editor
{
    /// <summary>
    /// Defines editor canvas platform contract.
    /// </summary>
    public interface IEditorCanvasPlatform
    {
        /// <summary>
        /// Gets or sets invalidate action.
        /// </summary>
        /// <remarks>Invalidate current container control.</remarks>
        Action InvalidateControl { get; set; }

        /// <summary>
        /// Gets or sets reset zoom action.
        /// </summary>
        /// <remarks>Reset view size to defaults.</remarks>
        Action ResetZoom { get; set; }

        /// <summary>
        /// Gets or sets fill zoom action.
        /// </summary>
        /// <remarks>Fill view to the available extents.</remarks>
        Action FillZoom { get; set; }

        /// <summary>
        /// Gets or sets uniform fill zoom action.
        /// </summary>
        /// <remarks>Uniform fill view to the available extents.</remarks>
        Action UniformZoom { get; set; }

        /// <summary>
        /// Gets or sets uniform to fill zoom action.
        /// </summary>
        /// <remarks>Uniform to fill view to the available extents.</remarks>
        Action UniformToFillZoom { get; set; }

        /// <summary>
        /// Gets or sets auto-fit zoom action.
        /// </summary>
        /// <remarks>Auto-fit view to the available extents.</remarks>
        Action AutoFitZoom { get; set; }







        /// <summary>
        /// Gets or sets zoom control.
        /// </summary>
        object Zoom { get; set; }
    }
}
