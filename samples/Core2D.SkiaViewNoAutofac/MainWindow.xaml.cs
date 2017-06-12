// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;

namespace Core2D.SkiaViewNoAutofac
{
    public partial class MainWindow : Window
    {
        private NoAutofacSkiaViewHelper Helper { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            Helper = new NoAutofacSkiaViewHelper(CanvasElement);
            Loaded += MainWindow_Loaded;
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
