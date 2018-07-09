// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Markup.Xaml;
using Dock.Avalonia.Controls;

namespace Core2D.Avalonia.Windows
{
    /// <summary>
    /// Interaction logic for <see cref="DocumentWindow"/> xaml.
    /// </summary>
    public class DocumentWindow : HostWindowBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentWindow"/> class.
        /// </summary>
        public DocumentWindow()
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
