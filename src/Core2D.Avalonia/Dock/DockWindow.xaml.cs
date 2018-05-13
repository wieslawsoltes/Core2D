// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Dock.Model;

namespace Core2D.Avalonia.Dock
{
    /// <summary>
    /// Interaction logic for <see cref="DockWindow"/> xaml.
    /// </summary>
    public class DockWindow : Window, IDockWindow
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DockWindow"/> class.
        /// </summary>
        public DockWindow()
        {
            this.InitializeComponent();
            this.AttachDevTools();
        }

        /// <summary>
        /// Initialize the Xaml components.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <inheritdoc/>
        public void Present()
        {
            this.Show();
        }

        /// <inheritdoc/>
        public void Destroy()
        {
            this.Close();
        }

        /// <inheritdoc/>
        public void SetPosition(double x, double y)
        {
            Position = new Point(x, y);
        }

        /// <inheritdoc/>
        public void GetPosition(ref double x, ref double y)
        {
            x = this.Position.X;
            y = this.Position.Y;
        }

        /// <inheritdoc/>
        public void SetSize(double width, double height)
        {
            this.Width = width;
            this.Height = height;
        }

        /// <inheritdoc/>
        public void GetSize(ref double width, ref double height)
        {
            width = this.Width;
            height = this.Height;
        }

        /// <inheritdoc/>
        public void SetTitle(string title)
        {
            this.Title = title;
        }

        /// <inheritdoc/>
        public void SetContext(object context)
        {
            this.DataContext = context;
        }
    }
}
