// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Style;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Renderer.Avalonia;
using System.Collections.Immutable;

namespace Core2D.Avalonia.Views
{
    /// <summary>
    /// Interaction logic for <see cref="ContainerViewControl"/> xaml.
    /// </summary>
    public class ContainerViewControl : UserControl
    {
        public static readonly AvaloniaProperty<XContainer> ContainerProperty =
            AvaloniaProperty.Register<ContainerViewControl, XContainer>(nameof(Container));

        public static readonly AvaloniaProperty<ShapeRenderer> RendererProperty =
            AvaloniaProperty.Register<ContainerViewControl, ShapeRenderer>(nameof(Renderer));

        /// <summary>
        /// Gets or set container property.
        /// </summary>
        public XContainer Container
        {
            get { return GetValue(ContainerProperty); }
            set { SetValue(ContainerProperty, value); }
        }

        /// <summary>
        ///  Gets or set renderer property.
        /// </summary>
        public ShapeRenderer Renderer
        {
            get { return GetValue(RendererProperty); }
            set { SetValue(RendererProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerViewControl"/> class.
        /// </summary>
        public ContainerViewControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initialize the Xaml components.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Draws background rectangle with specified color.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        /// <param name="c">The background color.</param>
        /// <param name="width">The width of background rectangle.</param>
        /// <param name="height">The height of background rectangle.</param>
        private void DrawBackground(DrawingContext dc, ArgbColor c, double width, double height)
        {
            dc.FillRectangle(
                new SolidColorBrush(Color.FromArgb(c.A, c.R, c.G, c.B)), 
                new Rect(0, 0, width, height));
        }

        /// <summary>
        /// Draw container.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        /// <param name="renderer">The shape renderer.</param>
        /// <param name="container">The page to draw.</param>
        private void Draw(DrawingContext dc, ShapeRenderer renderer, XContainer container)
        {
            DrawBackground(dc, container.Background, container.Width, container.Height);

            var db = container.Data == null ? default(ImmutableArray<XProperty>) : container.Data.Properties;
            var r = container.Data == null ? default(XRecord) : container.Data.Record;

            renderer.Draw(dc, container, db, r);

            if (container.WorkingLayer != null)
            {
                renderer.Draw(dc, container.WorkingLayer, db, r);
            }

            if (container.HelperLayer != null)
            {
                renderer.Draw(dc, container.HelperLayer, db, r);
            }
        }

        /// <summary>
        /// Renders container control contents.
        /// </summary>
        /// <param name="context">The drawing context.</param>
        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (Container != null)
            {
                if (Renderer != null)
                {
                    Draw(context, Renderer, Container);
                }
                else
                {
                    var renderer = GetValue(RendererOptions.RendererProperty);
                    if (renderer != null)
                    {
                        Draw(context, renderer, Container);
                    }
                }
            }
        }
    }
}
