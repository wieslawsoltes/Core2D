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

namespace Core2D.Views
{
    public enum PresenterType
    {
        None = 0,

        Data = 1,

        Template = 2,

        Editor = 3,

        Export = 4
    }

    internal struct CustomState
    {
        public PageContainer Container;
        public IShapeRenderer Renderer;
        public DataFlow DataFlow;
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

    public class PresenterControl : UserControl
    {
        private static readonly IContainerPresenter s_editorPresenter = new EditorPresenter();
        private static readonly IContainerPresenter s_templatePresenter = new TemplatePresenter();
        private static readonly IContainerPresenter s_exportPresenter = new ExportPresenter();

        public static readonly StyledProperty<ZoomBorder> ZoomBorderProperty =
            AvaloniaProperty.Register<PresenterControl, ZoomBorder>(nameof(ZoomBorder), null);

        public static readonly StyledProperty<PageContainer> ContainerProperty =
            AvaloniaProperty.Register<PresenterControl, PageContainer>(nameof(Container), null);

        public static readonly StyledProperty<IShapeRenderer> RendererProperty =
            AvaloniaProperty.Register<PresenterControl, IShapeRenderer>(nameof(Renderer), null);

        public static readonly StyledProperty<DataFlow> DataFlowProperty =
            AvaloniaProperty.Register<PresenterControl, DataFlow>(nameof(DataFlow), null);

        public static readonly StyledProperty<PresenterType> PresenterTypeProperty =
            AvaloniaProperty.Register<PresenterControl, PresenterType>(nameof(PresenterType), PresenterType.None);

        public ZoomBorder ZoomBorder
        {
            get => GetValue(ZoomBorderProperty);
            set => SetValue(ZoomBorderProperty, value);
        }

        public PageContainer Container
        {
            get => GetValue(ContainerProperty);
            set => SetValue(ContainerProperty, value);
        }

        public IShapeRenderer Renderer
        {
            get => GetValue(RendererProperty);
            set => SetValue(RendererProperty, value);
        }

        public DataFlow DataFlow
        {
            get => GetValue(DataFlowProperty);
            set => SetValue(DataFlowProperty, value);
        }

        public PresenterType PresenterType
        {
            get => GetValue(PresenterTypeProperty);
            set => SetValue(PresenterTypeProperty, value);
        }

        public PresenterControl()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

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
