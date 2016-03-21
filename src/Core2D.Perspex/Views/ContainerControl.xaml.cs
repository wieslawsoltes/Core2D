// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Editor;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Style;
using Perspex;
using Perspex.Controls;
using Perspex.Input;
using Perspex.Markup.Xaml;
using Perspex.Media;
using System.Collections.Immutable;
using PAZ = Core2D.Perspex.Controls.PanAndZoom;

namespace Core2D.Perspex.Views
{
    /// <summary>
    /// Interaction logic for <see cref="ContainerControl"/> xaml.
    /// </summary>
    public class ContainerControl : UserControl
    {
        public static PerspexProperty<ProjectEditor> EditorProperty =
            PerspexProperty.Register<ContainerControl, ProjectEditor>(nameof(Editor));

        public static PerspexProperty<XContainer> ContainerProperty =
            PerspexProperty.Register<ContainerControl, XContainer>(nameof(Container));

        public static PerspexProperty<ShapeRenderer> RendererProperty =
            PerspexProperty.Register<ContainerControl, ShapeRenderer>(nameof(Renderer));

        public ProjectEditor Editor
        {
            get { return GetValue(EditorProperty); }
            set { SetValue(EditorProperty, value); }
        }

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

            this.AttachedToVisualTree += (sender, e) => Initialize();
        }

        /// <summary>
        /// Initialize the Xaml components.
        /// </summary>
        private void InitializeComponent()
        {
            PerspexXamlLoader.Load(this);
        }

        /// <summary>
        /// Initialize container control.
        /// </summary>
        public void Initialize()
        {
            var panAndZoom = this.Parent as PAZ.PanAndZoom;
            if (Editor != null && Renderer != null && panAndZoom != null)
            {
                Editor.Invalidate = () => this.InvalidateVisual();
                Editor.ResetZoom = () => panAndZoom.Reset();
                Editor.AutoFitZoom = () => panAndZoom.AutoFit();

                panAndZoom.InvalidatedChild =
                    (zoomX, zoomY, offsetX, offsetY) =>
                    {
                        var state = Renderer.State;
                        bool invalidate = state.ZoomX != zoomX || state.ZoomY != zoomY;
                        state.ZoomX = zoomX;
                        state.ZoomY = zoomY;
                        state.PanX = offsetX;
                        state.PanY = offsetY;
                        if (invalidate)
                        {
                            Editor.InvalidateCache(isZooming: true);
                        }
                    };

                panAndZoom.PointerPressed +=
                    (sender, e) =>
                    {
                        var p = e.GetPosition(this);
                        p = panAndZoom.FixInvalidPointPosition(p);

                        if (e.MouseButton == MouseButton.Left)
                        {
                            if (Editor.IsLeftDownAvailable())
                            {
                                Editor.LeftDown(p.X, p.Y);
                            }
                        }

                        if (e.MouseButton == MouseButton.Right)
                        {
                            this.Cursor = new Cursor(StandardCursorType.Hand);
                            if (Editor.IsRightDownAvailable())
                            {
                                Editor.RightDown(p.X, p.Y);
                            }
                        }
                    };

                panAndZoom.PointerReleased +=
                    (sender, e) =>
                    {
                        var p = e.GetPosition(this);
                        p = panAndZoom.FixInvalidPointPosition(p);

                        if (e.MouseButton == MouseButton.Left)
                        {
                            if (Editor.IsLeftUpAvailable())
                            {
                                Editor.LeftUp(p.X, p.Y);
                            }
                        }

                        if (e.MouseButton == MouseButton.Right)
                        {
                            this.Cursor = new Cursor(StandardCursorType.Arrow);
                            if (Editor.IsRightUpAvailable())
                            {
                                Editor.RightUp(p.X, p.Y);
                            }
                        }
                    };

                panAndZoom.PointerMoved +=
                    (sender, e) =>
                    {
                        var p = e.GetPosition(this);
                        p = panAndZoom.FixInvalidPointPosition(p);

                        if (Editor.IsMoveAvailable())
                        {
                            Editor.Move(p.X, p.Y);
                        }
                    };
            }
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
