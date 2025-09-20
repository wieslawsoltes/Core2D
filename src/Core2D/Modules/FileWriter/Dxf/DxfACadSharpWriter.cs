// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.IO;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.Modules.Renderer.Dwg;
using Core2D.ViewModels.Containers;

namespace Core2D.Modules.FileWriter.Dxf;

public sealed class DxfACadSharpWriter : IFileWriter
{
    private readonly IServiceProvider? _serviceProvider;

    public DxfACadSharpWriter(IServiceProvider? serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public string Name { get; } = "Dxf (ACadSharp)";

    // Note: using a distinct extension to disambiguate writer selection in the picker.
    // The produced file is a valid DXF and can be renamed to .dxf if desired.
    public string Extension { get; } = "dxf-acad";

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

        var exporter = new DwgRenderer(_serviceProvider);

        var renderer = (IShapeRenderer)exporter;
        if (renderer.State is { })
        {
            renderer.State.DrawShapeState = ShapeStateFlags.Printable;
            renderer.State.ImageCache = ic;
        }

        if (item is PageContainerViewModel page)
        {
            exporter.SaveDxf(stream, page);
        }
        else if (item is DocumentContainerViewModel document)
        {
            exporter.SaveDxf(stream, document);
        }
        else if (item is ProjectContainerViewModel project)
        {
            exporter.SaveDxf(stream, project);
        }
    }
}

