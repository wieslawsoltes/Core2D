#nullable disable
using System;
using Core2D.ViewModels.Renderer;

namespace Core2D.Modules.Renderer
{
    public class AvaloniaRendererViewModel : NodeRendererViewModel
    {
        public AvaloniaRendererViewModel(IServiceProvider serviceProvider)
            : base(serviceProvider, new AvaloniaDrawNodeFactory())
        {
        }
    }
}
