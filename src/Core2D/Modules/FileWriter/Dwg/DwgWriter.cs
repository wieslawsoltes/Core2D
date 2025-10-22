// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.IO;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.Modules.Renderer.Dwg;
using Core2D.ViewModels.Containers;

namespace Core2D.Modules.FileWriter.Dwg;

public sealed class DwgWriter : IFileWriter
{
    private readonly IServiceProvider? _serviceProvider;

    public DwgWriter(IServiceProvider? serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public string Name { get; } = "Dwg (ACadSharp)";

    public string Extension { get; } = "dwg";

    public void Save(Stream stream, object? item, object? options)
    {
        if (item is null)
        {
            return;
        }

        var ic = options as IImageCache;
        if (options is null)
        {
            return;
        }

        IProjectExporter exporter = new DwgRenderer(_serviceProvider);

        var renderer = (IShapeRenderer)exporter;
        if (renderer.State is { })
        {
            renderer.State.DrawShapeState = ShapeStateFlags.Printable;
            renderer.State.ImageCache = ic;
        }

        if (item is PageContainerViewModel page)
        {
            exporter.Save(stream, page);
        }
        else if (item is DocumentContainerViewModel document)
        {
            exporter.Save(stream, document);
        }
        else if (item is ProjectContainerViewModel project)
        {
            exporter.Save(stream, project);
        }
    }
}

