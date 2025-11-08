// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.IO;
using System.Linq;
using Core2D.Model;
using Core2D.Model.OpenXml;
using Core2D.Modules.Renderer.OpenXml;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Dw = DocumentFormat.OpenXml.Drawing.Wordprocessing;
using A = DocumentFormat.OpenXml.Drawing;
using Pic = DocumentFormat.OpenXml.Drawing.Pictures;

namespace Core2D.Modules.FileWriter.OpenXml;

public sealed class WordOpenXmlWriter : IFileWriter
{
    private readonly IServiceProvider? _serviceProvider;

    public WordOpenXmlWriter(IServiceProvider? serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public string Name => "Word (OpenXML)";

    public string Extension => "docx";

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
        var wordAdapter = new OpenXmlWordShapeAdapter();
        var renderedPages = OpenXmlExportUtilities.RenderPages(renderer, project, pages).ToList();
        if (renderedPages.Count == 0)
        {
            return;
        }

        using var document = WordprocessingDocument.Create(stream, WordprocessingDocumentType.Document);
        var mainPart = document.AddMainDocumentPart();
        mainPart.Document = new Document(new Body());
        var imageIndex = 1U;

        foreach (var rendered in renderedPages)
        {
            var descriptors = drawingRenderer.Render(rendered.Page).ToList();
            if (descriptors.Count > 0)
            {
                var paragraph = new Paragraph();
                foreach (var descriptor in descriptors)
                {
                    var vectorDrawing = wordAdapter.CreateDrawing(descriptor);
                    paragraph.Append(new Run(vectorDrawing));
                }
                mainPart.Document.Body?.Append(paragraph);
            }
            else
            {
                var imagePart = mainPart.AddImagePart(ImagePartType.Png);
                using (var imageStream = new MemoryStream(rendered.Image))
                {
                    imagePart.FeedData(imageStream);
                }

                var relationshipId = mainPart.GetIdOfPart(imagePart);
                var drawing = CreateImageDrawing(relationshipId, rendered, imageIndex++);
                var run = new Run(drawing);
                var paragraph = new Paragraph(run);
                mainPart.Document.Body?.Append(paragraph);
            }

            mainPart.Document.Body?.Append(new Paragraph(new Run(new Break())));
        }

        mainPart.Document.Save();

        var payloadJson = OpenXmlPayloadSerializer.Create(jsonSerializer, item);
        OpenXmlPackagePartHelper.WritePayload(mainPart, payloadJson);
    }

    private static Drawing CreateImageDrawing(string relationshipId, OpenXmlRenderedPage rendered, uint id)
    {
        var width = OpenXmlExportUtilities.ToEmus(rendered.Width);
        var height = OpenXmlExportUtilities.ToEmus(rendered.Height);

        return new Drawing(
            new Dw.Inline(
                new Dw.Extent { Cx = width, Cy = height },
                new Dw.EffectExtent
                {
                    LeftEdge = 0L,
                    TopEdge = 0L,
                    RightEdge = 0L,
                    BottomEdge = 0L
                },
                new Dw.DocProperties { Id = id, Name = $"Picture {id}" },
                new Dw.NonVisualGraphicFrameDrawingProperties(new A.GraphicFrameLocks { NoChangeAspect = true }),
                new A.Graphic(
                    new A.GraphicData(
                        new Pic.Picture(
                            new Pic.NonVisualPictureProperties(
                                new Pic.NonVisualDrawingProperties { Id = id, Name = $"Picture {id}" },
                                new Pic.NonVisualPictureDrawingProperties()),
                            new Pic.BlipFill(
                                new A.Blip { Embed = relationshipId },
                                new A.Stretch(new A.FillRectangle())),
                            new Pic.ShapeProperties(
                                new A.Transform2D(
                                    new A.Offset { X = 0L, Y = 0L },
                                    new A.Extents { Cx = width, Cy = height }),
                                new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle })))
                    {
                        Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture"
                    }))
            {
                DistanceFromBottom = 0U,
                DistanceFromLeft = 0U,
                DistanceFromRight = 0U,
                DistanceFromTop = 0U
            });
    }
}
