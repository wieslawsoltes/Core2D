// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows;
using System.Windows.Controls;

namespace Core2D.Wpf.Controls.Project
{
    /// <summary>
    /// Interaction logic for <see cref="PageControl"/> xaml.
    /// </summary>
    public partial class PageControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PageControl"/> class.
        /// </summary>
        public PageControl()
        {
            InitializeComponent();
            InitializeDrop();
        }

        /// <summary>
        /// Initializes control drag and drop handler.
        /// </summary>
        public void InitializeDrop()
        {
            this.AllowDrop = true;

            this.DragEnter +=
                (s, e) =>
                {
                    if (!e.Data.GetDataPresent(typeof(Core2D.Template)))
                    {
                        e.Effects = DragDropEffects.None;
                        e.Handled = true;
                    }
                };

            this.Drop +=
                (s, e) =>
                {
                    if (e.Data.GetDataPresent(typeof(Core2D.Template)))
                    {
                        try
                        {
                            var template = e.Data.GetData(typeof(Core2D.Template)) as Core2D.Template;
                            if (template != null)
                            {
                                if (this.DataContext != null)
                                {
                                    var page = this.DataContext as Core2D.Page;
                                    if (page != null)
                                    {
                                        page.Template = template;
                                    }
                                }
                                e.Handled = true;
                            }
                        }
                        catch (Exception) { }
                    }
                };
        }
    }
}
