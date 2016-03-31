// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Shape;
using Renderer.Dxf;

namespace FileWriter.Dxf
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

            if (item is XPage)
            {
                r.Save(path, item as XPage);
            }
            else if (item is XDocument)
            {
                r.Save(path, item as XDocument);
            }
            else if (item is XProject)
            {
                r.Save(path, item as XProject);
            }
        }
    }
}
