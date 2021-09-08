#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Core2D.Model;
using Core2D.Model.Input;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor.History;
using Core2D.ViewModels.Editor.Recent;
using Core2D.ViewModels.Layout;
using Core2D.ViewModels.Scripting;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using Core2D.Spatial;

namespace Core2D.ViewModels.Editor
{
    public partial class ProjectEditorViewModel
    {
        public (decimal sx, decimal sy) TryToSnap(InputArgs args)
        {
            if (Project is { } && Project.Options.SnapToGrid == true)
            {
                return (
                    PointUtil.Snap((decimal)args.X, (decimal)Project.Options.SnapX),
                    PointUtil.Snap((decimal)args.Y, (decimal)Project.Options.SnapY));
            }
            else
            {
                return ((decimal)args.X, (decimal)args.Y);
            }
        }

        public string GetName(object? item)
        {
            if (item is ViewModelBase observable)
            {
                return observable.Name;
            }
            return string.Empty;
        }

        public void SetShapeName(BaseShapeViewModel shape, IEnumerable<BaseShapeViewModel>? source = null)
        {
            var input = source ?? _project.GetAllShapes();
            var shapes = input.Where(s => s.GetType() == shape.GetType() && s != shape).ToList();
            var count = shapes.Count + 1;
            var update = string.IsNullOrEmpty(shape.Name) || input.Any(x => x != shape && x.Name == shape.Name);
            if (update)
            {
                shape.Name = shape.GetType().Name.Replace("ShapeViewModel", " ") + count;
            }
        }

        public void OnNew(object? item)
        {
            switch (item)
            {
                case LayerContainerViewModel layer:
                {
                    if (layer.Owner is PageContainerViewModel page)
                    {
                        OnAddLayer(page);
                    }

                    break;
                }
                case PageContainerViewModel page:
                {
                    OnNewPage(page);
                    break;
                }
                case DocumentContainerViewModel document:
                {
                    OnNewPage(document);
                    break;
                }
                case ProjectContainerViewModel:
                {
                    OnNewDocument();
                    break;
                }
                case ProjectEditorViewModel:
                case null when Project is null:
                {
                    OnNewProject();
                    break;
                }
                case null when Project.CurrentDocument is null:
                {
                    OnNewDocument();
                    break;
                }
                case null:
                {
                    OnNewPage(Project.CurrentDocument);
                    break;
                }
            }
        }

        public void OnNewPage(PageContainerViewModel? selected)
        {
            var document = Project?.Documents.FirstOrDefault(d => d.Pages.Contains(selected));
            if (document is { })
            {
                var page =
                    ContainerFactory?.GetPage(Project, ProjectEditorConfiguration.DefaultPageName)
                    ?? ViewModelFactory.CreatePageContainer(ProjectEditorConfiguration.DefaultPageName);

                Project?.AddPage(document, page);
                Project?.SetCurrentContainer(page);
            }
        }

        public void OnNewPage(DocumentContainerViewModel? selected)
        {
            var page =
                ContainerFactory?.GetPage(Project, ProjectEditorConfiguration.DefaultPageName)
                ?? ViewModelFactory.CreatePageContainer(ProjectEditorConfiguration.DefaultPageName);

            Project?.AddPage(selected, page);
            Project?.SetCurrentContainer(page);
        }

        public void OnNewDocument()
        {
            var document =
                ContainerFactory?.GetDocument(Project, ProjectEditorConfiguration.DefaultDocumentName)
                ?? ViewModelFactory.CreateDocumentContainer(ProjectEditorConfiguration.DefaultDocumentName);

            Project?.AddDocument(document);
            Project?.SetCurrentDocument(document);
            Project?.SetCurrentContainer(document?.Pages.FirstOrDefault());
        }

        public void OnNewProject()
        {
            OnUnload();
            OnLoad(ContainerFactory?.GetProject() ?? ViewModelFactory.CreateProjectContainer(), string.Empty);
            CanvasPlatform?.ResetZoom?.Invoke();
            CanvasPlatform?.InvalidateControl?.Invoke();
            NavigateTo("Home");
        }

        public void OnOpenProject(string path)
        {
            try
            {
                if (FileSystem is { } && JsonSerializer is { })
                {
                    if (!string.IsNullOrEmpty(path) && FileSystem.Exists(path))
                    {
                        var project = ViewModelFactory.OpenProjectContainer(path, FileSystem, JsonSerializer);
                        if (project is { })
                        {
                            OnOpenProjectImpl(project, path);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        private void OnOpenProjectImpl(ProjectContainerViewModel? project, string path)
        {
            if (project is null)
            {
                return;
            }
            try
            {
                OnUnload();
                OnLoad(project, path);
                OnAddRecent(path, project.Name);
                CanvasPlatform?.ResetZoom?.Invoke();
                CanvasPlatform?.InvalidateControl?.Invoke();
                NavigateTo("Home");
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnCloseProject()
        {
            Project?.History?.Reset();
            OnUnload();
            NavigateTo("Dashboard");
        }

        public void OnSaveProject(string path)
        {
            if (Project is null || FileSystem is null || JsonSerializer is null)
            {
                return;
            }
            
            try
            {
                var isDecoratorVisible = PageState?.Decorator?.IsVisible == true;
                if (isDecoratorVisible)
                {
                    OnHideDecorator();
                }

                ViewModelFactory?.SaveProjectContainer(Project, path, FileSystem, JsonSerializer);
                OnAddRecent(path, Project.Name);

                if (string.IsNullOrEmpty(ProjectPath))
                {
                    ProjectPath = path;
                }

                IsProjectDirty = false;

                if (isDecoratorVisible)
                {
                    OnShowDecorator();
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnImportData(ProjectContainerViewModel? project, string path, ITextFieldReader<DatabaseViewModel>? reader)
        {
            if (project is null)
            {
                return;
            }
            
            try
            {
                using var stream = FileSystem?.Open(path);
                if (stream is null)
                {
                    return;
                }
                var db = reader?.Read(stream);
                if (db is { })
                {
                    project.AddDatabase(db);
                    project.SetCurrentDatabase(db);
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnExportData(string path, DatabaseViewModel? database, ITextFieldWriter<DatabaseViewModel>? writer)
        {
            try
            {
                using var stream = FileSystem?.Create(path);
                if (stream is null)
                {
                    return;
                }
                writer?.Write(stream, database);
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnUpdateData(string path, DatabaseViewModel database, ITextFieldReader<DatabaseViewModel> reader)
        {
            try
            {
                using var stream = FileSystem?.Open(path);
                if (stream is null)
                {
                    return;
                }
                var db = reader?.Read(stream);
                if (db is { })
                {
                    Project?.UpdateDatabase(database, db);
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnImportObject(object item, bool restore)
        {
            if (Project is null)
            {
                return;
            }

            if (item is ShapeStyleViewModel style)
            {
                Project?.AddStyle(Project?.CurrentStyleLibrary, style);
            }
            else if (item is IList<ShapeStyleViewModel> styleList)
            {
                Project.AddItems(Project?.CurrentStyleLibrary, styleList);
            }
            else if (item is GroupShapeViewModel group)
            {
                if (restore)
                {
                    var shapes = Enumerable.Repeat(@group, 1);
                    TryToRestoreRecords(shapes);
                }

                Project.AddGroup(Project?.CurrentGroupLibrary, @group);
            }
            else if (item is BaseShapeViewModel model)
            {
                Project?.AddShape(Project?.CurrentContainer?.CurrentLayer, model);
            }
            else if (item is IList<GroupShapeViewModel> groups)
            {
                if (restore)
                {
                    TryToRestoreRecords(groups);
                }

                Project.AddItems(Project?.CurrentGroupLibrary, groups);
            }
            else if (item is LibraryViewModel sl && sl.Items.All(x => x is ShapeStyleViewModel))
            {
                Project.AddStyleLibrary(sl);
            }
            else if (item is IList<LibraryViewModel> sll && sll.All(x => x.Items.All(_ => _ is ShapeStyleViewModel)))
            {
                Project.AddStyleLibraries(sll);
            }
            else if (item is LibraryViewModel gl && gl.Items.All(x => x is GroupShapeViewModel))
            {
                TryToRestoreRecords(gl.Items.Cast<GroupShapeViewModel>());
                Project.AddGroupLibrary(gl);
            }
            else if (item is IList<LibraryViewModel> gll && gll.All(x => x.Items.All(_ => _ is GroupShapeViewModel)))
            {
                var shapes = gll.SelectMany(x => x.Items);
                TryToRestoreRecords(shapes.Cast<GroupShapeViewModel>());
                Project.AddGroupLibraries(gll);
            }
            else if (item is DatabaseViewModel db)
            {
                Project?.AddDatabase(db);
                Project?.SetCurrentDatabase(db);
            }
            else if (item is LayerContainerViewModel layer)
            {
                if (restore)
                {
                    TryToRestoreRecords(layer.Shapes);
                }

                Project?.AddLayer(Project?.CurrentContainer, layer);
            }
            else if (item is TemplateContainerViewModel template)
            {
                if (restore)
                {
                    var shapes = template.Layers.SelectMany(x => x.Shapes);
                    TryToRestoreRecords(shapes);
                }

                Project?.AddTemplate(template);
            }
            else if (item is PageContainerViewModel page)
            {
                if (restore)
                {
                    var shapes = Enumerable.Concat(
                        page.Layers.SelectMany(x => x.Shapes),
                        page.Template?.Layers.SelectMany(x => x.Shapes));
                    TryToRestoreRecords(shapes);
                }

                Project?.AddPage(Project?.CurrentDocument, page);
            }
            else if (item is IList<TemplateContainerViewModel> templates)
            {
                if (restore)
                {
                    var shapes = templates.SelectMany(x => x.Layers).SelectMany(x => x.Shapes);
                    TryToRestoreRecords(shapes);
                }

                // Import as templates.
                Project.AddTemplates(templates);
            }
            else if (item is IList<ScriptViewModel> scripts)
            {
                // Import as scripts.
                Project.AddScripts(scripts);
            }
            else if (item is ScriptViewModel script)
            {
                Project?.AddScript(script);
            }
            else if (item is DocumentContainerViewModel document)
            {
                if (restore)
                {
                    var shapes = Enumerable.Concat(
                        document.Pages.SelectMany(x => x.Layers).SelectMany(x => x.Shapes),
                        document.Pages.SelectMany(x => x.Template.Layers).SelectMany(x => x.Shapes));
                    TryToRestoreRecords(shapes);
                }

                Project?.AddDocument(document);
            }
            else if (item is OptionsViewModel options)
            {
                if (Project is { })
                {
                    Project.Options = options;
                }
            }
            else if (item is ProjectContainerViewModel project)
            {
                OnUnload();
                OnLoad(project, string.Empty);
            }
            else
            {
                throw new NotSupportedException("Not supported import object.");
            }
        }

        public void OnImportJson(string path)
        {
            try
            {
                var json = FileSystem?.ReadUtf8Text(path);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    var item = JsonSerializer?.Deserialize<object>(json);
                    if (item is { })
                    {
                        OnImportObject(item, true);
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnImportSvg(string path)
        {
            if (SvgConverter is null)
            {
                return;
            }
            var shapes = SvgConverter.Convert(path, out _, out _);
            if (shapes is { })
            {
                OnPasteShapes(shapes);
            }
        }

        public void OnExportJson(string path, object item)
        {
            try
            {
                var json = JsonSerializer?.Serialize(item);
                if (!string.IsNullOrWhiteSpace(json))
                {
                    FileSystem?.WriteUtf8Text(path, json);
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnExport(string path, object item, IFileWriter writer)
        {
            try
            {
                using var stream = FileSystem?.Create(path);
                if (stream is null)
                {
                    return;
                }
                writer?.Save(stream, item, Project);
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public async Task OnExecuteCode(string csharp)
        {
            if (ScriptRunner is null)
            {
                return;
            }
            
            try
            {
                if (!string.IsNullOrWhiteSpace(csharp))
                {
                    await ScriptRunner.Execute(csharp, null);
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public async Task OnExecuteRepl(string csharp)
        {
            if (ScriptRunner is null)
            {
                return;
            }

            try
            {
                if (!string.IsNullOrWhiteSpace(csharp))
                {
                    ScriptState = await ScriptRunner.Execute(csharp, ScriptState);
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnResetRepl()
        {
            ScriptState = null;
        }

        public async Task OnExecuteScriptFile(string path)
        {
            try
            {
                var csharp = FileSystem?.ReadUtf8Text(path);
                if (!string.IsNullOrWhiteSpace(csharp))
                {
                    await OnExecuteCode(csharp);
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public async Task OnExecuteScriptFile(string[] paths)
        {
            foreach (var path in paths)
            {
                await OnExecuteScriptFile(path);
            }
        }

        public async Task OnExecuteScript(ScriptViewModel script)
        {
            try
            {
                var csharp = script?.Code;
                if (!string.IsNullOrWhiteSpace(csharp))
                {
                    await OnExecuteRepl(csharp);
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnToggleDefaultIsStroked()
        {
            if (Project?.Options is { })
            {
                Project.Options.DefaultIsStroked = !Project.Options.DefaultIsStroked;
            }
        }

        public void OnToggleDefaultIsFilled()
        {
            if (Project?.Options is { })
            {
                Project.Options.DefaultIsFilled = !Project.Options.DefaultIsFilled;
            }
        }

        public void OnToggleDefaultIsClosed()
        {
            if (Project?.Options is { })
            {
                Project.Options.DefaultIsClosed = !Project.Options.DefaultIsClosed;
            }
        }

        public void OnToggleSnapToGrid()
        {
            if (Project?.Options is { })
            {
                Project.Options.SnapToGrid = !Project.Options.SnapToGrid;
            }
        }

        public void OnToggleTryToConnect()
        {
            if (Project?.Options is { })
            {
                Project.Options.TryToConnect = !Project.Options.TryToConnect;
            }
        }

        public void OnAddDatabase()
        {
            if (Project is null)
            {
                return;
            }

            var db = ViewModelFactory?.CreateDatabase(ProjectEditorConfiguration.DefaultDatabaseName);
            if (db is null)
            {
                return;
            }
            Project.AddDatabase(db);
            Project?.SetCurrentDatabase(db);
        }

        public void OnRemoveDatabase(DatabaseViewModel db)
        {
            if (Project is null)
            {
                return;
            }

            Project.RemoveDatabase(db);
            Project?.SetCurrentDatabase(Project.Databases.FirstOrDefault());
        }

        public void OnAddColumn(DatabaseViewModel db)
        {
            if (Project is null)
            {
                return;
            }

            Project.AddColumn(db, ViewModelFactory?.CreateColumn(db, ProjectEditorConfiguration.DefaulColumnName));
        }

        public void OnRemoveColumn(ColumnViewModel column)
        {
            if (Project is null)
            {
                return;
            }

            Project.RemoveColumn(column);
        }

        public void OnAddRecord(DatabaseViewModel db)
        {
            if (Project is null)
            {
                return;
            }

            Project.AddRecord(db, ViewModelFactory?.CreateRecord(db, ProjectEditorConfiguration.DefaulValue));
        }

        public void OnRemoveRecord(RecordViewModel record)
        {
            if (Project is null)
            {
                return;
            }

            Project.RemoveRecord(record);
        }

        public void OnResetRecord(IDataObject data)
        {
            if (Project is null)
            {
                return;
            }

            Project.ResetRecord(data);
        }

        public void OnApplyRecord(RecordViewModel? record)
        {
            if (Project is null)
            {
                return;
            }

            if (record is null)
            {
                return;
            }
            
            if (Project?.SelectedShapes?.Count > 0)
            {
                foreach (var shape in Project.SelectedShapes)
                {
                    Project.ApplyRecord(shape, record);
                }
            }

            if (Project?.SelectedShapes is null)
            {
                var container = Project?.CurrentContainer;
                if (container is { })
                {
                    Project?.ApplyRecord(container, record);
                }
            }
        }

        public void OnAddProperty(ViewModelBase owner)
        {
            if (Project is null)
            {
                return;
            }

            if (owner is not IDataObject data)
            {
                return;
            }
            Project.AddProperty(data, ViewModelFactory?.CreateProperty(owner, ProjectEditorConfiguration.DefaulPropertyName, ProjectEditorConfiguration.DefaulValue));
        }

        public void OnRemoveProperty(PropertyViewModel property)
        {
            if (Project is null)
            {
                return;
            }

            Project.RemoveProperty(property);
        }

        public void OnAddGroupLibrary()
        {
            if (Project is null)
            {
                return;
            }

            var gl = ViewModelFactory?.CreateLibrary(ProjectEditorConfiguration.DefaulGroupLibraryName);
            if (gl is null)
            {
                return;
            }
            
            Project.AddGroupLibrary(gl);
            Project?.SetCurrentGroupLibrary(gl);
        }

        public void OnRemoveGroupLibrary(LibraryViewModel libraryViewModel)
        {
            if (Project is null)
            {
                return;
            }

            Project.RemoveGroupLibrary(libraryViewModel);
            Project?.SetCurrentGroupLibrary(Project?.GroupLibraries.FirstOrDefault());
        }

        public void OnAddGroup(LibraryViewModel? libraryViewModel)
        {
            if (Project is null)
            {
                return;
            }

            if (libraryViewModel is null)
            {
                return;
            }
            
            if (Project.SelectedShapes?.Count == 1 && Project.SelectedShapes?.FirstOrDefault() is GroupShapeViewModel group)
            {
                var clone = @group.CopyShared(new Dictionary<object, object>());
                if (clone is { })
                {
                    Project?.AddGroup(libraryViewModel, clone);
                }
            }
        }

        public void OnRemoveGroup(GroupShapeViewModel? group)
        {
            if (Project is null)
            {
                return;
            }

            if (group is null)
            {
                return;
            }
            
            var library = Project.RemoveGroup(@group);
            library?.SetSelected(library.Items.FirstOrDefault());
        }

        public void OnInsertGroup(GroupShapeViewModel group)
        {
            if (Project?.CurrentContainer is { })
            {
                OnDropShapeAsClone(group, 0.0, 0.0);
            }
        }

        public void OnAddLayer(FrameContainerViewModel? container)
        {
            if (Project is null)
            {
                return;
            }

            if (container is null)
            {
                return;
            }
            
            Project.AddLayer(container, ViewModelFactory?.CreateLayerContainer(ProjectEditorConfiguration.DefaultLayerName, container));
        }

        public void OnRemoveLayer(LayerContainerViewModel? layer)
        {
            if (Project is null)
            {
                return;
            }

            if (layer is null)
            {
                return;
            }
            
            Project.RemoveLayer(layer);
            if (layer.Owner is FrameContainerViewModel owner)
            {
                owner.SetCurrentLayer(owner.Layers.FirstOrDefault());
            }
        }

        public void OnAddStyleLibrary()
        {
            if (Project is null)
            {
                return;
            }

            var sl = ViewModelFactory?.CreateLibrary(ProjectEditorConfiguration.DefaulStyleLibraryName);
            if (sl is null)
            {
                return;
            }
            
            Project.AddStyleLibrary(sl);
            Project?.SetCurrentStyleLibrary(sl);
        }

        public void OnRemoveStyleLibrary(LibraryViewModel libraryViewModel)
        {
            if (Project is null)
            {
                return;
            }

            Project.RemoveStyleLibrary(libraryViewModel);
            Project?.SetCurrentStyleLibrary(Project?.StyleLibraries.FirstOrDefault());
        }

        public void OnAddStyle(LibraryViewModel libraryViewModel)
        {
            if (Project is null)
            {
                return;
            }

            if (Project?.SelectedShapes is { })
            {
                foreach (var shape in Project.SelectedShapes)
                {
                    if (shape.Style is { })
                    {
                        var style = (ShapeStyleViewModel)shape.Style.Copy(null);
                        Project.AddStyle(libraryViewModel, style);
                    }
                }
            }
            else
            {
                var style = ViewModelFactory?.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
                if (style is { })
                {
                    Project.AddStyle(libraryViewModel, style);
                }
            }
        }

        public void OnRemoveStyle(ShapeStyleViewModel? style)
        {
            if (Project is null)
            {
                return;
            }

            var library = Project.RemoveStyle(style);
            library?.SetSelected(library.Items.FirstOrDefault());
        }

        public void OnApplyStyle(ShapeStyleViewModel? style)
        {
            if (Project is null)
            {
                return;
            }

            if (style is null)
            {
                return;
            }

            if (!(Project?.SelectedShapes?.Count > 0))
            {
                return;
            }
            
            foreach (var shape in Project.SelectedShapes)
            {
                Project?.ApplyStyle(shape, style);
            }
        }

        public void OnAddShape(BaseShapeViewModel? shape)
        {
            if (Project is null)
            {
                return;
            }

            var layer = Project.CurrentContainer?.CurrentLayer;
            if (layer is { } && shape is { })
            {
                Project.AddShape(layer, shape);
            }
        }

        public void OnRemoveShape(BaseShapeViewModel? shape)
        {
            if (Project is null)
            {
                return;
            }

            var layer = Project.CurrentContainer?.CurrentLayer;
            if (layer is null || shape is null)
            {
                return;
            }
            Project.RemoveShape(layer, shape);
            if (Project?.CurrentContainer is { })
            {
                Project.CurrentContainer.CurrentShape = layer.Shapes.FirstOrDefault();
            }
        }

        public void OnAddTemplate()
        {
            if (Project is null)
            {
                return;
            }
            
            var template = ContainerFactory?.GetTemplate(Project, "Empty") 
                           ?? ViewModelFactory?.CreateTemplateContainer(ProjectEditorConfiguration.DefaultTemplateName);
            if (template is { })
            {
                Project.AddTemplate(template);
            }
        }

        public void OnRemoveTemplate(TemplateContainerViewModel? template)
        {
            if (Project is null)
            {
                return;
            }

            if (template is null)
            {
                return;
            }
            Project.RemoveTemplate(template);
            Project.SetCurrentTemplate(Project?.Templates.FirstOrDefault());
        }

        public void OnEditTemplate(FrameContainerViewModel? template)
        {
            if (Project is null)
            {
                return;
            }

            if (template is null)
            {
                return;
            }

            Project.SetCurrentContainer(template);
            Project.CurrentContainer?.InvalidateLayer();
        }

        public void OnAddScript()
        {
            if (Project is null)
            {
                return;
            }
            
            var script = ViewModelFactory?.CreateScript(ProjectEditorConfiguration.DefaultScriptName);
            Project.AddScript(script);
        }

        public void OnRemoveScript(ScriptViewModel? script)
        {
            if (Project is null)
            {
                return;
            }
            
            if (script is null)
            {
                return;
            }
            
            Project.RemoveScript(script);
            Project.SetCurrentScript(Project?.Scripts.FirstOrDefault());
        }

        public void OnApplyTemplate(TemplateContainerViewModel? template)
        {
            if (Project is null)
            {
                return;
            }
            
            var container = Project.CurrentContainer;
            if (container is PageContainerViewModel page)
            {
                Project.ApplyTemplate(page, template);
                Project.CurrentContainer?.InvalidateLayer();
            }
        }

        public string? OnGetImageKey(string path)
        {
            if (Project is null)
            {
                return default;
            }
            
            using var stream = FileSystem?.Open(path);
            if (stream is null)
            {
                return default;
            }
            var bytes = FileSystem?.ReadBinary(stream);
            if (bytes is null)
            {
                return default;
            }
            if (Project is IImageCache imageCache)
            {
                var key = imageCache.AddImageFromFile(path, bytes);
                return key;
            }
            return default;
        }

        public async Task<string?> OnAddImageKey(string? path)
        {
            if (Project is null)
            {
                return default;
            }

            if (path is null || string.IsNullOrEmpty(path))
            {
                var key = await (ImageImporter?.GetImageKeyAsync() ?? Task.FromResult(default(string)));
                if (key is null || string.IsNullOrEmpty(key))
                {
                    return default;
                }

                return key;
            }

            using var stream = FileSystem?.Open(path);
            if (stream is null)
            {
                return default;
            }

            var bytes = FileSystem?.ReadBinary(stream);
            if (bytes is null)
            {
                return default;
            }
            
            if (Project is IImageCache imageCache)
            {
                var key = imageCache.AddImageFromFile(path, bytes);
                return key;
            }
            
            return default;
        }

        public void OnRemoveImageKey(string? key)
        {
            if (Project is null)
            {
                return;
            }
            
            if (key is null)
            {
                return;
            }
            if (Project is IImageCache imageCache)
            {
                imageCache.RemoveImage(key);
            }
        }

        public void OnAddPage(object item)
        {
            if (Project is null)
            {
                return;
            }
            
            if (Project.CurrentDocument is null)
            {
                return;
            }
            
            var page =
                ContainerFactory?.GetPage(Project, ProjectEditorConfiguration.DefaultPageName)
                ?? ViewModelFactory?.CreatePageContainer(ProjectEditorConfiguration.DefaultPageName);
            if (page is null)
            {
                return;
            }
            Project.AddPage(Project.CurrentDocument, page);
            Project.SetCurrentContainer(page);
        }

        public void OnInsertPageBefore(object item)
        {
            if (Project is null)
            {
                return;
            }
            
            if (Project.CurrentDocument is null)
            {
                return;
            }

            if (item is not PageContainerViewModel selected)
            {
                return;
            }
            var index = Project.CurrentDocument.Pages.IndexOf(selected);
            var page =
                ContainerFactory?.GetPage(Project, ProjectEditorConfiguration.DefaultPageName)
                ?? ViewModelFactory?.CreatePageContainer(ProjectEditorConfiguration.DefaultPageName);
            if (page is null)
            {
                return;
            }
            Project.AddPageAt(Project.CurrentDocument, page, index);
            Project.SetCurrentContainer(page);
        }

        public void OnInsertPageAfter(object item)
        {
            if (Project is null)
            {
                return;
            }
            
            if (Project.CurrentDocument is null)
            {
                return;
            }

            if (item is not PageContainerViewModel selected)
            {
                return;
            }
            var index = Project.CurrentDocument.Pages.IndexOf(selected);
            var page =
                ContainerFactory?.GetPage(Project, ProjectEditorConfiguration.DefaultPageName)
                ?? ViewModelFactory?.CreatePageContainer(ProjectEditorConfiguration.DefaultPageName);
            if (page is null)
            {
                return;
            }
            Project.AddPageAt(Project.CurrentDocument, page, index + 1);
            Project.SetCurrentContainer(page);
        }

        public void OnAddDocument(object item)
        {
            if (Project is null)
            {
                return;
            }
            
            var document =
                ContainerFactory?.GetDocument(Project, ProjectEditorConfiguration.DefaultDocumentName)
                ?? ViewModelFactory?.CreateDocumentContainer(ProjectEditorConfiguration.DefaultDocumentName);
            if (document is null)
            {
                return;
            }
            
            Project.AddDocument(document);
            Project.SetCurrentDocument(document);
            Project.SetCurrentContainer(document?.Pages.FirstOrDefault());
        }

        public void OnInsertDocumentBefore(object item)
        {
            if (Project is null)
            {
                return;
            }

            if (item is not DocumentContainerViewModel selected)
            {
                return;
            }
            
            var index = Project.Documents.IndexOf(selected);
            var document =
                ContainerFactory?.GetDocument(Project, ProjectEditorConfiguration.DefaultDocumentName)
                ?? ViewModelFactory?.CreateDocumentContainer(ProjectEditorConfiguration.DefaultDocumentName);
            if (document is null)
            {
                return;
            }
            Project.AddDocumentAt(document, index);
            Project.SetCurrentDocument(document);
            Project.SetCurrentContainer(document?.Pages.FirstOrDefault());
        }

        public void OnInsertDocumentAfter(object item)
        {
            if (Project is null)
            {
                return;
            }

            if (item is not DocumentContainerViewModel selected)
            {
                return;
            }
            var index = Project.Documents.IndexOf(selected);
            var document =
                ContainerFactory?.GetDocument(Project, ProjectEditorConfiguration.DefaultDocumentName)
                ?? ViewModelFactory?.CreateDocumentContainer(ProjectEditorConfiguration.DefaultDocumentName);
            if (document is null)
            {
                return;
            }
            Project.AddDocumentAt(document, index + 1);
            Project.SetCurrentDocument(document);
            Project.SetCurrentContainer(document?.Pages.FirstOrDefault());
        }

        private void SetRenderersImageCache(IImageCache? cache)
        {
            if (Renderer is null)
            {
                return;
            }
            Renderer.ClearCache();
            if (Renderer.State is null)
            {
                return;
            }
            Renderer.State.ImageCache = cache;
        }

        public void OnLoad(ProjectContainerViewModel? project, string? path = null)
        {
            if (project is null)
            {
                return;
            }
            
            Deselect();
            
            if (project is IImageCache imageCache)
            {
                SetRenderersImageCache(imageCache);
            }
            
            Project = project;
            Project.History = new StackHistory();
            ProjectPath = path;
            IsProjectDirty = false;

            var propertyChangedSubject = new Subject<(object? sender, PropertyChangedEventArgs e)>();
            var propertyChangedDisposable = Project.Subscribe(propertyChangedSubject);
            var observable = propertyChangedSubject.Subscribe(ProjectChanged);

            Observer = new CompositeDisposable(propertyChangedDisposable, observable, propertyChangedSubject);

            void ProjectChanged((object? sender, PropertyChangedEventArgs e) arg)
            {
                // Debug.WriteLine($"[Changed] {arg.sender}.{arg.e.PropertyName}");
                // _project?.CurrentContainer?.InvalidateLayer();
                CanvasPlatform?.InvalidateControl?.Invoke();
                IsProjectDirty = true;
            }
        }

        public void OnUnload()
        {
            if (Observer is { })
            {
                Observer?.Dispose();
                Observer = null;
            }

            if (Project?.History is { })
            {
                Project.History.Reset();
                Project.History = null;
            }

            if (Project is null)
            {
                return;
            }
            
            if (Project is IImageCache imageCache)
            {
                imageCache.PurgeUnusedImages(new HashSet<string>());
            }
            
            Deselect();
            SetRenderersImageCache(null);
            Project = null;
            ProjectPath = string.Empty;
            IsProjectDirty = false;
            GC.Collect();
        }

        public void OnInvalidateCache()
        {
            try
            {
                Renderer?.ClearCache();
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnAddRecent(string path, string name)
        {
            var q = _recentProjects.Where(x => x.Path?.ToLower() == path.ToLower()).ToList();
            var builder = _recentProjects.ToBuilder();
            if (q.Count > 0)
            {
                foreach (var r in q)
                {
                    builder.Remove(r);
                }
            }

            builder.Insert(0, RecentFileViewModel.Create(ServiceProvider, name, path));

            RecentProjects = builder.ToImmutable();
            CurrentRecentProject = _recentProjects.FirstOrDefault();
        }

        public void OnLoadRecent(string path)
        {
            if (JsonSerializer is null)
            {
                return;
            }
            
            try
            {
                var json = FileSystem?.ReadUtf8Text(path);
                if (json is null)
                {
                    return;
                }
                var recent = JsonSerializer.Deserialize<RecentsViewModel>(json);
                if (recent is null)
                {
                    return;
                }
                var remove = recent.Files.Where(x => FileSystem?.Exists(x.Path) == false).ToList();
                var builder = recent.Files.ToBuilder();

                foreach (var file in remove)
                {
                    builder.Remove(file);
                }

                RecentProjects = builder.ToImmutable();

                if (recent.Current?.Path is { }
                    && (FileSystem?.Exists(recent.Current.Path) ?? false))
                {
                    CurrentRecentProject = recent.Current;
                }
                else
                {
                    CurrentRecentProject = _recentProjects.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnSaveRecent(string path)
        {
            if (JsonSerializer is null)
            {
                return;
            }
            
            try
            {
                var recent = RecentsViewModel.Create(ServiceProvider, _recentProjects, _currentRecentProject);
                var json = JsonSerializer.Serialize(recent);
                FileSystem?.WriteUtf8Text(path, json);
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public bool CanUndo()
        {
            return Project?.History?.CanUndo() ?? false;
        }

        public bool CanRedo()
        {
            return Project?.History?.CanRedo() ?? false;
        }

        public void OnUndo()
        {
            try
            {
                if (Project?.History?.CanUndo() ?? false)
                {
                    Deselect();
                    Project?.History.Undo();
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public void OnRedo()
        {
            try
            {
                if (Project?.History?.CanRedo() ?? false)
                {
                    Deselect();
                    Project?.History.Redo();
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public async Task<bool> OnDropFiles(string[]? files, double x, double y)
        {
            try
            {
                if (files is null || files.Length < 1)
                {
                    return false;
                }

                var result = false;

                foreach (var path in files)
                {
                    if (string.IsNullOrEmpty(path))
                    {
                        continue;
                    }

                    string ext = System.IO.Path.GetExtension(path);

                    if (string.Compare(ext, ProjectEditorConfiguration.ProjectExtension, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        OnOpenProject(path);
                        result = true;
                    }
                    else if (string.Compare(ext, ProjectEditorConfiguration.CsvExtension, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        var reader = TextFieldReaders.FirstOrDefault(x => x.Extension == "csv");
                        if (reader is { })
                        {
                            OnImportData(Project, path, reader);
                            result = true;
                        }
                    }
                    else if (string.Compare(ext, ProjectEditorConfiguration.XlsxExtension, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        var reader = TextFieldReaders.FirstOrDefault(x => x.Extension == "xlsx");
                        if (reader is { })
                        {
                            OnImportData(Project, path, reader);
                            result = true;
                        }
                    }
                    else if (string.Compare(ext, ProjectEditorConfiguration.JsonExtension, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        OnImportJson(path);
                        result = true;
                    }
                    else if (string.Compare(ext, ProjectEditorConfiguration.ScriptExtension, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        await OnExecuteScriptFile(path);
                        result = true;
                    }
                    else if (string.Compare(ext, ProjectEditorConfiguration.SvgExtension, StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        OnImportSvg(path);
                        result = true;
                    }
                    else if (ProjectEditorConfiguration.ImageExtensions.Any(x => string.Compare(ext, x, StringComparison.OrdinalIgnoreCase) == 0))
                    {
                        var key = OnGetImageKey(path);
                        if (key is { } && !string.IsNullOrEmpty(key))
                        {
                            OnDropImageKey(key, x, y);
                        }
                        result = true;
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }

            return false;
        }

        public void OnDropImageKey(string key, double x, double y)
        {
            if (Project is null)
            {
                return;
            }
            
            var selected = Project?.CurrentStyleLibrary?.Selected is { } ?
                Project.CurrentStyleLibrary.Selected :
                ViewModelFactory?.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var style = (ShapeStyleViewModel?)selected?.Copy(null);
            var layer = Project?.CurrentContainer?.CurrentLayer;
            var sx = Project.Options.SnapToGrid ? PointUtil.Snap((decimal)x, (decimal)Project.Options.SnapX) : (decimal)x;
            var sy = Project.Options.SnapToGrid ? PointUtil.Snap((decimal)y, (decimal)Project.Options.SnapY) : (decimal)y;

            var image = ViewModelFactory?.CreateImageShape((double)sx, (double)sy, style, key);
            image.BottomRight.X = (double)(sx + 320m);
            image.BottomRight.Y = (double)(sy + 180m);

            Project.AddShape(layer, image);
        }

        public bool OnDropShape(BaseShapeViewModel shape, double x, double y, bool bExecute = true)
        {
            if (Project is null)
            {
                return false;
            }
            
            try
            {
                var layer = Project?.CurrentContainer?.CurrentLayer;
                if (layer is { })
                {
                    if (bExecute)
                    {
                        OnDropShapeAsClone(shape, x, y);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
            return false;
        }

        public void OnDropShapeAsClone<T>(T shape, double x, double y) where T : BaseShapeViewModel
        {
            if (Project is null)
            {
                return;
            }
            
            if (Project.Options is null)
            {
                return;
            }

            var sx = Project.Options.SnapToGrid ? PointUtil.Snap((decimal)x, (decimal)Project.Options.SnapX) : (decimal)x;
            var sy = Project.Options.SnapToGrid ? PointUtil.Snap((decimal)y, (decimal)Project.Options.SnapY) : (decimal)y;

            try
            {
                var clone = shape.CopyShared(new Dictionary<object, object>());
                if (clone is { })
                {
                    Deselect(Project.CurrentContainer?.CurrentLayer);
                    clone.Move(null, sx, sy);

                    Project.AddShape(Project?.CurrentContainer?.CurrentLayer, clone);

                    // Select(Project?.CurrentContainer?.CurrentLayer, clone);

                    if (Project.Options.TryToConnect)
                    {
                        if (clone is GroupShapeViewModel group)
                        {
                            var shapes = Project?.CurrentContainer?.CurrentLayer?.Shapes.GetAllShapes<LineShapeViewModel>().ToList();
                            TryToConnectLines(shapes, group.Connectors);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
        }

        public bool OnDropRecord(RecordViewModel record, double x, double y, bool bExecute = true)
        {
            if (Project is null)
            {
                return false;
            }
            
            if (Project.Options is null)
            {
                return false;
            }

            try
            {
                if (Project.SelectedShapes?.Count > 0)
                {
                    if (bExecute)
                    {
                        OnApplyRecord(record);
                    }
                    return true;
                }
                else
                {
                    var layer = Project?.CurrentContainer?.CurrentLayer;
                    if (layer is { })
                    {
                        var shapes = layer.Shapes.Reverse();
                        var radius = Project.Options.HitThreshold / PageState.ZoomX;
                        var result = HitTest.TryToGetShape(shapes, new Point2(x, y), radius, PageState.ZoomX);
                        if (result is { })
                        {
                            if (bExecute)
                            {
                                Project?.ApplyRecord(result, record);
                            }
                            return true;
                        }
                        else
                        {
                            if (bExecute)
                            {
                                OnDropRecordAsGroup(record, x, y);
                            }
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
            return false;
        }

        public void OnDropRecordAsGroup(RecordViewModel record, double x, double y)
        {
            if (Project is null)
            {
                return;
            }
            
            if (Project.Options is null)
            {
                return;
            }

            var selected = Project.CurrentStyleLibrary?.Selected is { } ?
                Project.CurrentStyleLibrary.Selected :
                ViewModelFactory?.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
            var style = (ShapeStyleViewModel?)selected?.Copy(null);
            var layer = Project.CurrentContainer?.CurrentLayer;
            var sx = Project.Options.SnapToGrid ? PointUtil.Snap((decimal)x, (decimal)Project.Options.SnapX) : (decimal)x;
            var sy = Project.Options.SnapToGrid ? PointUtil.Snap((decimal)y, (decimal)Project.Options.SnapY) : (decimal)y;

            var g = ViewModelFactory?.CreateGroupShape(ProjectEditorConfiguration.DefaulGroupName);

            g.Record = record;

            var length = record.Values.Length;
            var px = (double)sx;
            var py = (double)sy;
            double width = 150;
            double height = 15;

            var db = record.Owner as DatabaseViewModel;

            for (var i = 0; i < length; i++)
            {
                var column = db.Columns[i];
                if (column.IsVisible)
                {
                    var binding = "{" + db.Columns[i].Name + "}";
                    var text = ViewModelFactory?.CreateTextShape(px, py, px + width, py + height, style, binding);
                    g.AddShape(text);
                    py += height;
                }
            }

            var rectangle = ViewModelFactory?.CreateRectangleShape((double)sx, (double)sy, (double)sx + width, (double)sy + (length * height), style);
            g.AddShape(rectangle);

            var pt = ViewModelFactory?.CreatePointShape((double)sx + (width / 2), (double)sy);
            var pb = ViewModelFactory?.CreatePointShape((double)sx + (width / 2), (double)sy + (length * height));
            var pl = ViewModelFactory?.CreatePointShape((double)sx, (double)sy + ((length * height) / 2));
            var pr = ViewModelFactory?.CreatePointShape((double)sx + width, (double)sy + ((length * height) / 2));

            g.AddConnectorAsNone(pt);
            g.AddConnectorAsNone(pb);
            g.AddConnectorAsNone(pl);
            g.AddConnectorAsNone(pr);

            Project.AddShape(layer, g);
        }

        public bool OnDropStyle(ShapeStyleViewModel style, double x, double y, bool bExecute = true)
        {
            if (Project is null)
            {
                return false;
            }

            if (Project.Options is null)
            {
                return false;
            }

            try
            {
                if (Project.SelectedShapes?.Count > 0)
                {
                    if (bExecute)
                    {
                        OnApplyStyle(style);
                    }
                    return true;
                }
                else
                {
                    var layer = Project.CurrentContainer?.CurrentLayer;
                    if (layer is { })
                    {
                        var shapes = layer.Shapes.Reverse();
                        var radius = Project.Options.HitThreshold / PageState.ZoomX;
                        var result = HitTest.TryToGetShape(shapes, new Point2(x, y), radius, PageState.ZoomX);
                        if (result is { })
                        {
                            if (bExecute == true)
                            {
                                Project.ApplyStyle(result, style);
                            }
                            return true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
            return false;
        }

        public bool OnDropTemplate(TemplateContainerViewModel? template, double x, double y, bool bExecute = true)
        {
            if (Project is null)
            {
                return false;
            }

            try
            {
                var container = Project.CurrentContainer;
                if (container is PageContainerViewModel && template is { })
                {
                    if (bExecute)
                    {
                        OnApplyTemplate(template);
                    }
                    return true;
                }
            }
            catch (Exception ex)
            {
                Log?.LogException(ex);
            }
            return false;
        }
    }
}
