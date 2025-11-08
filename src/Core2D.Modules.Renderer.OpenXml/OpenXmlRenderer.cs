// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.IO;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.Modules.Renderer.SkiaSharp;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Renderer.Presenters;
using SkiaSharp;

namespace Core2D.Modules.Renderer.OpenXml;

public sealed class OpenXmlRenderedPage
{
    public OpenXmlRenderedPage(PageContainerViewModel page, byte[] image, int width, int height)
    {
        Page = page;
        Image = image;
        Width = width;
        Height = height;
    }

    public PageContainerViewModel Page { get; }

    public byte[] Image { get; }

    public int Width { get; }

    public int Height { get; }
}

public sealed class OpenXmlRenderer
{
    private readonly IServiceProvider? _serviceProvider;

    public OpenXmlRenderer(IServiceProvider? serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public OpenXmlRenderedPage? Render(PageContainerViewModel page, ProjectContainerViewModel project)
    {
        if (page.Template is null)
        {
            return null;
        }

        var width = (int)Math.Max(1, Math.Round(page.Template.Width));
        var height = (int)Math.Max(1, Math.Round(page.Template.Height));

        var renderer = new SkiaSharpRendererViewModel(_serviceProvider);
        if (renderer.State is { })
        {
            renderer.State.DrawShapeState = ShapeStateFlags.Printable;
            if (project is IImageCache cache)
            {
                renderer.State.ImageCache = cache;
            }
        }

        var presenter = new ExportPresenter();

        var dataFlow = _serviceProvider.GetService<DataFlow>();
        if (dataFlow is { })
        {
            var db = (object)page.Properties;
            var record = (object?)page.Record;
            dataFlow.Bind(page.Template, db, record);
            dataFlow.Bind(page, db, record);
        }

        var info = new SKImageInfo(width, height, SKImageInfo.PlatformColorType, SKAlphaType.Premul);
        using var surface = SKSurface.Create(info);
        surface.Canvas.Clear(SKColors.Transparent);
        presenter.Render(surface.Canvas, renderer, null, page, 0, 0);
        surface.Canvas.Flush();

        using var snapshot = surface.Snapshot();
        using var data = snapshot.Encode(SKEncodedImageFormat.Png, 100);
        using var ms = new MemoryStream();
        data.SaveTo(ms);

        return new OpenXmlRenderedPage(page, ms.ToArray(), width, height);
    }
}
