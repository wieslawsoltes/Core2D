// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System.Globalization;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;

namespace Core2D.Modules.Renderer.OpenXml;

internal static class OpenXmlShapeFactory
{
    public static ShapeProperties CreateShapeProperties(OpenXmlShapeDescriptor descriptor)
    {
        var transform = new Transform2D(
            new Offset { X = OpenXmlUnits.ToEmus(descriptor.Left), Y = OpenXmlUnits.ToEmus(descriptor.Top) },
            new Extents { Cx = OpenXmlUnits.ToEmus(descriptor.Width), Cy = OpenXmlUnits.ToEmus(descriptor.Height) });

        OpenXmlElement geometry = descriptor.Kind switch
        {
            OpenXmlShapeKind.Rectangle or OpenXmlShapeKind.Text => new PresetGeometry(new AdjustValueList())
            {
                Preset = ShapeTypeValues.Rectangle
            },
            OpenXmlShapeKind.Ellipse => new PresetGeometry(new AdjustValueList())
            {
                Preset = ShapeTypeValues.Ellipse
            },
            OpenXmlShapeKind.Line => new CustomGeometry(
                new AdjustValueList(),
                new ShapeGuideList(),
                new PathList(
                    new Path(
                        new MoveTo(new Point
                        {
                            X = OpenXmlUnits.ToEmus(descriptor.LineX1 - descriptor.Left).ToString(CultureInfo.InvariantCulture),
                            Y = OpenXmlUnits.ToEmus(descriptor.LineY1 - descriptor.Top).ToString(CultureInfo.InvariantCulture)
                        }),
                        new LineTo(new Point
                        {
                            X = OpenXmlUnits.ToEmus(descriptor.LineX2 - descriptor.Left).ToString(CultureInfo.InvariantCulture),
                            Y = OpenXmlUnits.ToEmus(descriptor.LineY2 - descriptor.Top).ToString(CultureInfo.InvariantCulture)
                        }))))
            ,
            _ => new PresetGeometry(new AdjustValueList())
            {
                Preset = ShapeTypeValues.Rectangle
            }
        };

        var props = new ShapeProperties(transform, geometry);
        ApplyFill(descriptor, props);
        ApplyOutline(descriptor, props);
        return props;
    }

    public static TextBody CreateTextBody(OpenXmlShapeDescriptor descriptor)
    {
        var body = new TextBody(
            new BodyProperties(),
            new ListStyle());

        var paragraph = new Paragraph();
        if (!string.IsNullOrWhiteSpace(descriptor.Text))
        {
            paragraph.AppendChild(new Run(new Text(descriptor.Text)));
        }

        paragraph.AppendChild(new EndParagraphRunProperties { Language = "en-US" });
        body.Append(paragraph);
        return body;
    }

    private static void ApplyFill(OpenXmlShapeDescriptor descriptor, ShapeProperties props)
    {
        if (!descriptor.IsFilled || descriptor.Fill is null)
        {
            props.AppendChild(new NoFill());
            return;
        }

        props.AppendChild(descriptor.Fill.Value.ToSolidFill());
    }

    private static void ApplyOutline(OpenXmlShapeDescriptor descriptor, ShapeProperties props)
    {
        if (!descriptor.IsStroked || descriptor.Stroke is null)
        {
            props.AppendChild(new Outline(new NoFill()));
            return;
        }

        var outline = new Outline
        {
            Width = (Int32Value)(int)System.Math.Min(System.Math.Max(OpenXmlUnits.ToEmus(descriptor.StrokeThickness), 0), int.MaxValue)
        };
        outline.Append(descriptor.Stroke.Value.ToSolidFill());
        props.AppendChild(outline);
    }
}
