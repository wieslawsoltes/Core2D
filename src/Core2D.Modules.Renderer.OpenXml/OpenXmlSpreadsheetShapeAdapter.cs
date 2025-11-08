// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using Xdr = DocumentFormat.OpenXml.Drawing.Spreadsheet;

namespace Core2D.Modules.Renderer.OpenXml;

public sealed class OpenXmlSpreadsheetShapeAdapter
{
    private uint _shapeId = 1U;

    public Xdr.AbsoluteAnchor CreateAnchor(OpenXmlShapeDescriptor descriptor)
    {
        var shape = CreateShape(descriptor);
        return new Xdr.AbsoluteAnchor(
            new Xdr.Position
            {
                X = OpenXmlUnits.ToEmus(descriptor.Left),
                Y = OpenXmlUnits.ToEmus(descriptor.Top)
            },
            new Xdr.Extent
            {
                Cx = OpenXmlUnits.ToEmus(descriptor.Width),
                Cy = OpenXmlUnits.ToEmus(descriptor.Height)
            },
            shape,
            new Xdr.ClientData());
    }

    private Xdr.Shape CreateShape(OpenXmlShapeDescriptor descriptor)
    {
        var id = _shapeId++;
        var name = string.IsNullOrWhiteSpace(descriptor.Name) ? $"Shape {id}" : descriptor.Name;

        var properties = OpenXmlShapeFactory.CreateShapeProperties(descriptor);
        var textBody = OpenXmlShapeFactory.CreateTextBody(descriptor);

        return new Xdr.Shape(
            new Xdr.NonVisualShapeProperties(
                new Xdr.NonVisualDrawingProperties { Id = id, Name = name },
                new Xdr.NonVisualShapeDrawingProperties(new ShapeLocks { NoChangeAspect = true })),
            properties,
            textBody);
    }
}
