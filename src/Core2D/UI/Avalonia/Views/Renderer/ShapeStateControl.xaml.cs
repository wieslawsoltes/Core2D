// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views.Renderer
{
    /// <summary>
    /// Interaction logic for <see cref="ShapeStateControl"/> xaml.
    /// </summary>
    public class ShapeStateControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeStateControl"/> class.
        /// </summary>
        public ShapeStateControl()
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
