// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows;
using Core2D.FileSystem.DotNet;
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Renderer.Presenters;
using Core2D.Renderer.SkiaSharp;
using Core2D.Serializer.Newtonsoft;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

namespace Core2D.SkiaView
{
    public class SKElementHelper
    {
        private readonly IFileSystem _fileSystem;
        private readonly IJsonSerializer _jsonSerializer;
        private readonly ContainerPresenter _presenter;
        private readonly ShapeRenderer _renderer;
        private readonly SKElement _element;

        public XProject Project { get; set; }

        public SKElementHelper(SKElement element)
        {
            _fileSystem = new DotNetFileSystem();
            _jsonSerializer = new NewtonsoftJsonSerializer();
            _presenter = new EditorPresenter();
            _renderer = new SkiaSharpRenderer(true, 96.0);

            _element = element;
            _element.PaintSurface += PaintSurface;
        }

        public SKElementHelper(SKElement element, IServiceProvider serviceProvider)
        {
            _fileSystem = serviceProvider.GetService<IFileSystem>();
            _jsonSerializer = serviceProvider.GetService<IJsonSerializer>();
            _presenter = serviceProvider.GetService<ContainerPresenter>();
            _renderer = serviceProvider.GetService<ShapeRenderer>();

            _element = element;
            _element.PaintSurface += PaintSurface;
        }

        private void PaintSurface(SKCanvas canvas, int width, int height)
        {
            var container = Project?.CurrentContainer;
            if (container != null)
            {
                var matrix = PresentationSource.FromVisual(_element).CompositionTarget.TransformToDevice;
                double offsetX = (_element.ActualWidth * matrix.M11 - container.Width) / 2.0;
                double offsetY = (_element.ActualHeight * matrix.M22 - container.Height) / 2.0;
                canvas.Clear(SKColors.White);
                _presenter.Render(canvas, _renderer, container, offsetX, offsetY);
            }
        }

        private void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }

        public void RefreshRequested(object sender, EventArgs e)
        {
            _element.InvalidateVisual();
        }

        public void UpdateCache(XProject project)
        {
            _renderer.ClearCache(isZooming: false);
            _renderer.State.ImageCache = project;
        }

        public void OpenProject(string path)
        {
            var project = XProject.Open(path, _fileSystem, _jsonSerializer);
            if (project != null)
            {
                Project = project;
                UpdateCache(project);
            }
        }

        public void CloseProject()
        {
            Project = null;
            UpdateCache(null);
        }
    }
}
