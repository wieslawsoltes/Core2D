using System;
using System.IO;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.Modules.Renderer.Dxf;
using Core2D.ViewModels.Containers;

namespace Core2D.Modules.FileWriter.Dxf
{
    public sealed class DxfWriter : IFileWriter
    {
        private readonly IServiceProvider _serviceProvider;

        public DxfWriter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string Name { get; } = "Dxf (netDxf)";

        public string Extension { get; } = "dxf";

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

            IProjectExporter exporter = new DxfRenderer(_serviceProvider);

            IShapeRenderer renderer = (IShapeRenderer)exporter;
            renderer.State.DrawShapeState = ShapeStateFlags.Printable;
            renderer.State.ImageCache = ic;

            if (item is PageContainerViewModel page)
            {
                exporter.Save(stream, page);
            }
            else if (item is DocumentContainerViewModel document)
            {
                exporter.Save(stream, document);
            }
            else if (item is ProjectContainerViewModel project)
            {
                exporter.Save(stream, project);
            }
        }
    }
}
