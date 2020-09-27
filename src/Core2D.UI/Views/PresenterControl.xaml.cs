#if USE_DIAGNOSTICS
using System.Diagnostics;
#endif
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
#if USE_SKIA
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
#endif
using Core2D.Containers;
using Core2D.Data;
using Core2D.Renderer;
using Core2D.Renderer.Presenters;
using Core2D.UI.Renderer;

namespace Core2D.UI.Views
{
    /// <summary>
    /// Specifies container presenter type.
    /// </summary>
    public enum PresenterType
    {
        /// <summary>
        /// None presenter.
        /// </summary>
        None = 0,

        /// <summary>
        /// Data presenter.
        /// </summary>
        Data = 1,

        /// <summary>
        /// Template mode.
        /// </summary>
        Template = 2,

        /// <summary>
        /// Editor presenter.
        /// </summary>
        Editor = 3,

        /// <summary>
        /// Export presenter.
        /// </summary>
        Export = 4
    }

    internal struct CustomState
    {
        public IPageContainer Container;
        public IShapeRenderer Renderer;
        public IDataFlow DataFlow;
        public PresenterType PresenterType;
    }

#if USE_SKIA
        internal class CustomDrawOperation : ICustomDrawOperation
        {
            public PresenterControl PresenterControl { get; set; }

            public CustomState CustomState { get; set; }

            public Rect Bounds { get; set; }

            public void Dispose()
            {
            }

            public bool HitTest(Point p) => false;

            public bool Equals(ICustomDrawOperation other) => false;

            public void Render(IDrawingContextImpl context)
            {
                var canvas = (context as ISkiaDrawingContextImpl)?.SkCanvas;
                if (canvas == null)
                {
                    return;
                }

                canvas.Save();

                PresenterControl.Draw(CustomState, canvas);

                canvas.Restore();
            }
        }
#endif

    /// <summary>
    /// Interaction logic for <see cref="PresenterControl"/> xaml.
    /// </summary>
    public class PresenterControl : UserControl
    {
        private static readonly IContainerPresenter s_editorPresenter = new EditorPresenter();
        private static readonly IContainerPresenter s_templatePresenter = new TemplatePresenter();
        private static readonly IContainerPresenter s_exportPresenter = new ExportPresenter();

        /// <summary>
        /// Gets or sets zoom border property.
        /// </summary>
        public static readonly StyledProperty<ZoomBorder> ZoomBorderProperty =
            AvaloniaProperty.Register<PresenterControl, ZoomBorder>(nameof(ZoomBorder), null);

        /// <summary>
        /// Gets or sets container property.
        /// </summary>
        public static readonly StyledProperty<IPageContainer> ContainerProperty =
            AvaloniaProperty.Register<PresenterControl, IPageContainer>(nameof(Container), null);

        /// <summary>
        /// Gets or sets renderer property.
        /// </summary>
        public static readonly StyledProperty<IShapeRenderer> RendererProperty =
            AvaloniaProperty.Register<PresenterControl, IShapeRenderer>(nameof(Renderer), null);

        /// <summary>
        /// Gets or sets data flow property.
        /// </summary>
        public static readonly StyledProperty<IDataFlow> DataFlowProperty =
            AvaloniaProperty.Register<PresenterControl, IDataFlow>(nameof(DataFlow), null);

        /// <summary>
        /// Gets or sets data flow property.
        /// </summary>
        public static readonly StyledProperty<PresenterType> PresenterTypeProperty =
            AvaloniaProperty.Register<PresenterControl, PresenterType>(nameof(PresenterType), PresenterType.None);

        /// <summary>
        /// Gets or sets zoom border property.
        /// </summary>
        public ZoomBorder ZoomBorder
        {
            get => GetValue(ZoomBorderProperty);
            set => SetValue(ZoomBorderProperty, value);
        }

        /// <summary>
        /// Gets or sets container property.
        /// </summary>
        public IPageContainer Container
        {
            get => GetValue(ContainerProperty);
            set => SetValue(ContainerProperty, value);
        }

        /// <summary>
        ///  Gets or sets renderer property.
        /// </summary>
        public IShapeRenderer Renderer
        {
            get => GetValue(RendererProperty);
            set => SetValue(RendererProperty, value);
        }

        /// <summary>
        ///  Gets or sets data flow property.
        /// </summary>
        public IDataFlow DataFlow
        {
            get => GetValue(DataFlowProperty);
            set => SetValue(DataFlowProperty, value);
        }

        /// <summary>
        ///  Gets or sets presenter type property.
        /// </summary>
        public PresenterType PresenterType
        {
            get => GetValue(PresenterTypeProperty);
            set => SetValue(PresenterTypeProperty, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PresenterControl"/> class.
        /// </summary>
        public PresenterControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Initialize the Xaml components.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Renders presenter control contents.
        /// </summary>
        /// <param name="context">The drawing context.</param>
        public override void Render(DrawingContext context)
        {
            base.Render(context);

            var customState = new CustomState()
            {
                Container = Container,
                Renderer = Renderer ?? GetValue(RendererOptions.RendererProperty),
                DataFlow = DataFlow ?? GetValue(RendererOptions.DataFlowProperty),
                PresenterType = PresenterType,
            };
#if USE_SKIA
            var customDrawOperation = new CustomDrawOperation
            {
                PresenterControl = this,
                CustomState = customState,
                Bounds = ZoomBorder != null ? ZoomBorder.Bounds : this.Bounds
            };
            context.Custom(customDrawOperation);
#else
            Draw(customState, context);
#endif
        }

        /// <summary>
        /// Draws presenter control contents.
        /// </summary>
        /// <param name="customState">The custom state.</param>
        /// <param name="context">The drawing context.</param>
        internal void Draw(CustomState customState, object context)
        {
            switch (customState.PresenterType)
            {
                case PresenterType.None:
                    break;
                case PresenterType.Data:
                    {
                        if (customState.Container != null && customState.DataFlow != null)
                        {
                            var db = (object)customState.Container.Data.Properties;
                            var record = (object)customState.Container.Data.Record;
#if USE_DIAGNOSTICS
                            var swDataFlow = Stopwatch.StartNew();
#endif
                            if (customState.Container.Template != null)
                            {
                                customState.DataFlow.Bind(customState.Container.Template, db, record);
                            }
                            customState.DataFlow.Bind(customState.Container, db, record);
#if USE_DIAGNOSTICS
                            swDataFlow.Stop();
                            Trace.WriteLine($"DataFlow {swDataFlow.Elapsed.TotalMilliseconds}ms");
#endif
                        }
                    }
                    break;
                case PresenterType.Template:
                    {
                        if (customState.Container != null && customState.Renderer != null)
                        {
                            s_templatePresenter.Render(context, customState.Renderer, customState.Container, 0.0, 0.0);
                            customState.Container?.Template?.Invalidate();
                        }
                    }
                    break;
                case PresenterType.Editor:
                    {
                        if (customState.Container != null && customState.Renderer != null)
                        {
#if USE_DIAGNOSTICS
                            var swRender = Stopwatch.StartNew();
#endif
                            s_editorPresenter.Render(context, customState.Renderer, customState.Container, 0.0, 0.0);
#if USE_DIAGNOSTICS
                            swRender.Stop();
                            Trace.WriteLine($"Render {swRender.Elapsed.TotalMilliseconds}ms");
#endif
#if USE_DIAGNOSTICS
                            var swInvalidate = Stopwatch.StartNew();
#endif
                            customState.Container?.Invalidate();
                            customState.Renderer.State.PointStyle.Invalidate();
                            customState.Renderer.State.SelectedPointStyle.Invalidate();
#if USE_DIAGNOSTICS
                            swInvalidate.Stop();
                            Trace.WriteLine($"Invalidate {swInvalidate.Elapsed.TotalMilliseconds}ms");
#endif
                        }
                    }
                    break;
                case PresenterType.Export:
                    {
                        if (customState.Container != null && customState.Renderer != null)
                        {
                            s_exportPresenter.Render(context, customState.Renderer, customState.Container, 0.0, 0.0);
                        }
                    }
                    break;
            }
        }
    }
}
