// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Containers;
using Core2D.Interfaces;
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
        void IProjectExporter.Save(string path, IPageContainer container)
        {
            _outputPath = System.IO.Path.GetDirectoryName(path);
            var dxf = new DXF.DxfDocument(DXFH.DxfVersion.AutoCad2010);

            Add(dxf, container);

            dxf.Save(path);
            ClearCache(isZooming: false);
        }

        /// <inheritdoc/>
        void IProjectExporter.Save(string path, IDocumentContainer document)
        {
            _outputPath = System.IO.Path.GetDirectoryName(path);
            var dxf = new DXF.DxfDocument(DXFH.DxfVersion.AutoCad2010);

            Add(dxf, document);

            dxf.Save(path);
            ClearCache(isZooming: false);
        }

        /// <inheritdoc/>
        void IProjectExporter.Save(string path, IProjectContainer project)
        {
            _outputPath = System.IO.Path.GetDirectoryName(path);
            var dxf = new DXF.DxfDocument(DXFH.DxfVersion.AutoCad2010);

            Add(dxf, project);

            dxf.Save(path);
            ClearCache(isZooming: false);
        }

        private void Add(DXF.DxfDocument dxf, IPageContainer container)
        {
            var db = (object)container.Data.Properties;
            var record = (object)container.Data.Record;

            if (container.Template != null)
            {
                _pageWidth = container.Template.Width;
                _pageHeight = container.Template.Height;
                Draw(dxf, container.Template, 0.0, 0.0, db, record);
            }
            else
            {
                throw new NullReferenceException("Container template must be set.");
            }

            Draw(dxf, container, 0.0, 0.0, db, record);
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
