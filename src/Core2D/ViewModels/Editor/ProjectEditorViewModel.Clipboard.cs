#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core2D.Model;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Editor
{
    public interface IClipboardService
    {
        void OnCopyShapes(IList<BaseShapeViewModel> shapes);
        void OnPasteShapes(IEnumerable<BaseShapeViewModel>? shapes);
        void OnTryPaste(string text);
        bool CanCopy();
        Task<bool> CanPaste();
        void OnCut(object? item);
        void OnCopy(object? item);
        void OnPaste(object? item);
        void OnDelete(object? item);
    }

    public class ClipboardServiceViewModel : ViewModelBase, IClipboardService
    {
        public ClipboardServiceViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
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
            var Editor = ServiceProvider.GetService<ProjectEditorViewModel>();
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null || shapes is null)
            {
                return;
            }
            
            var all = Project.GetAllShapes().ToList();
            var source = new List<BaseShapeViewModel>();

            foreach (var shape in shapes)
            {
                Editor.SetShapeName(shape, all.Concat(source));
                source.Add(shape);
            }
        }

        private IDictionary<string, RecordViewModel>? GenerateRecordDictionaryById()
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return default;
            }
            
            return Project.Databases
                .Where(d => d.Records.Length > 0)
                .SelectMany(d => d.Records)
                .ToDictionary(s => s.Id);
        }

        private void TryToRestoreRecords(IEnumerable<BaseShapeViewModel> shapes)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            var ViewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
            
            try
            {
                if (Project?.Databases is null)
                {
                    return;
                }

                var records = GenerateRecordDictionaryById();
                if (records is null)
                {
                    return;
                }

                // Try to restore shape record.
                foreach (var shape in shapes.GetAllShapes())
                {
                    if (shape.Record is null)
                    {
                        continue;
                    }

                    if (records.TryGetValue(shape.Record.Id, out var record))
                    {
                        // Use existing record.
                        shape.Record = record;
                    }
                    else
                    {
                        // Create Imported database.
                        if (Project?.CurrentDatabase is null && shape.Record.Owner is DatabaseViewModel owner)
                        {
                            var db = ViewModelFactory?.CreateDatabase(
                                ProjectEditorConfiguration.ImportedDatabaseName,
                                owner.Columns);
                            Project.AddDatabase(db);
                            Project.SetCurrentDatabase(db);
                        }

                        // Add missing data record.
                        shape.Record.Owner = Project?.CurrentDatabase;
                        Project?.AddRecord(Project?.CurrentDatabase, shape.Record);

                        // Recreate records dictionary.
                        records = GenerateRecordDictionaryById();
                        if (records is null)
                        {
                            return;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                ServiceProvider.GetService<ILog>()?.LogException(ex);
            }
        }

        private void Delete(object? item)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return;
            }

            switch (item)
            {
                case BaseShapeViewModel shape:
                {
                    Project?.RemoveShape(shape);
                    ServiceProvider.GetService<ISelectionService>()?.OnDeselectAll();
                    break;
                }
                case LayerContainerViewModel layer:
                {
                    Project?.RemoveLayer(layer);

                    var selected = Project?.CurrentContainer?.Layers.FirstOrDefault();
                    if (layer.Owner is FrameContainerViewModel owner)
                    {
                        owner.SetCurrentLayer(selected);
                    }

                    break;
                }
                case PageContainerViewModel page:
                {
                    Project?.RemovePage(page);

                    var selected = Project?.CurrentDocument?.Pages.FirstOrDefault();
                    Project?.SetCurrentContainer(selected);
                    break;
                }
                case DocumentContainerViewModel document:
                {
                    Project?.RemoveDocument(document);

                    var selected = Project?.Documents.FirstOrDefault();
                    Project?.SetCurrentDocument(selected);
                    Project?.SetCurrentContainer(selected?.Pages.FirstOrDefault());
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

        private List<BaseShapeViewModel>? Copy(IList<BaseShapeViewModel> shapes)
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
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null )
            {
                return;
            }

            try
            {
                ServiceProvider.GetService<ISelectionService>()?.Deselect(Project?.CurrentContainer?.CurrentLayer);
                // TODO:
                // TryToRestoreRecords(shapes);
                UpdateShapeNames(shapes);
                Project.AddShapes(Project?.CurrentContainer?.CurrentLayer, shapes);
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
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null)
            {
                return false;
            }

            return Project.SelectedShapes is { };
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
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null )
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
                    Project?.RemovePage(page);
                    Project?.SetCurrentContainer(Project?.CurrentDocument?.Pages.FirstOrDefault());
                    break;
                }
                case DocumentContainerViewModel document:
                {
                    PageToCopy = default;
                    DocumentToCopy = document;
                    Project?.RemoveDocument(document);

                    var selected = Project?.Documents.FirstOrDefault();
                    Project?.SetCurrentDocument(selected);
                    Project?.SetCurrentContainer(selected?.Pages.FirstOrDefault());
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
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null )
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
                        if (Project?.SelectedShapes is { })
                        {
                            OnCopyShapes(Project.SelectedShapes.ToList());
                        }
                    }

                    break;
                }
            }
        }

        public async void OnPaste(object? item)
        {
            var Project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
            if (Project is null )
            {
                return;
            }

            switch (Project)
            {
                case { } when item is BaseShapeViewModel shape:
                {
                    // TODO:
                    break;
                }
                case { } when item is PageContainerViewModel page:
                {
                    if (PageToCopy is { })
                    {
                        var document = Project?.Documents.FirstOrDefault(d => d.Pages.Contains(page));
                        if (document is { })
                        {
                            int index = document.Pages.IndexOf(page);
                            var clone = PageToCopy?.CopyShared(new Dictionary<object, object>());
                            Project.ReplacePage(document, clone, index);
                            Project?.SetCurrentContainer(clone);
                        }
                    }

                    break;
                }
                case { } when item is DocumentContainerViewModel document:
                {
                    if (PageToCopy is { })
                    {
                        var clone = PageToCopy?.CopyShared(new Dictionary<object, object>());
                        Project?.AddPage(document, clone);
                        Project?.SetCurrentContainer(clone);
                    }
                    else if (DocumentToCopy is { })
                    {
                        int index = Project.Documents.IndexOf(document);
                        var clone = DocumentToCopy?.CopyShared(new Dictionary<object, object>());
                        Project.ReplaceDocument(clone, index);
                        Project.SetCurrentDocument(clone);
                        Project.SetCurrentContainer(clone?.Pages.FirstOrDefault());
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
