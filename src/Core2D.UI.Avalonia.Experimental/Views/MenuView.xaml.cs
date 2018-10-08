// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Core2D.Editor;

namespace Core2D.UI.Avalonia.Views
{
    public partial class MenuView : UserControl
    {
        public static readonly StyledProperty<bool> DrawDirtyRectsProperty =
            AvaloniaProperty.Register<MenuView, bool>(nameof(DrawDirtyRects));

        public bool DrawDirtyRects
        {
            get { return GetValue(DrawDirtyRectsProperty); }
            set { SetValue(DrawDirtyRectsProperty, value); }
        }

        public static readonly StyledProperty<bool> DrawFpsProperty =
            AvaloniaProperty.Register<MenuView, bool>(nameof(DrawFps));

        public bool DrawFps
        {
            get { return GetValue(DrawFpsProperty); }
            set { SetValue(DrawFpsProperty, value); }
        }

        public static readonly StyledProperty<bool> LoggerEnabledProperty =
            AvaloniaProperty.Register<MenuView, bool>(nameof(LoggerEnabled));

        public bool LoggerEnabled
        {
            get { return GetValue(LoggerEnabledProperty); }
            set { SetValue(LoggerEnabledProperty, value); }
        }

        public MenuView()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
        {
            base.OnAttachedToVisualTree(e);

            DrawDirtyRects = VisualRoot.Renderer.DrawDirtyRects;
            DrawFps = VisualRoot.Renderer.DrawFps;
            LoggerEnabled = Log.Enabled;
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

        private void LoggerEnabled_Click(object sender, RoutedEventArgs e)
        {
            Log.Enabled = !Log.Enabled;
            LoggerEnabled = Log.Enabled;
        }
    }
}
