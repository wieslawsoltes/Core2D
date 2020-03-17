// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;
using Core2D.Containers;
using Core2D.Interfaces;
using Core2D.Renderer;
using SkiaSharp;

namespace Core2D.FileWriter.SkiaSharpWebp
{
    /// <summary>
    /// SkiaSharp webp <see cref="IProjectExporter"/> implementation.
    /// </summary>
    public sealed class WebpSkiaSharpExporter : IProjectExporter
    {
        private readonly IShapeRenderer _renderer;
        private readonly IContainerPresenter _presenter;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebpSkiaSharpExporter"/> class.
        /// </summary>
        /// <param name="renderer">The shape renderer.</param>
        /// <param name="presenter">The container presenter.</param>
        public WebpSkiaSharpExporter(IShapeRenderer renderer, IContainerPresenter presenter)
        {
            _renderer = renderer;
            _presenter = presenter;
        }

        /// <inheritdoc/>
        void IProjectExporter.Save(string path, IPageContainer container)
        {
            Save(path, container);
        }

        /// <inheritdoc/>
        void IProjectExporter.Save(string path, IDocumentContainer document)
        {
            throw new NotSupportedException("Saving documents as webp drawing is not supported.");
        }

        /// <inheritdoc/>
        void IProjectExporter.Save(string path, IProjectContainer project)
        {
            throw new NotSupportedException("Saving projects as webp drawing is not supported.");
        }

        private void Save(string path, IPageContainer container)
        {
            var info = new SKImageInfo((int)container.Width, (int)container.Height);
            using (var bitmap = new SKBitmap(info))
            {
                using (var canvas = new SKCanvas(bitmap))
                {
                    _presenter.Render(canvas, _renderer, container, 0, 0);
                }
                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Webp, 100))
                using (var stream = File.OpenWrite(path))
                {
                    data.SaveTo(stream);
                }
            }
        }
    }
}
