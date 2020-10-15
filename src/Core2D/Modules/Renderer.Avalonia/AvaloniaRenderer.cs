using System;
using System.Collections.Generic;
using Core2D.Renderer;

namespace Core2D.Renderer
{
    /// <summary>
    /// Native Avalonia shape renderer.
    /// </summary>
    public class AvaloniaRenderer : NodeRenderer
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AvaloniaRenderer"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public AvaloniaRenderer(IServiceProvider serviceProvider)
            : base(serviceProvider, new AvaloniaDrawNodeFactory())
        {
        }

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }
    }
}
