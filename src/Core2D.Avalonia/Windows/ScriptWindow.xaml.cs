// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Dock.Avalonia;

namespace Core2D.Avalonia.Windows
{
    /// <summary>
    /// Interaction logic for <see cref="ScriptWindow"/> xaml.
    /// </summary>
    public class ScriptWindow : HostWindowBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScriptWindow"/> class.
        /// </summary>
        public ScriptWindow()
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
    }
}
