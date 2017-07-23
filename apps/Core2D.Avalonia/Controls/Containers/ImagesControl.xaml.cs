// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Avalonia.Controls.Containers
{
    /// <summary>
    /// Interaction logic for <see cref="ImagesControl"/> xaml.
    /// </summary>
    public class ImagesControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ImagesControl"/> class.
        /// </summary>
        public ImagesControl()
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
