#nullable disable
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Core2D.Model.Renderer;
using Core2D.Modules.Renderer;
using Core2D.Model;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Renderer.Presenters;

namespace Core2D.Views.Renderer
{
    public enum DrawType
    {
        None = 0,
        Data = 1,
        Template = 2,
        Editor = 3,
        Export = 4
    }

    public class DrawState
    {
        public FrameContainerViewModel Container { get; set; }
        public IShapeRenderer Renderer { get; set; }
        public ISelection Selection { get; set; }
        public DataFlow DataFlow { get; set; }
        public DrawType DrawType { get; set; }
    }

    public class CustomDrawOperation : ICustomDrawOperation
    {
        private static readonly IContainerPresenter s_editorPresenter = new EditorPresenter();

        private static readonly IContainerPresenter s_templatePresenter = new TemplatePresenter();

        private static readonly IContainerPresenter s_exportPresenter = new ExportPresenter();

        private static void DrawData(DrawState drawState, object context)
        {
            if (drawState.Container is null || drawState.DataFlow is null)
            {
                return;
            }

            var db = (object) drawState.Container.Properties;
            var record = (object) drawState.Container.Record;

            if (drawState.Container is PageContainerViewModel page)
            {
                drawState.DataFlow.Bind(page.Template, db, record);
            }

            drawState.DataFlow.Bind(drawState.Container, db, record);
        }

        private static void DrawTemplate(DrawState drawState, object context)
        {
            if (drawState.Container is null || drawState.Renderer is null)
            {
                return;
            }
 
            s_templatePresenter.Render(context, drawState.Renderer, drawState.Selection, drawState.Container, 0.0, 0.0);
 
            if (drawState.Container is PageContainerViewModel page)
            {
                page.Template?.Invalidate();
            }
        }

        private static void DrawEditor(DrawState drawState, object context)
        {
            if (drawState.Container is null || drawState.Renderer is null)
            {
                return;
            }
 
            s_editorPresenter.Render(context, drawState.Renderer, drawState.Selection, drawState.Container, 0.0, 0.0);

            drawState.Container?.Invalidate();
            drawState.Renderer.State.PointStyle.Invalidate();
            drawState.Renderer.State.SelectedPointStyle.Invalidate();
        }

        private static void DrawExport(DrawState drawState, object context)
        {
            if (drawState.Container is null || drawState.Renderer is null)
            {
                return;
            }

            s_exportPresenter.Render(context, drawState.Renderer, drawState.Selection, drawState.Container, 0.0, 0.0);
        }

        private static void Draw(DrawState drawState, object context)
        {
            switch (drawState.DrawType)
            {
                case DrawType.None:
                    break;
                case DrawType.Data:
                    DrawData(drawState, context);
                    break;
                case DrawType.Template:
                    DrawTemplate(drawState, context);
                    break;
                case DrawType.Editor:
                    DrawEditor(drawState, context);
                    break;
                case DrawType.Export:
                    DrawExport(drawState, context);
                    break;
            }
        }

        public DrawState DrawState { get; set; }

        public Rect Bounds { get; set; }

        public void Dispose()
        {
        }

        public bool HitTest(Point p) => false;

        public bool Equals(ICustomDrawOperation other) => false;

        public void Render(IDrawingContextImpl context)
        {
            Draw(DrawState, context);
        }
    }

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

        public static readonly StyledProperty<DrawType> PresenterTypeProperty =
            AvaloniaProperty.Register<RenderView, DrawType>(nameof(DrawType));

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

        public DrawType DrawType
        {
            get => GetValue(PresenterTypeProperty);
            set => SetValue(PresenterTypeProperty, value);
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

            var drawState = new DrawState()
            {
                Container = Container,
                Renderer = Renderer ?? GetValue(RendererOptions.RendererProperty),
                Selection = Selection ?? GetValue(RendererOptions.SelectionProperty),
                DataFlow = DataFlow ?? GetValue(RendererOptions.DataFlowProperty),
                DrawType = DrawType,
            };

            var customDrawOperation = new CustomDrawOperation
            {
                DrawState = drawState,
                Bounds = bounds
            };

            context.Custom(customDrawOperation);
        }
    }
}
