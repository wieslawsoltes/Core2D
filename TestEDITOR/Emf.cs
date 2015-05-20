// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
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

namespace TestEMF
{
    /// <summary>
    /// 
    /// </summary>
    public static class Emf
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="container"></param>
        public static void PutOnClipboard(Container container)
        {
            Bitmap bitmap = default(Bitmap);
            MemoryStream ms = default(MemoryStream);

            try
            {
                bitmap = new Bitmap((int)container.Width, (int)container.Height);
                ms = MakeMetafileStream(bitmap, container);

                var data = new WPF.DataObject();
                data.SetData(WPF.DataFormats.EnhancedMetafile, ms);
                WPF.Clipboard.SetDataObject(data, true);
            }
            finally
            {
                if (bitmap != null)
                {
                    bitmap.Dispose();
                }

                if (ms != null)
                {
                    ms.Dispose();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="container"></param>
        public static void Save(string path, Container container)
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
        /// <param name="bitmap"></param>
        /// <param name="container"></param>
        /// <returns></returns>
        private static MemoryStream MakeMetafileStream(Bitmap bitmap, Container container)
        {
            Graphics g = default(Graphics);
            Metafile mf = default(Metafile);
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
                    var r = EmfRenderer.Create();

                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    g.TextRenderingHint = TextRenderingHint.ClearTypeGridFit;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                    g.PageUnit = GraphicsUnit.Point;
    
                    if (container.Template != null)
                    {
                        r.Draw(g, container.Template, container.Properties);
                    }

                    r.Draw(g, container, container.Properties);
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
    }
}
