using System;
using System.Collections.Generic;
using Core2D.ViewModels.Renderer;

namespace Core2D.Renderer.SkiaSharp
{
    public class SkiaSharpRendererViewModel : NodeRendererViewModel
    {
        public SkiaSharpRendererViewModel(IServiceProvider serviceProvider)
            : base(serviceProvider, new SkiaSharpDrawNodeFactory())
        {
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }
    }
}
