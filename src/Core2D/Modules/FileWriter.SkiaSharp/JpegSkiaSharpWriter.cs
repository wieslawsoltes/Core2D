using System;
using System.IO;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.Renderer.SkiaSharp;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Renderer.Presenters;

namespace Core2D.FileWriter.SkiaSharpJpeg
{
    public sealed class JpegSkiaSharpWriter : IFileWriter
    {
        private readonly IServiceProvider _serviceProvider;

        public JpegSkiaSharpWriter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string Name { get; } = "Jpeg (SkiaSharp)";

        public string Extension { get; } = "jpg";

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

            var renderer = new SkiaSharpRendererViewModel(_serviceProvider);
            renderer.State.DrawShapeState = ShapeStateFlags.Printable;
            renderer.State.ImageCache = ic;

            var presenter = new ExportPresenter();

            IProjectExporter exporter = new JpegSkiaSharpExporter(renderer, presenter);

            if (item is PageContainerViewModel page)
            {
                var dataFlow = _serviceProvider.GetService<DataFlow>();
                var db = (object)page.Properties;
                var record = (object)page.Record;

                dataFlow.Bind(page.Template, db, record);
                dataFlow.Bind(page, db, record);

                exporter.Save(stream, page);
            }
            else if (item is DocumentContainerViewModel document)
            {
                throw new NotSupportedException("Saving documents as jpeg drawing is not supported.");
            }
            else if (item is ProjectContainerViewModel project)
            {
                throw new NotSupportedException("Saving projects as jpeg drawing is not supported.");
            }
        }
    }
}
