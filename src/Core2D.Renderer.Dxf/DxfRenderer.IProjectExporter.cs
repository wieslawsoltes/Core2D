// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Interfaces;
using Core2D.Containers;
using DXF=netDxf;
using DXFH=netDxf.Header;
using DXFO=netDxf.Objects;

namespace Core2D.Renderer.Dxf
{
    /// <summary>
    /// netDxf dxf <see cref="IProjectExporter"/> implementation.
    /// </summary>
    public partial class DxfRenderer : ShapeRenderer, IProjectExporter
    {
        /// <inheritdoc/>
        void IProjectExporter.Save(string path, PageContainer container)
        {
            _outputPath = System.IO.Path.GetDirectoryName(path);
            var dxf = new DXF.DxfDocument(DXFH.DxfVersion.AutoCad2010);

            Add(dxf, container);

            dxf.Save(path);
            ClearCache(isZooming: false);
        }

        /// <inheritdoc/>
        void IProjectExporter.Save(string path, DocumentContainer document)
        {
            _outputPath = System.IO.Path.GetDirectoryName(path);
            var dxf = new DXF.DxfDocument(DXFH.DxfVersion.AutoCad2010);

            Add(dxf, document);

            dxf.Save(path);
            ClearCache(isZooming: false);
        }

        /// <inheritdoc/>
        void IProjectExporter.Save(string path, ProjectContainer project)
        {
            _outputPath = System.IO.Path.GetDirectoryName(path);
            var dxf = new DXF.DxfDocument(DXFH.DxfVersion.AutoCad2010);

            Add(dxf, project);

            dxf.Save(path);
            ClearCache(isZooming: false);
        }

        private void Add(DXF.DxfDocument dxf, PageContainer container)
        {
            if (container.Template != null)
            {
                _pageWidth = container.Template.Width;
                _pageHeight = container.Template.Height;
                Draw(dxf, container.Template, 0.0, 0.0, (object)container.Data.Properties, (object)container.Data.Record);
            }
            else
            {
                throw new NullReferenceException("Container template must be set.");
            }

            Draw(dxf, container, 0.0, 0.0, (object)container.Data.Properties, (object)container.Data.Record);
        }

        private void Add(DXF.DxfDocument dxf, DocumentContainer document)
        {
            foreach (var page in document.Pages)
            {
                var layout = new DXFO.Layout(page.Name)
                {
                    PlotSettings = new DXFO.PlotSettings()
                    {
                        PaperSizeName = $"{page.Template.Name}_({page.Template.Width}_x_{page.Template.Height}_MM)",
                        LeftMargin = 0.0,
                        BottomMargin = 0.0,
                        RightMargin = 0.0,
                        TopMargin = 0.0,
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

        private void Add(DXF.DxfDocument dxf, ProjectContainer project)
        {
            foreach (var document in project.Documents)
            {
                Add(dxf, document);
            }
        }
    }
}
