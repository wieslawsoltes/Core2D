// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Test2d;

namespace Test.Viewer
{
    /// <summary>
    /// 
    /// </summary>
    public static class ProjectViewer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="renderer"></param>
        /// <returns></returns>
        private static Viewbox ToViewbox(Container container, IRenderer renderer)
        {
            var visual = new DrawingVisual();

            using (var dc = visual.RenderOpen())
            {
                renderer.Draw(dc, container, container.Properties, null);
            }

            visual.Drawing.Freeze();

            var host = new VisualHost()
            {
                Width = container.Width,
                Height = container.Height
            };

            host.Visuals.Add(visual);

            var vb = new Viewbox() 
            { 
                Stretch = Stretch.Uniform 
            };

            vb.Child = host;

            return vb;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containers"></param>
        public static void Show(IList<Container> containers)
        {
            var renderer = new WpfRenderer();
            renderer.State.DrawShapeState = ShapeState.Printable;

            var sw = new ScrollViewer()
            {
                HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto
            };

            var sp = new StackPanel()
            {
                Margin = new Thickness(5),
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };

            sw.Content = sp;

            foreach (var container in containers)
            {
                var vb = ToViewbox(container, renderer);
                vb.Margin = new Thickness(5);
                sp.Children.Add(vb);
            }

            var window = new Window()
            {
                Title = "Test2d Viewer",
                Width = 900,
                Height = 680,
                WindowStartupLocation = WindowStartupLocation.CenterScreen,
                SnapsToDevicePixels = true,
                UseLayoutRounding = true
            };
            TextOptions.SetTextFormattingMode(window, TextFormattingMode.Display);

            window.Content = sw;
            window.ShowDialog();
        }
    }
}
