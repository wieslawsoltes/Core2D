// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Renderer.Presenters;
using SkiaSharp;

namespace Core2D.FileWriter.SkiaSharpPdf
{
    /// <summary>
    /// SkiaSharp pdf <see cref="IProjectExporter"/> implementation.
    /// </summary>
    public sealed class PdfSkiaSharpExporter : IProjectExporter
    {
        private readonly ShapeRenderer _renderer;
        private readonly ContainerPresenter _presenter;
        private readonly float _targetDpi;

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSkiaSharpExporter"/> class.
        /// </summary>
        /// <param name="renderer">The shape renderer.</param>
        /// <param name="presenter">The container presenter.</param>
        /// <param name="targetDpi">The target renderer dpi.</param>
        public PdfSkiaSharpExporter(ShapeRenderer renderer, ContainerPresenter presenter, float targetDpi = 72.0f)
        {
            _renderer = renderer;
            _presenter = presenter;
            _targetDpi = targetDpi;
        }

        /// <inheritdoc/>
        void IProjectExporter.Save(string path, XContainer container)
        {
            using (var stream = new SKFileWStream(path))
            {
                using (var pdf = SKDocument.CreatePdf(stream, _targetDpi))
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
                using (var pdf = SKDocument.CreatePdf(stream, _targetDpi))
                {
                    foreach (var container in document.Pages)
                    {
                        Add(pdf, container);
                    }

                    pdf.Close();
                    _renderer.ClearCache(isZooming: false);
                }
            }
        }

        /// <inheritdoc/>
        void IProjectExporter.Save(string path, XProject project)
        {
            using (var stream = new SKFileWStream(path))
            {
                using (var pdf = SKDocument.CreatePdf(stream, _targetDpi))
                {
                    foreach (var document in project.Documents)
                    {
                        foreach (var container in document.Pages)
                        {
                            Add(pdf, container);
                        }
                    }

                    pdf.Close();
                    _renderer.ClearCache(isZooming: false);
                }
            }
        }

        private void Add(SKDocument pdf, XContainer container)
        {
            using (var canvas = pdf.BeginPage((float)container.Template.Width, (float)container.Template.Height))
            {
                _presenter.Render(canvas, _renderer, container, 0, 0);
            }
        }
    }
}
