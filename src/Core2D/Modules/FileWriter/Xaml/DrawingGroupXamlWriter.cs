﻿#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.Modules.XamlExporter.Avalonia;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Shapes;

namespace Core2D.Modules.FileWriter.Xaml;

public sealed class DrawingGroupXamlWriter : IFileWriter
{
    private readonly IServiceProvider? _serviceProvider;

    public DrawingGroupXamlWriter(IServiceProvider? serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public string Name { get; } = "Xaml (DrawingGroup)";

    public string Extension { get; } = "xaml";

    public void Save(Stream stream, object? item, object? options)
    {
        if (item is null)
        {
            return;
        }

        var _ = options as IImageCache;
        if (options is null)
        {
            return;
        }

        var exporter = new DrawingGroupXamlExporter(_serviceProvider);

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

            var shapes = new List<BaseShapeViewModel>();
            if (page.Template is { } template)
            {
                shapes.AddRange(template.Layers.SelectMany(x => x.Shapes));
            }
            shapes.AddRange(page.Layers.SelectMany(x => x.Shapes));

            {
                var key = page.Name;
                var xaml = exporter.Create(shapes, key);
                if (!string.IsNullOrEmpty(xaml))
                {
                    byte[] bytes = Encoding.UTF8.GetBytes(xaml);
                    stream.Write(bytes, 0, bytes.Length);
                }
            }
        }
        else if (item is DocumentContainerViewModel _)
        {
            throw new NotSupportedException("Saving documents as xaml drawing is not supported.");
        }
        else if (item is ProjectContainerViewModel _)
        {
            throw new NotSupportedException("Saving projects as xaml drawing is not supported.");
        }
    }
}
