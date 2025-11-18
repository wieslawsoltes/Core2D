// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using Core2D.Model;
using Core2D.Model.OpenXml;
using Core2D.Modules.Renderer.OpenXml;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;

namespace Core2D.Modules.FileWriter.OpenXml;

public sealed class VisioOpenXmlWriter : IFileWriter
{
    private const double PixelsPerInch = VisioExportUnits.PixelsPerInch;
    private static readonly XNamespace VisioNs = "http://schemas.microsoft.com/office/visio/2012/main";

    private readonly IServiceProvider? _serviceProvider;

    public VisioOpenXmlWriter(IServiceProvider? serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public string Name => "Visio (OpenXML)";

    public string Extension => "vsdx";

    public void Save(Stream stream, object? item, object? options)
    {
        if (item is null || options is not ProjectContainerViewModel project)
        {
            return;
        }

        var jsonSerializer = _serviceProvider.GetService<IJsonSerializer>();
        if (jsonSerializer is null)
        {
            return;
        }

        var pages = OpenXmlExportUtilities.ExtractPages(item);
        if (pages.Count == 0)
        {
            return;
        }

        var renderer = new OpenXmlRenderer(_serviceProvider);
        var drawingRenderer = new OpenXmlDrawingRenderer();
        var renderedPages = OpenXmlExportUtilities.RenderPages(renderer, project, pages).ToList();
        if (renderedPages.Count == 0)
        {
            return;
        }

        var visioPages = new List<VisioPageDefinition>();
        var masterDefinitions = new List<VisioMasterDefinition>();
        var masterMap = new Dictionary<BlockShapeViewModel, VisioMasterDefinition>(ReferenceEqualityComparer<BlockShapeViewModel>.Instance);
        uint nextMasterId = 1;
        foreach (var (rendered, index) in renderedPages.Select((rp, i) => (rp, i)))
        {
            var widthInches = Math.Max(VisioExportUnits.ToInches(rendered.Width), 0.01);
            var heightInches = Math.Max(VisioExportUnits.ToInches(rendered.Height), 0.01);
            var shapes = BuildPageShapes(rendered.Page, widthInches, heightInches, drawingRenderer, masterMap, masterDefinitions, ref nextMasterId);
            var name = VisioExportUtilities.CreatePageName(rendered.Page, index);
            visioPages.Add(new VisioPageDefinition(name, widthInches, heightInches, shapes));
        }

        if (visioPages.Count == 0)
        {
            return;
        }

        if (stream.CanSeek)
        {
            stream.SetLength(0);
            stream.Position = 0;
        }

        var payload = OpenXmlPayloadSerializer.Create(jsonSerializer, item);
        var builder = new VisioPackageBuilder(visioPages, masterDefinitions, payload, project.Name ?? "Core2D Export");
        builder.Save(stream);
    }

    private IReadOnlyList<XElement> BuildPageShapes(PageContainerViewModel page, double widthInches, double heightInches, OpenXmlDrawingRenderer drawingRenderer, IDictionary<BlockShapeViewModel, VisioMasterDefinition> masterMap, IList<VisioMasterDefinition> masterDefinitions, ref uint nextMasterId)
    {
        var descriptors = drawingRenderer.Render(page).ToList();
        var converter = new VisioShapeConverter(heightInches);
        var shapes = converter.Convert(descriptors).ToList();
        var nextShapeId = GetNextShapeId(shapes);

        foreach (var layer in page.Layers)
        {
            foreach (var shape in layer.Shapes)
            {
                if (shape is not InsertShapeViewModel insert || insert.Block is null)
                {
                    continue;
                }

                var master = GetOrCreateMaster(insert.Block, drawingRenderer, masterMap, masterDefinitions, ref nextMasterId);
                if (master is null)
                {
                    continue;
                }

                var masterShape = CreateMasterInstanceElement(insert, master, heightInches, ref nextShapeId);
                if (masterShape is { })
                {
                    shapes.Add(masterShape);
                }
            }
        }

        return shapes;
    }

    private VisioMasterDefinition? GetOrCreateMaster(BlockShapeViewModel block, OpenXmlDrawingRenderer drawingRenderer, IDictionary<BlockShapeViewModel, VisioMasterDefinition> masterMap, IList<VisioMasterDefinition> masterDefinitions, ref uint nextMasterId)
    {
        if (masterMap.TryGetValue(block, out var definition))
        {
            return definition;
        }

        var descriptors = drawingRenderer.Render(block).ToList();
        if (descriptors.Count == 0)
        {
            return null;
        }

        var bounds = ComputeBounds(descriptors);
        var normalized = descriptors.Select(d => NormalizeDescriptor(d, bounds.MinX, bounds.MinY)).ToList();
        var heightInches = Math.Max(bounds.Height / PixelsPerInch, 0.01);
        var converter = new VisioShapeConverter(heightInches);
        var shapes = converter.Convert(normalized).ToList();
        if (shapes.Count == 0)
        {
            return null;
        }

        var widthInches = Math.Max(bounds.Width / PixelsPerInch, 0.01);
        var masterId = nextMasterId++;
        var name = VisioExportUtilities.SanitizeName(block.Name, $"Block {masterId}");
        var uniqueId = Guid.NewGuid().ToString("B").ToUpperInvariant();
        var master = new VisioMasterDefinition(masterId, name, widthInches, heightInches, bounds.Width, bounds.Height, shapes, uniqueId);
        masterDefinitions.Add(master);
        masterMap[block] = master;
        return master;
    }

    private static int GetNextShapeId(IEnumerable<XElement> shapes)
    {
        var maxId = 0;
        foreach (var shape in shapes)
        {
            if (int.TryParse(shape.Attribute("ID")?.Value, out var id))
            {
                maxId = Math.Max(maxId, id);
            }
        }
        return maxId;
    }

    private static XElement? CreateMasterInstanceElement(InsertShapeViewModel insert, VisioMasterDefinition master, double pageHeightInches, ref int nextShapeId)
    {
        var dx = insert.Point?.X ?? 0.0;
        var dy = insert.Point?.Y ?? 0.0;
        var widthPixels = master.WidthPixels;
        var heightPixels = master.HeightPixels;
        if (widthPixels <= 0.0 || heightPixels <= 0.0)
        {
            return null;
        }

        var centerX = dx + (widthPixels / 2.0);
        var centerY = dy + (heightPixels / 2.0);
        var pinX = VisioExportUnits.ToInches(centerX);
        var pinY = pageHeightInches - (centerY / PixelsPerInch);
        var width = master.WidthInches;
        var height = master.HeightInches;

        var shape = new XElement(VisioNs + "Shape",
            new XAttribute("ID", ++nextShapeId),
            new XAttribute("Type", "Shape"),
            new XAttribute("Master", master.Id),
            new XAttribute("LineStyle", "0"),
            new XAttribute("FillStyle", "0"),
            new XAttribute("TextStyle", "0"),
            new XAttribute("Name", VisioExportUtilities.SanitizeName(insert.Name, $"Insert {nextShapeId}")));

        AddCell(shape, "PinX", pinX);
        AddCell(shape, "PinY", pinY);
        AddCell(shape, "Width", width);
        AddCell(shape, "Height", height);
        AddCell(shape, "Angle", 0.0);
        AddCell(shape, "LocPinX", width / 2.0);
        AddCell(shape, "LocPinY", height / 2.0);
        return shape;
    }

    private static void AddCell(XElement shape, string name, double value)
    {
        shape.Add(new XElement(VisioNs + "Cell",
            new XAttribute("N", name),
            new XAttribute("V", value.ToString("G17"))));
    }

    private static OpenXmlShapeDescriptor NormalizeDescriptor(OpenXmlShapeDescriptor descriptor, double offsetX, double offsetY)
    {
        return descriptor.Kind switch
        {
            OpenXmlShapeKind.Line => descriptor with
            {
                Left = descriptor.Left - offsetX,
                Top = descriptor.Top - offsetY,
                LineX1 = descriptor.LineX1 - offsetX,
                LineY1 = descriptor.LineY1 - offsetY,
                LineX2 = descriptor.LineX2 - offsetX,
                LineY2 = descriptor.LineY2 - offsetY
            },
            _ => descriptor with
            {
                Left = descriptor.Left - offsetX,
                Top = descriptor.Top - offsetY
            }
        };
    }

    private static DescriptorBounds ComputeBounds(IEnumerable<OpenXmlShapeDescriptor> descriptors)
    {
        var minX = double.MaxValue;
        var minY = double.MaxValue;
        var maxX = double.MinValue;
        var maxY = double.MinValue;

        foreach (var descriptor in descriptors)
        {
            var left = descriptor.Left;
            var top = descriptor.Top;
            var right = descriptor.Left + Math.Max(descriptor.Width, 0.0);
            var bottom = descriptor.Top + Math.Max(descriptor.Height, 0.0);

            if (descriptor.Kind == OpenXmlShapeKind.Line)
            { 
                left = Math.Min(left, Math.Min(descriptor.LineX1, descriptor.LineX2));
                right = Math.Max(right, Math.Max(descriptor.LineX1, descriptor.LineX2));
                top = Math.Min(top, Math.Min(descriptor.LineY1, descriptor.LineY2));
                bottom = Math.Max(bottom, Math.Max(descriptor.LineY1, descriptor.LineY2));
            }

            minX = Math.Min(minX, left);
            minY = Math.Min(minY, top);
            maxX = Math.Max(maxX, right);
            maxY = Math.Max(maxY, bottom);
        }

        if (double.IsInfinity(minX) || double.IsInfinity(minY) || double.IsInfinity(maxX) || double.IsInfinity(maxY))
        {
            minX = minY = 0.0;
            maxX = maxY = 1.0;
        }

        return new DescriptorBounds(minX, minY, maxX, maxY);
    }

    private sealed record DescriptorBounds(double MinX, double MinY, double MaxX, double MaxY)
    {
        public double Width => Math.Max(MaxX - MinX, 1.0);
        public double Height => Math.Max(MaxY - MinY, 1.0);
    }

    private sealed class ReferenceEqualityComparer<T> : IEqualityComparer<T>
        where T : class
    {
        public static ReferenceEqualityComparer<T> Instance { get; } = new();

        public bool Equals(T? x, T? y) => ReferenceEquals(x, y);

        public int GetHashCode(T obj) => RuntimeHelpers.GetHashCode(obj);
    }
}
