using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Core2D.Model.Renderer;
#if USE_SKIA
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
#endif
using Core2D.Renderer;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Renderer.Presenters;

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
        public PageContainerViewModel _containerViewModel;
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

    public class PresenterView : UserControl
    {
        private static readonly IContainerPresenter s_editorPresenter = new EditorPresenter();
        private static readonly IContainerPresenter s_templatePresenter = new TemplatePresenter();
        private static readonly IContainerPresenter s_exportPresenter = new ExportPresenter();

        public static readonly StyledProperty<ZoomBorder> ZoomBorderProperty =
            AvaloniaProperty.Register<PresenterView, ZoomBorder>(nameof(ZoomBorder), null);

        public static readonly StyledProperty<PageContainerViewModel> ContainerProperty =
            AvaloniaProperty.Register<PresenterView, PageContainerViewModel>(nameof(Container), null);

        public static readonly StyledProperty<IShapeRenderer> RendererProperty =
            AvaloniaProperty.Register<PresenterView, IShapeRenderer>(nameof(Renderer), null);

        public static readonly StyledProperty<DataFlow> DataFlowProperty =
            AvaloniaProperty.Register<PresenterView, DataFlow>(nameof(DataFlow), null);

        public static readonly StyledProperty<PresenterType> PresenterTypeProperty =
            AvaloniaProperty.Register<PresenterView, PresenterType>(nameof(PresenterType), PresenterType.None);

        public ZoomBorder ZoomBorder
        {
            get => GetValue(ZoomBorderProperty);
            set => SetValue(ZoomBorderProperty, value);
        }

        public PageContainerViewModel Container
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

        public PresenterView()
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
                _containerViewModel = Container,
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
                        if (customState._containerViewModel != null && customState.DataFlow != null)
                        {
                            var db = (object)customState._containerViewModel.Properties;
                            var record = (object)customState._containerViewModel.Record;

                            if (customState._containerViewModel.Template != null)
                            {
                                customState.DataFlow.Bind(customState._containerViewModel.Template, db, record);
                            }
                            customState.DataFlow.Bind(customState._containerViewModel, db, record);
                        }
                    }
                    break;

                case PresenterType.Template:
                    {
                        if (customState._containerViewModel != null && customState.Renderer != null)
                        {
                            s_templatePresenter.Render(context, customState.Renderer, customState._containerViewModel, 0.0, 0.0);
                            customState._containerViewModel?.Template?.Invalidate();
                        }
                    }
                    break;

                case PresenterType.Editor:
                    {
                        if (customState._containerViewModel != null && customState.Renderer != null)
                        {
                            s_editorPresenter.Render(context, customState.Renderer, customState._containerViewModel, 0.0, 0.0);

                            customState._containerViewModel?.Invalidate();
                            customState.Renderer.State.PointStyle.Invalidate();
                            customState.Renderer.State.SelectedPointStyle.Invalidate();
                        }
                    }
                    break;

                case PresenterType.Export:
                    {
                        if (customState._containerViewModel != null && customState.Renderer != null)
                        {
                            s_exportPresenter.Render(context, customState.Renderer, customState._containerViewModel, 0.0, 0.0);
                        }
                    }
                    break;
            }
        }
    }
}
