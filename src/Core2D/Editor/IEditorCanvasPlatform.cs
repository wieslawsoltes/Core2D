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
        Action Invalidate { get; set; }

        /// <summary>
        /// Gets or sets reset zoom action.
        /// </summary>
        /// <remarks>Reset view size to defaults.</remarks>
        Action ResetZoom { get; set; }

        /// <summary>
        /// Gets or sets extent zoom action.
        /// </summary>
        /// <remarks>Auto-fit view to the available extents.</remarks>
        Action AutoFitZoom { get; set; }

        /// <summary>
        /// Gets or sets zoom control.
        /// </summary>
        object Zoom { get; set; }
    }
}
