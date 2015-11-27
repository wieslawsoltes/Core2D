// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Core2D.Wpf.Controls
{
    /// <summary>
    /// The custom layer control.
    /// </summary>
    public class LayerElement : FrameworkElement
    {
        /// <summary>
        /// Gets the <see cref="Renderer"/> from <see cref="DependencyProperty"/> object.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyProperty"/> object.</param>
        /// <returns>The <see cref="Renderer"/> value.</returns>
        public static Renderer GetRenderer(DependencyObject obj)
        {
            return (Renderer)obj.GetValue(RendererProperty);
        }

        /// <summary>
        /// Sets the <see cref="DependencyProperty"/> object value as <see cref="Renderer"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyProperty"/> object.</param>
        /// <param name="value">The <see cref="Renderer"/> value.</param>
        public static void SetRenderer(DependencyObject obj, Renderer value)
        {
            obj.SetValue(RendererProperty, value);
        }

        /// <summary>
        /// The attached <see cref="DependencyProperty"/> for <see cref="Renderer"/> type.
        /// </summary>
        public static readonly DependencyProperty RendererProperty =
            DependencyProperty.RegisterAttached(
                "Renderer",
                typeof(Renderer),
                typeof(LayerElement),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.Inherits |
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        private bool _isLoaded = false;
        private Layer _layer = default(Layer);

        /// <summary>
        /// Initializes a new instance of the <see cref="LayerElement"/> class.
        /// </summary>
        public LayerElement()
        {
            Loaded +=
                (s, e) =>
                {
                    if (_isLoaded)
                        return;
                    else
                        _isLoaded = true;

                    Initialize();
                };

            Unloaded +=
                (s, e) =>
                {
                    if (!_isLoaded)
                        return;
                    else
                        _isLoaded = false;

                    DeInitialize();
                };

            DataContextChanged +=
                (s, e) =>
                {
                    if (!_isLoaded)
                        _isLoaded = true;

                    if (_layer != null)
                    {
                        var layer = DataContext as Layer;
                        if (layer == _layer)
                            return;
                    }

                    Initialize();
                };

            RenderOptions.SetBitmapScalingMode(
                this,
                BitmapScalingMode.HighQuality);
        }

        private void Invalidate(object sender, InvalidateLayerEventArgs e)
        {
            Dispatcher.Invoke(
                () =>
                {
                    this.InvalidateVisual();
                });
        }

        private void Initialize()
        {
            if (_layer != null)
            {
                DeInitialize();
            }

            var layer = DataContext as Layer;
            if (layer != null)
            {
                _layer = layer;
                _layer.InvalidateLayer += Invalidate;
            }
        }

        private void DeInitialize()
        {
            if (_layer != null)
            {
                _layer.InvalidateLayer -= Invalidate;
                _layer = default(Layer);
            }
        }

        /// <inheritdoc/>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            Render(drawingContext);
        }

        private void Render(DrawingContext drawingContext)
        {
            var layer = DataContext as Layer;
            if (layer != null && layer.IsVisible)
            {
                var renderer = LayerElement.GetRenderer(this);
                if (renderer != null)
                {
                    renderer.Draw(
                        drawingContext,
                        layer,
                        layer.Owner != null ? layer.Owner.Data.Properties : default(ImmutableArray<Property>),
                        null);
                }
            }
        }
    }
}
