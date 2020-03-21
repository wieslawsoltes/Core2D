using System;
using Core2D.Containers;
using Core2D.Interfaces;
using Core2D.Renderer;
using SkiaSharp;

namespace Core2D.FileWriter.SkiaSharpSvg
{
    /// <summary>
    /// SkiaSharp svg <see cref="IProjectExporter"/> implementation.
    /// </summary>
    public sealed class SvgSkiaSharpExporter : IProjectExporter
    {
        private readonly IShapeRenderer _renderer;
        private readonly IContainerPresenter _presenter;

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgSkiaSharpExporter"/> class.
        /// </summary>
        /// <param name="renderer">The shape renderer.</param>
        /// <param name="presenter">The container presenter.</param>
        public SvgSkiaSharpExporter(IShapeRenderer renderer, IContainerPresenter presenter)
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
            throw new NotSupportedException("Saving documents as svg drawing is not supported.");
        }

        /// <inheritdoc/>
        void IProjectExporter.Save(string path, IProjectContainer project)
        {
            throw new NotSupportedException("Saving projects as svg drawing is not supported.");
        }

        private void Save(string path, IPageContainer container)
        {
            using var stream = new SKFileWStream(path);
            using var writer = new SKXmlStreamWriter(stream);
            using var canvas = SKSvgCanvas.Create(SKRect.Create(0, 0, (int)container.Width, (int)container.Height), writer);
            _presenter.Render(canvas, _renderer, container, 0, 0);
        }
    }
}
