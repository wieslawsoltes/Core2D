// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
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
        /// <summary>
        /// Initializes a new instance of the <see cref="EditorMenuControl"/> class.
        /// </summary>
        public EditorMenuControl()
        {
            this.InitializeComponent();
            this.InitializeControl();
        }

        /// <summary>
        /// Initialize the Xaml components.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Initializes the control.
        /// </summary>
        private void InitializeControl()
        {
            this.FindControl<MenuItem>("DebugDrawDirtyRects").Click += DebugDrawDirtyRects_Click;
            this.FindControl<MenuItem>("DebugDrawFps").Click += DebugDrawFps_Click;
        }

        private void DebugDrawDirtyRects_Click(object sender, RoutedEventArgs e)
        {
            ToggleDrawDirtyRects();
        }

        private void DebugDrawFps_Click(object sender, RoutedEventArgs e)
        {
            ToggleDrawFps();
        }

        private void ToggleDrawDirtyRects()
        {
            bool value = !VisualRoot.Renderer.DrawDirtyRects;
            VisualRoot.Renderer.DrawDirtyRects = value;
            this.FindControl<CheckBox>("DebugDrawDirtyRectsCheckBox").IsChecked = value;
        }

        private void ToggleDrawFps()
        {
            bool value = !VisualRoot.Renderer.DrawFps;
            VisualRoot.Renderer.DrawFps = value;
            this.FindControl<CheckBox>("DebugDebugDrawFpsCheckBox").IsChecked = value;
        }
    }
}
