// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using System.Windows;
using System.Windows.Media;

namespace Core2D.Wpf.Controls.Custom
{
    /// <summary>
    /// The custom layer control.
    /// </summary>
    public sealed class LayerElement : FrameworkElement
    {
        /// <summary>
        /// Gets the <see cref="Core2D.Data"/> from <see cref="DependencyProperty"/> object.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyProperty"/> object.</param>
        /// <returns>The <see cref="Data"/> value.</returns>
        public static Core2D.Data GetData(DependencyObject obj)
        {
            return (Core2D.Data)obj.GetValue(DataProperty);
        }

        /// <summary>
        /// Sets the <see cref="DependencyProperty"/> object value as <see cref="Renderer"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyProperty"/> object.</param>
        /// <param name="value">The <see cref="Core2D.Data"/> value.</param>
        public static void SetData(DependencyObject obj, Core2D.Data value)
        {
            obj.SetValue(DataProperty, value);
        }

        /// <summary>
        /// The attached <see cref="DependencyProperty"/> for <see cref="Core2D.Data"/> type.
        /// </summary>
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.RegisterAttached(
                "Data",
                typeof(Core2D.Data),
                typeof(LayerElement),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.Inherits |
                    FrameworkPropertyMetadataOptions.AffectsRender |
                    FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.SubPropertiesDoNotAffectRender));

        /// <summary>
        /// Gets the <see cref="Core2D.Renderer"/> from <see cref="DependencyProperty"/> object.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyProperty"/> object.</param>
        /// <returns>The <see cref="Core2D.Renderer"/> value.</returns>
        public static Core2D.Renderer GetRenderer(DependencyObject obj)
        {
            return (Core2D.Renderer)obj.GetValue(RendererProperty);
        }

        /// <summary>
        /// Sets the <see cref="DependencyProperty"/> object value as <see cref="Core2D.Renderer"/>.
        /// </summary>
        /// <param name="obj">The <see cref="DependencyProperty"/> object.</param>
        /// <param name="value">The <see cref="Core2D.Renderer"/> value.</param>
        public static void SetRenderer(DependencyObject obj, Core2D.Renderer value)
        {
            obj.SetValue(RendererProperty, value);
        }

        /// <summary>
        /// The attached <see cref="DependencyProperty"/> for <see cref="Core2D.Renderer"/> type.
        /// </summary>
        public static readonly DependencyProperty RendererProperty =
            DependencyProperty.RegisterAttached(
                "Renderer",
                typeof(Core2D.Renderer),
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
                    var data = LayerElement.GetData(this);
                    var properties = data != null ? data.Properties : default(ImmutableArray<Property>);
                    var record = data != null ? data.Record : default(Record);

                    renderer.Draw(
                        drawingContext,
                        layer,
                        properties,
                        record);
                }
            }
        }
    }
}
