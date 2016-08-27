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

            if (item is XContainer)
            {
                exporter.Save(path, item as XContainer, renderer);
            }
            else if (item is XDocument)
            {
                exporter.Save(path, item as XDocument, renderer);
            }
            else if (item is XProject)
            {
                exporter.Save(path, item as XProject, renderer);
            }
        }
    }
}
