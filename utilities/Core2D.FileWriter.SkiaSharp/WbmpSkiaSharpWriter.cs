// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Renderer.Presenters;
using Core2D.Shape;
using Core2D.Renderer.SkiaSharp;

namespace Core2D.FileWriter.SkiaSharpWbmp
{
    /// <summary>
    /// SkiaSharp wbmp <see cref="IFileWriter"/> implementation.
    /// </summary>
    public sealed class WbmpSkiaSharpWriter : IFileWriter
    {
        /// <inheritdoc/>
        string IFileWriter.Name { get; } = "Wbmp (SkiaSharp)";

        /// <inheritdoc/>
        string IFileWriter.Extension { get; } = "wbmp";

        /// <inheritdoc/>
        void IFileWriter.Save(string path, object item, object options)
        {
            if (string.IsNullOrEmpty(path) || item == null)
                return;

            var ic = options as IImageCache;
            if (options == null)
                return;

            var renderer = new SkiaSharpRenderer(true, 96.0);
            renderer.State.DrawShapeState.Flags = ShapeStateFlags.Printable;
            renderer.State.ImageCache = ic;

            var presenter = new ExportPresenter();

            IProjectExporter exporter = new WbmpSkiaSharpExporter(renderer, presenter);

            if (item is XContainer)
            {
                exporter.Save(path, item as XContainer);
            }
            else if (item is XDocument)
            {
                throw new NotSupportedException("Saving documents as wbmp drawing is not supported.");
            }
            else if (item is XProject)
            {
                throw new NotSupportedException("Saving projects as wbmp drawing is not supported.");
            }
        }
    }
}
