using System;
using System.IO;
using System.Linq;
using System.Text;
using Core2D;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Renderer;
using Core2D.SvgExporter.Svg;

namespace Core2D.FileWriter.Svg
{
    /// <summary>
    /// Svg file writer.
    /// </summary>
    public sealed class SvgSvgWriter : IFileWriter
    {
        private readonly IServiceProvider _serviceProvider;

        /// <summary>
        /// Initializes a new instance of the <see cref="SvgSvgWriter"/> class.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public SvgSvgWriter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <inheritdoc/>
        public string Name { get; } = "Svg (Svg)";

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

            var exporter = new SvgSvgExporter(_serviceProvider);

            if (item is PageContainer page)
            {
                var dataFlow = _serviceProvider.GetService<DataFlow>();
                var db = (object)page.Data.Properties;
                var record = (object)page.Data.Record;

                dataFlow.Bind(page.Template, db, record);
                dataFlow.Bind(page, db, record);

                var shapes = page.Layers.SelectMany(x => x.Shapes);
                if (shapes != null)
                {
                    var xaml = exporter.Create(shapes, page.Width, page.Height);
                    if (!string.IsNullOrEmpty(xaml))
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(xaml);
                        stream.Write(bytes, 0, bytes.Length);
                    }
                }
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
