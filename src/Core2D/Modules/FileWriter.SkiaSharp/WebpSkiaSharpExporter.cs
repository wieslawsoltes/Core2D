using System;
using System.IO;
using Core2D;
using Core2D.Containers;
using Core2D.Renderer;
using SkiaSharp;

namespace Core2D.FileWriter.SkiaSharpWebp
{
    public sealed class WebpSkiaSharpExporter : IProjectExporter
    {
        private readonly IShapeRenderer _renderer;
        private readonly IContainerPresenter _presenter;

        public WebpSkiaSharpExporter(IShapeRenderer renderer, IContainerPresenter presenter)
        {
            _renderer = renderer;
            _presenter = presenter;
        }

        public void Save(Stream stream, PageContainer container)
        {
            var info = new SKImageInfo((int)container.Template.Width, (int)container.Template.Height, SKImageInfo.PlatformColorType, SKAlphaType.Unpremul);
            using var bitmap = new SKBitmap(info);
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear();
                _presenter.Render(canvas, _renderer, container, 0, 0);
            }
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Webp, 100);
            data.SaveTo(stream);
        }

        public void Save(Stream stream, DocumentContainer document)
        {
            throw new NotSupportedException("Saving documents as webp drawing is not supported.");
        }

        public void Save(Stream stream, ProjectContainer project)
        {
            throw new NotSupportedException("Saving projects as webp drawing is not supported.");
        }
    }
}
