// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace Core2D.Avalonia.Views
{
    /// <summary>
    /// Interaction logic for <see cref="EditorMenuControl"/> xaml.
    /// </summary>
    public class EditorMenuControl : UserControl
    {
        public static readonly StyledProperty<bool> DrawDirtyRectsProperty =
            AvaloniaProperty.Register<EditorMenuControl, bool>(nameof(DrawDirtyRects));

        public bool DrawDirtyRects
        {
            get { return GetValue(DrawDirtyRectsProperty); }
            set { SetValue(DrawDirtyRectsProperty, value); }
        }

        public static readonly StyledProperty<bool> DrawFpsProperty =
            AvaloniaProperty.Register<EditorMenuControl, bool>(nameof(DrawFps));

        public bool DrawFps
        {
            get { return GetValue(DrawFpsProperty); }
            set { SetValue(DrawFpsProperty, value); }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EditorMenuControl"/> class.
        /// </summary>
        public EditorMenuControl()
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

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            DrawDirtyRects = VisualRoot.Renderer.DrawDirtyRects;
            DrawFps = VisualRoot.Renderer.DrawFps;
        }

        protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnDetachedFromVisualTree(e);
        }

        private void DebugDrawDirtyRects_Click(object sender, RoutedEventArgs e)
        {
            VisualRoot.Renderer.DrawDirtyRects = !VisualRoot.Renderer.DrawDirtyRects;
            DrawDirtyRects = VisualRoot.Renderer.DrawDirtyRects;
        }

        private void DebugDrawFps_Click(object sender, RoutedEventArgs e)
        {
            VisualRoot.Renderer.DrawFps = !VisualRoot.Renderer.DrawFps;
            DrawFps = VisualRoot.Renderer.DrawFps;
        }
    }
}
