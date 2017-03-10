// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Shape;
using Core2D.Style;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Core2D.Wpf.Views
{
    /// <summary>
    /// Interaction logic for <see cref="ShapeControl"/> xaml.
    /// </summary>
    public partial class ShapeControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ShapeControl"/> class.
        /// </summary>
        public ShapeControl()
        {
            InitializeComponent();
            InitializeDrop();
        }

        /// <summary>
        /// Initializes control drag and drop handler.
        /// </summary>
        public void InitializeDrop()
        {
            content.AllowDrop = true;

            content.DragEnter +=
                (s, e) =>
                {
                    if (!e.Data.GetDataPresent(typeof(ShapeStyle)))
                    {
                        e.Effects = DragDropEffects.None;
                        e.Handled = true;
                    }
                };

            content.Drop +=
                (s, e) =>
                {
                    if (e.Data.GetDataPresent(typeof(ShapeStyle)))
                    {
                        try
                        {
                            var style = e.Data.GetData(typeof(ShapeStyle)) as ShapeStyle;
                            if (style != null)
                            {
                                if (content.Content != null)
                                {
                                    var shape = content.Content as BaseShape;
                                    if (shape != null)
                                    {
                                        shape.Style = style;
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
