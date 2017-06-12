// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows;
using Core2D.FileSystem.DotNet;
using Core2D.Project;
using Core2D.Renderer.Presenters;
using Core2D.Renderer.SkiaSharp;
using Core2D.Serializer.Newtonsoft;
using Core2D.SkiaView;
using SkiaSharp;
using SkiaSharp.Views.Desktop;
using SkiaSharp.Views.WPF;

namespace Core2D.SkiaViewNoAutofac
{
    public class NoAutofacSkiaViewHelper : SkiaViewHelper
    {
        private readonly SKElement _element;

        public NoAutofacSkiaViewHelper()
        {
            FileSystem = new DotNetFileSystem();
            JsonSerializer = new NewtonsoftJsonSerializer();
            Presenter = new EditorPresenter();
            Renderer = new SkiaSharpRenderer(true, 96.0);
        }

        public NoAutofacSkiaViewHelper(SKElement element)
            : this()
        {
            _element = element;
            _element.PaintSurface += PaintSurface;
        }

        public override void GetOffset(XContainer container, out double offsetX, out double offsetY)
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
