// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Editor;
using Perspex.Controls;
using Perspex.Markup.Xaml;

namespace Core2D.Perspex.Views
{
    /// <summary>
    /// Interaction logic for <see cref="EditorMenuControl"/> xaml.
    /// </summary>
    public class EditorMenuControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EditorMenuControl"/> class.
        /// </summary>
        public EditorMenuControl()
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
