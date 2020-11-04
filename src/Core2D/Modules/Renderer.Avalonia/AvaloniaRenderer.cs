using System;
using System.Collections.Generic;
using Core2D.Renderer;

namespace Core2D.Renderer
{
    public class AvaloniaRenderer : NodeRenderer
    {
        public AvaloniaRenderer(IServiceProvider serviceProvider)
            : base(serviceProvider, new AvaloniaDrawNodeFactory())
        {
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }
    }
}
