// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Core2D.Avalonia.Renderer;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Renderer.Presenters;

namespace Core2D.Avalonia.Views
{
    /// <summary>
    /// Interaction logic for <see cref="ContainerViewControl"/> xaml.
    /// </summary>
    public class ContainerViewControl : UserControl
    {
        private static ContainerPresenter _presenter = new EditorPresenter();

        /// <summary>
        /// Gets or sets container property.
        /// </summary>
        public static readonly AvaloniaProperty<XContainer> ContainerProperty =
            AvaloniaProperty.Register<ContainerViewControl, XContainer>(nameof(Container));

        /// <summary>
        /// Gets or sets renderer property.
        /// </summary>
        public static readonly AvaloniaProperty<ShapeRenderer> RendererProperty =
            AvaloniaProperty.Register<ContainerViewControl, ShapeRenderer>(nameof(Renderer));

        /// <summary>
        /// Gets or sets container property.
        /// </summary>
        public XContainer Container
        {
            get { return GetValue(ContainerProperty); }
            set { SetValue(ContainerProperty, value); }
        }

        /// <summary>
        ///  Gets or sets renderer property.
        /// </summary>
        public ShapeRenderer Renderer
        {
            get { return GetValue(RendererProperty); }
            set { SetValue(RendererProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ContainerViewControl"/> class.
        /// </summary>
        public ContainerViewControl()
        {
            this.InitializeComponent();
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
                    _presenter.Render(context, Renderer, Container, 0.0, 0.0);
                }
                else
                {
                    var renderer = GetValue(RendererOptions.RendererProperty);
                    if (renderer != null)
                    {
                        _presenter.Render(context, renderer, Container, 0.0, 0.0);
                    }
                }
            }
        }
    }
}
