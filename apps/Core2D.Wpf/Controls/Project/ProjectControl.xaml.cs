// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows.Controls;
using System.Windows.Input;

namespace Core2D.Wpf.Controls.Project
{
    /// <summary>
    /// Interaction logic for <see cref="ProjectControl"/> xaml.
    /// </summary>
    public partial class ProjectControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProjectControl"/> class.
        /// </summary>
        public ProjectControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Occurs when the right mouse button is pressed while the mouse pointer is over this element.
        /// </summary>
        /// <param name="sender">The object where the event handler is attached.</param>
        /// <param name="e">The event data.</param>
        private void TreeViewItem_MouseRightButtonDown(object sender, MouseEventArgs e)
        {
            if (sender is TreeViewItem item)
            {
                item.IsSelected = true;
                item.BringIntoView();
                e.Handled = true;
            }
        }
    }
}
