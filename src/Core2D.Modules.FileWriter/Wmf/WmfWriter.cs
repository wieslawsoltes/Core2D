// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.IO;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.Modules.Renderer.Wmf;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;

namespace Core2D.Modules.FileWriter.Wmf;

public sealed class WmfWriter : IFileWriter
{
    private readonly IServiceProvider? _serviceProvider;

    public WmfWriter(IServiceProvider? serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public string Name => "Wmf (Oxage)";

    public string Extension => "wmf";

    public void Save(Stream stream, object? item, object? options)
    {
        if (item is null)
        {
            return;
        }

        if (item is PageContainerViewModel page)
        {
            var dataFlow = _serviceProvider.GetService<DataFlow>();
            var db = (object)page.Properties;
            var record = (object?)page.Record;

            if (dataFlow is { })
            {
                dataFlow.Bind(page.Template, db, record);
                dataFlow.Bind(page, db, record);
            }

            var exporter = new WmfRenderer(_serviceProvider);
            if (exporter.State is { })
            {
                exporter.State.DrawShapeState = ShapeStateFlags.Printable;
            }

            exporter.Save(stream, page);
        }
        else if (item is DocumentContainerViewModel)
        {
            throw new NotSupportedException("Saving documents as wmf drawing is not supported.");
        }
        else if (item is ProjectContainerViewModel)
        {
            throw new NotSupportedException("Saving projects as wmf drawing is not supported.");
        }
    }
}
