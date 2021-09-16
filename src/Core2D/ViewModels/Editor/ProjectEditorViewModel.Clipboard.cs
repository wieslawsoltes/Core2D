#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core2D.Model;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Editor
{
    public class ClipboardServiceViewModel : ViewModelBase, IClipboardService
    {
        public ClipboardServiceViewModel(IServiceProvider? serviceProvider) : base(serviceProvider)
        {
        }

        private IList<BaseShapeViewModel>? ShapesToCopy { get; set; }

        private PageContainerViewModel? PageToCopy { get; set; }

        private DocumentContainerViewModel? DocumentToCopy { get; set; }

        public override object Copy(IDictionary<object, object>? shared)
        {
            throw new NotImplementedException();
        }

        private void UpdateShapeNames(IEnumerable<BaseShapeViewModel>? shapes)
        {
            var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null || shapes is null)
            {
                return;
            }
            
            var all = project.GetAllShapes().ToList();
            var source = new List<BaseShapeViewModel>();

            foreach (var shape in shapes)
            {
                editor.SetShapeName(shape, all.Concat(source));
                source.Add(shape);
            }
        }

        private void Delete(object? item)
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null)
            {
                return;
            }

            switch (item)
            {
                case BaseShapeViewModel shape:
                {
                    project?.RemoveShape(shape);
                    ServiceProvider.GetService<ISelectionService>()?.OnDeselectAll();
                    break;
                }
                case LayerContainerViewModel layer:
                {
                    project?.RemoveLayer(layer);

                    var selected = project?.CurrentContainer?.Layers.FirstOrDefault();
                    if (layer.Owner is FrameContainerViewModel owner)
                    {
                        owner.SetCurrentLayer(selected);
                    }

                    break;
                }
                case PageContainerViewModel page:
                {
                    project?.RemovePage(page);

                    var selected = project?.CurrentDocument?.Pages.FirstOrDefault();
                    project?.SetCurrentContainer(selected);
                    break;
                }
                case DocumentContainerViewModel document:
                {
                    project?.RemoveDocument(document);

                    var selected = project?.Documents.FirstOrDefault();
                    project?.SetCurrentDocument(selected);
                    project?.SetCurrentContainer(selected?.Pages.FirstOrDefault());
                    break;
                }
                case ProjectEditorViewModel:
                case null:
                {
                    ServiceProvider.GetService<ISelectionService>()?.OnDeleteSelected();
                    break;
                }
            }
        }

        public IList<BaseShapeViewModel>? Copy(IList<BaseShapeViewModel> shapes)
        {
            if (shapes.Count > 0)
            {
                var shapesToCopy = new List<BaseShapeViewModel>(shapes.Count);
                var shared = new Dictionary<object, object>();

                foreach (var shape in shapes)
                {
                    var copy = shape.CopyShared(shared);
                    if (copy is { })
                    {
                        shapesToCopy.Add(copy);
                    }
                }

                return shapesToCopy;
            }

            return null;
        }

        public void OnCopyShapes(IList<BaseShapeViewModel> shapes)
        {
            try
            {
                var copy = Copy(shapes);
                if (copy is { })
                {
                    ShapesToCopy = copy;
                }
 
                // TODO:
                // var json = JsonSerializer?.Serialize(shapes);
                // if (!string.IsNullOrEmpty(json))
                // {
                //     TextClipboard?.SetText(json);
                // }
            }
            catch (Exception ex)
            {
                ServiceProvider.GetService<ILog>()?.LogException(ex);
            }
        }

        public void OnPasteShapes(IEnumerable<BaseShapeViewModel>? shapes)
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null )
            {
                return;
            }

            try
            {
                ServiceProvider.GetService<ISelectionService>()?.Deselect(project?.CurrentContainer?.CurrentLayer);
                // TODO:
                // TryToRestoreRecords(shapes);
                UpdateShapeNames(shapes);
                project.AddShapes(project?.CurrentContainer?.CurrentLayer, shapes);
                ServiceProvider.GetService<ISelectionService>()?.OnSelect(shapes);
            }
            catch (Exception ex)
            {
                ServiceProvider.GetService<ILog>()?.LogException(ex);
            }
        }

        public void OnTryPaste(string text)
        {
            try
            {
                if (!string.IsNullOrEmpty(text))
                {
                    
                    var pathShape = ServiceProvider.GetService<IPathConverter>()?.FromSvgPathData(text, isStroked: false, isFilled: true);
                    if (pathShape is { })
                    {
                        OnPasteShapes(Enumerable.Repeat<BaseShapeViewModel>(pathShape, 1));
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                ServiceProvider.GetService<ILog>()?.LogException(ex);
            }

            try
            {
                var shapes = ServiceProvider.GetService<IJsonSerializer>()?.Deserialize<IList<BaseShapeViewModel>?>(text);
                if (shapes?.Count > 0)
                {
                    OnPasteShapes(shapes);
                }
            }
            catch (Exception ex)
            {
                ServiceProvider.GetService<ILog>()?.LogException(ex);
            }
        }

        public bool CanCopy()
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null)
            {
                return false;
            }

            return project.SelectedShapes is { };
        }

        public async Task<bool> CanPaste()
        {
            try
            {
                // TODO:
                // return await (TextClipboard?.ContainsText() ?? Task.FromResult(false));
                return await Task.Run(() => ShapesToCopy is { } && ShapesToCopy.Count > 0);
            }
            catch (Exception ex)
            {
                ServiceProvider.GetService<ILog>()?.LogException(ex);
            }
            return false;
        }

        public void OnCut(object? item)
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null )
            {
                return;
            }

            switch (item)
            {
                case BaseShapeViewModel shape:
                {
                    // TODO:
                    break;
                }
                case PageContainerViewModel page:
                {
                    PageToCopy = page;
                    DocumentToCopy = default;
                    project?.RemovePage(page);
                    project?.SetCurrentContainer(project?.CurrentDocument?.Pages.FirstOrDefault());
                    break;
                }
                case DocumentContainerViewModel document:
                {
                    PageToCopy = default;
                    DocumentToCopy = document;
                    project?.RemoveDocument(document);

                    var selected = project?.Documents.FirstOrDefault();
                    project?.SetCurrentDocument(selected);
                    project?.SetCurrentContainer(selected?.Pages.FirstOrDefault());
                    break;
                }
                case ProjectEditorViewModel:
                case null:
                {
                    if (CanCopy())
                    {
                        OnCopy(item);
                        ServiceProvider.GetService<ISelectionService>()?.OnDeleteSelected();
                    }

                    break;
                }
            }
        }

        public void OnCopy(object? item)
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null )
            {
                return;
            }

            switch (item)
            {
                case BaseShapeViewModel shape:
                {
                    // TODO:
                    break;
                }
                case PageContainerViewModel page:
                {
                    PageToCopy = page;
                    DocumentToCopy = default;
                    break;
                }
                case DocumentContainerViewModel document:
                {
                    PageToCopy = default;
                    DocumentToCopy = document;
                    break;
                }
                case ProjectEditorViewModel:
                case null:
                {
                    if (CanCopy())
                    {
                        if (project.SelectedShapes is { })
                        {
                            OnCopyShapes(project.SelectedShapes.ToList());
                        }
                    }

                    break;
                }
            }
        }

        public async void OnPaste(object? item)
        {
            var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (project is null )
            {
                return;
            }

            switch (project)
            {
                case { } when item is BaseShapeViewModel:
                {
                    // TODO:
                    break;
                }
                case { } when item is PageContainerViewModel page:
                {
                    if (PageToCopy is { })
                    {
                        PastePageIntoPage(project, page);
                    }

                    break;
                }
                case { } when item is DocumentContainerViewModel document:
                {
                    if (PageToCopy is { })
                    {
                        PastePageIntoDocument(project, document);
                    }
                    else if (DocumentToCopy is { })
                    {
                        PasteDocumentIntoDocument(project, document);
                    }

                    break;
                }
                default:
                {
                    switch (item)
                    {
                        case ProjectEditorViewModel:
                        case null:
                        {
                            if (await CanPaste())
                            {
                                // TODO:
                                // var text = await (TextClipboard?.GetText() ?? Task.FromResult(string.Empty));
                                // if (!string.IsNullOrEmpty(text))
                                // {
                                //     OnTryPaste(text);
                                // }

                                if (ShapesToCopy is { })
                                {
                                    var copy = Copy(ShapesToCopy);
                                    if (copy is { })
                                    {
                                        OnPasteShapes(copy);
                                    }
                                }
                            }

                            break;
                        }
                        case string text:
                        {
                            if (!string.IsNullOrEmpty(text))
                            {
                                OnTryPaste(text);
                            }

                            break;
                        }
                    }

                    break;
                }
            }
        }

        private void PastePageIntoPage(ProjectContainerViewModel project, PageContainerViewModel page)
        {
            var document = project.Documents.FirstOrDefault(d => d.Pages.Contains(page));
            if (document is null)
            {
                return;
            }
            var index = document.Pages.IndexOf(page);
            var clone = PageToCopy?.CopyShared(new Dictionary<object, object>());
            project.ReplacePage(document, clone, index);
            project.SetCurrentContainer(clone);
        }

        private void PastePageIntoDocument(ProjectContainerViewModel project, DocumentContainerViewModel document)
        {
            var clone = PageToCopy?.CopyShared(new Dictionary<object, object>());
            if (clone is null)
            {
                return;
            }
            project.AddPage(document, clone);
            project.SetCurrentContainer(clone);
        }

        private void PasteDocumentIntoDocument(ProjectContainerViewModel project, DocumentContainerViewModel document)
        {
            var index = project.Documents.IndexOf(document);
            var clone = DocumentToCopy?.CopyShared(new Dictionary<object, object>());
            if (clone is null)
            {
                return;
            }
            project.ReplaceDocument(clone, index);
            project.SetCurrentDocument(clone);
            project.SetCurrentContainer(clone?.Pages.FirstOrDefault());
        }

        public void OnDelete(object? item)
        {
            if (item is IList<object> objects)
            {
                var copy = objects.ToList();

                foreach (var obj in copy)
                {
                    Delete(obj);
                }
            }
            else
            {
                Delete(item);
            }
        }
    }
}
