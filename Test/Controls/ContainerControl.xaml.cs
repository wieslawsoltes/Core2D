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
using System.Windows.Navigation;
using System.Windows.Shapes;
using Test2d;

namespace Test.Controls
{
    /// <summary>
    /// 
    /// </summary>
    public partial class ContainerControl : UserControl
    {
        private bool _isLoaded = false;

        /// <summary>
        /// 
        /// </summary>
        public ContainerControl()
        {
            InitializeComponent();

            Loaded += 
                (s, e) =>
                {
                    if (_isLoaded)
                        return;
                    else
                        _isLoaded = true;

                    InitializeCanvas();
                };
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeCanvas()
        {
            var editor = canvas.DataContext as Editor;

            canvas.PreviewMouseLeftButtonDown +=
                (s, e) =>
                {
                    canvas.Focus();
                    if (editor.IsLeftDownAvailable())
                    {
                        var p = e.GetPosition(canvas);
                        editor.LeftDown(p.X, p.Y);
                    }
                };
            
            canvas.PreviewMouseLeftButtonUp +=
                (s, e) =>
                {
                    canvas.Focus();
                    if (editor.IsLeftUpAvailable())
                    {
                        var p = e.GetPosition(canvas);
                        editor.LeftUp(p.X, p.Y);
                    }
                };

            canvas.PreviewMouseRightButtonDown +=
                (s, e) =>
                {
                    canvas.Focus();
                    if (editor.IsRightDownAvailable())
                    {
                        var p = e.GetPosition(canvas);
                        editor.RightDown(p.X, p.Y);
                    }
                };

            canvas.PreviewMouseRightButtonUp +=
                (s, e) =>
                {
                    canvas.Focus();
                    if (editor.IsRightUpAvailable())
                    {
                        var p = e.GetPosition(canvas);
                        editor.RightUp(p.X, p.Y);
                    }
                };
            
            canvas.PreviewMouseMove +=
                (s, e) =>
                {
                    canvas.Focus();
                    if (editor.IsMoveAvailable())
                    {
                        var p = e.GetPosition(canvas);
                        editor.Move(p.X, p.Y);
                    }
                };

            canvas.Focus();
        }
    }
}
