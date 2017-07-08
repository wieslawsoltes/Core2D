// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Renderer;
using SkiaSharp;

namespace Core2D.FileWriter.SkiaSharpIco
{
    /// <summary>
    /// SkiaSharp ico <see cref="IProjectExporter"/> implementation.
    /// </summary>
    public sealed class IcoSkiaSharpExporter : IProjectExporter
    {
        private readonly ShapeRenderer _renderer;
        private readonly ContainerPresenter _presenter;

        /// <summary>
        /// Initializes a new instance of the <see cref="IcoSkiaSharpExporter"/> class.
        /// </summary>
        /// <param name="renderer">The shape renderer.</param>
        /// <param name="presenter">The container presenter.</param>
        public IcoSkiaSharpExporter(ShapeRenderer renderer, ContainerPresenter presenter)
        {
            _renderer = renderer;
            _presenter = presenter;
        }

        /// <inheritdoc/>
        void IProjectExporter.Save(string path, PageContainer container)
        {
            Save(path, container);
        }

        /// <inheritdoc/>
        void IProjectExporter.Save(string path, DocumentContainer document)
        {
            throw new NotSupportedException("Saving documents as ico drawing is not supported.");
        }

        /// <inheritdoc/>
        void IProjectExporter.Save(string path, ProjectContainer project)
        {
            throw new NotSupportedException("Saving projects as ico drawing is not supported.");
        }

        void Save(string path, PageContainer container)
        {
            var info = new SKImageInfo((int)container.Width, (int)container.Height);
            using (var bitmap = new SKBitmap(info))
            {
                using (var canvas = new SKCanvas(bitmap))
                {
                    _presenter.Render(canvas, _renderer, container, 0, 0);
                }
                using (var image = SKImage.FromBitmap(bitmap))
                using (var data = image.Encode(SKEncodedImageFormat.Ico, 100))
                using (var stream = File.OpenWrite(path))
                {
                    data.SaveTo(stream);
                }
            }
        }
    }
}
