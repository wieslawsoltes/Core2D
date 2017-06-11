// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Diagnostics;
using System.Windows;
using Core2D.SkiaView;
using Microsoft.Win32;

namespace Core2D.SkiaViewNoAutofac
{
    public partial class MainWindow : Window
    {
        private SKElementHelper _helper;

        public MainWindow()
        {
            InitializeComponent();
            Initialize();
        }

        private void Initialize()
        {
            _helper = new SKElementHelper(CanvasElement);

            Loaded += MainWindow_Loaded;

            FileOpen.Click += FileOpen_Click;
            FileClose.Click += FileClose_Click;
            FileExit.Click += FileExit_Click;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CanvasElement.Focusable = true;
            CanvasElement.Focus();

            _helper.RefreshRequested(null, null);
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
                    _helper.OpenProject(dlg.FileName);
                    _helper.RefreshRequested(null, null);
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
                _helper.CloseProject();
                _helper.RefreshRequested(null, null);
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
