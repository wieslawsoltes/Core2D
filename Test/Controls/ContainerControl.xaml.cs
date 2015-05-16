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

        /// <summary>
        /// 
        /// </summary>
        public IRenderer Renderer
        {
            get { return (IRenderer)GetValue(RendererProperty); }
            set { SetValue(RendererProperty, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        private Layer _layer = default(Layer);

        /// <summary>
        /// 
        /// </summary>
        public LayerElement()
        {
            DataContextChanged += (s, e) => Initialize();
            Unloaded += (s, e) => DeInitialize();

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
            this.InvalidateVisual();
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
                if (Renderer != null)
                {
                    Renderer.Draw(drawingContext, layer, (Tag as Container).Properties);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public partial class ContainerControl : UserControl
    {
        /// <summary>
        /// 
        /// </summary>
        public ContainerControl()
        {
            InitializeComponent();

            Loaded += (s, e) => InitializeCanvas(); 
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeCanvas()
        {
            var editor = canvas.DataContext as Editor;

            canvas.PreviewMouseLeftButtonDown +=
                (s, e) =>
                {
                    canvas.Focus();
                    if (editor.IsLeftDownAvailable())
                    {
                        var p = e.GetPosition(canvas);
                        editor.LeftDown(p.X, p.Y);
                    }
                };
            
            canvas.PreviewMouseLeftButtonUp +=
                (s, e) =>
                {
                    canvas.Focus();
                    if (editor.IsLeftUpAvailable())
                    {
                        var p = e.GetPosition(canvas);
                        editor.LeftUp(p.X, p.Y);
                    }
                };

            canvas.PreviewMouseRightButtonDown +=
                (s, e) =>
                {
                    canvas.Focus();
                    if (editor.IsRightDownAvailable())
                    {
                        var p = e.GetPosition(canvas);
                        editor.RightDown(p.X, p.Y);
                    }
                };

            canvas.PreviewMouseRightButtonUp +=
                (s, e) =>
                {
                    canvas.Focus();
                    if (editor.IsRightUpAvailable())
                    {
                        var p = e.GetPosition(canvas);
                        editor.RightUp(p.X, p.Y);
                    }
                };
            
            canvas.PreviewMouseMove +=
                (s, e) =>
                {
                    canvas.Focus();
                    if (editor.IsMoveAvailable())
                    {
                        var p = e.GetPosition(canvas);
                        editor.Move(p.X, p.Y);
                    }
                };

            canvas.Focus();
        }
    }
}
