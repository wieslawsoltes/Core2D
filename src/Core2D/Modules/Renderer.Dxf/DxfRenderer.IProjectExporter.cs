using System;
using System.IO;
using Core2D;
using Core2D.Containers;
using Core2D.Data;
using DXF = netDxf;
using DXFH = netDxf.Header;
using DXFO = netDxf.Objects;

namespace Core2D.Renderer.Dxf
{
    /// <summary>
    /// netDxf dxf <see cref="IProjectExporter"/> implementation.
    /// </summary>
    public partial class DxfRenderer : IProjectExporter
    {
        /// <inheritdoc/>
        public void Save(Stream stream, IPageContainer container)
        {
            if (stream is FileStream fileStream)
            {
                _outputPath = System.IO.Path.GetDirectoryName(fileStream.Name);
            }
            else
            {
                _outputPath = string.Empty;
            }

            var dxf = new DXF.DxfDocument(DXFH.DxfVersion.AutoCad2010);

            Add(dxf, container);

            dxf.Save(stream);
            ClearCache(isZooming: false);
        }

        /// <inheritdoc/>
        public void Save(Stream stream, IDocumentContainer document)
        {
            if (stream is FileStream fileStream)
            {
                _outputPath = System.IO.Path.GetDirectoryName(fileStream.Name);
            }
            else
            {
                _outputPath = string.Empty;
            }

            var dxf = new DXF.DxfDocument(DXFH.DxfVersion.AutoCad2010);

            Add(dxf, document);

            dxf.Save(stream);
            ClearCache(isZooming: false);
        }

        /// <inheritdoc/>
        public void Save(Stream stream, IProjectContainer project)
        {
            if (stream is FileStream fileStream)
            {
                _outputPath = System.IO.Path.GetDirectoryName(fileStream.Name);
            }
            else
            {
                _outputPath = string.Empty;
            }

            var dxf = new DXF.DxfDocument(DXFH.DxfVersion.AutoCad2010);

            Add(dxf, project);

            dxf.Save(stream);
            ClearCache(isZooming: false);
        }

        private void Add(DXF.DxfDocument dxf, IPageContainer container)
        {
            var dataFlow = _serviceProvider.GetService<IDataFlow>();
            var db = (object)container.Data.Properties;
            var record = (object)container.Data.Record;

            dataFlow.Bind(container.Template, db, record);
            dataFlow.Bind(container, db, record);

            if (container.Template != null)
            {
                _pageWidth = container.Template.Width;
                _pageHeight = container.Template.Height;
                Draw(dxf, container.Template, 0.0, 0.0);
            }
            else
            {
                throw new NullReferenceException("Container template must be set.");
            }

            Draw(dxf, container, 0.0, 0.0);
        }

        private void Add(DXF.DxfDocument dxf, IDocumentContainer document)
        {
            foreach (var page in document.Pages)
            {
                var layout = new DXFO.Layout(page.Name)
                {
                    PlotSettings = new DXFO.PlotSettings()
                    {
                        PaperSizeName = $"{page.Template.Name}_({page.Template.Width}_x_{page.Template.Height}_MM)",
                        PaperMargin = new DXFO.PaperMargin(0, 0, 0, 0),
                        PaperSize = new DXF.Vector2(page.Template.Width, page.Template.Height),
                        Origin = new DXF.Vector2(0.0, 0.0),
                        PaperUnits = DXFO.PlotPaperUnits.Milimeters,
                        PaperRotation = DXFO.PlotRotation.NoRotation
                    }
                };
                dxf.Layouts.Add(layout);
                dxf.ActiveLayout = layout.Name;

                Add(dxf, page);
            }
        }

        private void Add(DXF.DxfDocument dxf, IProjectContainer project)
        {
            foreach (var document in project.Documents)
            {
                Add(dxf, document);
            }
        }
    }
}
