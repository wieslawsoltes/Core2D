// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Test2d;

namespace Test.Controls
{
    /// <summary>
    /// 
    /// </summary>
    internal class VisualHost : FrameworkElement
    {
        /// <summary>
        /// 
        /// </summary>
        public IList<DrawingVisual> Visuals { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public VisualHost()
        {
            Visuals = new List<DrawingVisual>();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="drawingContext"></param>
        protected override void OnRender(DrawingContext drawingContext)
        {
            base.OnRender(drawingContext);

            foreach (var visual in Visuals)
            {
                drawingContext.DrawDrawing(visual.Drawing);
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public static class ProjectViewer
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="argbColor"></param>
        /// <returns></returns>
        private static SolidColorBrush ToBrush(ArgbColor argbColor)
        {
            var color = Color.FromArgb(
                argbColor.A, 
                argbColor.R, 
                argbColor.G, 
                argbColor.B);
            var brush = new SolidColorBrush(color);
            brush.Freeze();
            return brush;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="renderer"></param>
        /// <returns></returns>
        private static Viewbox ToViewbox(Container container, WpfRenderer renderer)
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
        /// <param name="container"></param>
        public static void Show(Container container)
        {
            try
            {
                var renderer = new WpfRenderer();
                renderer.State.DrawShapeState = ShapeState.Printable;

                var vb = ToViewbox(container, renderer);

                var window = new Window()
                {
                    Title = "Viewer",
                    Width = 800,
                    Height = 600,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                window.Content = vb;

                window.Show();
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="containers"></param>
        public static void Show(IList<Container> containers)
        {
            try
            {
                var renderer = new WpfRenderer();
                renderer.State.DrawShapeState = ShapeState.Printable;

                var sw = new ScrollViewer();

                var sp = new StackPanel()
                {
                    Margin = new Thickness(5)
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
                    Title = "Viewer",
                    Width = 800,
                    Height = 600,
                    WindowStartupLocation = WindowStartupLocation.CenterScreen
                };

                window.Content = sw;

                window.Show();
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }
    }
}
