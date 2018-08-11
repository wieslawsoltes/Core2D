// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Containers.Interfaces;
using Core2D.Interfaces;
using Core2D.Renderer;
using Core2D.Renderer.Dxf;
using Core2D.Shapes.Interfaces;

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

            if (item is IPageContainer page)
            {
                exporter.Save(path, page);
            }
            else if (item is IDocumentContainer document)
            {
                exporter.Save(path, document);
            }
            else if (item is IProjectContainer project)
            {
                exporter.Save(path, project);
            }
        }
    }
}
