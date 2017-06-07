// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Diagnostics;
using System.Windows;
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Renderer;
using Microsoft.Win32;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Core2D.SkiaView
{
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private ContainerPresenter _presenter;
        private ShapeRenderer _renderer;
        private XProject _project;

        public MainWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            _serviceProvider = serviceProvider;
            _presenter = _serviceProvider.GetService<ContainerPresenter>();
            _renderer = _serviceProvider.GetService<ShapeRenderer>();

            Loaded += MainWindow_Loaded;
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            canvas.Focusable = true;
            canvas.Focus();
            OnRefreshRequested(null, null);
        }

        private void OnPaintCanvas(object sender, SKPaintSurfaceEventArgs e)
        {
            OnPaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }

        private void OnRefreshRequested(object sender, EventArgs e)
        {
            canvas.InvalidateVisual();
        }

        private void OnPaintSurface(SKCanvas canvas, int width, int height)
        {
            var container = _project?.CurrentContainer;
            if (container != null)
            {
                var matrix = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
                double offsetX = (this.canvas.ActualWidth * matrix.M11 - container?.Width ?? 0) / 2.0;
                double offsetY = (this.canvas.ActualHeight * matrix.M22 - container?.Height ?? 0) / 2.0;
                canvas.Clear(SKColors.White);
                _presenter?.Render(canvas, _renderer, container, offsetX, offsetY);
            }
        }

        public void OpenProject(string path)
        {
            try
            {
                var fileIO = _serviceProvider.GetService<IFileSystem>();
                var jsonSerializer = _serviceProvider.GetService<IJsonSerializer>();
                var project = XProject.Open(path, fileIO, jsonSerializer);
                if (project != null)
                {
                    _project = project;
                    _renderer.ClearCache(isZooming: false);
                    _renderer.State.ImageCache = project;
                    OnRefreshRequested(null, null);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private void OpenProject()
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "Project (*.project)|*.project|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = ""
            };

            if (dlg.ShowDialog(this) == true)
            {
                OpenProject(dlg.FileName);
            }
        }

        public void CloseProject()
        {
            _project = null;
            _renderer.ClearCache(isZooming: false);
            _renderer.State.ImageCache = null;
            OnRefreshRequested(null, null);
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            OpenProject();
        }

        private void FileClose_Click(object sender, RoutedEventArgs e)
        {
            CloseProject();
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
