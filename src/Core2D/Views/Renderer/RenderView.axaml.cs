#nullable disable
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Core2D.Model.Renderer;
using Core2D.Modules.Renderer;
using Core2D.Model;
using Core2D.Modules.Renderer.Avalonia;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;

namespace Core2D.Views.Renderer
{
    public class RenderView : UserControl
    {
        public static readonly StyledProperty<ZoomBorder> ZoomBorderProperty =
            AvaloniaProperty.Register<RenderView, ZoomBorder>(nameof(ZoomBorder));

        public static readonly StyledProperty<FrameContainerViewModel> ContainerProperty =
            AvaloniaProperty.Register<RenderView, FrameContainerViewModel>(nameof(Container));

        public static readonly StyledProperty<IShapeRenderer> RendererProperty =
            AvaloniaProperty.Register<RenderView, IShapeRenderer>(nameof(Renderer));

        public static readonly StyledProperty<ISelection> SelectionProperty =
            AvaloniaProperty.Register<RenderView, ISelection>(nameof(Selection));

        public static readonly StyledProperty<DataFlow> DataFlowProperty =
            AvaloniaProperty.Register<RenderView, DataFlow>(nameof(DataFlow));

        public static readonly StyledProperty<RenderType> RenderTypeProperty =
            AvaloniaProperty.Register<RenderView, RenderType>(nameof(RenderType));

        public ZoomBorder ZoomBorder
        {
            get => GetValue(ZoomBorderProperty);
            set => SetValue(ZoomBorderProperty, value);
        }

        public FrameContainerViewModel Container
        {
            get => GetValue(ContainerProperty);
            set => SetValue(ContainerProperty, value);
        }

        public IShapeRenderer Renderer
        {
            get => GetValue(RendererProperty);
            set => SetValue(RendererProperty, value);
        }

        public ISelection Selection
        {
            get => GetValue(SelectionProperty);
            set => SetValue(SelectionProperty, value);
        }

        public DataFlow DataFlow
        {
            get => GetValue(DataFlowProperty);
            set => SetValue(DataFlowProperty, value);
        }

        public RenderType RenderType
        {
            get => GetValue(RenderTypeProperty);
            set => SetValue(RenderTypeProperty, value);
        }

        public RenderView()
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

            var offset = this.TranslatePoint(new Point(0, 0), ZoomBorder) ?? default;
            var bounds = new Rect(
                offset.X > 0.0 ? -offset.X : 0.0,
                offset.Y > 0.0 ? -offset.Y : 0.0,
                ZoomBorder.Bounds.Width + (offset.X > 0.0 ? offset.X : -offset.X),
                ZoomBorder.Bounds.Height + (offset.Y > 0.0 ? offset.Y : -offset.Y));

            var drawState = new RenderState()
            {
                Container = Container,
                Renderer = Renderer ?? GetValue(RendererOptions.RendererProperty),
                Selection = Selection ?? GetValue(RendererOptions.SelectionProperty),
                DataFlow = DataFlow ?? GetValue(RendererOptions.DataFlowProperty),
                RenderType = RenderType,
            };

            var customDrawOperation = new RenderDrawOperation
            {
                RenderState = drawState,
                Bounds = bounds
            };

            context.Custom(customDrawOperation);
        }
    }
}
