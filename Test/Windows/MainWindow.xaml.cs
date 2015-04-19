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

namespace Test.Windows
{
    public partial class MainWindow : Window, IView
    {
        private EditorContext _context;

        public MainWindow()
        {
            InitializeComponent();

            _context = new EditorContext();

            _context.Initialize(this);

            _context.LayersWindowCommand = new DelegateCommand(
                () =>
                {
                    (new LayersWindow() { Owner = this, DataContext = _context }).Show();
                });

            _context.StyleWindowCommand = new DelegateCommand(
                () =>
                {
                    (new StyleWindow() { Owner = this, DataContext = _context }).Show();
                });

            _context.StylesWindowCommand = new DelegateCommand(
                () =>
                {
                    (new StylesWindow() { Owner = this, DataContext = _context }).Show();
                });

            _context.ShapesWindowCommand = new DelegateCommand(
                () =>
                {
                    (new ShapesWindow() { Owner = this, DataContext = _context }).Show();
                });

            _context.ContainerWindowCommand = new DelegateCommand(
                () =>
                {
                    (new ContainerWindow() { Owner = this, DataContext = _context }).Show();
                });

            Loaded +=
                (s, e) =>
                {
                    //Demo.All(_context.Editor.Container, 10);
                };

            DataContext = _context;
        }
    }
}
