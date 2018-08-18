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
            AllowDrop = true;

            DragEnter +=
                (s, e) =>
                {
                    if (!e.Data.GetDataPresent(typeof(IRecord)))
                    {
                        e.Effects = DragDropEffects.None;
                        e.Handled = true;
                    }
                };

            Drop +=
                (s, e) =>
                {
                    if (e.Data.GetDataPresent(typeof(IRecord)))
                    {
                        try
                        {
                            if (e.Data.GetData(typeof(IRecord)) is IRecord record)
                            {
                                if (DataContext != null)
                                {
                                    if (DataContext is IContext data)
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
