#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Shapes;

namespace Core2D.ViewModels.Editor
{
    public partial class ProjectEditorViewModel
    {
        private IList<BaseShapeViewModel>? ShapesToCopy { get; set; }

        private PageContainerViewModel? PageToCopy { get; set; }

        private DocumentContainerViewModel? DocumentToCopy { get; set; }

        private BaseShapeViewModel? HoveredShapeViewModel { get; set; }

        private void UpdateShapeNames(IEnumerable<BaseShapeViewModel>? shapes)
        {
            if (Project is null || shapes is null)
            {
                return;
            }
            
            var all = Project.GetAllShapes().ToList();
            var source = new List<BaseShapeViewModel>();

            foreach (var shape in shapes)
            {
                SetShapeName(shape, all.Concat(source));
                source.Add(shape);
            }
        }

        private IDictionary<string, RecordViewModel>? GenerateRecordDictionaryById()
        {
            return Project?.Databases
                .Where(d => d.Records.Length > 0)
                .SelectMany(d => d.Records)
                .ToDictionary(s => s.Id);
        }

        private void TryToRestoreRecords(IEnumerable<BaseShapeViewModel> shapes)
        {
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
                            Project?.SetCurrentDatabase(db);
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
                Log?.LogException(ex);
            }
        }

        private void RestoreShape(BaseShapeViewModel shape)
        {
            var shapes = Enumerable.Repeat(shape, 1).ToList();
            TryToRestoreRecords(shapes);
        }

        public T? CloneShape<T>(T shape) where T : BaseShapeViewModel
        {
            try
            {
                if (JsonSerializer is { } serializer)
                {
                    var json = serializer.Serialize(shape);
                    if (!string.IsNullOrEmpty(json))
                    {
                        var clone = serializer.Deserialize<T>(json);
                        if (clone is { })
                        {
                            RestoreShape(clone);
                            return clone;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }

            return default;
        }

        public LayerContainerViewModel? Clone(LayerContainerViewModel? container)
        {
            try
            {
                var json = JsonSerializer?.Serialize(container);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = JsonSerializer?.Deserialize<LayerContainerViewModel>(json);
                    if (clone is { })
                    {
                        var shapes = clone.Shapes;
                        TryToRestoreRecords(shapes);
                        return clone;
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }

            return default;
        }

        public PageContainerViewModel? Clone(PageContainerViewModel? container)
        {
            try
            {
                var template = container?.Template;
                var json = JsonSerializer?.Serialize(container);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = JsonSerializer?.Deserialize<PageContainerViewModel>(json);
                    if (clone is { })
                    {
                        var shapes = clone.Layers.SelectMany(l => l.Shapes);
                        TryToRestoreRecords(shapes);
                        clone.Template = template;
                        return clone;
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }

            return default;
        }

        public DocumentContainerViewModel? Clone(DocumentContainerViewModel? document)
        {
            try
            {
                var json = JsonSerializer?.Serialize(document);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = JsonSerializer?.Deserialize<DocumentContainerViewModel>(json);
                    if (clone is { })
                    {
                        for (int i = 0; i < clone.Pages.Length; i++)
                        {
                            var container = clone.Pages[i];
                            var shapes = container.Layers.SelectMany(l => l.Shapes);
                            TryToRestoreRecords(shapes);
                        }
                        return clone;
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }

            return default;
        }

        private void Delete(object? item)
        {
            if (item is BaseShapeViewModel shape)
            {
                Project?.RemoveShape(shape);
                OnDeselectAll();
            }
            else if (item is LayerContainerViewModel layer)
            {
                Project?.RemoveLayer(layer);

                var selected = Project?.CurrentContainer?.Layers.FirstOrDefault();
                if (layer.Owner is FrameContainerViewModel owner)
                {
                    owner.SetCurrentLayer(selected);
                }
            }
            else if (item is PageContainerViewModel page)
            {
                Project?.RemovePage(page);

                var selected = Project?.CurrentDocument?.Pages.FirstOrDefault();
                Project?.SetCurrentContainer(selected);
            }
            else if (item is DocumentContainerViewModel document)
            {
                Project?.RemoveDocument(document);

                var selected = Project?.Documents.FirstOrDefault();
                Project?.SetCurrentDocument(selected);
                Project?.SetCurrentContainer(selected?.Pages.FirstOrDefault());
            }
            else if (item is ProjectEditorViewModel || item is null)
            {
                OnDeleteSelected();
            }
        }

        public void OnCopyShapes(IList<BaseShapeViewModel> shapes)
        {
            try
            {
                if (shapes.Count > 0)
                {
                    ShapesToCopy = new List<BaseShapeViewModel>(shapes.Count);

                    var shared = new Dictionary<object, object>();
 
                    foreach (var shape in shapes)
                    {
                        var copy = shape.CopyShared(shared);
                        if (copy is { })
                        {
                            ShapesToCopy.Add(copy);
                        }
                    }
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
                Log?.LogException(ex);
            }
        }

        public void OnPasteShapes(IEnumerable<BaseShapeViewModel>? shapes)
        {
            try
            {
                Deselect(Project?.CurrentContainer?.CurrentLayer);
                // TODO:
                // TryToRestoreRecords(shapes);
                UpdateShapeNames(shapes);
                Project.AddShapes(Project?.CurrentContainer?.CurrentLayer, shapes);
                OnSelect(shapes);
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnTryPaste(string text)
        {
            try
            {
                if (!string.IsNullOrEmpty(text))
                {
                    var pathShape = PathConverter?.FromSvgPathData(text, isStroked: false, isFilled: true);
                    if (pathShape is { })
                    {
                        OnPasteShapes(Enumerable.Repeat<BaseShapeViewModel>(pathShape, 1));
                        return;
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }

            try
            {
                var shapes = JsonSerializer?.Deserialize<IList<BaseShapeViewModel>?>(text);
                if (shapes?.Count > 0)
                {
                    OnPasteShapes(shapes);
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public bool CanCopy()
        {
            return Project?.SelectedShapes is { };
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
                Log?.LogException(ex);
            }
            return false;
        }

        public void OnCut(object? item)
        {
            if (item is BaseShapeViewModel shape)
            {
                // TODO:
            }
            else if (item is PageContainerViewModel page)
            {
                PageToCopy = page;
                DocumentToCopy = default;
                Project?.RemovePage(page);
                Project?.SetCurrentContainer(Project?.CurrentDocument?.Pages.FirstOrDefault());
            }
            else if (item is DocumentContainerViewModel document)
            {
                PageToCopy = default;
                DocumentToCopy = document;
                Project?.RemoveDocument(document);

                var selected = Project?.Documents.FirstOrDefault();
                Project?.SetCurrentDocument(selected);
                Project?.SetCurrentContainer(selected?.Pages.FirstOrDefault());
            }
            else if (item is ProjectEditorViewModel || item is null)
            {
                if (CanCopy())
                {
                    OnCopy(item);
                    OnDeleteSelected();
                }
            }
        }

        public void OnCopy(object? item)
        {
            if (item is BaseShapeViewModel shape)
            {
                // TODO:
            }
            else if (item is PageContainerViewModel page)
            {
                PageToCopy = page;
                DocumentToCopy = default;
            }
            else if (item is DocumentContainerViewModel document)
            {
                PageToCopy = default;
                DocumentToCopy = document;
            }
            else if (item is ProjectEditorViewModel || item is null)
            {
                if (CanCopy())
                {
                    if (Project?.SelectedShapes is { })
                    {
                        OnCopyShapes(Project.SelectedShapes.ToList());
                    }
                }
            }
        }

        public async void OnPaste(object? item)
        {
            if (Project is { } && item is BaseShapeViewModel shape)
            {
                // TODO:
            }
            else if (Project is { } && item is PageContainerViewModel page)
            {
                if (PageToCopy is { })
                {
                    var document = Project?.Documents.FirstOrDefault(d => d.Pages.Contains(page));
                    if (document is { })
                    {
                        int index = document.Pages.IndexOf(page);
                        var clone = Clone(PageToCopy);
                        Project.ReplacePage(document, clone, index);
                        Project?.SetCurrentContainer(clone);
                    }
                }
            }
            else if (Project is { } && item is DocumentContainerViewModel document)
            {
                if (PageToCopy is { })
                {
                    var clone = Clone(PageToCopy);
                    Project?.AddPage(document, clone);
                    Project?.SetCurrentContainer(clone);
                }
                else if (DocumentToCopy is { })
                {
                    int index = Project.Documents.IndexOf(document);
                    var clone = Clone(DocumentToCopy);
                    Project.ReplaceDocument(clone, index);
                    Project.SetCurrentDocument(clone);
                    Project.SetCurrentContainer(clone?.Pages.FirstOrDefault());
                }
            }
            else if (item is ProjectEditorViewModel || item is null)
            {
                if (await CanPaste())
                {
                    // TODO:
                    // var text = await (TextClipboard?.GetText() ?? Task.FromResult(string.Empty));
                    // if (!string.IsNullOrEmpty(text))
                    // {
                    //     OnTryPaste(text);
                    // }
                    
                    if (ShapesToCopy is { } && ShapesToCopy.Count > 0)
                    {
                        var shapesToPaste = new List<BaseShapeViewModel>(ShapesToCopy.Count);
                        var shared = new Dictionary<object, object>();
 
                        foreach (var s in ShapesToCopy)
                        {
                            var copy = s.CopyShared(shared);
                            if (copy is { })
                            {
                                shapesToPaste.Add(copy);
                            }
                        }
                        
                        OnPasteShapes(shapesToPaste);
                    }
                }
            }
            else if (item is string text)
            {
                if (!string.IsNullOrEmpty(text))
                {
                    OnTryPaste(text);
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
