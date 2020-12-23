#nullable disable
using System;
using System.IO;
using Avalonia;
using Avalonia.Controls;
using Core2D.Util.Rendering;

namespace Core2D.Util
{
    public static class Renderer
    {
        public static void Render(Control control, Size size, string path)
        {
            if (control is null)
            {
                return;
            }
            
            if (path.EndsWith("png", StringComparison.OrdinalIgnoreCase))
            {
                PngRenderer.Render(control, size, path);
            }
            
            if (path.EndsWith("skp", StringComparison.OrdinalIgnoreCase))
            {
                using var stream = File.Create(path);
                SkpRenderer.Render(control, size, stream);
            }

            if (path.EndsWith("svg", StringComparison.OrdinalIgnoreCase))
            {
                using var stream = File.Create(path);
                SvgRenderer.Render(control, size, stream);
            }
            
            if (path.EndsWith("pdf", StringComparison.OrdinalIgnoreCase))
            {
                using var stream = File.Create(path);
                PdfRenderer.Render(control, size, stream);
            }
        }
    }
}
