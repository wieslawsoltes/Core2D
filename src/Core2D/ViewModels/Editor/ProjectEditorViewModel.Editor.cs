// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor.History;
using Core2D.ViewModels.Layout;
using Core2D.ViewModels.Scripting;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using Core2D.Spatial;

namespace Core2D.ViewModels.Editor;

public partial class ProjectEditorViewModel
{
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
        if (_project is null)
        {
            return;
        }
        var input = source?.ToList() ?? _project.GetAllShapes().ToList();
        var shapes = input.Where(s => s.GetType() == shape.GetType() && s != shape).ToList();
        var count = shapes.Count + 1;
        var update = string.IsNullOrEmpty(shape.Name) || input.Any(x => x != shape && x.Name == shape.Name);
        if (update)
        {
            shape.Name = shape.GetType().Name.Replace("ShapeViewModel", " ") + count;
        }
    }

    private IDictionary<string, RecordViewModel>? GenerateRecordDictionaryById()
    {
        var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
        if (project is null)
        {
            return default;
        }
            
        return project.Databases
            .Where(d => d.Records.Length > 0)
            .SelectMany(d => d.Records)
            .ToDictionary(s => s.Id);
    }

    private void TryToRestoreRecords(IEnumerable<BaseShapeViewModel> shapes)
    {
        var project = ServiceProvider.GetService<ProjectEditorViewModel>()?.Project;
        if (project is null)
        {
            return;
        }

        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
            
        try
        {
            if (project.Databases.Length == 0)
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
                    if (project.CurrentDatabase is null && shape.Record.Owner is DatabaseViewModel owner)
                    {
                        var db = viewModelFactory?.CreateDatabase(
                            ProjectEditorConfiguration.DefaultImportedDatabaseName,
                            owner.Columns);
                        project.AddDatabase(db);
                        project.SetCurrentDatabase(db);
                    }

                    // Add missing data record.
                    shape.Record.Owner = project.CurrentDatabase;
                    project.AddRecord(project.CurrentDatabase, shape.Record);

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

    public void OnNew(object? item)
    {
        switch (item)
        {
            case LayerContainerViewModel layer:
            {
                if (layer.Owner is PageContainerViewModel page)
                {
                    Project?.OnAddLayer(page);
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
        if (Project is null || selected is null)
        {
            return;
        }

        var document = Project.Documents.FirstOrDefault(d => d.Pages.Contains(selected));
        if (document is null)
        {
            return;
        }

        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
        var containerFactory = ServiceProvider.GetService<IContainerFactory>();
        var page =
            containerFactory?.GetPage(Project, ProjectEditorConfiguration.DefaultPageName)
            ?? viewModelFactory?.CreatePageContainer(ProjectEditorConfiguration.DefaultPageName);
        if (page is null)
        {
            return;
        }

        Project?.AddPage(document, page);
        Project?.SetCurrentContainer(page);
    }

    public void OnNewPage(DocumentContainerViewModel? selected)
    {
        if (Project is null)
        {
            return;
        }

        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
        var containerFactory = ServiceProvider.GetService<IContainerFactory>();
        var page =
            containerFactory?.GetPage(Project, ProjectEditorConfiguration.DefaultPageName)
            ?? viewModelFactory?.CreatePageContainer(ProjectEditorConfiguration.DefaultPageName);
        if (page is null)
        {
            return;
        }

        Project.AddPage(selected, page);
        Project.SetCurrentContainer(page);
    }

    public void OnNewDocument()
    {
        if (Project is null)
        {
            return;
        }

        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
        var containerFactory = ServiceProvider.GetService<IContainerFactory>();
        var document =
            containerFactory?.GetDocument(Project, ProjectEditorConfiguration.DefaultDocumentName)
            ?? viewModelFactory?.CreateDocumentContainer(ProjectEditorConfiguration.DefaultDocumentName);
        if (document is null)
        {
            return;
        }

        Project.AddDocument(document);
        Project.SetCurrentDocument(document);
        Project.SetCurrentContainer(document.Pages.FirstOrDefault());
    }

    public void OnNewProject()
    {
        OnUnload();

        var containerFactory = ServiceProvider.GetService<IContainerFactory>();
        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
        var project = containerFactory?.GetProject() ?? viewModelFactory?.CreateProjectContainer();
        if (project is null)
        {
            return;
        }

        OnLoad(project, string.Empty);
        CanvasPlatform?.ResetZoom?.Invoke();
        CanvasPlatform?.InvalidateControl?.Invoke();
        NavigateTo?.Invoke("Home");
    }

    public void OnOpenProject(Stream stream, string name)
    {
        try
        {
            var fileSystem = ServiceProvider.GetService<IFileSystem>();
            var jsonSerializer = ServiceProvider.GetService<IJsonSerializer>();
            var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
            if (fileSystem is null || jsonSerializer is null || viewModelFactory is null)
            {
                return;
            }

            var project = viewModelFactory.OpenProjectContainer(stream, fileSystem, jsonSerializer);
            if (project is { })
            {
                OnOpenProjectImpl(project, name);
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    private void OnOpenProjectImpl(ProjectContainerViewModel? project, string name)
    {
        if (project is null)
        {
            return;
        }
        try
        {
            OnUnload();
            OnLoad(project, name);
            CanvasPlatform?.ResetZoom?.Invoke();
            CanvasPlatform?.InvalidateControl?.Invoke();
            NavigateTo?.Invoke("Home");
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public void OnCloseProject()
    {
        Platform?.OnClose();
        Project?.History?.Reset();
        OnUnload();
        NavigateTo?.Invoke("Dashboard");
    }

    public void OnSaveProject(Stream stream, string name)
    {
        if (Project is null)
        {
            return;
        }

        try
        {
            var isDecoratorVisible = PageState?.Decorator?.IsVisible == true;
            if (isDecoratorVisible)
            {
                ServiceProvider.GetService<ISelectionService>()?.OnHideDecorator();
            }

            var fileSystem = ServiceProvider.GetService<IFileSystem>();
            var jsonSerializer = ServiceProvider.GetService<IJsonSerializer>();
            var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
            if (fileSystem is { } && jsonSerializer is { } && viewModelFactory is { })
            {
                viewModelFactory.SaveProjectContainer(Project, Project, stream, fileSystem, jsonSerializer);

                if (string.IsNullOrEmpty(ProjectName))
                {
                    ProjectName = name;
                }

                IsProjectDirty = false;
            }
                
            if (isDecoratorVisible)
            {
                ServiceProvider.GetService<ISelectionService>()?.OnShowDecorator();
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public void OnImportData(ProjectContainerViewModel? project, Stream stream, ITextFieldReader<DatabaseViewModel>? reader)
    {
        if (project is null)
        {
            return;
        }
            
        var fileSystem = ServiceProvider.GetService<IFileSystem>();
        if (fileSystem is null)
        {
            return;
        }

        try
        {
            var db = reader?.Read(stream);
            if (db is { })
            {
                project.AddDatabase(db);
                project.SetCurrentDatabase(db);
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public void OnExportData(Stream stream, DatabaseViewModel? database, ITextFieldWriter<DatabaseViewModel>? writer)
    {
        var fileSystem = ServiceProvider.GetService<IFileSystem>();
        if (fileSystem is null)
        {
            return;
        }

        try
        {
            writer?.Write(stream, database);
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public void OnUpdateData(Stream stream, DatabaseViewModel database, ITextFieldReader<DatabaseViewModel> reader)
    {
        var fileSystem = ServiceProvider.GetService<IFileSystem>();
        if (fileSystem is null)
        {
            return;
        }

        try
        {
            var db = reader.Read(stream);
            if (db is { })
            {
                Project?.UpdateDatabase(database, db);
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
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
                    page.Template is null ? Enumerable.Empty<BaseShapeViewModel>() : page.Template.Layers.SelectMany(x => x.Shapes));
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
                    document.Pages.SelectMany(x => x.Template?.Layers ?? Enumerable.Empty<LayerContainerViewModel>()).SelectMany(x => x.Shapes));
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

    public void OnImportJson(Stream stream)
    {
        var fileSystem = ServiceProvider.GetService<IFileSystem>();
        if (fileSystem is null)
        {
            return;
        }

        var jsonSerializer = ServiceProvider.GetService<IJsonSerializer>();
        if (jsonSerializer is null)
        {
            return;
        }

        try
        {
            var json = fileSystem.ReadUtf8Text(stream);
            if (json is not null && !string.IsNullOrWhiteSpace(json))
            {
                var item = jsonSerializer.Deserialize<object>(json);
                if (item is { })
                {
                    OnImportObject(item, true);
                }
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public void OnImportSvg(Stream stream)
    {
        var svgConverter = ServiceProvider.GetService<ISvgConverter>();
        if (svgConverter is null)
        {
            return;
        }
        var shapes = svgConverter.Convert(stream, out _, out _);
        if (shapes is { })
        {
            ServiceProvider.GetService<IClipboardService>()?.OnPasteShapes(shapes);
        }
    }

    public void OnExportJson(Stream stream, object item)
    {
        var fileSystem = ServiceProvider.GetService<IFileSystem>();
        if (fileSystem is null)
        {
            return;
        }

        var jsonSerializer = ServiceProvider.GetService<IJsonSerializer>();
        if (jsonSerializer is null)
        {
            return;
        }

        try
        {
            var json = jsonSerializer.Serialize(item);
            if (!string.IsNullOrWhiteSpace(json))
            {
                fileSystem.WriteUtf8Text(stream, json);
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public void OnExport(Stream stream, object item, IFileWriter writer)
    {
        try
        {
            writer.Save(stream, item, Project);
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public async Task OnExecuteScript(Stream stream)
    {
        var fileSystem = ServiceProvider.GetService<IFileSystem>();
        if (fileSystem is null)
        {
            return;
        }

        try
        {
            var csharp = fileSystem.ReadUtf8Text(stream);
            if (!string.IsNullOrWhiteSpace(csharp))
            {
                if (Project is null)
                {
                    return;
                }
                await Project.OnExecuteCode(csharp);
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
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
    
    public void OnToggleSinglePressMode()
    {
        if (Project?.Options is { })
        {
            Project.Options.SinglePressMode = !Project.Options.SinglePressMode;
        }
    }

    public string? OnGetImageKey(Stream stream, string name)
    {
        if (Project is null)
        {
            return default;
        }
           
        var fileSystem = ServiceProvider.GetService<IFileSystem>();
        if (fileSystem is null)
        {
            return default;
        }

        var bytes = fileSystem.ReadBinary(stream);
        if (bytes is null)
        {
            return default;
        }
        if (Project is IImageCache imageCache)
        {
            var key = imageCache.AddImageFromFile(name, bytes);
            return key;
        }
        return default;
    }

    private void SetRenderersImageCache(IImageCache? cache)
    {
        void ConfigureRenderer(IShapeRenderer? renderer)
        {
            if (renderer is null)
            {
                return;
            }

            renderer.ClearCache();

            if (renderer.State is null)
            {
                return;
            }

            renderer.State.ImageCache = cache;
        }

        ConfigureRenderer(Renderer);
        ConfigureRenderer(LibraryRenderer);
    }

    public void OnLoad(ProjectContainerViewModel? project, string? name = null)
    {
        if (project is null)
        {
            return;
        }
            
        ServiceProvider.GetService<ISelectionService>()?.Deselect();
            
        if (project is IImageCache imageCache)
        {
            SetRenderersImageCache(imageCache);
        }
            
        Project = project;
        Project.History = new StackHistory();
        ProjectName = name;
        IsProjectDirty = false;

        var propertyChangedSubject = new Subject<(object? sender, PropertyChangedEventArgs e)>();
        var propertyChangedDisposable = Project.Subscribe(propertyChangedSubject);
        var observable = propertyChangedSubject.Subscribe(ProjectChanged);

        Observer = new CompositeDisposable(propertyChangedDisposable, observable, propertyChangedSubject);

        void ProjectChanged((object? sender, PropertyChangedEventArgs e) arg)
        {
            HandleProjectPropertyChanged(arg.sender, arg.e);
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
            
        ServiceProvider.GetService<ISelectionService>()?.Deselect();
        SetRenderersImageCache(null);
        Project = null;
        ProjectName = string.Empty;
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
            ServiceProvider.GetService<ILog>()?.LogException(ex);
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
                ServiceProvider.GetService<ISelectionService>()?.Deselect();
                Project?.History.Undo();
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public void OnRedo()
    {
        try
        {
            if (Project?.History?.CanRedo() ?? false)
            {
                ServiceProvider.GetService<ISelectionService>()?.Deselect();
                Project?.History.Redo();
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public async Task<bool> OnDropFiles(IStorageItem[]? files, double x, double y)
    {
        try
        {
            if (files is null || files.Length < 1)
            {
                return false;
            }

            var result = false;

            foreach (var file in files)
            {
                var path = file.TryGetLocalPath();
                if (string.IsNullOrEmpty(path))
                {
                    continue;
                }

                string ext = System.IO.Path.GetExtension(path);

                if (string.Compare(ext, ProjectEditorConfiguration.DefaultProjectExtension, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var name = System.IO.Path.GetFileNameWithoutExtension(path);
                    await using var stream = ServiceProvider.GetService<IFileSystem>()?.Open(path);
                    if (stream is { })
                    {
                        OnOpenProject(stream, name);
                        result = true;
                    }
                }
                else if (string.Compare(ext, ProjectEditorConfiguration.DefaultCsvExtension, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var reader = TextFieldReaders.FirstOrDefault(r => r.Extension == "csv");
                    if (reader is { })
                    {
                        await using var stream = ServiceProvider.GetService<IFileSystem>()?.Open(path);
                        if (stream is { })
                        {
                            OnImportData(Project, stream, reader);
                            result = true;
                        }
                    }
                }
                else if (string.Compare(ext, ProjectEditorConfiguration.DefaultXlsxExtension, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var reader = TextFieldReaders.FirstOrDefault(r => r.Extension == "xlsx");
                    if (reader is { })
                    {
                        await using var stream = ServiceProvider.GetService<IFileSystem>()?.Open(path);
                        if (stream is { })
                        {
                            OnImportData(Project, stream, reader);
                            result = true;
                        }
                    }
                }
                else if (string.Compare(ext, ProjectEditorConfiguration.DefaultJsonExtension, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    await using var stream = ServiceProvider.GetService<IFileSystem>()?.Open(path);
                    if (stream is { })
                    {
                        OnImportJson(stream);
                        result = true;
                    }
                }
                else if (string.Compare(ext, ProjectEditorConfiguration.DefaultScriptExtension, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    await using var stream = ServiceProvider.GetService<IFileSystem>()?.Open(path);
                    if (stream is { })
                    {
                        await OnExecuteScript(stream);
                        result = true;
                    }
                }
                else if (string.Compare(ext, ProjectEditorConfiguration.DefaultSvgExtension, StringComparison.OrdinalIgnoreCase) == 0)
                {
                    await using var stream = ServiceProvider.GetService<IFileSystem>()?.Open(path);
                    if (stream is { })
                    {
                        OnImportSvg(stream);
                        result = true;
                    }
                }
                else if (ProjectEditorConfiguration.DefaultImageExtensions.Any(r => string.Compare(ext, r, StringComparison.OrdinalIgnoreCase) == 0))
                {
                    await using var stream = ServiceProvider.GetService<IFileSystem>()?.Open(path);
                    if (stream is { })
                    {
                        var key = OnGetImageKey(stream, path);
                        if (key is { } && !string.IsNullOrEmpty(key))
                        {
                            OnDropImageKey(key, x, y);
                            result = true;
                        }
                    }
                }
            }

            return result;
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }

        return false;
    }

    public void OnDropImageKey(string key, double x, double y)
    {
        if (Project is null)
        {
            return;
        }
            
        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
        var selected = Project.CurrentStyleLibrary?.Selected ?? viewModelFactory?.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);
        var style = (ShapeStyleViewModel?)selected?.Copy(null);
        var layer = Project.CurrentContainer?.CurrentLayer;
        var sx = Project.Options is not null && Project.Options.SnapToGrid 
            ? PointUtil.Snap((decimal)x, (decimal)Project.Options.SnapX) 
            : (decimal)x;
        var sy = Project.Options is not null && Project.Options.SnapToGrid 
            ? PointUtil.Snap((decimal)y, (decimal)Project.Options.SnapY) 
            : (decimal)y;

        var image = viewModelFactory?.CreateImageShape((double)sx, (double)sy, style, key);
        if (image is { })
        {
            if (image.BottomRight is { })
            {
                image.BottomRight.X = (double)(sx + 320m);
                image.BottomRight.Y = (double)(sy + 180m);
            }

            Project.AddShape(layer, image);
        }
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
            ServiceProvider.GetService<ILog>()?.LogException(ex);
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
                ServiceProvider.GetService<ISelectionService>()?.Deselect(Project.CurrentContainer?.CurrentLayer);
                clone.Move(null, sx, sy);

                Project.AddShape(Project.CurrentContainer?.CurrentLayer, clone);

                // Select(Project?.CurrentContainer?.CurrentLayer, clone);

                if (Project.Options.TryToConnect)
                {
                    if (clone is GroupShapeViewModel group)
                    {
                        var shapes = Project?.CurrentContainer?.CurrentLayer?.Shapes.GetAllShapes<LineShapeViewModel>().ToList();
                        if (shapes is not null)
                        {
                            ServiceProvider.GetService<ISelectionService>()?.TryToConnectLines(shapes, group.Connectors);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
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
                    Project.OnApplyRecord(record);
                }
                return true;
            }
            else
            {
                var layer = Project.CurrentContainer?.CurrentLayer;
                if (layer is { })
                {
                    var shapes = layer.Shapes.Reverse();
                    var radius = Project.Options is not null && PageState is not null
                        ? Project.Options.HitThreshold / PageState.ZoomX
                        :  7.0;
                    var result = ServiceProvider.GetService<IHitTest>()?.TryToGetShape(shapes, new Point2(x, y), radius, PageState?.ZoomX ?? 1.0);
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
            ServiceProvider.GetService<ILog>()?.LogException(ex);
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

        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();

        var selected = Project.CurrentStyleLibrary?.Selected ?? viewModelFactory?.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);
        var style = (ShapeStyleViewModel?)selected?.Copy(null);
        var layer = Project.CurrentContainer?.CurrentLayer;
        var sx = Project.Options.SnapToGrid ? PointUtil.Snap((decimal)x, (decimal)Project.Options.SnapX) : (decimal)x;
        var sy = Project.Options.SnapToGrid ? PointUtil.Snap((decimal)y, (decimal)Project.Options.SnapY) : (decimal)y;
        var g = viewModelFactory?.CreateGroupShape(ProjectEditorConfiguration.DefaultGroupName);
        if (g is null)
        {
            return;
        }

        g.Record = record;

        var length = record.Values.Length;
        var px = (double)sx;
        var py = (double)sy;
        var width = 150.0;
        var height = 15.0;

        if (record.Owner is DatabaseViewModel { } db)
        {
            for (var i = 0; i < length; i++)
            {
                var column = db.Columns[i];
                if (!column.IsVisible)
                {
                    continue;
                }

                var binding = "{" + db.Columns[i].Name + "}";
                var text = viewModelFactory?.CreateTextShape(px, py, px + width, py + height, style, binding);
                if (text is { })
                {
                    g.AddShape(text);
                }

                py += height;
            }
        }

        var rectangle = viewModelFactory?.CreateRectangleShape((double)sx, (double)sy, (double)sx + width, (double)sy + (length * height), style);
        if (rectangle is { })
        {
            g.AddShape(rectangle);
        }

        var pt = viewModelFactory?.CreatePointShape((double)sx + (width / 2), (double)sy);
        if (pt is { })
        {
            g.AddConnectorAsNone(pt);
        }
            
        var pb = viewModelFactory?.CreatePointShape((double)sx + (width / 2), (double)sy + (length * height));
        if (pb is { })
        {
            g.AddConnectorAsNone(pb);
        }
            
        var pl = viewModelFactory?.CreatePointShape((double)sx, (double)sy + ((length * height) / 2));
        if (pl is { })
        {
            g.AddConnectorAsNone(pl);
        }
            
        var pr = viewModelFactory?.CreatePointShape((double)sx + width, (double)sy + ((length * height) / 2));
        if (pr is { })
        {
            g.AddConnectorAsNone(pr);
        }

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
                    Project.OnApplyStyle(style);
                }
                return true;
            }
            else
            {
                var layer = Project.CurrentContainer?.CurrentLayer;
                if (layer is { })
                {
                    var shapes = layer.Shapes.Reverse();
                    var radius = Project.Options is not null && PageState is not null
                        ? Project.Options.HitThreshold / PageState.ZoomX
                        :  7.0;
                    var result = ServiceProvider.GetService<IHitTest>()?.TryToGetShape(shapes, new Point2(x, y), radius, PageState?.ZoomX ?? 1.0);
                    if (result is { })
                    {
                        if (bExecute)
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
            ServiceProvider.GetService<ILog>()?.LogException(ex);
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
                    Project.OnApplyTemplate(template);
                }
                return true;
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
        return false;
    }
}
