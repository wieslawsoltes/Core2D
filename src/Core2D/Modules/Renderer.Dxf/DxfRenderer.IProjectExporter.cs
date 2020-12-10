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
    public partial class DxfRenderer : IProjectExporter
    {
        public void Save(Stream stream, PageContainerViewModel container)
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
            ClearCache();
        }

        public void Save(Stream stream, DocumentContainerViewModel document)
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
            ClearCache();
        }

        public void Save(Stream stream, ProjectContainerViewModel project)
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
            ClearCache();
        }

        private void Add(DXF.DxfDocument dxf, PageContainerViewModel container)
        {
            var dataFlow = _serviceProvider.GetService<DataFlow>();
            var db = (object)container.Properties;
            var record = (object)container.RecordViewModel;

            dataFlow.Bind(container.Template, db, record);
            dataFlow.Bind(container, db, record);

            if (container.Template != null)
            {
                _pageWidth = container.Template.Width;
                _pageHeight = container.Template.Height;
                DrawPage(dxf, container.Template);
            }
            else
            {
                throw new NullReferenceException("Container template must be set.");
            }

            DrawPage(dxf, container);
        }

        private void Add(DXF.DxfDocument dxf, DocumentContainerViewModel document)
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

        private void Add(DXF.DxfDocument dxf, ProjectContainerViewModel project)
        {
            foreach (var document in project.Documents)
            {
                Add(dxf, document);
            }
        }
    }
}
