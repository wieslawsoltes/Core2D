// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Core2D.Wpf.Views.Data
{
    /// <summary>
    /// Interaction logic for <see cref="DataControl"/> xaml.
    /// </summary>
    public partial class DataControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DataControl"/> class.
        /// </summary>
        public DataControl()
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
                    if (!e.Data.GetDataPresent(typeof(Record)))
                    {
                        e.Effects = DragDropEffects.None;
                        e.Handled = true;
                    }
                };

            this.Drop +=
                (s, e) =>
                {
                    if (e.Data.GetDataPresent(typeof(Record)))
                    {
                        try
                        {
                            if (e.Data.GetData(typeof(Record)) is Record record)
                            {
                                if (this.DataContext != null)
                                {
                                    if (this.DataContext is Context data)
                                    {
                                        data.Record = record;
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
