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
using Test.Core;

namespace Test.Windows
{
    public partial class ContainerWindow : Window
    {
        public ContainerWindow()
        {
            InitializeComponent();

            Loaded += (s, e) => InitializeCanvas(); 
        }

        private void InitializeCanvas()
        {
            var editor = DataContext as Editor;

            canvas.PreviewMouseLeftButtonDown +=
                (s, e) =>
                {
                    canvas.Focus();
                    if (editor.IsLeftAvailable())
                    {
                        var p = e.GetPosition(canvas);
                        editor.Left(p.X, p.Y);
                    }
                };

            canvas.PreviewMouseRightButtonDown +=
                (s, e) =>
                {
                    canvas.Focus();
                    if (editor.IsRightAvailable())
                    {
                        var p = e.GetPosition(canvas);
                        editor.Right(p.X, p.Y);
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
        }
    }
}
