// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
    public class LayerElement : FrameworkElement
    {
        public static readonly DependencyProperty RendererProperty =
            DependencyProperty.Register(
                "Renderer", 
                typeof(IRenderer), 
                typeof(LayerElement),
                new FrameworkPropertyMetadata(
                    null, 
                    FrameworkPropertyMetadataOptions.AffectsRender | 
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        public IRenderer Renderer
        {
            get { return (IRenderer) GetValue(RendererProperty); }
            set { SetValue(RendererProperty, value); }
        }

        public LayerElement()
        {
            DataContextChanged += (s, e) =>
            {
                var layer = DataContext as ILayer;
                if (layer != null)
                {
                    layer.SetInvalidate(() => this.InvalidateVisual());
                }
            };
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            var layer = DataContext as ILayer;
            if (layer != null && layer.IsVisible)
            {
                if (Renderer != null)
                {
                    Renderer.Render(drawingContext, layer);
                }
            }
        }
    }
}
