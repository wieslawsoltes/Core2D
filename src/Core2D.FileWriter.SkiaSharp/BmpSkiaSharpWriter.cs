// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Core2D.Containers;
using Core2D.Interfaces;
using Core2D.Renderer;
using Core2D.Renderer.Presenters;
using Core2D.Renderer.SkiaSharp;

namespace Core2D.FileWriter.SkiaSharpBmp
{
    /// <summary>
    /// SkiaSharp bmp <see cref="IFileWriter"/> implementation.
    /// </summary>
    public sealed class BmpSkiaSharpWriter : IFileWriter
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="BmpSkiaSharpWriter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public BmpSkiaSharpWriter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        string IFileWriter.Name { get; } = "Bmp (SkiaSharp)";

        /// <inheritdoc/>
        string IFileWriter.Extension { get; } = "bmp";

        /// <inheritdoc/>
        void IFileWriter.Save(string path, object item, object options)
        {
            if (string.IsNullOrEmpty(path) || item == null)
                return;

            var ic = options as IImageCache;
            if (options == null)
                return;

            var renderer = new SkiaSharpRenderer(_serviceProvider, true, 96.0);
            renderer.State.DrawShapeState.Flags = ShapeStateFlags.Printable;
            renderer.State.ImageCache = ic;

            var presenter = new ExportPresenter();

            IProjectExporter exporter = new BmpSkiaSharpExporter(renderer, presenter);

            if (item is IPageContainer page)
            {
                exporter.Save(path, page);
            }
            else if (item is IDocumentContainer document)
            {
                throw new NotSupportedException("Saving documents as bmp drawing is not supported.");
            }
            else if (item is IProjectContainer project)
            {
                throw new NotSupportedException("Saving projects as bmp drawing is not supported.");
            }
        }
    }
}
