using System;
using System.IO;
using Core2D;
using Core2D.Containers;
using Core2D.Data;
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace Core2D.Renderer.PdfSharp
{
    /// <summary>
    /// PdfSharp pdf <see cref="IProjectExporter"/> implementation.
    /// </summary>
    public partial class PdfSharpRenderer : IProjectExporter
    {
        /// <inheritdoc/>
        public void Save(Stream stream, IPageContainer container)
        {
            using var pdf = new PdfDocument();
            Add(pdf, container);
            pdf.Save(stream);
        }

        /// <inheritdoc/>
        public void Save(Stream stream, IDocumentContainer document)
        {
            using var pdf = new PdfDocument();
            var documentOutline = default(PdfOutline);

            foreach (var container in document.Pages)
            {
                var page = Add(pdf, container);

                if (documentOutline == null)
                {
                    documentOutline = pdf.Outlines.Add(
                        document.Name,
                        page,
                        true,
                        PdfOutlineStyle.Regular,
                        XColors.Black);
                }

                documentOutline.Outlines.Add(
                    container.Name,
                    page,
                    true,
                    PdfOutlineStyle.Regular,
                    XColors.Black);
            }

            pdf.Save(stream);
            ClearCache(isZooming: false);
        }

        /// <inheritdoc/>
        public void Save(Stream stream, IProjectContainer project)
        {
            using var pdf = new PdfDocument();
            var projectOutline = default(PdfOutline);

            foreach (var document in project.Documents)
            {
                var documentOutline = default(PdfOutline);

                foreach (var container in document.Pages)
                {
                    var page = Add(pdf, container);

                    if (projectOutline == null)
                    {
                        projectOutline = pdf.Outlines.Add(
                            project.Name,
                            page,
                            true,
                            PdfOutlineStyle.Regular,
                            XColors.Black);
                    }

                    if (documentOutline == null)
                    {
                        documentOutline = projectOutline.Outlines.Add(
                            document.Name,
                            page,
                            true,
                            PdfOutlineStyle.Regular,
                            XColors.Black);
                    }

                    documentOutline.Outlines.Add(
                        container.Name,
                        page,
                        true,
                        PdfOutlineStyle.Regular,
                        XColors.Black);
                }
            }

            pdf.Save(stream);
            ClearCache(isZooming: false);
        }

        private PdfPage Add(PdfDocument pdf, IPageContainer container)
        {
            // Create A3 page size with Landscape orientation.
            var pdfPage = pdf.AddPage();
            pdfPage.Size = PageSize.A3;
            pdfPage.Orientation = PageOrientation.Landscape;

            var dataFlow = _serviceProvider.GetService<IDataFlow>();
            var db = (object)container.Data.Properties;
            var record = (object)container.Data.Record;

            dataFlow.Bind(container.Template, db, record);
            dataFlow.Bind(container, db, record);

            using (XGraphics gfx = XGraphics.FromPdfPage(pdfPage))
            {
                // Calculate x and y page scale factors.
                double scaleX = pdfPage.Width.Value / container.Template.Width;
                double scaleY = pdfPage.Height.Value / container.Template.Height;
                double scale = Math.Min(scaleX, scaleY);

                // Set scaling function.
                _scaleToPage = (value) => value * scale;

                // Draw container template contents to pdf graphics.
                Fill(gfx, 0, 0, pdfPage.Width.Value / scale, pdfPage.Height.Value / scale, container.Template.Background);

                // Draw template contents to pdf graphics.
                Draw(gfx, container.Template, 0.0, 0.0);

                // Draw page contents to pdf graphics.
                Draw(gfx, container, 0.0, 0.0);
            }

            return pdfPage;
        }
    }
}
