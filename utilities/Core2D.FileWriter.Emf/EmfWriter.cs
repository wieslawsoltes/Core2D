// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Shape;
using Core2D.Renderer.WinForms;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using WPF = System.Windows;

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="shapes"></param>
        /// <param name="properties"></param>
        /// <param name="record"></param>
        /// <param name="ic"></param>
        /// <returns></returns>
        public static MemoryStream MakeMetafileStream(Bitmap bitmap, IEnumerable<BaseShape> shapes, ImmutableArray<XProperty> properties, XRecord record, IImageCache ic)
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
        public static MemoryStream MakeMetafileStream(Bitmap bitmap, XContainer container, IImageCache ic)
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

                    r.Draw(g, container.Template, 0.0, 0.0, container.Data.Properties, container.Data.Record);
                    r.Draw(g, container, 0.0, 0.0, container.Data.Properties, container.Data.Record);

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
        public static void SetClipboard(IEnumerable<BaseShape> shapes, double width, double height, ImmutableArray<XProperty> properties, XRecord record, IImageCache ic)
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
        public static void SetClipboard(XContainer container, IImageCache ic)
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
        public static void Save(string path, XContainer container, IImageCache ic)
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

        /// <inheritdoc/>
        void IFileWriter.Save(string path, object item, object options)
        {
            if (string.IsNullOrEmpty(path) || item == null)
                return;

            var ic = options as IImageCache;
            if (options == null)
                return;

            if (item is XContainer)
            {
                Save(path, item as XContainer, ic);
            }
            else if (item is XDocument)
            {
                throw new NotSupportedException("Saving documents as emf drawing is not supported.");
            }
            else if (item is XProject)
            {
                throw new NotSupportedException("Saving projects as emf drawing is not supported.");
            }
        }
    }
}
