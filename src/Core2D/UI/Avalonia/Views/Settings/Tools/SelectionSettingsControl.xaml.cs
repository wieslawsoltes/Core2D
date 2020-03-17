// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views.Settings.Tools
{
    /// <summary>
    /// Interaction logic for <see cref="SelectionSettingsControl"/> xaml.
    /// </summary>
    public class SelectionSettingsControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SelectionSettingsControl"/> class.
        /// </summary>
        public SelectionSettingsControl()
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
    }
}
