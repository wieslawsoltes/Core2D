using System;
using System.IO;
using Core2D;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Renderer;
using Core2D.Renderer.Presenters;
using Core2D.Renderer.SkiaSharp;

namespace Core2D.FileWriter.SkiaSharpPng
{
    public sealed class PngSkiaSharpWriter : IFileWriter
    {
        private readonly IServiceProvider _serviceProvider;

        public PngSkiaSharpWriter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string Name { get; } = "Png (SkiaSharp)";

        public string Extension { get; } = "png";

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
            renderer.StateViewModel.DrawShapeState = ShapeStateFlags.Printable;
            renderer.StateViewModel.ImageCache = ic;

            var presenter = new ExportPresenter();

            IProjectExporter exporter = new PngSkiaSharpExporter(renderer, presenter);

            if (item is PageContainerViewModel page)
            {
                var dataFlow = _serviceProvider.GetService<DataFlow>();
                var db = (object)page.Properties;
                var record = (object)page.RecordViewModel;

                dataFlow.Bind(page.Template, db, record);
                dataFlow.Bind(page, db, record);

                exporter.Save(stream, page);
            }
            else if (item is DocumentContainerViewModel document)
            {
                throw new NotSupportedException("Saving documents as png drawing is not supported.");
            }
            else if (item is ProjectContainerViewModel project)
            {
                throw new NotSupportedException("Saving projects as png drawing is not supported.");
            }
        }
    }
}
