using System;
using System.Collections.Generic;

namespace Core2D.Renderer.SkiaSharp
{
    public class SkiaSharpRenderer : NodeRenderer
    {
        public SkiaSharpRenderer(IServiceProvider serviceProvider)
            : base(serviceProvider, new SkiaSharpDrawNodeFactory())
        {
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }
    }
}
