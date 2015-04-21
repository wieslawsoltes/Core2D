// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Practices.Prism.Commands;
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

namespace Test.Windows
{
    public partial class MainWindow : Window, IView
    {
        public MainWindow()
        {
            InitializeComponent();

            InitializeContext();
        }

        private void InitializeContext()
        {
            var context = new EditorContext();

            context.Initialize(this);

            context.Commands.LayersWindowCommand = new DelegateCommand(
                () =>
                {
                    (new LayersWindow() { Owner = this, DataContext = context }).Show();
                });

            context.Commands.StyleWindowCommand = new DelegateCommand(
                () =>
                {
                    (new StyleWindow() { Owner = this, DataContext = context }).Show();
                });

            context.Commands.StylesWindowCommand = new DelegateCommand(
                () =>
                {
                    (new StylesWindow() { Owner = this, DataContext = context }).Show();
                });

            context.Commands.ShapesWindowCommand = new DelegateCommand(
                () =>
                {
                    (new ShapesWindow() { Owner = this, DataContext = context }).Show();
                });

            context.Commands.ContainerWindowCommand = new DelegateCommand(
                () =>
                {
                    (new ContainerWindow() { Owner = this, DataContext = context }).Show();
                });

            AllowDrop = true;
            
            Drop += 
                (s, e) =>
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    try
                    {
                        var files = (string[])e.Data.GetData(DataFormats.FileDrop);
                        if (files != null && files.Length == 1)
                        {
                            string path = files[0];
                            if (!string.IsNullOrEmpty(path))
                            {
                                context.Open(path);
                                e.Handled = true;
                            }
                        }
                    }
                    catch { }
                }
            };
         
            Loaded +=
                (s, e) =>
                {
                    //Demo.All(_context.Editor.Container, 10);
                };

            DataContext = context;
        }
    }
}
