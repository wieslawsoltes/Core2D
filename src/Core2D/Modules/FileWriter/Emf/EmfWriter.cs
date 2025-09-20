// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.Modules.Renderer.WinForms;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Renderer.Presenters;
using Core2D.ViewModels.Shapes;

namespace Core2D.Modules.FileWriter.Emf;

public sealed class EmfWriter : IFileWriter, IMetafileExporter
{
    private readonly IServiceProvider? _serviceProvider;

    public EmfWriter(IServiceProvider? serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public string Name { get; } = "Emf (WinForms)";

    public string Extension { get; } = "emf";

    public MemoryStream? MakeMetafileStream(object bitmap, IEnumerable<BaseShapeViewModel> shapes, IImageCache ic)
    {
        var g = default(Graphics);
        var mf = default(Metafile);
        var ms = new MemoryStream();

        if (bitmap is not Bitmap image)
        {
            return null;
        }

        try
        {
            using (g = Graphics.FromImage(image))
            {
                var hdc = g.GetHdc();
                mf = new Metafile(ms, hdc);
                g.ReleaseHdc(hdc);
            }

            using (g = Graphics.FromImage(mf))
            {
                var r = new WinFormsRenderer(_serviceProvider, 72.0 / 96.0);
                if (r.State is { })
                {
                    r.State.DrawShapeState = ShapeStateFlags.Printable;
                    r.State.ImageCache = ic;
                }

                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.PageUnit = GraphicsUnit.Display;

                foreach (var shape in shapes)
                {
                    shape.DrawShape(g, r, null);
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

    public MemoryStream? MakeMetafileStream(object bitmap, FrameContainerViewModel container, IImageCache? ic)
    {
        var g = default(Graphics);
        var mf = default(Metafile);
        var ms = new MemoryStream();

        if (bitmap is not Bitmap image)
        {
            return null;
        }

        try
        {
            using (g = Graphics.FromImage(image))
            {
                var hdc = g.GetHdc();
                mf = new Metafile(ms, hdc);
                g.ReleaseHdc(hdc);
            }

            using (g = Graphics.FromImage(mf))
            {
                var p = new ExportPresenter();
                var r = new WinFormsRenderer(_serviceProvider, 72.0 / 96.0);
                if (r.State is { })
                {
                    r.State.DrawShapeState = ShapeStateFlags.Printable;
                    r.State.ImageCache = ic;
                }

                g.SmoothingMode = SmoothingMode.HighQuality;
                g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                g.CompositingQuality = CompositingQuality.HighQuality;
                g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                g.PageUnit = GraphicsUnit.Display;

                if (container is PageContainerViewModel page)
                {
                    p.Render(g, r, null, page.Template, 0, 0);
                }
                p.Render(g, r, null, container, 0, 0);

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

    public void Save(Stream stream, PageContainerViewModel container, IImageCache? ic)
    {
        if (container.Template is { })
        {
            var width = (int)container.Template.Width;
            var height = (int)container.Template.Height;
            using var bitmap = new Bitmap(width, height);
            using var ms = MakeMetafileStream(bitmap, container, ic);
            if (ms is { })
            {
                ms.WriteTo(stream);
            }
        }
    }

    public void Save(Stream stream, object? item, object? options)
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

        if (item is PageContainerViewModel page)
        {
            var dataFlow = _serviceProvider.GetService<DataFlow>();
            var db = (object)page.Properties;
            var record = (object?)page.Record;

            if (dataFlow is { })
            {
                dataFlow.Bind(page.Template, db, record);
                dataFlow.Bind(page, db, record);
            }

            Save(stream, page, ic);
        }
        else if (item is DocumentContainerViewModel _)
        {
            throw new NotSupportedException("Saving documents as emf drawing is not supported.");
        }
        else if (item is ProjectContainerViewModel _)
        {
            throw new NotSupportedException("Saving projects as emf drawing is not supported.");
        }
    }
}
