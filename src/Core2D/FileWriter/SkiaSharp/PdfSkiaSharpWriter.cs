using System;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Interfaces;
using Core2D.Renderer;
using Core2D.Renderer.Presenters;
using Core2D.Renderer.SkiaSharp;

namespace Core2D.FileWriter.SkiaSharpPdf
{
    /// <summary>
    /// SkiaSharp pdf <see cref="IFileWriter"/> implementation.
    /// </summary>
    public sealed class PdfSkiaSharpWriter : IFileWriter
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSkiaSharpWriter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public PdfSkiaSharpWriter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        string IFileWriter.Name { get; } = "Pdf (SkiaSharp)";

        /// <inheritdoc/>
        string IFileWriter.Extension { get; } = "pdf";

        /// <inheritdoc/>
        void IFileWriter.Save(string path, object item, object options)
        {
            if (string.IsNullOrEmpty(path) || item == null)
            {
                return;
            }

            var ic = options as IImageCache;
            if (options == null)
            {
                return;
            }

            IShapeRenderer renderer = new SkiaSharpRenderer(_serviceProvider, true, 72.0);
            renderer.State.DrawShapeState.Flags = ShapeStateFlags.Printable;
            renderer.State.ImageCache = ic;

            var presenter = new ExportPresenter();

            IProjectExporter exporter = new PdfSkiaSharpExporter(renderer, presenter, 72.0f);

            if (item is IPageContainer page)
            {
                var dataFlow = _serviceProvider.GetService<IDataFlow>();
                var db = (object)page.Data.Properties;
                var record = (object)page.Data.Record;

                dataFlow.Bind(page.Template, db, record);
                dataFlow.Bind(page, db, record);

                exporter.Save(path, page);
            }
            else if (item is IDocumentContainer document)
            {
                var dataFlow = _serviceProvider.GetService<IDataFlow>();

                dataFlow.Bind(document);

                exporter.Save(path, document);
            }
            else if (item is IProjectContainer project)
            {
                var dataFlow = _serviceProvider.GetService<IDataFlow>();

                dataFlow.Bind(project);

                exporter.Save(path, project);
            }
        }
    }
}
