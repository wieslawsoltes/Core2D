using System.Diagnostics;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.PanAndZoom;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Skia;
using Avalonia.Threading;
using Avalonia.VisualTree;
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

        internal struct CustomState
        {
            public IPageContainer Container;
            public IShapeRenderer Renderer;
            public IDataFlow DataFlow;
            public PresenterType PresenterType;
        }

        internal class CustomDrawOperation : ICustomDrawOperation
        {
            public PresenterControl PresenterControl { get; set; }

            public CustomState CustomState { get; set; }

            public Rect Bounds { get; set; }

            public void Dispose()
            {
            }

            public bool HitTest(Point p) => false;

            public bool Equals(ICustomDrawOperation other) => this.Equals(other);

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

        //private RenderTargetBitmap _renderTarget;
#if USE_SKIA
        private CustomDrawOperation _customDrawOperation;
#endif

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

            //double width = Bounds.Width;
            //double height = Bounds.Height;

            //if (width > 0 && height > 0)
            //{
            //    if (_renderTarget == null)
            //    {
            //        _renderTarget = new RenderTargetBitmap(new PixelSize((int)width, (int)height), new Vector(96, 96));
            //    }
            //    else if (_renderTarget.PixelSize.Width != (int)width || _renderTarget.PixelSize.Height != (int)height)
            //    {
            //        _renderTarget.Dispose();
            //        _renderTarget = new RenderTargetBitmap(new PixelSize((int)width, (int)height), new Vector(96, 96));
            //    }

            //    using var drawingContextImpl = _renderTarget.CreateDrawingContext(null);
            //    var skiaDrawingContextImpl = drawingContextImpl as ISkiaDrawingContextImpl;

            //    var canvas = skiaDrawingContextImpl.SkCanvas;

            //    canvas.Clear();
            //    canvas.Save();

            //    Draw(customState, canvas);

            //    canvas.Restore();

            //    context.DrawImage(_renderTarget,
            //        new Rect(0, 0, _renderTarget.PixelSize.Width, _renderTarget.PixelSize.Height),
            //        new Rect(0, 0, width, height));
            //}

#if USE_SKIA
            if (_customDrawOperation == null)
            {
                _customDrawOperation = new CustomDrawOperation();
            }

            _customDrawOperation.PresenterControl = this;
            _customDrawOperation.CustomState = customState;
            _customDrawOperation.Bounds = ZoomBorder != null ? ZoomBorder.Bounds : this.Bounds;

            context.Custom(_customDrawOperation);
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

                            if (customState.Container.Template != null)
                            {
                                customState.DataFlow.Bind(customState.Container.Template, db, record);
                            }

                            customState.DataFlow.Bind(customState.Container, db, record);
                        }
                    }
                    break;
                case PresenterType.Template:
                    {
                        if (customState.Container != null && customState.Renderer != null)
                        {
                            s_templatePresenter.Render(context, customState.Renderer, customState.Container, 0.0, 0.0);
                        }
                    }
                    break;
                case PresenterType.Editor:
                    {
                        if (customState.Container != null && customState.Renderer != null)
                        {
                            s_editorPresenter.Render(context, customState.Renderer, customState.Container, 0.0, 0.0);
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
