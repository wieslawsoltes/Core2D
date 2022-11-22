﻿#nullable enable
using System;
using System.IO;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.Modules.Renderer.PdfSharp;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;

namespace Core2D.Modules.FileWriter.PdfSharp;

public sealed class PdfSharpWriter : IFileWriter
{
    private readonly IServiceProvider? _serviceProvider;

    public PdfSharpWriter(IServiceProvider? serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public string Name => "Pdf (PdfSharp)";

    public string Extension => "pdf";

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

        IProjectExporter exporter = new PdfSharpRenderer(_serviceProvider);

        var renderer = (IShapeRenderer)exporter;
        if (renderer.State is { })
        {
            renderer.State.DrawShapeState = ShapeStateFlags.Printable;
            renderer.State.ImageCache = ic;
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

            exporter.Save(stream, page);
        }
        else if (item is DocumentContainerViewModel document)
        {
            var dataFlow = _serviceProvider.GetService<DataFlow>();
            if (dataFlow is { })
            {
                dataFlow.Bind(document);
            }
            exporter.Save(stream, document);
        }
        else if (item is ProjectContainerViewModel project)
        {
            var dataFlow = _serviceProvider.GetService<DataFlow>();
            if (dataFlow is { })
            {
                dataFlow.Bind(project);
            }
            exporter.Save(stream, project);
        }
    }
}
