#nullable disable
using System.IO;
using Core2D.Model;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using SkiaSharp;

namespace Core2D.Modules.FileWriter.SkiaSharpPdf
{
    public sealed class PdfSkiaSharpExporter : IProjectExporter
    {
        private readonly IShapeRenderer _renderer;
        private readonly IContainerPresenter _presenter;
        private readonly float _targetDpi;

        public PdfSkiaSharpExporter(IShapeRenderer renderer, IContainerPresenter presenter, float targetDpi = 72.0f)
        {
            _renderer = renderer;
            _presenter = presenter;
            _targetDpi = targetDpi;
        }

        private void Add(SKDocument pdf, PageContainerViewModel container)
        {
            using var canvas = pdf.BeginPage((float)container.Template.Width, (float)container.Template.Height);
            _presenter.Render(canvas, _renderer, null, container, 0, 0);
        }

        public void Save(Stream stream, PageContainerViewModel container)
        {
            using var pdf = SKDocument.CreatePdf(stream, _targetDpi);
            Add(pdf, container);
            pdf.Close();
        }

        public void Save(Stream stream, DocumentContainerViewModel document)
        {
            using var pdf = SKDocument.CreatePdf(stream, _targetDpi);
            foreach (var container in document.Pages)
            {
                Add(pdf, container);
            }
            pdf.Close();
            _renderer.ClearCache();
        }

        public void Save(Stream stream, ProjectContainerViewModel project)
        {
            using var pdf = SKDocument.CreatePdf(stream, _targetDpi);
            foreach (var document in project.Documents)
            {
                foreach (var container in document.Pages)
                {
                    Add(pdf, container);
                }
            }
            pdf.Close();
            _renderer.ClearCache();
        }
    }
}
