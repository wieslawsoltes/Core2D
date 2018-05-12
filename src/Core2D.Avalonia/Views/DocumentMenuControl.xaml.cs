// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Avalonia.Views
{
    /// <summary>
    /// Interaction logic for <see cref="DocumentMenuControl"/> xaml.
    /// </summary>
    public class DocumentMenuControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentMenuControl"/> class.
        /// </summary>
        public DocumentMenuControl()
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
    }
}
