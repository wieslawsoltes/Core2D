using System;
using System.IO;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using SkiaSharp;

namespace Core2D.Modules.FileWriter.SkiaSharpSkp
{
    public sealed class SkpSkiaSharpExporter : IProjectExporter
    {
        private readonly IShapeRenderer _renderer;
        private readonly IContainerPresenter _presenter;

        public SkpSkiaSharpExporter(IShapeRenderer renderer, IContainerPresenter presenter)
        {
            _renderer = renderer;
            _presenter = presenter;
        }

        public void Save(Stream stream, PageContainerViewModel container)
        {
            using var pictureRecorder = new SKPictureRecorder();
            using var canvas = pictureRecorder.BeginRecording(SKRect.Create(0, 0, (int)container.Template.Width, (int)container.Template.Height));
            _presenter.Render(canvas, _renderer, null, container, 0, 0);
            using var picture = pictureRecorder.EndRecording();
            picture.Serialize(stream);
        }

        public void Save(Stream stream, DocumentContainerViewModel document)
        {
            throw new NotSupportedException("Saving documents as skp drawing is not supported.");
        }

        public void Save(Stream stream, ProjectContainerViewModel project)
        {
            throw new NotSupportedException("Saving projects as skp drawing is not supported.");
        }
    }
}
