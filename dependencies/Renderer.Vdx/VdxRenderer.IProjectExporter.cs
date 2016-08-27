// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Renderer;
using VisioAutomation.VDX;
using VisioAutomation.VDX.Elements;

namespace Renderer.Vdx
{
    /// <summary>
    /// Visio vdx <see cref="IProjectExporter"/> implementation.
    /// </summary>
    public partial class VdxRenderer : ShapeRenderer, IProjectExporter
    {
        /// <inheritdoc/>
        void IProjectExporter.Save(string path, XContainer container, ShapeRenderer renderer)
        {
            var template = new Template();
            var drawing = new Drawing(template);

            Add(drawing, container, renderer);

            drawing.Save(path);
        }

        /// <inheritdoc/>
        void IProjectExporter.Save(string path, XDocument document, ShapeRenderer renderer)
        {
            var template = new Template();
            var drawing = new Drawing(template);

            foreach (var container in document.Pages)
            {
                Add(drawing, container, renderer);
            }

            drawing.Save(path);
            ClearCache(isZooming: false);
        }

        /// <inheritdoc/>
        void IProjectExporter.Save(string path, XProject project, ShapeRenderer renderer)
        {
            var template = new Template();
            var drawing = new Drawing(template);

            foreach (var document in project.Documents)
            {
                foreach (var container in document.Pages)
                {
                    Add(drawing, container, renderer);
                }
            }

            drawing.Save(path);
            ClearCache(isZooming: false);
        }

        private void Add(Drawing drawing, XContainer container, ShapeRenderer renderer)
        {
            var width = container.Template.Width;
            var height = container.Template.Height;

            var page = new Page(width, height);
            drawing.Pages.Add(page);

            if (container.Template.Background.A > 0)
            {
                renderer.Fill(page, 0, 0, width, height, container.Template.Background);
            }

            renderer.Draw(page, container.Template, 0.0, 0.0, container.Data.Properties, container.Data.Record);
            renderer.Draw(page, container, 0.0, 0.0, container.Data.Properties, container.Data.Record);
        }
    }
}
