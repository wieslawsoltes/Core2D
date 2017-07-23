// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Interfaces;
using Core2D.Containers;
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
        public ProjectContainer Project { get; set; }

        public abstract void GetOffset(PageContainer container, out double offsetX, out double offsetY);

        public abstract void RefreshRequested(object sender, EventArgs e);

        public SKColor ToSKColor(ArgbColor color) => new SKColor(color.R, color.G, color.B, color.A);

        public void PaintSurface(PageContainer container, SKCanvas canvas, int width, int height)
        {
            GetOffset(container, out double offsetX, out double offsetY);
            canvas.Clear(ToSKColor(container.Background));
            Presenter.Render(canvas, Renderer, container, offsetX, offsetY);
        }

        public void UpdateCache(ProjectContainer project)
        {
            Renderer.ClearCache(isZooming: false);
            Renderer.State.ImageCache = project;
        }

        public void OpenProject(string path)
        {
            var project = ProjectContainer.Open(path, FileSystem, JsonSerializer);
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
