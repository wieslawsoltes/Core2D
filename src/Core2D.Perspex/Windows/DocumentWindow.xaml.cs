// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Perspex.Controls;
using Perspex.Markup.Xaml;

namespace Core2D.Perspex.Windows
{
    /// <summary>
    /// Interaction logic for <see cref="DocumentWindow"/> xaml.
    /// </summary>
    public class DocumentWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentWindow"/> class.
        /// </summary>
        public DocumentWindow()
        {
            this.InitializeComponent();
            App.AttachDevTools(this);
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
