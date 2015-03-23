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

            if (_layer.IsVisible)
            {
                _renderer.Render(drawingContext, _layer);
            }
        }

        public static WpfElement Create(
            IRenderer renderer,
            ILayer layer,
            double width,
            double height)
        {
            var element = new WpfElement(layer, renderer)
            {
                Width = width,
                Height = height
            };

            layer.SetInvalidate(element.Invalidate);

            return element;
        }
    }
}
