// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Core2D.Avalonia.Renderer;
using Core2D.Containers;
using Core2D.Renderer;
using Core2D.Renderer.Presenters;

namespace Core2D.Avalonia.Views
{
    /// <summary>
    /// Interaction logic for <see cref="ContainerControl"/> xaml.
    /// </summary>
    public class ContainerControl : UserControl
    {
        private static IContainerPresenter s_editorPresenter = new EditorPresenter();
        private static IContainerPresenter s_templatePresenter = new TemplatePresenter();

        /// <summary>
        /// Gets or sets container property.
        /// </summary>
        public static readonly AvaloniaProperty<IPageContainer> ContainerProperty =
            AvaloniaProperty.Register<ContainerControl, IPageContainer>(nameof(Container));

        /// <summary>
        /// Gets or sets renderer property.
        /// </summary>
        public static readonly AvaloniaProperty<IShapeRenderer> RendererProperty =
            AvaloniaProperty.Register<ContainerControl, IShapeRenderer>(nameof(Renderer));

        /// <summary>
        /// Gets or sets container property.
        /// </summary>
        public IPageContainer Container
        {
            get { return GetValue(ContainerProperty); }
            set { SetValue(ContainerProperty, value); }
        }

        /// <summary>
        ///  Gets or sets renderer property.
        /// </summary>
        public IShapeRenderer Renderer
        {
            get { return GetValue(RendererProperty); }
            set { SetValue(RendererProperty, value); }
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

            if (Container != null)
            {
                if (Renderer != null)
                {
                    s_templatePresenter.Render(context, Renderer, Container, 0.0, 0.0);
                    s_editorPresenter.Render(context, Renderer, Container, 0.0, 0.0);
                }
                else
                {
                    var renderer = GetValue(RendererOptions.RendererProperty);
                    if (renderer != null)
                    {
                        s_templatePresenter.Render(context, renderer, Container, 0.0, 0.0);
                        s_editorPresenter.Render(context, renderer, Container, 0.0, 0.0);
                    }
                }
            }
        }
    }
}
