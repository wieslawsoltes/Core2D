// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Containers;
using Core2D.Shapes;
using Core2D.Style;
using System;
using System.Windows;
using System.Windows.Controls;

namespace Core2D.UI.Wpf.Views.Containers
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
                    if (!e.Data.GetDataPresent(typeof(IBaseShape)))
                    {
                        e.Effects = DragDropEffects.None;
                        e.Handled = true;
                    }
                };

            pointShape.Drop +=
                (s, e) =>
                {
                    if (e.Data.GetDataPresent(typeof(IBaseShape)))
                    {
                        try
                        {
                            if (e.Data.GetData(typeof(IBaseShape)) is IBaseShape shape)
                            {
                                if (pointShape.DataContext != null)
                                {
                                    if (pointShape.DataContext is IOptions options)
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
                    if (!e.Data.GetDataPresent(typeof(IShapeStyle)))
                    {
                        e.Effects = DragDropEffects.None;
                        e.Handled = true;
                    }
                };

            pointStyle.Drop +=
                (s, e) =>
                {
                    if (e.Data.GetDataPresent(typeof(IShapeStyle)))
                    {
                        try
                        {
                            if (e.Data.GetData(typeof(IShapeStyle)) is IShapeStyle style)
                            {
                                if (pointStyle.DataContext != null)
                                {
                                    if (pointStyle.DataContext is IOptions options)
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
                    if (!e.Data.GetDataPresent(typeof(IShapeStyle)))
                    {
                        e.Effects = DragDropEffects.None;
                        e.Handled = true;
                    }
                };

            selectionStyle.Drop +=
                (s, e) =>
                {
                    if (e.Data.GetDataPresent(typeof(IShapeStyle)))
                    {
                        try
                        {
                            if (e.Data.GetData(typeof(IShapeStyle)) is IShapeStyle style)
                            {
                                if (selectionStyle.DataContext != null)
                                {
                                    if (selectionStyle.DataContext is IOptions options)
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
                    if (!e.Data.GetDataPresent(typeof(IShapeStyle)))
                    {
                        e.Effects = DragDropEffects.None;
                        e.Handled = true;
                    }
                };

            helperStyle.Drop +=
                (s, e) =>
                {
                    if (e.Data.GetDataPresent(typeof(IShapeStyle)))
                    {
                        try
                        {
                            if (e.Data.GetData(typeof(IShapeStyle)) is IShapeStyle style)
                            {
                                if (helperStyle.DataContext != null)
                                {
                                    if (helperStyle.DataContext is IOptions options)
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
