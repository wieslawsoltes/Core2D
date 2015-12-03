// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Perspex.Controls;
using Perspex.Markup.Xaml;

namespace Core2D.Perspex.Controls.Shapes
{
    /// <summary>
    /// Interaction logic for <see cref="PointControl"/> xaml.
    /// </summary>
    public class PointControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PointControl"/> class.
        /// </summary>
        public PointControl()
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
