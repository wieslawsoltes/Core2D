// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Diagnostics;
using System.Windows;
using Autofac;
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
        private readonly IContainer _container;
        private readonly IServiceProvider _serviceProvider;
        private ContainerPresenter _presenter;
        private ShapeRenderer _renderer;
        private XProject _project;

        public MainWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();
            InitializeContainer();
            
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            
            FileOpen.Click += FileOpen_Click;
            FileClose.Click += FileClose_Click;
            FileExit.Click += FileExit_Click;
            
            CanvasElement.PaintSurface += PaintSurface;
        }

        private void InitializeContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyModules(typeof(MainWindow).Assembly);

            _container = builder.Build();

            _serviceProvider = _container.Resolve<IServiceProvider>();

            _presenter = _serviceProvider.GetService<ContainerPresenter>();
            _renderer = _serviceProvider.GetService<ShapeRenderer>();
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CanvasElement.Focusable = true;
            CanvasElement.Focus();
            OnRefreshRequested(null, null);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _container.Dispose();
        }

        private void FileOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenProject();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        private void FileClose_Click(object sender, RoutedEventArgs e)
        {
            CloseProject();
        }

        private void FileExit_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }

        private void RefreshRequested(object sender, EventArgs e)
        {
            CanvasElement.InvalidateVisual();
        }

        private void PaintSurface(SKCanvas canvas, int width, int height)
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

        private void SetProject(XProject project)
        {
            _project = project;
            _renderer.ClearCache(isZooming: false);
            _renderer.State.ImageCache = project;
        }

        private void OpenProject(string path)
        {
            var project = XProject.Open(path, 
                _serviceProvider.GetService<IFileSystem>(), 
                _serviceProvider.GetService<IJsonSerializer>());

            SetProject(project);
            RefreshRequested(null, null);
        }

        private void OpenProject()
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "Project (*.project)|*.project|All (*.*)|*.*"
            };

            if (dlg.ShowDialog(this) == true)
            {
                OpenProject(dlg.FileName);
            }
        }

        private void CloseProject()
        {
            SetProject(null);
            RefreshRequested(null, null);
        }
    }
}
