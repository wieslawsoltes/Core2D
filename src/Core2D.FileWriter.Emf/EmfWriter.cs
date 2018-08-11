// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Containers.Interfaces;
using Core2D.Data;
using Core2D.Interfaces;
using Core2D.Renderer;
using Core2D.Renderer.WinForms;
using Core2D.Shapes.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;

#if _WINDOWS
using WPF = System.Windows;
#endif

namespace Core2D.FileWriter.Emf
{
    /// <summary>
    /// WinForms file writer.
    /// </summary>
    public sealed class EmfWriter : IFileWriter
    {
        /// <inheritdoc/>
        string IFileWriter.Name { get; } = "Emf (WinForms)";

        /// <inheritdoc/>
        string IFileWriter.Extension { get; } = "emf";

#if _WINDOWS
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="shapes"></param>
        /// <param name="properties"></param>
        /// <param name="record"></param>
        /// <param name="ic"></param>
        /// <returns></returns>
        public static MemoryStream MakeMetafileStream(Bitmap bitmap, IEnumerable<IBaseShape> shapes, ImmutableArray<Property> properties, Record record, IImageCache ic)
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
                    var r = new WinFormsRenderer(72.0 / 96.0);
                    r.State.DrawShapeState.Flags = ShapeStateFlags.Printable;
                    r.State.ImageCache = ic;

                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    g.PageUnit = GraphicsUnit.Display;

                    if (shapes != null)
                    {
                        foreach (var shape in shapes)
                        {
                            shape.Draw(g, r, 0, 0, properties, record);
                        }
                    }

                    r.ClearCache(isZooming: false);
                }
            }
            finally
            {
                if (g != null)
                {
                    g.Dispose();
                }

                if (mf != null)
                {
                    mf.Dispose();
                }
            }
            return ms;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="container"></param>
        /// <param name="ic"></param>
        /// <returns></returns>
        public static MemoryStream MakeMetafileStream(Bitmap bitmap, IPageContainer container, IImageCache ic)
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
                    var r = new WinFormsRenderer(72.0 / 96.0);
                    r.State.DrawShapeState.Flags = ShapeStateFlags.Printable;
                    r.State.ImageCache = ic;

                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    g.PageUnit = GraphicsUnit.Display;

                    r.Draw(g, container.Template, 0.0, 0.0, (object)container.Data.Properties, (object)container.Data.Record);
                    r.Draw(g, container, 0.0, 0.0, (object)container.Data.Properties, (object)container.Data.Record);

                    r.ClearCache(isZooming: false);
                }
            }
            finally
            {
                if (g != null)
                {
                    g.Dispose();
                }

                if (mf != null)
                {
                    mf.Dispose();
                }
            }
            return ms;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="shapes"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="properties"></param>
        /// <param name="record"></param>
        /// <param name="ic"></param>
        public static void SetClipboard(IEnumerable<IBaseShape> shapes, double width, double height, ImmutableArray<Property> properties, Record record, IImageCache ic)
        {
            try
            {
                using (var bitmap = new Bitmap((int)width, (int)height))
                {
                    using (var ms = MakeMetafileStream(bitmap, shapes, properties, record, ic))
                    {
                        var data = new WPF.DataObject();
                        data.SetData(WPF.DataFormats.EnhancedMetafile, ms);
                        WPF.Clipboard.SetDataObject(data, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        /// <param name="ic"></param>
        public static void SetClipboard(IPageContainer container, IImageCache ic)
        {
            try
            {
                if (container == null || container.Template == null)
                    return;
                
                using (var bitmap = new Bitmap((int)container.Template.Width, (int)container.Template.Height))
                {
                    using (var ms = MakeMetafileStream(bitmap, container, ic))
                    {
                        var data = new WPF.DataObject();
                        data.SetData(WPF.DataFormats.EnhancedMetafile, ms);
                        WPF.Clipboard.SetDataObject(data, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Debug.WriteLine(ex.StackTrace);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="container"></param>
        /// <param name="ic"></param>
        public static void Save(string path, IPageContainer container, IImageCache ic)
        {
            if (container == null || container.Template == null)
                return;
            
            using (var bitmap = new Bitmap((int)container.Template.Width, (int)container.Template.Height))
            {
                using (var ms = MakeMetafileStream(bitmap, container, ic))
                {
                    using (var fs = File.Create(path))
                    {
                        ms.WriteTo(fs);
                    }
                }
            }
        }
#endif
        /// <inheritdoc/>
        void IFileWriter.Save(string path, object item, object options)
        {
            if (string.IsNullOrEmpty(path) || item == null)
                return;

            var ic = options as IImageCache;
            if (options == null)
                return;

            if (item is IPageContainer page)
            {
#if _WINDOWS
                Save(path, page, ic);
#else
                throw new NotImplementedException("Not implemented for this platform.");
#endif
            }
            else if (item is IDocumentContainer document)
            {
                throw new NotSupportedException("Saving documents as emf drawing is not supported.");
            }
            else if (item is IProjectContainer project)
            {
                throw new NotSupportedException("Saving projects as emf drawing is not supported.");
            }
        }
    }
}
