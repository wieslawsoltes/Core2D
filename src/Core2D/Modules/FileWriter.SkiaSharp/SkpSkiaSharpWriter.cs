#nullable disable
using System;
using System.IO;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.Modules.Renderer.SkiaSharp;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Renderer.Presenters;

namespace Core2D.Modules.FileWriter.SkiaSharpSkp
{
    public sealed class SkpSkiaSharpWriter : IFileWriter
    {
        private readonly IServiceProvider _serviceProvider;

        public SkpSkiaSharpWriter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string Name { get; } = "Skp (SkiaSharp)";

        public string Extension { get; } = "skp";

        public void Save(Stream stream, object item, object options)
        {
            if (item is null)
            {
                return;
            }

            var ic = options as IImageCache;
            if (options is null)
            {
                return;
            }

            var renderer = new SkiaSharpRendererViewModel(_serviceProvider);
            renderer.State.DrawShapeState = ShapeStateFlags.Printable;
            renderer.State.ImageCache = ic;

            var presenter = new ExportPresenter();

            IProjectExporter exporter = new SkpSkiaSharpExporter(renderer, presenter);

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
                throw new NotSupportedException("Saving documents as skp drawing is not supported.");
            }
            else if (item is ProjectContainerViewModel project)
            {
                throw new NotSupportedException("Saving projects as skp drawing is not supported.");
            }
        }
    }
}
