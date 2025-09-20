// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.IO;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using SkiaSharp;

namespace Core2D.Modules.FileWriter.SkiaSharp;

public sealed class PngSkiaSharpExporter : IProjectExporter
{
    private readonly IShapeRenderer _renderer;
    private readonly IContainerPresenter _presenter;

    public PngSkiaSharpExporter(IShapeRenderer renderer, IContainerPresenter presenter)
    {
        _renderer = renderer;
        _presenter = presenter;
    }

    public void Save(Stream stream, PageContainerViewModel container)
    {
        if (container.Template is null)
        {
            return;
        }
        var width = (int)container.Template.Width;
        var height = (int)container.Template.Height;
        var info = new SKImageInfo(width, height, SKImageInfo.PlatformColorType, SKAlphaType.Unpremul);
        using var bitmap = new SKBitmap(info);
        using (var canvas = new SKCanvas(bitmap))
        {
            canvas.Clear();
            _presenter.Render(canvas, _renderer, null, container, 0, 0);
        }
        using var image = SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SKEncodedImageFormat.Png, 100);
        data.SaveTo(stream);
    }

    public void Save(Stream stream, DocumentContainerViewModel document)
    {
        throw new NotSupportedException("Saving documents as png drawing is not supported.");
    }

    public void Save(Stream stream, ProjectContainerViewModel project)
    {
        throw new NotSupportedException("Saving projects as png drawing is not supported.");
    }
}
