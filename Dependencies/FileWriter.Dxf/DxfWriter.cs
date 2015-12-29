// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D;

namespace Dependencies
{
    /// <summary>
    /// netDxf file writer.
    /// </summary>
    public class DxfWriter : IFileWriter
    {
        /// <inheritdoc/>
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

            if (item is Page)
            {
                r.Save(path, item as Page);
            }
        }
    }
}
