// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using netDxf;
using netDxf.Header;
using netDxf.Objects;

namespace Core2D.Renderer.Dxf
{
    /// <summary>
    /// netDxf dxf <see cref="Core2D.Interfaces.IProjectExporter"/> implementation.
    /// </summary>
    public partial class DxfRenderer : Core2D.Renderer.ShapeRenderer, Core2D.Interfaces.IProjectExporter
    {
        /// <inheritdoc/>
        void Core2D.Interfaces.IProjectExporter.Save(string path, Core2D.Project.XContainer container)
        {
            _outputPath = System.IO.Path.GetDirectoryName(path);
            var dxf = new DxfDocument(DxfVersion.AutoCad2010);

            Add(dxf, container);

            dxf.Save(path);
            ClearCache(isZooming: false);
        }

        /// <inheritdoc/>
        void Core2D.Interfaces.IProjectExporter.Save(string path, Core2D.Project.XDocument document)
        {
            _outputPath = System.IO.Path.GetDirectoryName(path);
            var dxf = new DxfDocument(DxfVersion.AutoCad2010);

            Add(dxf, document);

            dxf.Save(path);
            ClearCache(isZooming: false);
        }

        /// <inheritdoc/>
        void Core2D.Interfaces.IProjectExporter.Save(string path, Core2D.Project.XProject project)
        {
            _outputPath = System.IO.Path.GetDirectoryName(path);
            var dxf = new DxfDocument(DxfVersion.AutoCad2010);

            Add(dxf, project);

            dxf.Save(path);
            ClearCache(isZooming: false);
        }

        private void Add(DxfDocument dxf, Core2D.Project.XContainer container)
        {
            if (container.Template != null)
            {
                _pageWidth = container.Template.Width;
                _pageHeight = container.Template.Height;
                Draw(dxf, container.Template, 0.0, 0.0, container.Data.Properties, container.Data.Record);
            }
            else
            {
                throw new NullReferenceException("Container template must be set.");
            }

            Draw(dxf, container, 0.0, 0.0, container.Data.Properties, container.Data.Record);
        }

        private void Add(DxfDocument dxf, Core2D.Project.XDocument document)
        {
            foreach (var page in document.Pages)
            {
                var layout = new Layout(page.Name)
                {
                    PlotSettings = new PlotSettings()
                    {
                        PaperSizeName = $"{page.Template.Name}_({page.Template.Width}_x_{page.Template.Height}_MM)",
                        LeftMargin = 0.0,
                        BottomMargin = 0.0,
                        RightMargin = 0.0,
                        TopMargin = 0.0,
                        PaperSize = new Vector2(page.Template.Width, page.Template.Height),
                        Origin = new Vector2(0.0, 0.0),
                        PaperUnits = PlotPaperUnits.Milimeters,
                        PaperRotation = PlotRotation.NoRotation
                    }
                };
                dxf.Layouts.Add(layout);
                dxf.ActiveLayout = layout.Name;

                Add(dxf, page);
            }
        }

        private void Add(DxfDocument dxf, Core2D.Project.XProject project)
        {
            foreach (var document in project.Documents)
            {
                Add(dxf, document);
            }
        }
    }
}
