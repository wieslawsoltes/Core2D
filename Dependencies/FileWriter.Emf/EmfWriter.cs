// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Test2d;
using WPF = System.Windows;

namespace Test2d
{
    /// <summary>
    /// 
    /// </summary>
    public class EmfWriter : IFileWriter
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="shapes"></param>
        /// <param name="properties"></param>
        /// <returns></returns>
        public MemoryStream MakeMetafileStream(Bitmap bitmap, IEnumerable<BaseShape> shapes, ImmutableArray<ShapeProperty> properties)
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
                    var r = new EmfRenderer(72.0 / 96.0);

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
                            shape.Draw(g, r, 0, 0, properties, null);
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
        /// <returns></returns>
        public MemoryStream MakeMetafileStream(Bitmap bitmap, Container container)
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
                    var r = new EmfRenderer(72.0 / 96.0);

                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    g.PageUnit = GraphicsUnit.Display;

                    if (container.Template != null)
                    {
                        r.Draw(g, container.Template, container.Properties, null);
                    }

                    r.Draw(g, container, container.Properties, null);
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
        public void SetClipboard(IEnumerable<BaseShape> shapes, double width, double height, ImmutableArray<ShapeProperty> properties)
        {
            try
            {
                using (var bitmap = new Bitmap((int)width, (int)height))
                {
                    using (var ms = MakeMetafileStream(bitmap, shapes, properties))
                    {
                        var data = new WPF.DataObject();
                        data.SetData(WPF.DataFormats.EnhancedMetafile, ms);
                        WPF.Clipboard.SetDataObject(data, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public void SetClipboard(Container container)
        {
            try
            {
                using (var bitmap = new Bitmap((int)container.Width, (int)container.Height))
                {
                    using (var ms = MakeMetafileStream(bitmap, container))
                    {
                        var data = new WPF.DataObject();
                        data.SetData(WPF.DataFormats.EnhancedMetafile, ms);
                        WPF.Clipboard.SetDataObject(data, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.Print(ex.Message);
                Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="container"></param>
        public void Save(string path, Container container)
        {
            using (var bitmap = new Bitmap((int)container.Width, (int)container.Height))
            {
                using (var ms = MakeMetafileStream(bitmap, container))
                {
                    using (var fs = File.Create(path))
                    {
                        ms.WriteTo(fs);
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="item"></param>
        /// <param name="options"></param>
        public void Save(string path, object item, object options)
        {
            if (string.IsNullOrEmpty(path) || item == null)
                return;

            if (item is Container)
            {
                this.Save(path, item as Container);
            }
        }
    }
}
