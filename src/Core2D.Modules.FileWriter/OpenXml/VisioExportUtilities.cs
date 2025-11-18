// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml.Linq;
using Core2D.Modules.Renderer.OpenXml;
using Core2D.ViewModels.Containers;

namespace Core2D.Modules.FileWriter.OpenXml;

internal static class VisioExportUnits
{
    public const double PixelsPerInch = 96.0;

    public static double ToInches(double value)
        => Math.Max(value / PixelsPerInch, 0.0);
}

internal sealed record class VisioPageDefinition(string Name, double WidthInches, double HeightInches, IReadOnlyList<XElement> Shapes);

internal sealed record class VisioMasterDefinition(
    uint Id,
    string Name,
    double WidthInches,
    double HeightInches,
    double WidthPixels,
    double HeightPixels,
    IReadOnlyList<XElement> Shapes,
    string UniqueId);

internal static class VisioExportUtilities
{ 
    private static readonly char[] InvalidNameChars = { '/', '\\', '?', '*', '[', ']', ':' };

    public static string SanitizeName(string? name, string fallback)
    {
        var result = string.IsNullOrWhiteSpace(name)
            ? fallback
            : name.Trim();

        foreach (var invalid in InvalidNameChars)
        {
            result = result.Replace(invalid, '-');
        }

        return string.IsNullOrWhiteSpace(result) ? fallback : result;
    }

    public static string CreatePageName(PageContainerViewModel page, int index)
    {
        return SanitizeName(page.Name, $"Page {index + 1}");
    }

    public static double GetPageWidth(PageContainerViewModel page)
    {
        var width = page.Template?.Width ?? 0.0;
        return Math.Max(width, 1.0);
    }

    public static double GetPageHeight(PageContainerViewModel page)
    {
        var height = page.Template?.Height ?? 0.0;
        return Math.Max(height, 1.0);
    }
}

internal sealed class VisioShapeConverter
{
    private static readonly XNamespace VisioNs = "http://schemas.microsoft.com/office/visio/2012/main";
    private readonly double _pageHeightInches;
    private readonly CultureInfo _culture = CultureInfo.InvariantCulture;
    private int _shapeId;

    public VisioShapeConverter(double pageHeightInches)
    {
        _pageHeightInches = pageHeightInches;
    }

    public IReadOnlyList<XElement> Convert(IEnumerable<OpenXmlShapeDescriptor> descriptors)
    {
        var result = new List<XElement>();
        foreach (var descriptor in descriptors)
        {
            var shape = descriptor.Kind switch
            {
                OpenXmlShapeKind.Rectangle => CreateRectangle(descriptor),
                OpenXmlShapeKind.Text => CreateText(descriptor),
                OpenXmlShapeKind.Ellipse => CreateEllipse(descriptor),
                OpenXmlShapeKind.Line => CreateLine(descriptor),
                _ => null
            };

            if (shape is { })
            {
                result.Add(shape);
            }
        }

        return result;
    }

    private XElement? CreateRectangle(OpenXmlShapeDescriptor descriptor)
        => CreateBox(descriptor, includeText: false);

    private XElement? CreateText(OpenXmlShapeDescriptor descriptor)
        => CreateBox(descriptor, includeText: true);

    private XElement? CreateBox(OpenXmlShapeDescriptor descriptor, bool includeText)
    {
        var (pinX, pinY, width, height) = GetBoxMetrics(descriptor);
        if (width <= 0 || height <= 0)
        {
            return null;
        }

        var shape = CreateBaseShape(pinX, pinY, width, height);
        shape.Add(CreateCell("LocPinX", width * 0.5));
        shape.Add(CreateCell("LocPinY", height * 0.5));
        AppendStyle(descriptor, shape, isClosedShape: true);

        var section = new XElement(VisioNs + "Section",
            new XAttribute("N", "Geometry"),
            new XAttribute("IX", "0"));
        section.Add(CreateGeometryFlag("NoFill", descriptor.IsFilled));
        section.Add(CreateGeometryFlag("NoLine", descriptor.IsStroked));
        section.Add(CreateRelRow("RelMoveTo", 1, 0.0, 0.0));
        section.Add(CreateRelRow("RelLineTo", 2, 1.0, 0.0));
        section.Add(CreateRelRow("RelLineTo", 3, 1.0, 1.0));
        section.Add(CreateRelRow("RelLineTo", 4, 0.0, 1.0));
        section.Add(CreateRelRow("RelLineTo", 5, 0.0, 0.0));
        shape.Add(section);

        if (includeText && !string.IsNullOrWhiteSpace(descriptor.Text))
        {
            shape.Add(new XElement(VisioNs + "Text", descriptor.Text));
        }

        return shape;
    }

    private XElement? CreateEllipse(OpenXmlShapeDescriptor descriptor)
    {
        var (pinX, pinY, width, height) = GetBoxMetrics(descriptor);
        if (width <= 0 || height <= 0)
        {
            return null;
        }

        var shape = CreateBaseShape(pinX, pinY, width, height);
        shape.Add(CreateCell("LocPinX", width * 0.5));
        shape.Add(CreateCell("LocPinY", height * 0.5));
        AppendStyle(descriptor, shape, isClosedShape: true);

        var section = new XElement(VisioNs + "Section",
            new XAttribute("N", "Geometry"),
            new XAttribute("IX", "0"));
        section.Add(CreateGeometryFlag("NoFill", descriptor.IsFilled));
        section.Add(CreateGeometryFlag("NoLine", descriptor.IsStroked));

        var row = new XElement(VisioNs + "Row",
            new XAttribute("T", "Ellipse"),
            new XAttribute("IX", "1"));
        row.Add(CreateGeometryCell("X", width * 0.5));
        row.Add(CreateGeometryCell("Y", height * 0.5));
        row.Add(CreateGeometryCell("A", width));
        row.Add(CreateGeometryCell("B", height * 0.5));
        row.Add(CreateGeometryCell("C", width * 0.5));
        row.Add(CreateGeometryCell("D", height));
        section.Add(row);
        shape.Add(section);

        return shape;
    }

    private XElement? CreateLine(OpenXmlShapeDescriptor descriptor)
    {
        var startX = VisioExportUnits.ToInches(descriptor.LineX1);
        var endX = VisioExportUnits.ToInches(descriptor.LineX2);
        var startY = ToVisioY(descriptor.LineY1);
        var endY = ToVisioY(descriptor.LineY2);

        var dx = endX - startX;
        var dy = endY - startY;
        var length = Math.Sqrt((dx * dx) + (dy * dy));
        if (length <= 0)
        {
            return null;
        }

        var pinX = startX + dx * 0.5;
        var pinY = startY + dy * 0.5;

        var shape = CreateBaseShape(pinX, pinY, length, 0.0);
        shape.Add(CreateCell("LocPinX", length * 0.5));
        shape.Add(CreateCell("LocPinY", 0.0));
        shape.Add(CreateCell("BeginX", startX));
        shape.Add(CreateCell("BeginY", startY));
        shape.Add(CreateCell("EndX", endX));
        shape.Add(CreateCell("EndY", endY));
        shape.Add(CreateCell("Angle", Math.Atan2(dy, dx)));
        AppendStyle(descriptor, shape, isClosedShape: false);

        var section = new XElement(VisioNs + "Section",
            new XAttribute("N", "Geometry"),
            new XAttribute("IX", "0"));
        section.Add(CreateGeometryFlag("NoFill", false));
        section.Add(CreateGeometryFlag("NoLine", descriptor.IsStroked));
        section.Add(CreateRow("MoveTo", 1, CreateGeometryCell("X", 0.0), CreateGeometryCell("Y", 0.0)));
        section.Add(CreateRow("LineTo", 2, CreateGeometryCell("X", 1.0), CreateGeometryCell("Y", 0.0)));
        shape.Add(section);

        return shape;
    }

    private XElement CreateBaseShape(double pinX, double pinY, double width, double height)
    {
        var id = ++_shapeId;
        var shape = new XElement(VisioNs + "Shape",
            new XAttribute("ID", id),
            new XAttribute("Type", "Shape"),
            new XAttribute("LineStyle", "0"),
            new XAttribute("FillStyle", "0"),
            new XAttribute("TextStyle", "0"));

        shape.Add(CreateCell("PinX", pinX));
        shape.Add(CreateCell("PinY", pinY));
        shape.Add(CreateCell("Width", width));
        shape.Add(CreateCell("Height", Math.Max(height, 0.0)));
        shape.Add(CreateCell("Angle", 0.0));
        shape.Add(CreateCell("FlipX", 0));
        shape.Add(CreateCell("FlipY", 0));
        shape.Add(CreateCell("ResizeMode", 0));
        return shape;
    }

    private void AppendStyle(OpenXmlShapeDescriptor descriptor, XElement shape, bool isClosedShape)
    {
        if (!descriptor.IsFilled || descriptor.Fill is null)
        {
            shape.Add(CreateCell("FillPattern", 0));
        }
        else
        {
            shape.Add(CreateCell("FillForegnd", ToColor(descriptor.Fill.Value)));
            shape.Add(CreateCell("FillPattern", 1));
            if (descriptor.Fill.Value.A < byte.MaxValue)
            {
                var transparency = 1.0 - descriptor.Fill.Value.A / 255.0;
                shape.Add(CreateCell("FillForegndTrans", transparency));
            }
        }

        if (!descriptor.IsStroked || descriptor.Stroke is null)
        {
            shape.Add(CreateCell("LinePattern", 0));
        }
        else
        {
            shape.Add(CreateCell("LineColor", ToColor(descriptor.Stroke.Value)));
            var thickness = VisioExportUnits.ToInches(descriptor.StrokeThickness);
            shape.Add(CreateCell("LineWeight", Math.Max(thickness, 0.01)));
            shape.Add(CreateCell("LinePattern", 1));
            if (descriptor.Stroke.Value.A < byte.MaxValue)
            {
                var transparency = 1.0 - descriptor.Stroke.Value.A / 255.0;
                shape.Add(CreateCell("LineColorTrans", transparency));
            }
        }

    }

    private XElement CreateCell(string name, double value)
    {
        return new XElement(VisioNs + "Cell",
            new XAttribute("N", name),
            new XAttribute("V", Format(value)));
    }

    private XElement CreateCell(string name, string value)
    {
        return new XElement(VisioNs + "Cell",
            new XAttribute("N", name),
            new XAttribute("V", value));
    }

    private static XElement CreateGeometryCell(string name, double value)
    {
        return new XElement(VisioNs + "Cell",
            new XAttribute("N", name),
            new XAttribute("V", value.ToString("G17", CultureInfo.InvariantCulture)));
    }

    private XElement CreateGeometryFlag(string name, bool isEnabled)
    {
        var value = isEnabled ? 0 : 1;
        return new XElement(VisioNs + "Cell",
            new XAttribute("N", name),
            new XAttribute("V", value));
    }

    private XElement CreateRelRow(string type, int index, double x, double y)
    {
        return CreateRow(type, index, CreateGeometryCell("X", x), CreateGeometryCell("Y", y));
    }

    private XElement CreateRow(string type, int index, params XElement[] cells)
    {
        var row = new XElement(VisioNs + "Row",
            new XAttribute("T", type),
            new XAttribute("IX", index));
        foreach (var cell in cells)
        {
            row.Add(cell);
        }
        return row;
    }

    private (double PinX, double PinY, double Width, double Height) GetBoxMetrics(OpenXmlShapeDescriptor descriptor)
    {
        var width = VisioExportUnits.ToInches(descriptor.Width);
        var height = VisioExportUnits.ToInches(descriptor.Height);
        var left = VisioExportUnits.ToInches(descriptor.Left);
        var top = VisioExportUnits.ToInches(descriptor.Top);
        var pinX = left + width * 0.5;
        var pinY = ToVisioY(descriptor.Top + descriptor.Height * 0.5);
        return (pinX, pinY, width, height);
    }

    private double ToVisioY(double pixelValue)
    {
        var inches = VisioExportUnits.ToInches(pixelValue);
        return _pageHeightInches - inches;
    }

    private string Format(double value)
        => value.ToString("G17", _culture);
    
    private static string ToColor(OpenXmlColor color)
        => $"#{color.R:X2}{color.G:X2}{color.B:X2}";
}
