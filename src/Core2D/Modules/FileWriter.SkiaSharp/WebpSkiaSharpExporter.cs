#nullable disable
using System;
using System.IO;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using SkiaSharp;

namespace Core2D.Modules.FileWriter.SkiaSharpWebp
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

        public void Save(Stream stream, PageContainerViewModel container)
        {
            var info = new SKImageInfo((int)container.Template.Width, (int)container.Template.Height, SKImageInfo.PlatformColorType, SKAlphaType.Unpremul);
            using var bitmap = new SKBitmap(info);
            using (var canvas = new SKCanvas(bitmap))
            {
                canvas.Clear();
                _presenter.Render(canvas, _renderer, null, container, 0, 0);
            }
            using var image = SKImage.FromBitmap(bitmap);
            using var data = image.Encode(SKEncodedImageFormat.Webp, 100);
            data.SaveTo(stream);
        }

        public void Save(Stream stream, DocumentContainerViewModel document)
        {
            throw new NotSupportedException("Saving documents as webp drawing is not supported.");
        }

        public void Save(Stream stream, ProjectContainerViewModel project)
        {
            throw new NotSupportedException("Saving projects as webp drawing is not supported.");
        }
    }
}
