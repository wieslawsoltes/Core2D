// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Diagnostics;
using System.Windows;
using Autofac;
using Microsoft.Win32;

namespace Core2D.SkiaViewAutofac
{
    public partial class MainWindow : Window
    {
        private IContainer Container { get; set; }
        private AutofacSkiaViewHelper Helper { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(typeof(MainWindow).Assembly);
            Container = builder.Build();
            Helper = new AutofacSkiaViewHelper(CanvasElement, Container.Resolve<IServiceProvider>());

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;

            FileOpen.Click += FileOpen_Click;
            FileClose.Click += FileClose_Click;
            FileExit.Click += FileExit_Click;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CanvasElement.Focusable = true;
            CanvasElement.Focus();
            Helper.RefreshRequested(null, null);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Container.Dispose();
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var dlg = new OpenFileDialog()
                {
                    Filter = "Project (*.project)|*.project|All (*.*)|*.*"
                };

                if (dlg.ShowDialog(this) == true)
                {
                    Helper.OpenProject(dlg.FileName);
                    Helper.RefreshRequested(null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private void FileClose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Helper.CloseProject();
                Helper.RefreshRequested(null, null);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
