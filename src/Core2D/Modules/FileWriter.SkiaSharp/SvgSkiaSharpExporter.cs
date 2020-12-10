using System;
using System.IO;
using Core2D;
using Core2D.Containers;
using Core2D.Renderer;
using SkiaSharp;

namespace Core2D.FileWriter.SkiaSharpSvg
{
    public sealed class SvgSkiaSharpExporter : IProjectExporter
    {
        private readonly IShapeRenderer _renderer;
        private readonly IContainerPresenter _presenter;

        public SvgSkiaSharpExporter(IShapeRenderer renderer, IContainerPresenter presenter)
        {
            _renderer = renderer;
            _presenter = presenter;
        }

        public void Save(Stream stream, PageContainerViewModel container)
        {
            using var wstream = new SKManagedWStream(stream);
            using var canvas = SKSvgCanvas.Create(SKRect.Create(0, 0, (int)container.Template.Width, (int)container.Template.Height), stream);
            _presenter.Render(canvas, _renderer, container, 0, 0);
        }

        public void Save(Stream stream, DocumentContainerViewModel document)
        {
            throw new NotSupportedException("Saving documents as svg drawing is not supported.");
        }

        public void Save(Stream stream, ProjectContainerViewModel project)
        {
            throw new NotSupportedException("Saving projects as svg drawing is not supported.");
        }
    }
}
