using System;
using System.IO;
using System.Linq;
using System.Text;
using Core2D;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Renderer;
using Core2D.XamlExporter.Avalonia;

namespace Core2D.FileWriter.Xaml
{
    public sealed class DrawingGroupXamlWriter : IFileWriter
    {
        private readonly IServiceProvider _serviceProvider;

        public DrawingGroupXamlWriter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string Name { get; } = "Xaml (DrawingGroup)";

        public string Extension { get; } = "xaml";

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

            var exporter = new DrawingGroupXamlExporter(_serviceProvider);

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
                    var key = page?.Name;
                    var xaml = exporter.Create(shapes, key);
                    if (!string.IsNullOrEmpty(xaml))
                    {
                        byte[] bytes = Encoding.UTF8.GetBytes(xaml);
                        stream.Write(bytes, 0, bytes.Length);
                    }
                }
            }
            else if (item is DocumentContainer document)
            {
                throw new NotSupportedException("Saving documents as xaml drawing is not supported.");
            }
            else if (item is ProjectContainer project)
            {
                throw new NotSupportedException("Saving projects as xaml drawing is not supported.");
            }
        }
    }
}
