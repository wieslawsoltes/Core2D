// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Renderer.Presenters;
using Core2D.Shape;
using Renderer.SkiaSharp;

namespace FileWriter.PdfSkiaSharp
{
    /// <summary>
    /// SkiaSharp pdf <see cref="IFileWriter"/> implementation.
    /// </summary>
    public sealed class PdfWriter : IFileWriter
    {
        /// <inheritdoc/>
        string IFileWriter.Name { get; } = "Pdf (SkiaSharp)";

        /// <inheritdoc/>
        string IFileWriter.Extension { get; } = "pdf";

        /// <inheritdoc/>
        void IFileWriter.Save(string path, object item, object options)
        {
            if (string.IsNullOrEmpty(path) || item == null)
                return;

            var ic = options as IImageCache;
            if (options == null)
                return;

            ShapeRenderer renderer = new SkiaRenderer(true, 72.0);
            renderer.State.DrawShapeState.Flags = ShapeStateFlags.Printable;
            renderer.State.ImageCache = ic;

            var presenter = new ContainerPresenter();

            IProjectExporter exporter = new PdfExporter(renderer, presenter, 72.0f);

            if (item is XContainer)
            {
                exporter.Save(path, item as XContainer);
            }
            else if (item is XDocument)
            {
                exporter.Save(path, item as XDocument);
            }
            else if (item is XProject)
            {
                exporter.Save(path, item as XProject);
            }
        }
    }
}
