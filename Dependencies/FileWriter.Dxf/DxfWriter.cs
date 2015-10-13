// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using netDxf;
using Core2D;

namespace Core2D
{
    /// <summary>
    /// 
    /// </summary>
    public class DxfWriter : IFileWriter
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

            var r = new DxfRenderer();
            r.State.DrawShapeState.Flags = ShapeStateFlags.Printable;
            r.State.ImageCache = ic;

            if (item is Container)
            {
                r.Save(path, item as Container);
            }
        }
    }
}
