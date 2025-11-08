// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using DocumentFormat.OpenXml;
using A = DocumentFormat.OpenXml.Drawing;
using DW = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using W = DocumentFormat.OpenXml.Wordprocessing;

namespace Core2D.Modules.Renderer.OpenXml;

public sealed class OpenXmlWordShapeAdapter
{
    private uint _shapeId = 1U;

    public W.Drawing CreateDrawing(OpenXmlShapeDescriptor descriptor)
    {
        var id = _shapeId++;
        var props = OpenXmlShapeFactory.CreateShapeProperties(descriptor);
        var textBody = OpenXmlShapeFactory.CreateTextBody(descriptor);

        var shape = new A.Shape(
            new A.NonVisualShapeProperties(
                new A.NonVisualDrawingProperties { Id = id, Name = descriptor.Name ?? $"Shape {id}" },
                new A.NonVisualShapeDrawingProperties()),
            props,
            textBody);

        var graphic = new A.Graphic(new A.GraphicData(shape)
        {
            Uri = "http://schemas.openxmlformats.org/drawingml/2006/main"
        });

        var inline = new DW.Inline(
            new DW.Extent
            {
                Cx = OpenXmlUnits.ToEmus(descriptor.Width),
                Cy = OpenXmlUnits.ToEmus(descriptor.Height)
            },
            new DW.EffectExtent
            {
                LeftEdge = 0L,
                RightEdge = 0L,
                TopEdge = 0L,
                BottomEdge = 0L
            },
            new DW.DocProperties { Id = id, Name = descriptor.Name ?? $"Shape {id}" },
            new DW.NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks { NoChangeAspect = true }),
            graphic)
        {
            DistanceFromBottom = 0U,
            DistanceFromLeft = 0U,
            DistanceFromRight = 0U,
            DistanceFromTop = 0U
        };

        return new W.Drawing(inline);
    }
}
