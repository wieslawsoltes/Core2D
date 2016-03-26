// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Perspex.Controls;
using Perspex.Markup.Xaml;

namespace Core2D.Perspex.Controls.Shapes
{
    /// <summary>
    /// Interaction logic for <see cref="RectangleControl"/> xaml.
    /// </summary>
    public class RectangleControl : UserControl
    {
        /// <summary>
        /// Gets an instance of a <see cref="RectangleControl"/>.
        /// </summary>
        public static RectangleControl Instance = new RectangleControl();

        /// <summary>
        /// Initializes a new instance of the <see cref="RectangleControl"/> class.
        /// </summary>
        public RectangleControl()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Initialize the Xaml components.
        /// </summary>
        private void InitializeComponent()
        {
            PerspexXamlLoader.Load(this);
        }
    }
}
