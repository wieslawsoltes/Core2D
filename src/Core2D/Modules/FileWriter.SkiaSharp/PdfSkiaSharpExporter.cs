using System.IO;
using Core2D;
using Core2D.Containers;
using Core2D.Renderer;
using SkiaSharp;

namespace Core2D.FileWriter.SkiaSharpPdf
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

        private void Add(SKDocument pdf, PageContainerViewModel containerViewModel)
        {
            using var canvas = pdf.BeginPage((float)containerViewModel.Template.Width, (float)containerViewModel.Template.Height);
            _presenter.Render(canvas, _renderer, containerViewModel, 0, 0);
        }

        public void Save(Stream stream, PageContainerViewModel containerViewModel)
        {
            using var pdf = SKDocument.CreatePdf(stream, _targetDpi);
            Add(pdf, containerViewModel);
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
