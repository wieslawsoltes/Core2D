using System;
using System.IO;
using Core2D;
using Core2D.Containers;
using Core2D.Renderer;
using Core2D.Renderer.Dxf;

namespace Core2D.FileWriter.Dxf
{
    /// <summary>
    /// netDxf file writer.
    /// </summary>
    public sealed class DxfWriter : IFileWriter
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="DxfWriter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public DxfWriter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public string Name { get; } = "Dxf (netDxf)";

        /// <inheritdoc/>
        public string Extension { get; } = "dxf";

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

            IProjectExporter exporter = new DxfRenderer(_serviceProvider);

            IShapeRenderer renderer = (IShapeRenderer)exporter;
            renderer.State.DrawShapeState.Flags = ShapeStateFlags.Printable;
            renderer.State.ImageCache = ic;

            if (item is PageContainer page)
            {
                exporter.Save(stream, page);
            }
            else if (item is DocumentContainer document)
            {
                exporter.Save(stream, document);
            }
            else if (item is ProjectContainer project)
            {
                exporter.Save(stream, project);
            }
        }
    }
}
