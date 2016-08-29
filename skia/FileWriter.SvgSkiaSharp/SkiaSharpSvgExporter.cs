// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Renderer.Presenters;
using SkiaSharp;

namespace FileWriter.SvgSkiaSharp
{
    /// <summary>
    /// SkiaSharp svg <see cref="IProjectExporter"/> implementation.
    /// </summary>
    public class SvgExporter : IProjectExporter
    {
        /// <inheritdoc/>
        void IProjectExporter.Save(string path, XContainer container, ShapeRenderer renderer)
        {
            Save(path, container, renderer);
        }

        /// <inheritdoc/>
        void IProjectExporter.Save(string path, XDocument document, ShapeRenderer renderer)
        {
        }

        /// <inheritdoc/>
        void IProjectExporter.Save(string path, XProject project, ShapeRenderer renderer)
        {
        }

        void Save(string path, XContainer container, ShapeRenderer renderer)
        {
            using (SKFileWStream stream = new SKFileWStream(path))
            using (var writer = new SKXmlStreamWriter(stream))
            using (var canvas = SKSvgCanvas.Create(SKRect.Create(0, 0, (int)container.Width, (int)container.Height), writer))
            {
                var presenter = new ContainerPresenter();
                presenter.Render(canvas, renderer, container, 0, 0);
            }
        }
    }
}
