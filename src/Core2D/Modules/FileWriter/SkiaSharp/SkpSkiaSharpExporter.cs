// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.IO;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using SkiaSharp;

namespace Core2D.Modules.FileWriter.SkiaSharp;

public sealed class SkpSkiaSharpExporter : IProjectExporter
{
    private readonly IShapeRenderer _renderer;
    private readonly IContainerPresenter _presenter;

    public SkpSkiaSharpExporter(IShapeRenderer renderer, IContainerPresenter presenter)
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
        using var pictureRecorder = new SKPictureRecorder();
        using var canvas = pictureRecorder.BeginRecording(SKRect.Create(0, 0, width, height));
        _presenter.Render(canvas, _renderer, null, container, 0, 0);
        using var picture = pictureRecorder.EndRecording();
        picture.Serialize(stream);
    }

    public void Save(Stream stream, DocumentContainerViewModel document)
    {
        throw new NotSupportedException("Saving documents as skp drawing is not supported.");
    }

    public void Save(Stream stream, ProjectContainerViewModel project)
    {
        throw new NotSupportedException("Saving projects as skp drawing is not supported.");
    }
}
