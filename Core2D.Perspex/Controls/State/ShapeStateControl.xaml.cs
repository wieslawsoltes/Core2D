// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Perspex.Controls;
using Perspex.Markup.Xaml;

namespace Core2D.Perspex.Controls.State
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
