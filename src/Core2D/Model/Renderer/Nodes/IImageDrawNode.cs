using System;
using Core2D.ViewModels.Shapes;

namespace Core2D.Model.Renderer.Nodes
{
    public interface IImageDrawNode : IDrawNode
    {
        ImageShapeViewModel Image { get; set; }
        IImageCache ImageCache { get; set; }
        ICache<string, IDisposable> BitmapCache { get; set; }
    }
}
