using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using Core2D;
using Core2D.Containers;
using Core2D.Data;
using Core2D.Renderer;
using Core2D.Renderer.WinForms;
using Core2D.Shapes;

namespace Core2D.FileWriter.Emf
{
    public sealed class EmfWriter : IFileWriter
    {
        private readonly IServiceProvider _serviceProvider;

        public EmfWriter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string Name { get; } = "Emf (WinForms)";

        public string Extension { get; } = "emf";

        public MemoryStream MakeMetafileStream(Bitmap bitmap, IEnumerable<BaseShape> shapes, IImageCache ic)
        {
            var g = default(Graphics);
            var mf = default(Metafile);
            var ms = new MemoryStream();

            try
            {
                using (g = Graphics.FromImage(bitmap))
                {
                    var hdc = g.GetHdc();
                    mf = new Metafile(ms, hdc);
                    g.ReleaseHdc(hdc);
                }

                using (g = Graphics.FromImage(mf))
                {
                    var r = new WinFormsRenderer(_serviceProvider, 72.0 / 96.0);
                    r.State.DrawShapeState.Flags = ShapeStateFlags.Printable;
                    r.State.ImageCache = ic;

                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    g.PageUnit = GraphicsUnit.Display;

                    foreach (var shape in shapes)
                    {
                        shape.DrawShape(g, r);
                    }

                    r.ClearCache();
                }
            }
            finally
            {
                g?.Dispose();

                mf?.Dispose();
            }
            return ms;
        }

        public MemoryStream MakeMetafileStream(Bitmap bitmap, PageContainer container, IImageCache ic)
        {
            var g = default(Graphics);
            var mf = default(Metafile);
            var ms = new MemoryStream();

            try
            {
                using (g = Graphics.FromImage(bitmap))
                {
                    var hdc = g.GetHdc();
                    mf = new Metafile(ms, hdc);
                    g.ReleaseHdc(hdc);
                }

                using (g = Graphics.FromImage(mf))
                {
                    var r = new WinFormsRenderer(_serviceProvider, 72.0 / 96.0);
                    r.State.DrawShapeState.Flags = ShapeStateFlags.Printable;
                    r.State.ImageCache = ic;

                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    g.PageUnit = GraphicsUnit.Display;

                    r.DrawPage(g, container.Template);
                    r.DrawPage(g, container);

                    r.ClearCache();
                }
            }
            finally
            {
                g?.Dispose();

                mf?.Dispose();
            }
            return ms;
        }

        public void Save(Stream stream, PageContainer container, IImageCache ic)
        {
            if (container != null && container.Template != null)
            {
                using var bitmap = new Bitmap((int)container.Template.Width, (int)container.Template.Height);
                using var ms = MakeMetafileStream(bitmap, container, ic);
                ms.WriteTo(stream);
            }
        }

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

            if (item is PageContainer page)
            {
                var dataFlow = _serviceProvider.GetService<DataFlow>();
                var db = (object)page.Properties;
                var record = (object)page.Record;

                dataFlow.Bind(page.Template, db, record);
                dataFlow.Bind(page, db, record);

                Save(stream, page, ic);
            }
            else if (item is DocumentContainer document)
            {
                throw new NotSupportedException("Saving documents as emf drawing is not supported.");
            }
            else if (item is ProjectContainer project)
            {
                throw new NotSupportedException("Saving projects as emf drawing is not supported.");
            }
        }
    }
}
