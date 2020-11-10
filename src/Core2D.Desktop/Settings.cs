using System.IO;
using Core2D.Configuration.Themes;

namespace Core2D
{
    public class Settings
    {
        public ThemeName? Theme { get; set; } = null;
        public FileInfo[]? Scripts { get; set; }
        public FileInfo? Project { get; set; }
        public bool Repl { get; set; }
        public bool UseSkia { get; set; }
        public bool UseDirect2D1 { get; set; }
        public bool EnableMultiTouch { get; set; } = true;
        public bool UseGpu { get; set; } = true;
        public bool AllowEglInitialization { get; set; } = true;
        public bool UseDeferredRendering { get; set; } = true;
        public bool UseDirectX11 { get; set; }
        public bool UseManagedSystemDialogs { get; set; }
        public bool UseHeadless { get; set; }
        public bool UseHeadlessDrawing { get; set; }
        public bool UseHeadlessVnc { get; set; }
        public bool CreateHeadlessScreenshots { get; set; }
        public string? VncHost { get; set; } = null;
        public int VncPort { get; set; } = 5901;
    }
}
