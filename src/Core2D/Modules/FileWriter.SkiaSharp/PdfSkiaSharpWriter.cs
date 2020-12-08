using System;
using System.IO;
using Core2D;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Renderer;
using Core2D.Renderer.Presenters;
using Core2D.Renderer.SkiaSharp;

namespace Core2D.FileWriter.SkiaSharpPdf
{
    public sealed class PdfSkiaSharpWriter : IFileWriter
    {
        private readonly IServiceProvider _serviceProvider;

        public PdfSkiaSharpWriter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string Name { get; } = "Pdf (SkiaSharp)";

        public string Extension { get; } = "pdf";

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

            IShapeRenderer renderer = new SkiaSharpRenderer(_serviceProvider);
            renderer.State.DrawShapeState = ShapeStateFlags.Printable;
            renderer.State.ImageCache = ic;

            var presenter = new ExportPresenter();

            IProjectExporter exporter = new PdfSkiaSharpExporter(renderer, presenter, 72.0f);

            if (item is PageContainer page)
            {
                var dataFlow = _serviceProvider.GetService<DataFlow>();
                var db = (object)page.Properties;
                var record = (object)page.Record;

                dataFlow.Bind(page.Template, db, record);
                dataFlow.Bind(page, db, record);

                exporter.Save(stream, page);
            }
            else if (item is DocumentContainer document)
            {
                var dataFlow = _serviceProvider.GetService<DataFlow>();

                dataFlow.Bind(document);

                exporter.Save(stream, document);
            }
            else if (item is ProjectContainer project)
            {
                var dataFlow = _serviceProvider.GetService<DataFlow>();

                dataFlow.Bind(project);

                exporter.Save(stream, project);
            }
        }
    }
}
