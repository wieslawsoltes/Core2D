using System;
using Core2D.Renderer;
using Core2D.Shapes;

namespace Core2D.Renderer
{
    public interface IImageDrawNode : ITextDrawNode
    {
        IImageShape Image { get; set; }
        IImageCache ImageCache { get; set; }
        ICache<string, IDisposable> BitmapCache { get; set; }
    }
}
