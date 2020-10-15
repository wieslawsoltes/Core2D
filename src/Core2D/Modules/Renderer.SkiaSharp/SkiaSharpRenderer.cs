using System;
using System.Collections.Generic;

namespace Core2D.Renderer.SkiaSharp
{
    /// <summary>
    /// Native SkiaSharp shape renderer.
    /// </summary>
    public class SkiaSharpRenderer : NodeRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SkiaSharpRenderer"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public SkiaSharpRenderer(IServiceProvider serviceProvider)
            : base(serviceProvider, new SkiaSharpDrawNodeFactory())
        {
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }
    }
}
