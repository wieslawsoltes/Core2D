// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Project;
using Core2D.Shape;
using Core2D.Style;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Core2D.Wpf.Controls.Project
{
    /// <summary>
    /// Interaction logic for <see cref="OptionsControl"/> xaml.
    /// </summary>
    public partial class OptionsControl : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OptionsControl"/> class.
        /// </summary>
        public OptionsControl()
        {
            InitializeComponent();
            InitializeDropPointShape();
            InitializeDropPointStyle();
            InitializeDropSelectionStyle();
            InitializeDropHelperStyle();
        }

        /// <summary>
        /// Initializes control drag and drop handler.
        /// </summary>
        public void InitializeDropPointShape()
        {
            pointShape.AllowDrop = true;

            pointShape.DragEnter +=
                (s, e) =>
                {
                    if (!e.Data.GetDataPresent(typeof(BaseShape)))
                    {
                        e.Effects = DragDropEffects.None;
                        e.Handled = true;
                    }
                };

            pointShape.Drop +=
                (s, e) =>
                {
                    if (e.Data.GetDataPresent(typeof(BaseShape)))
                    {
                        try
                        {
                            var shape = e.Data.GetData(typeof(BaseShape)) as BaseShape;
                            if (shape != null)
                            {
                                if (pointShape.DataContext != null)
                                {
                                    var options = pointShape.DataContext as XOptions;
                                    if (options != null)
                                    {
                                        options.PointShape = shape;
                                    }
                                }
                                e.Handled = true;
                            }
                        }
                        catch (Exception) { }
                    }
                };
        }

        /// <summary>
        /// Initializes control drag and drop handler.
        /// </summary>
        public void InitializeDropPointStyle()
        {
            pointStyle.AllowDrop = true;

            pointStyle.DragEnter +=
                (s, e) =>
                {
                    if (!e.Data.GetDataPresent(typeof(ShapeStyle)))
                    {
                        e.Effects = DragDropEffects.None;
                        e.Handled = true;
                    }
                };

            pointStyle.Drop +=
                (s, e) =>
                {
                    if (e.Data.GetDataPresent(typeof(ShapeStyle)))
                    {
                        try
                        {
                            var style = e.Data.GetData(typeof(ShapeStyle)) as ShapeStyle;
                            if (style != null)
                            {
                                if (pointStyle.DataContext != null)
                                {
                                    var options = pointStyle.DataContext as XOptions;
                                    if (options != null)
                                    {
                                        options.PointStyle = style;
                                    }
                                }
                                e.Handled = true;
                            }
                        }
                        catch (Exception) { }
                    }
                };
        }

        /// <summary>
        /// Initializes control drag and drop handler.
        /// </summary>
        public void InitializeDropSelectionStyle()
        {
            selectionStyle.AllowDrop = true;

            selectionStyle.DragEnter +=
                (s, e) =>
                {
                    if (!e.Data.GetDataPresent(typeof(ShapeStyle)))
                    {
                        e.Effects = DragDropEffects.None;
                        e.Handled = true;
                    }
                };

            selectionStyle.Drop +=
                (s, e) =>
                {
                    if (e.Data.GetDataPresent(typeof(ShapeStyle)))
                    {
                        try
                        {
                            var style = e.Data.GetData(typeof(ShapeStyle)) as ShapeStyle;
                            if (style != null)
                            {
                                if (selectionStyle.DataContext != null)
                                {
                                    var options = selectionStyle.DataContext as XOptions;
                                    if (options != null)
                                    {
                                        options.SelectionStyle = style;
                                    }
                                }
                                e.Handled = true;
                            }
                        }
                        catch (Exception) { }
                    }
                };
        }

        /// <summary>
        /// Initializes control drag and drop handler.
        /// </summary>
        public void InitializeDropHelperStyle()
        {
            helperStyle.AllowDrop = true;

            helperStyle.DragEnter +=
                (s, e) =>
                {
                    if (!e.Data.GetDataPresent(typeof(ShapeStyle)))
                    {
                        e.Effects = DragDropEffects.None;
                        e.Handled = true;
                    }
                };

            helperStyle.Drop +=
                (s, e) =>
                {
                    if (e.Data.GetDataPresent(typeof(ShapeStyle)))
                    {
                        try
                        {
                            var style = e.Data.GetData(typeof(ShapeStyle)) as ShapeStyle;
                            if (style != null)
                            {
                                if (helperStyle.DataContext != null)
                                {
                                    var options = helperStyle.DataContext as XOptions;
                                    if (options != null)
                                    {
                                        options.HelperStyle = style;
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
