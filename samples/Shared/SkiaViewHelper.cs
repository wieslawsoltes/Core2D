// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Style;
using SkiaSharp;

namespace Core2D.SkiaView
{
    public abstract class SkiaViewHelper
    {
        public IFileSystem FileSystem { get; set; }
        public IJsonSerializer JsonSerializer { get; set; }
        public ContainerPresenter Presenter { get; set; }
        public ShapeRenderer Renderer { get; set; }
        public XProject Project { get; set; }

        public abstract void GetOffset(XContainer container, out double offsetX, out double offsetY);

        public abstract void RefreshRequested(object sender, EventArgs e);

        public SKColor ToSKColor(ArgbColor color) => new SKColor(color.R, color.G, color.B, color.A);

        public void PaintSurface(XContainer container, SKCanvas canvas, int width, int height)
        {
            GetOffset(container, out double offsetX, out double offsetY);
            canvas.Clear(ToSKColor(container.Background));
            Presenter.Render(canvas, Renderer, container, offsetX, offsetY);
        }

        public void UpdateCache(XProject project)
        {
            Renderer.ClearCache(isZooming: false);
            Renderer.State.ImageCache = project;
        }

        public void OpenProject(string path)
        {
            var project = XProject.Open(path, FileSystem, JsonSerializer);
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
