﻿#nullable enable
using System;
using System.IO;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Renderer.Presenters;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Core2D.Modules.Renderer.PdfSharp;

public partial class PdfSharpRenderer : IProjectExporter
{
    public void Save(Stream stream, PageContainerViewModel container)
    {
        var presenter = new ExportPresenter();
        using var pdf = new PdfDocument();
        Add(pdf, container, presenter);
        pdf.Save(stream);
    }

    public void Save(Stream stream, DocumentContainerViewModel document)
    {
        var presenter = new ExportPresenter();
        using var pdf = new PdfDocument();
        var documentOutline = default(PdfOutline);

        foreach (var container in document.Pages)
        {
            var page = Add(pdf, container, presenter);

            documentOutline ??= pdf.Outlines.Add(
                document.Name,
                page,
                true,
                PdfOutlineStyle.Regular,
                XColors.Black);

            documentOutline.Outlines.Add(
                container.Name,
                page,
                true,
                PdfOutlineStyle.Regular,
                XColors.Black);
        }

        pdf.Save(stream);
        ClearCache();
    }

    public void Save(Stream stream, ProjectContainerViewModel project)
    {
        var presenter = new ExportPresenter();
        using var pdf = new PdfDocument();
        var projectOutline = default(PdfOutline);

        foreach (var document in project.Documents)
        {
            var documentOutline = default(PdfOutline);

            foreach (var container in document.Pages)
            {
                var page = Add(pdf, container, presenter);

                projectOutline ??= pdf.Outlines.Add(
                    project.Name,
                    page,
                    true,
                    PdfOutlineStyle.Regular,
                    XColors.Black);

                documentOutline ??= projectOutline.Outlines.Add(
                    document.Name,
                    page,
                    true,
                    PdfOutlineStyle.Regular,
                    XColors.Black);

                documentOutline.Outlines.Add(
                    container.Name,
                    page,
                    true,
                    PdfOutlineStyle.Regular,
                    XColors.Black);
            }
        }

        pdf.Save(stream);
        ClearCache();
    }

    private PdfPage Add(PdfDocument pdf, PageContainerViewModel container, IContainerPresenter presenter)
    {
        // Create A3 page size with Landscape orientation.
        var pdfPage = pdf.AddPage();
        pdfPage.Size = PageSize.A3;
        pdfPage.Orientation = PageOrientation.Landscape;

        var dataFlow = ServiceProvider.GetService<DataFlow>();
        var db = (object)container.Properties;
        var record = (object?)container.Record;

        if (dataFlow is { })
        {
            dataFlow.Bind(container.Template, db, record);
            dataFlow.Bind(container, db, record);
        }

        using var gfx = XGraphics.FromPdfPage(pdfPage);
        // Calculate x and y page scale factors.
        double scaleX = pdfPage.Width.Value / (container.Template?.Width ?? 1d);
        double scaleY = pdfPage.Height.Value / (container.Template?.Height ?? 1d);
        double scale = Math.Min(scaleX, scaleY);

        // Set scaling function.
        _scaleToPage = (value) => value * scale;

        // Draw container template contents to pdf graphics.
        if (container.Template?.Background is { })
        {
            Fill(gfx, 0, 0, pdfPage.Width.Value / scale, pdfPage.Height.Value / scale, container.Template.Background);
        }

        // Draw template contents to pdf graphics.
        presenter.Render(gfx, this, null, container.Template, 0, 0);

        // Draw page contents to pdf graphics.
        presenter.Render(gfx, this, null, container, 0, 0);

        return pdfPage;
    }
}
