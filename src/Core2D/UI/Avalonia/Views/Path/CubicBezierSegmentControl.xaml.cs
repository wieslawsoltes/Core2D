// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Core2D.UI.Avalonia.Views.Path
{
    /// <summary>
    /// Interaction logic for <see cref="CubicBezierSegmentControl"/> xaml.
    /// </summary>
    public class CubicBezierSegmentControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CubicBezierSegmentControl"/> class.
        /// </summary>
        public CubicBezierSegmentControl()
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
