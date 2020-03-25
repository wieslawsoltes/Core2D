using System;
using System.IO;
using Core2D.Containers;
using Core2D.Interfaces;
using Core2D.Renderer;
using SkiaSharp;

namespace Core2D.FileWriter.SkiaSharpJpeg
{
    /// <summary>
    /// SkiaSharp jpeg <see cref="IProjectExporter"/> implementation.
    /// </summary>
    public sealed class JpegSkiaSharpExporter : IProjectExporter
    {
        private readonly IShapeRenderer _renderer;
        private readonly IContainerPresenter _presenter;

        /// <summary>
        /// Initializes a new instance of the <see cref="JpegSkiaSharpExporter"/> class.
        /// </summary>
        /// <param name="renderer">The shape renderer.</param>
        /// <param name="presenter">The container presenter.</param>
        public JpegSkiaSharpExporter(IShapeRenderer renderer, IContainerPresenter presenter)
        {
            _renderer = renderer;
            _presenter = presenter;
        }

        /// <inheritdoc/>
        public void Save(Stream stream, IPageContainer container)
        {
            var info = new SKImageInfo((int)container.Width, (int)container.Height, SKImageInfo.PlatformColorType, SKAlphaType.Unpremul);
            using var bitmap = new SKBitmap(info);
            using (var canvas = new SKCanvas(bitmap))
            {
                _presenter.Render(canvas, _renderer, container, 0, 0);
            }
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Jpeg, 100);
            data.SaveTo(stream);
        }

        /// <inheritdoc/>
        public void Save(Stream stream, IDocumentContainer document)
        {
            throw new NotSupportedException("Saving documents as jpeg drawing is not supported.");
        }

        /// <inheritdoc/>
        public void Save(Stream stream, IProjectContainer project)
        {
            throw new NotSupportedException("Saving projects as jpeg drawing is not supported.");
        }
    }
}
