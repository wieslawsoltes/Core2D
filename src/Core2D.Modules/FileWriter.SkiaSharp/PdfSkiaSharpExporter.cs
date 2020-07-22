using System.IO;
using Core2D;
using Core2D.Containers;
using Core2D.Renderer;
using SkiaSharp;

namespace Core2D.FileWriter.SkiaSharpPdf
{
    /// <summary>
    /// SkiaSharp pdf <see cref="IProjectExporter"/> implementation.
    /// </summary>
    public sealed class PdfSkiaSharpExporter : IProjectExporter
    {
        private readonly IShapeRenderer _renderer;
        private readonly IContainerPresenter _presenter;
        private readonly float _targetDpi;

        /// <summary>
        /// Initializes a new instance of the <see cref="PdfSkiaSharpExporter"/> class.
        /// </summary>
        /// <param name="renderer">The shape renderer.</param>
        /// <param name="presenter">The container presenter.</param>
        /// <param name="targetDpi">The target renderer dpi.</param>
        public PdfSkiaSharpExporter(IShapeRenderer renderer, IContainerPresenter presenter, float targetDpi = 72.0f)
        {
            _renderer = renderer;
            _presenter = presenter;
            _targetDpi = targetDpi;
        }

        private void Add(SKDocument pdf, IPageContainer container)
        {
            using var canvas = pdf.BeginPage((float)container.Template.Width, (float)container.Template.Height);
            _presenter.Render(canvas, _renderer, container, 0, 0);
        }

        /// <inheritdoc/>
        public void Save(Stream stream, IPageContainer container)
        {
            using var pdf = SKDocument.CreatePdf(stream, _targetDpi);
            Add(pdf, container);
            pdf.Close();
        }

        /// <inheritdoc/>
        public void Save(Stream stream, IDocumentContainer document)
        {
            using var pdf = SKDocument.CreatePdf(stream, _targetDpi);
            foreach (var container in document.Pages)
            {
                Add(pdf, container);
            }
            pdf.Close();
            _renderer.ClearCache();
        }

        /// <inheritdoc/>
        public void Save(Stream stream, IProjectContainer project)
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
