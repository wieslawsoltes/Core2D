// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Interfaces;
using Core2D.Renderer;
using Core2D.Renderer.Dxf;

namespace Core2D.FileWriter.Dxf
{
    /// <summary>
    /// netDxf file writer.
    /// </summary>
    public sealed class DxfWriter : IFileWriter
    {
        /// <inheritdoc/>
        string IFileWriter.Name { get; } = "Dxf (netDxf)";

        /// <inheritdoc/>
        string IFileWriter.Extension { get; } = "dxf";

        /// <inheritdoc/>
        void IFileWriter.Save(string path, object item, object options)
        {
            if (string.IsNullOrEmpty(path) || item == null)
                return;

            var ic = options as IImageCache;
            if (options == null)
                return;

            IProjectExporter exporter = new DxfRenderer();

            ShapeRenderer renderer = (DxfRenderer)exporter;
            renderer.State.DrawShapeState.Flags = ShapeStateFlags.Printable;
            renderer.State.ImageCache = ic;

            if (item is PageContainer)
            {
                exporter.Save(path, item as PageContainer);
            }
            else if (item is DocumentContainer)
            {
                exporter.Save(path, item as DocumentContainer);
            }
            else if (item is ProjectContainer)
            {
                exporter.Save(path, item as ProjectContainer);
            }
        }
    }
}
