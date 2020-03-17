// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Core2D.UI.Avalonia.Renderer;
using Core2D.Containers;
using Core2D.Renderer;
using Core2D.Renderer.Presenters;
using Core2D.Data;

namespace Core2D.UI.Avalonia.Views
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
        /// </summarypresenter
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
    /// Interaction logic for <see cref="ContainerControl"/> xaml.
    /// </summary>
    public class ContainerControl : UserControl
    {
        private static IContainerPresenter s_editorPresenter = new EditorPresenter();
        private static IContainerPresenter s_templatePresenter = new TemplatePresenter();
        private static IContainerPresenter s_exportPresenter = new ExportPresenter();

        /// <summary>
        /// Gets or sets container property.
        /// </summary>
        public static readonly AvaloniaProperty<IPageContainer> ContainerProperty =
            AvaloniaProperty.Register<ContainerControl, IPageContainer>(nameof(Container), null);

        /// <summary>
        /// Gets or sets renderer property.
        /// </summary>
        public static readonly AvaloniaProperty<IShapeRenderer> RendererProperty =
            AvaloniaProperty.Register<ContainerControl, IShapeRenderer>(nameof(Renderer), null);

        /// <summary>
        /// Gets or sets data flow property.
        /// </summary>
        public static readonly AvaloniaProperty<IDataFlow> DataFlowProperty =
            AvaloniaProperty.Register<ContainerControl, IDataFlow>(nameof(DataFlow), null);

        /// <summary>
        /// Gets or sets data flow property.
        /// </summary>
        public static readonly AvaloniaProperty<PresenterType> PresenterTypeProperty =
            AvaloniaProperty.Register<ContainerControl, PresenterType>(nameof(PresenterType), PresenterType.None);

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
        /// Initializes a new instance of the <see cref="ContainerControl"/> class.
        /// </summary>
        public ContainerControl()
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
        /// Renders container control contents.
        /// </summary>
        /// <param name="context">The drawing context.</param>
        public override void Render(DrawingContext context)
        {
            base.Render(context);

            var container = Container;
            var renderer = Renderer ?? GetValue(RendererOptions.RendererProperty);
            var dataFlow = DataFlow ?? GetValue(RendererOptions.DataFlowProperty);
            var presenterType = PresenterType;

            switch (presenterType)
            {
                case PresenterType.None:
                    break;
                case PresenterType.Data:
                    {
                        if (container != null && dataFlow != null)
                        {
                            var db = (object)container.Data.Properties;
                            var record = (object)container.Data.Record;

                            if (container.Template != null)
                            {
                                dataFlow.Bind(container.Template, db, record);
                            }

                            dataFlow.Bind(container, db, record);
                        }
                    }
                    break;
                case PresenterType.Template:
                    {
                        if (container != null && renderer != null)
                        {
                            s_templatePresenter.Render(context, renderer, Container, 0.0, 0.0);
                        }
                    }
                    break;
                case PresenterType.Editor:
                    {
                        if (container != null && renderer != null)
                        {
                            s_editorPresenter.Render(context, renderer, Container, 0.0, 0.0);
                        }
                    }
                    break;
                case PresenterType.Export:
                    {
                        if (container != null && renderer != null)
                        {
                            s_exportPresenter.Render(context, renderer, Container, 0.0, 0.0);
                        }
                    }
                    break;
            }
        }
    }
}
