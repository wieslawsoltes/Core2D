using System;

namespace Core2D.Editor
{
    public interface IEditorCanvasPlatform
    {
        Action InvalidateControl { get; set; }

        Action ResetZoom { get; set; }

        Action FillZoom { get; set; }

        Action UniformZoom { get; set; }

        Action UniformToFillZoom { get; set; }

        Action AutoFitZoom { get; set; }

        Action InZoom { get; set; }

        Action OutZoom { get; set; }

        object Zoom { get; set; }
    }
}
