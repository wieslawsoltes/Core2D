// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Dock.Avalonia.Controls;

namespace Core2D.UI.Avalonia.Windows
{
    /// <summary>
    /// Interaction logic for <see cref="BrowserWindow"/> xaml.
    /// </summary>
    public class BrowserWindow : HostWindowBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BrowserWindow"/> class.
        /// </summary>
        public BrowserWindow()
        {
            InitializeComponent();
            this.AttachDevTools();
        }

        /// <summary>
        /// Initialize the Xaml components.
        /// </summary>
        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
