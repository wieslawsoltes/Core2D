// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Test2d;
using TestEDITOR;

namespace Test.Windows
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ContainerWindow : Window
    {
        /// <summary>
        /// 
        /// </summary>
        public ContainerWindow()
        {
            InitializeComponent();

            grid.EnableAutoFit = true;

            border.InvalidateChild = (z, x, y) =>
            {
                var context = DataContext as EditorContext;
                context.Editor.Renderer.Zoom = z;
                context.Editor.Renderer.PanX = x;
                context.Editor.Renderer.PanY = y;
                //context.Editor.Renderer.ClearCache();
                //context.Editor.Project.CurrentContainer.Invalidate();
            };

            border.AutoFitChild = (width, height) =>
            {
                if (border != null && DataContext != null)
                {
                    var context = DataContext as EditorContext;
                    border.AutoFit(
                        width,
                        height,
                        context.Editor.Project.CurrentContainer.Width,
                        context.Editor.Project.CurrentContainer.Height);
                    //context.Editor.Renderer.ClearCache();
                    //context.Editor.Project.CurrentContainer.Invalidate();
                }
            };

            border.MouseDown += (s, e) =>
            {
                if (e.ChangedButton == MouseButton.Middle && e.ClickCount == 2)
                {
                    grid.AutoFit();
                }
                
                if (e.ChangedButton == MouseButton.Middle && e.ClickCount == 3)
                {
                    grid.ResetZoomAndPan();
                }
            };
            
            Loaded += (s, e) =>
            {
                ((DataContext as EditorContext).Editor.Renderer as ObservableObject).PropertyChanged +=
                (_s, _e) =>
                {
                    if (_e.PropertyName == "Zoom")
                    {
                        var context = DataContext as EditorContext;
                        double value = context.Editor.Renderer.Zoom;
                        border.Scale.ScaleX = value;
                        border.Scale.ScaleY = value;
                        //context.Editor.Renderer.ClearCache();
                        //context.Editor.Project.CurrentContainer.Invalidate();
                    }

                    if (_e.PropertyName == "PanX")
                    {
                        var context = DataContext as EditorContext;
                        double value = context.Editor.Renderer.PanX;
                        border.Translate.X = value;
                        //context.Editor.Renderer.ClearCache();
                        //context.Editor.Project.CurrentContainer.Invalidate();
                    }

                    if (_e.PropertyName == "PanY")
                    {
                        var context = DataContext as EditorContext;
                        double value = context.Editor.Renderer.PanY;
                        border.Translate.Y = value;
                        //context.Editor.Renderer.ClearCache();
                        //context.Editor.Project.CurrentContainer.Invalidate();
                    }
                };
            };
        }
    }
}
