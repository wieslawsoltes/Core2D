// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.Avalonia.Views.Shapes
{
    /// <summary>
    /// Interaction logic for <see cref="GroupControl"/> xaml.
    /// </summary>
    public class GroupControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GroupControl"/> class.
        /// </summary>
        public GroupControl()
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
