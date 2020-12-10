using System;
using System.Collections.Generic;
using Core2D.ViewModels.Renderer;

namespace Core2D.Renderer
{
    public class AvaloniaRendererViewModel : NodeRendererViewModel
    {
        public AvaloniaRendererViewModel(IServiceProvider serviceProvider)
            : base(serviceProvider, new AvaloniaDrawNodeFactory())
        {
        }
    }
}
