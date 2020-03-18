using System;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Interfaces;
using Core2D.Renderer;
using Core2D.Renderer.Presenters;
using Core2D.Renderer.SkiaSharp;

namespace Core2D.FileWriter.SkiaSharpSvg
{
    /// <summary>
    /// SkiaSharp svg <see cref="IFileWriter"/> implementation.
    /// </summary>
    public sealed class SvgSkiaSharpWriter : IFileWriter
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgSkiaSharpWriter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public SvgSkiaSharpWriter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        string IFileWriter.Name { get; } = "Svg (SkiaSharp)";

        /// <inheritdoc/>
        string IFileWriter.Extension { get; } = "svg";

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

            var renderer = new SkiaSharpRenderer(_serviceProvider, true, 96.0);
            renderer.State.DrawShapeState.Flags = ShapeStateFlags.Printable;
            renderer.State.ImageCache = ic;

            var presenter = new ExportPresenter();

            IProjectExporter exporter = new SvgSkiaSharpExporter(renderer, presenter);

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
                throw new NotSupportedException("Saving documents as svg drawing is not supported.");
            }
            else if (item is IProjectContainer project)
            {
                throw new NotSupportedException("Saving projects as svg drawing is not supported.");
            }
        }
    }
}
