// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
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
using Test2d;

namespace Test.Controls
{
    /// <summary>
    /// 
    /// </summary>
    internal class LayerElement : FrameworkElement
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static IRenderer GetRenderer(DependencyObject obj)
        {
            return (IRenderer)obj.GetValue(RendererProperty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="value"></param>
        public static void SetRenderer(DependencyObject obj, IRenderer value)
        {
            obj.SetValue(RendererProperty, value);
        }

        /// <summary>
        /// 
        /// </summary>
        public static readonly DependencyProperty RendererProperty =
            DependencyProperty.RegisterAttached(
                "Renderer",
                typeof(IRenderer),
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
        /// 
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Invalidate(object sender, InvalidateLayerEventArgs e)
        {
            Dispatcher.Invoke(
                () =>
                {
                    this.InvalidateVisual();
                });
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        private void DeInitialize()
        {
            if (_layer != null)
            {
                _layer.InvalidateLayer -= Invalidate;
                _layer = default(Layer);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            Render(drawingContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawingContext"></param>
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
                        layer.Owner.Properties,
                        null);
                }
            }
        }
    }
}