// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows;
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.SkiaView;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

namespace Core2D.SkiaViewAutofac
{
    public class AutofacSkiaViewHelper : SkiaViewHelper
    {
        private readonly SKElement _element;

        public AutofacSkiaViewHelper(IServiceProvider serviceProvider)
        {
            FileSystem = serviceProvider.GetService<IFileSystem>();
            JsonSerializer = serviceProvider.GetService<IJsonSerializer>();
            Presenter = serviceProvider.GetService<ContainerPresenter>();
            Renderer = serviceProvider.GetService<ShapeRenderer>();
        }

        public AutofacSkiaViewHelper(SKElement element, IServiceProvider serviceProvider)
            : this(serviceProvider)
        {
            _element = element;
            _element.PaintSurface += PaintSurface;
        }

        public override void GetOffset(PageContainer container, out double offsetX, out double offsetY)
        {
            var visual = _element;
            var matrix = PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice;
            offsetX = (visual.ActualWidth * matrix.M11 - container.Width) / 2.0;
            offsetY = (visual.ActualHeight * matrix.M22 - container.Height) / 2.0;
        }

        public override void RefreshRequested(object sender, EventArgs e)
        {
            _element.InvalidateVisual();
        }

        public void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            var container = Project?.CurrentContainer;
            if (container != null)
            {
                PaintSurface(container, e.Surface.Canvas, e.Info.Width, e.Info.Height);
            }
            else
            {
                e.Surface.Canvas.Clear(SKColors.Transparent);
            }
        }
    }
}
