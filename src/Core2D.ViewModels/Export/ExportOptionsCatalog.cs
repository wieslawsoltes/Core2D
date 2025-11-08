// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Core2D.ViewModels.Export.Options;

namespace Core2D.ViewModels.Export;

public sealed class ExportOptionsCatalog : IExportOptionsCatalog
{
    private readonly IServiceProvider? _serviceProvider;
    private readonly IReadOnlyDictionary<string, ExportOptionsDescriptor> _descriptors;
    private readonly IReadOnlyCollection<ExportOptionsDescriptor> _descriptorList;

    public ExportOptionsCatalog(IServiceProvider? serviceProvider)
    {
        _serviceProvider = serviceProvider;
        _descriptors = CreateDescriptorMap();
        _descriptorList = new ReadOnlyCollection<ExportOptionsDescriptor>(_descriptors.Values.ToList());
    }

    public bool TryCreate(string writerName, out ExportOptionsBase options)
    {
        if (!string.IsNullOrWhiteSpace(writerName)
            && _descriptors.TryGetValue(writerName, out var descriptor))
        {
            options = descriptor.Factory(_serviceProvider);
            options.DisplayName = descriptor.DisplayName;
            options.Description = descriptor.Description;
            return true;
        }

        options = CreateFallbackOptions(writerName);
        return false;
    }

    public ExportOptionsBase Create(string writerName)
    {
        TryCreate(writerName, out var options);
        return options;
    }

    public IReadOnlyCollection<ExportOptionsDescriptor> Describe()
        => _descriptorList;

    private IReadOnlyDictionary<string, ExportOptionsDescriptor> CreateDescriptorMap()
    {
        var comparer = StringComparer.OrdinalIgnoreCase;
        static IReadOnlyList<string> Caps(params string[] values)
            => Array.AsReadOnly(values ?? Array.Empty<string>());

        var descriptors = new Dictionary<string, ExportOptionsDescriptor>(comparer)
        {
            ["Pdf (PdfSharp)"] = new("Pdf (PdfSharp)", "Pdf (PdfSharp)", "Vector PDF export powered by PdfSharp.", "PDF",
                Caps("Vector", "Project", "Document", "Page"), "pdf", sp => new PdfSharpExportOptionsViewModel(sp)),
            ["Pdf (SkiaSharp)"] = new("Pdf (SkiaSharp)", "Pdf (SkiaSharp)", "SkiaSharp-backed PDF export.", "PDF",
                Caps("Vector", "Project", "Document", "Page"), "pdf", sp => new SkiaPdfExportOptionsViewModel(sp)),
            ["Png (SkiaSharp)"] = new("Png (SkiaSharp)", "PNG Image", "Raster export using SkiaSharp.", "Images",
                Caps("Raster", "Page"), "png", sp => new RasterExportOptionsViewModel(sp)),
            ["Jpeg (SkiaSharp)"] = new("Jpeg (SkiaSharp)", "JPEG Image", "JPEG export using SkiaSharp.", "Images",
                Caps("Raster", "Page"), "jpg", sp => new RasterExportOptionsViewModel(sp)),
            ["Webp (SkiaSharp)"] = new("Webp (SkiaSharp)", "WebP Image", "WebP export using SkiaSharp.", "Images",
                Caps("Raster", "Page"), "webp", sp => new RasterExportOptionsViewModel(sp)),
            ["Svg (Svg)"] = new("Svg (Svg)", "SVG Export", "Drawing-based SVG export.", "Vector",
                Caps("Vector", "Page"), "svg", sp => new SvgExportOptionsViewModel(sp)),
            ["Svg (SkiaSharp)"] = new("Svg (SkiaSharp)", "SVG Export (Skia)", "SkiaSharp SVG export.", "Vector",
                Caps("Vector", "Page"), "svg", sp => new SvgExportOptionsViewModel(sp)),
            ["Word (OpenXML)"] = new("Word (OpenXML)", "Word Document", "DOCX export leveraging DrawingML.", "OpenXML",
                Caps("Vector/Raster", "Project", "Document", "Page"), "docx", sp => new OpenXmlExportOptionsViewModel(sp)),
            ["Excel (OpenXML)"] = new("Excel (OpenXML)", "Excel Workbook", "XLSX export with per-sheet shapes.", "OpenXML",
                Caps("Vector/Raster", "Project", "Document", "Page"), "xlsx", sp => new OpenXmlExportOptionsViewModel(sp)),
            ["PowerPoint (OpenXML)"] = new("PowerPoint (OpenXML)", "PowerPoint Deck", "PPTX export with vector shapes.", "OpenXML",
                Caps("Vector/Raster", "Project", "Document", "Page"), "pptx", sp => new OpenXmlExportOptionsViewModel(sp)),
            ["Dwg (ACadSharp)"] = new("Dwg (ACadSharp)", "DWG Export", "AutoCAD-compatible DWG export.", "CAD",
                Caps("Vector", "Project", "Document", "Page"), "dwg", sp => new CadExportOptionsViewModel(sp)),
            ["Dxf (netDxf)"] = new("Dxf (netDxf)", "DXF Export", "DXF export via netDxf.", "CAD",
                Caps("Vector", "Project", "Document", "Page"), "dxf", sp => new CadExportOptionsViewModel(sp)),
            ["Dxf (ACadSharp)"] = new("Dxf (ACadSharp)", "DXF Export (ACadSharp)", "DXF export via ACadSharp.", "CAD",
                Caps("Vector", "Project", "Document", "Page"), "dxf", sp => new CadExportOptionsViewModel(sp)),
        };

        return descriptors;
    }

    private ExportOptionsBase CreateFallbackOptions(string? writerName)
    {
        var displayName = string.IsNullOrWhiteSpace(writerName) ? "Exporter" : writerName;
        var options = new DefaultExportOptionsViewModel(_serviceProvider)
        {
            DisplayName = displayName,
            Description = "Default options placeholder."
        };
        return options;
    }
}
