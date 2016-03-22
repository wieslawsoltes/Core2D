// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Style;
using Perspex;
using Perspex.Controls;
using Perspex.Markup.Xaml;
using Perspex.Media;
using System.Collections.Immutable;

namespace Core2D.Perspex.Views
{
    /// <summary>
    /// Interaction logic for <see cref="ContainerControl"/> xaml.
    /// </summary>
    public class ContainerControl : UserControl
    {
        public static PerspexProperty<XContainer> ContainerProperty =
            PerspexProperty.Register<ContainerControl, XContainer>(nameof(Container));

        public static PerspexProperty<ShapeRenderer> RendererProperty =
            PerspexProperty.Register<ContainerControl, ShapeRenderer>(nameof(Renderer));

        public XContainer Container
        {
            get { return GetValue(ContainerProperty); }
            set { SetValue(ContainerProperty, value); }
        }

        public ShapeRenderer Renderer
        {
            get { return GetValue(RendererProperty); }
            set { SetValue(RendererProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerControl"/> class.
        /// </summary>
        public ContainerControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initialize the Xaml components.
        /// </summary>
        private void InitializeComponent()
        {
            PerspexXamlLoader.Load(this);
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
            var color = Color.FromArgb(c.A, c.R, c.G, c.B);
            var brush = new SolidColorBrush(color);
            var rect = new Rect(0, 0, width, height);
            dc.FillRectangle(brush, rect);
        }

        /// <summary>
        /// Draw template container.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        /// <param name="renderer">The shape renderer.</param>
        /// <param name="template">The template to draw.</param>
        private void DrawTemplate(DrawingContext dc, ShapeRenderer renderer, XTemplate template)
        {
            DrawBackground(dc, template.Background, template.Width, template.Height);

            renderer.Draw(dc, template, default(ImmutableArray<XProperty>), default(XRecord));

            if (template.WorkingLayer != null)
            {
                renderer.Draw(dc, template.WorkingLayer, default(ImmutableArray<XProperty>), default(XRecord));
            }

            if (template.HelperLayer != null)
            {
                renderer.Draw(dc, template.HelperLayer, default(ImmutableArray<XProperty>), default(XRecord));
            }
        }

        /// <summary>
        /// Draw page container.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        /// <param name="renderer">The shape renderer.</param>
        /// <param name="page">The page to draw.</param>
        private void DrawPage(DrawingContext dc, ShapeRenderer renderer, XPage page)
        {
            DrawBackground(dc, page.Template.Background, page.Template.Width, page.Template.Height);

            renderer.Draw(dc, page, page.Data.Properties, page.Data.Record);

            if (page.WorkingLayer != null)
            {
                renderer.Draw(dc, page.WorkingLayer, page.Data.Properties, page.Data.Record);
            }

            if (page.HelperLayer != null)
            {
                renderer.Draw(dc, page.HelperLayer, page.Data.Properties, page.Data.Record);
            }
        }

        /// <summary>
        /// Draw container.
        /// </summary>
        /// <param name="dc">The drawing context.</param>
        /// <param name="renderer">The shape renderer.</param>
        /// <param name="container">The container to draw.</param>
        private void Draw(DrawingContext dc, ShapeRenderer renderer, XContainer container)
        {
            if (container is XTemplate)
            {
                DrawTemplate(dc, renderer, container as XTemplate);
            }

            if (container is XPage)
            {
                DrawPage(dc, renderer, container as XPage);
            }
        }

        /// <summary>
        /// Renders container control contents.
        /// </summary>
        /// <param name="context">The drawing context.</param>
        public override void Render(DrawingContext context)
        {
            base.Render(context);

            if (Renderer != null && Container != null)
            {
                Draw(context, Renderer, Container);
            }
        }
    }
}
