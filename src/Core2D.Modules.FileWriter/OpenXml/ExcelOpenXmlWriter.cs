// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Core2D.Model;
using Core2D.Model.OpenXml;
using Core2D.Modules.Renderer.OpenXml;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml;
using A = DocumentFormat.OpenXml.Drawing;
using Xdr = DocumentFormat.OpenXml.Drawing.Spreadsheet;

namespace Core2D.Modules.FileWriter.OpenXml;

public sealed class ExcelOpenXmlWriter : IFileWriter
{
    private readonly IServiceProvider? _serviceProvider;

    public ExcelOpenXmlWriter(IServiceProvider? serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public string Name => "Excel (OpenXML)";

    public string Extension => "xlsx";

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
        var spreadsheetAdapter = new OpenXmlSpreadsheetShapeAdapter();
        var renderedPages = OpenXmlExportUtilities.RenderPages(renderer, project, pages).ToList();
        if (renderedPages.Count == 0)
        {
            return;
        }

        using var document = SpreadsheetDocument.Create(stream, SpreadsheetDocumentType.Workbook);
        var workbookPart = document.AddWorkbookPart();
        workbookPart.Workbook = new Workbook();
        var sheets = workbookPart.Workbook.AppendChild(new Sheets());

        uint sheetIndex = 1;
        foreach (var (rendered, pageIndex) in renderedPages.Select((rp, idx) => (rp, idx)))
        {
            var worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
            worksheetPart.Worksheet = new Worksheet(new SheetData());

            var sheet = new Sheet
            {
                Id = workbookPart.GetIdOfPart(worksheetPart),
                SheetId = sheetIndex++,
                Name = OpenXmlExportUtilities.CreatePageName(rendered.Page, pageIndex)
            };
            sheets.Append(sheet);

            var descriptors = drawingRenderer.Render(rendered.Page).ToList();
            var drawingsPart = EnsureDrawingsPart(worksheetPart);
            if (descriptors.Count > 0)
            {
                var drawing = drawingsPart.WorksheetDrawing ??= new Xdr.WorksheetDrawing();

                foreach (var descriptor in descriptors)
                {
                    var anchor = spreadsheetAdapter.CreateAnchor(descriptor);
                    drawing.Append(anchor);
                }

                drawing.Save(drawingsPart);
            }
            else
            {
                InsertImage(drawingsPart, worksheetPart, rendered, (uint)(pageIndex + 1));
            }
        }

        workbookPart.Workbook.Save();

        var payloadJson = OpenXmlPayloadSerializer.Create(jsonSerializer, item);
        OpenXmlPackagePartHelper.WritePayload(workbookPart, payloadJson);
    }

    private static void InsertImage(DrawingsPart drawingsPart, WorksheetPart worksheetPart, OpenXmlRenderedPage rendered, uint imageId)
    {
        var drawing = drawingsPart.WorksheetDrawing ??= new Xdr.WorksheetDrawing();
        var imagePart = drawingsPart.AddImagePart(ImagePartType.Png);
        using (var imageStream = new MemoryStream(rendered.Image))
        {
            imagePart.FeedData(imageStream);
        }

        var relationshipId = drawingsPart.GetIdOfPart(imagePart);

        var picture = CreatePicture(relationshipId, $"Image {imageId}", imageId, rendered);
        var absoluteAnchor = new Xdr.AbsoluteAnchor(
            new Xdr.Position { X = 0L, Y = 0L },
            new Xdr.Extent
            {
                Cx = OpenXmlExportUtilities.ToEmus(rendered.Width),
                Cy = OpenXmlExportUtilities.ToEmus(rendered.Height)
            },
            picture,
            new Xdr.ClientData());

        drawing.Append(absoluteAnchor);
        drawing.Save(drawingsPart);
    }

    private static DrawingsPart EnsureDrawingsPart(WorksheetPart worksheetPart)
    {
        var drawingsPart = worksheetPart.DrawingsPart;
        if (drawingsPart is null)
        {
            drawingsPart = worksheetPart.AddNewPart<DrawingsPart>();
            drawingsPart.WorksheetDrawing = new Xdr.WorksheetDrawing();
            worksheetPart.Worksheet.Append(new Drawing { Id = worksheetPart.GetIdOfPart(drawingsPart) });
            worksheetPart.Worksheet.Save();
        }
        else if (drawingsPart.WorksheetDrawing is null)
        {
            drawingsPart.WorksheetDrawing = new Xdr.WorksheetDrawing();
        }

        return drawingsPart;
    }

    private static Xdr.Picture CreatePicture(string relationshipId, string name, uint id, OpenXmlRenderedPage rendered)
    {
        return new Xdr.Picture(
            new Xdr.NonVisualPictureProperties(
                new Xdr.NonVisualDrawingProperties { Id = id, Name = name },
                new Xdr.NonVisualPictureDrawingProperties(new A.PictureLocks { NoChangeAspect = true })),
            new Xdr.BlipFill(
                new A.Blip { Embed = relationshipId },
                new A.Stretch(new A.FillRectangle())),
            new Xdr.ShapeProperties(
                new A.Transform2D(
                    new A.Offset { X = 0L, Y = 0L },
                    new A.Extents
                    {
                        Cx = OpenXmlExportUtilities.ToEmus(rendered.Width),
                        Cy = OpenXmlExportUtilities.ToEmus(rendered.Height)
                    }),
                new A.PresetGeometry(new A.AdjustValueList()) { Preset = A.ShapeTypeValues.Rectangle }));
    }
}
