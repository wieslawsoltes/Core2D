using System;
using System.IO;
using Core2D;
using Core2D.Containers;
using Core2D.Data;
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
        public string Name { get; } = "Svg (SkiaSharp)";

        /// <inheritdoc/>
        public string Extension { get; } = "svg";

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

            var renderer = new SkiaSharpRenderer(_serviceProvider);
            renderer.State.DrawShapeState.Flags = ShapeStateFlags.Printable;
            renderer.State.ImageCache = ic;

            var presenter = new ExportPresenter();

            IProjectExporter exporter = new SvgSkiaSharpExporter(renderer, presenter);

            if (item is PageContainer page)
            {
                var dataFlow = _serviceProvider.GetService<DataFlow>();
                var db = (object)page.Data.Properties;
                var record = (object)page.Data.Record;

                dataFlow.Bind(page.Template, db, record);
                dataFlow.Bind(page, db, record);

                exporter.Save(stream, page);
            }
            else if (item is DocumentContainer document)
            {
                throw new NotSupportedException("Saving documents as svg drawing is not supported.");
            }
            else if (item is ProjectContainer project)
            {
                throw new NotSupportedException("Saving projects as svg drawing is not supported.");
            }
        }
    }
}
