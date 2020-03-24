using System;
using System.IO;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Interfaces;
using Core2D.Renderer;
using Core2D.Renderer.Presenters;
using Core2D.Renderer.SkiaSharp;

namespace Core2D.FileWriter.SkiaSharpWebp
{
    /// <summary>
    /// SkiaSharp webp <see cref="IFileWriter"/> implementation.
    /// </summary>
    public sealed class WebpSkiaSharpWriter : IFileWriter
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebpSkiaSharpWriter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public WebpSkiaSharpWriter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public string Name { get; } = "Webp (SkiaSharp)";

        /// <inheritdoc/>
        public string Extension { get; } = "webp";

        /// <inheritdoc/>
        public void Save(Stream stream, object item, object options)
        {
            if (item == null)
            {
                return;
            }

            var ic = options as IImageCache;
            if (options == null)
            {
                return;
            }

            var renderer = new SkiaSharpRenderer(_serviceProvider, true, 96.0);
            renderer.State.DrawShapeState.Flags = ShapeStateFlags.Printable;
            renderer.State.ImageCache = ic;

            var presenter = new ExportPresenter();

            IProjectExporter exporter = new WebpSkiaSharpExporter(renderer, presenter);

            if (item is IPageContainer page)
            {
                var dataFlow = _serviceProvider.GetService<IDataFlow>();
                var db = (object)page.Data.Properties;
                var record = (object)page.Data.Record;

                dataFlow.Bind(page.Template, db, record);
                dataFlow.Bind(page, db, record);

                exporter.Save(stream, page);
            }
            else if (item is IDocumentContainer document)
            {
                throw new NotSupportedException("Saving documents as webp drawing is not supported.");
            }
            else if (item is IProjectContainer project)
            {
                throw new NotSupportedException("Saving projects as webp drawing is not supported.");
            }
        }
    }
}
