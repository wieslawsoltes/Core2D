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
        private bool _isLoaded = false;

        /// <summary>
        /// 
        /// </summary>
        public ContainerWindow()
        {
            InitializeComponent();

            Initialize();
        }

        /// <summary>
        /// 
        /// </summary>
        private void Initialize()
        {
            border.InvalidateChild =
                (z, x, y) =>
                {
                    var context = DataContext as EditorContext;
                    bool invalidate = context.Editor.Renderer.State.Zoom != z;

                    context.Editor.Renderer.State.Zoom = z;
                    context.Editor.Renderer.State.PanX = x;
                    context.Editor.Renderer.State.PanY = y;

                    if (invalidate)
                    {
                        context.Invalidate();
                    }
                };

            border.AutoFitChild =
                (width, height) =>
                {
                    if (border != null && DataContext != null)
                    {
                        var context = DataContext as EditorContext;
                        if (!context.Renderer.State.EnableAutofit)
                            return;

                        border.AutoFit(
                            width,
                            height,
                            context.Editor.Project.CurrentContainer.Width,
                            context.Editor.Project.CurrentContainer.Height);
                    }
                };

            border.MouseDown +=
                (s, e) =>
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

            Loaded +=
                (s, e) =>
                {
                    if (_isLoaded)
                        return;
                    else
                        _isLoaded = true;

                    var context = DataContext as EditorContext;
                    grid.EnableAutoFit = context.Editor.Renderer.State.EnableAutofit;
                };
        }
    }
}
