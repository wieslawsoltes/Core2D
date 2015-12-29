﻿// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D;

namespace Dependencies
{
    /// <summary>
    /// 
    /// </summary>
    public class PdfWriter : IFileWriter
    {
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

            var ic = options as IImageCache;
            if (options == null)
                return;

            var r = new PdfRenderer();
            r.State.DrawShapeState.Flags = ShapeStateFlags.Printable;
            r.State.ImageCache = ic;

            if (item is Page)
            {
                r.Save(path, item as Page);
            }
            else if (item is Document)
            {
                r.Save(path, item as Document);
            }
            else if (item is Project)
            {
                r.Save(path, item as Project);
            }
        }
    }
}
