using System;
using System.IO;
using System.Linq;
using System.Text;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.SvgExporter.Svg;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;

namespace Core2D.FileWriter.Svg
{
    public sealed class SvgSvgWriter : IFileWriter
    {
        private readonly IServiceProvider _serviceProvider;

        public SvgSvgWriter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string Name { get; } = "Svg (Svg)";

        public string Extension { get; } = "svg";

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

            if (item is PageContainerViewModel page)
            {
                var dataFlow = _serviceProvider.GetService<DataFlow>();
                var db = (object)page.Properties;
                var record = (object)page.Record;

                dataFlow.Bind(page.Template, db, record);
                dataFlow.Bind(page, db, record);

                var shapes = page.Layers.SelectMany(x => x.Shapes);
                if (shapes != null)
                {
                    var xaml = exporter.Create(shapes, page.Template.Width, page.Template.Height);
                    if (!string.IsNullOrEmpty(xaml))
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(xaml);
                        stream.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            else if (item is DocumentContainerViewModel document)
            {
                throw new NotSupportedException("Saving documents as svg drawing is not supported.");
            }
            else if (item is ProjectContainerViewModel project)
            {
                throw new NotSupportedException("Saving projects as svg drawing is not supported.");
            }
        }
    }
}
