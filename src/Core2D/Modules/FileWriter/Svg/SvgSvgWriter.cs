// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.IO;
using System.Linq;
using System.Text;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.Modules.SvgExporter.Svg;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;

namespace Core2D.Modules.FileWriter.Svg;

public sealed class SvgSvgWriter : IFileWriter
{
    private readonly IServiceProvider? _serviceProvider;

    public SvgSvgWriter(IServiceProvider? serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public string Name => "Svg (Svg)";

    public string Extension => "svg";

    public void Save(Stream stream, object? item, object? options)
    {
        if (item is null)
        {
            return;
        }

        if (options is not IImageCache _)
        {
            return;
        }

        var dataFlow = _serviceProvider.GetService<DataFlow>();
        if (dataFlow is null)
        {
            return;
        }

        var exporter = new SvgSvgExporter(_serviceProvider);

        if (item is PageContainerViewModel page && page.Template is { })
        {
            var db = (object)page.Properties;
            var record = (object?)page.Record;

            dataFlow.Bind(page.Template, db, record);
            dataFlow.Bind(page, db, record);

            var shapes = page.Layers.SelectMany(x => x.Shapes);
            if (shapes.Any())
            {
                var xaml = exporter.Create(shapes, page.Template.Width, page.Template.Height);
                if (!string.IsNullOrEmpty(xaml))
                {
                    var bytes = Encoding.UTF8.GetBytes(xaml);
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
        }
        else if (item is DocumentContainerViewModel _)
        {
            throw new NotSupportedException("Saving documents as svg drawing is not supported.");
        }
        else if (item is ProjectContainerViewModel _)
        {
            throw new NotSupportedException("Saving projects as svg drawing is not supported.");
        }
    }
}
