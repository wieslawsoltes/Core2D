// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Interfaces;
using Core2D.Math;
using Core2D.Project;
using Core2D.Renderer;
using SkiaSharp;

namespace Renderer.SkiaSharp
{
    /// <summary>
    /// SkiaSharp pdf <see cref="IProjectExporter"/> implementation.
    /// </summary>
    public partial class SkiaRenderer : ShapeRenderer, IProjectExporter
    {
        /// <inheritdoc/>
        void IProjectExporter.Save(string path, XContainer container)
        {
            using (var stream = new SKFileWStream(path))
            {
                using (var pdf = SKDocument.CreatePdf(stream, (float)_targetDpi))
                {
                    Add(pdf, container);
                    pdf.Close();
                }
            }
        }

        /// <inheritdoc/>
        void IProjectExporter.Save(string path, XDocument document)
        {
            using (var stream = new SKFileWStream(path))
            {
                using (var pdf = SKDocument.CreatePdf(stream, (float)_targetDpi))
                {
                    foreach (var container in document.Pages)
                    {
                        Add(pdf, container);
                    }

                    pdf.Close();
                    ClearCache(isZooming: false);
                }
            }
        }

        /// <inheritdoc/>
        void IProjectExporter.Save(string path, XProject project)
        {
            using (var stream = new SKFileWStream(path))
            {
                using (var pdf = SKDocument.CreatePdf(stream, (float)_targetDpi))
                {
                    foreach (var document in project.Documents)
                    {
                        foreach (var container in document.Pages)
                        {
                            Add(pdf, container);
                        }
                    }

                    pdf.Close();
                    ClearCache(isZooming: false);
                }
            }
        }

        private void Add(SKDocument pdf, XContainer container)
        {
            float width = (float)container.Template.Width;
            float height = (float)container.Template.Height;
            using (SKCanvas canvas = pdf.BeginPage(width, height))
            {
                // Calculate x and y page scale factors.
                double scaleX = width / container.Template.Width;
                double scaleY = height / container.Template.Height;
                double scale = Math.Min(scaleX, scaleY);

                // Set scaling function.
                _scaleToPage = (value) => (float)(value * scale);

                // Draw container template contents to pdf graphics.
                if (container.Template.Background.A > 0)
                {
                    DrawBackgroundInternal(
                        canvas,
                        container.Template.Background,
                        Rect2.Create(0, 0, width / scale, height / scale));
                }

                // Draw template contents to pdf graphics.
                Draw(canvas, container.Template, container.Data.Properties, container.Data.Record);

                // Draw page contents to pdf graphics.
                Draw(canvas, container, container.Data.Properties, container.Data.Record);
            }
        }
    }
}
