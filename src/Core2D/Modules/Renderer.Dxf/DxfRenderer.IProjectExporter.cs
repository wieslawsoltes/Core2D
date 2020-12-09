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
        public void Save(Stream stream, PageContainerViewModel containerViewModel)
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

            Add(dxf, containerViewModel);

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

        private void Add(DXF.DxfDocument dxf, PageContainerViewModel containerViewModel)
        {
            var dataFlow = _serviceProvider.GetService<DataFlow>();
            var db = (object)containerViewModel.Properties;
            var record = (object)containerViewModel.RecordViewModel;

            dataFlow.Bind(containerViewModel.Template, db, record);
            dataFlow.Bind(containerViewModel, db, record);

            if (containerViewModel.Template != null)
            {
                _pageWidth = containerViewModel.Template.Width;
                _pageHeight = containerViewModel.Template.Height;
                DrawPage(dxf, containerViewModel.Template);
            }
            else
            {
                throw new NullReferenceException("Container template must be set.");
            }

            DrawPage(dxf, containerViewModel);
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
