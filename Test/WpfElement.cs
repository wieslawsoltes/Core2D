using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Test.Core;

namespace Test
{
    public class WpfElement : FrameworkElement, IElement
    {
        private readonly ILayer _layer;
        private readonly IRenderer _renderer;

        public WpfElement(ILayer layer, IRenderer renderer)
        {
            _layer = layer;
            _renderer = renderer;
        }

        public void Invalidate()
        {
            this.InvalidateVisual();
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            _renderer.Render(drawingContext, _layer);
        }
    }
}
