// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using DocumentFormat.OpenXml.Presentation;
using A = DocumentFormat.OpenXml.Drawing;

namespace Core2D.Modules.Renderer.OpenXml;

public sealed class OpenXmlPowerPointShapeAdapter
{
    private uint _shapeId = 1U;

    public Shape CreateShape(OpenXmlShapeDescriptor descriptor)
    {
        var id = _shapeId++;
        var name = string.IsNullOrWhiteSpace(descriptor.Name) ? $"Shape {id}" : descriptor.Name;

        var nonVisual = new NonVisualShapeProperties(
            new NonVisualDrawingProperties { Id = id, Name = name },
            new NonVisualShapeDrawingProperties(new A.ShapeLocks { NoChangeAspect = true }),
            new ApplicationNonVisualDrawingProperties());

        var properties = OpenXmlShapeFactory.CreateShapeProperties(descriptor);
        var textBody = CreateTextBody(descriptor);

        return new Shape(nonVisual, properties, textBody);
    }

    private static TextBody CreateTextBody(OpenXmlShapeDescriptor descriptor)
    {
        var paragraph = new A.Paragraph();
        if (OpenXmlShapeKind.Text == descriptor.Kind && !string.IsNullOrWhiteSpace(descriptor.Text))
        {
            paragraph.AppendChild(new A.Run(new A.Text(descriptor.Text)));
        }

        paragraph.AppendChild(new A.EndParagraphRunProperties { Language = "en-US" });

        return new TextBody(
            new A.BodyProperties(),
            new A.ListStyle(),
            paragraph);
    }
}
