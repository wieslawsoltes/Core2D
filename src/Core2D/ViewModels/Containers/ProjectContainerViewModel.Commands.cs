// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the GNU Affero General Public License v3.0. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Scripting;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Containers;

public partial class ProjectContainerViewModel
{
    [IgnoreDataMember]
    private object? ScriptState { get; set; }

    [IgnoreDataMember]
    public ICommand New { get; }

    [IgnoreDataMember]
    public ICommand AddStyleLibrary { get; }

    [IgnoreDataMember]
    public ICommand RemoveStyleLibrary { get; }

    [IgnoreDataMember]
    public ICommand ApplyStyle { get; }

    [IgnoreDataMember]
    public ICommand AddStyle { get; }

    [IgnoreDataMember]
    public ICommand RemoveStyle { get; }

    [IgnoreDataMember]
    public ICommand ExportStyle { get; }

    [IgnoreDataMember]
    public ICommand ApplyTemplate { get; }

    [IgnoreDataMember]
    public ICommand EditTemplate { get; }

    [IgnoreDataMember]
    public ICommand AddTemplate { get; }

    [IgnoreDataMember]
    public ICommand RemoveTemplate { get; }

    [IgnoreDataMember]
    public ICommand ExportTemplate { get; }

    [IgnoreDataMember]
    public ICommand AddGroupLibrary { get; }

    [IgnoreDataMember]
    public ICommand RemoveGroupLibrary { get; }

    [IgnoreDataMember]
    public ICommand AddGroup { get; }

    [IgnoreDataMember]
    public ICommand RemoveGroup { get; }

    [IgnoreDataMember]
    public ICommand EditGroup { get; }

    [IgnoreDataMember]
    public ICommand InsertGroup { get; }

    [IgnoreDataMember]
    public ICommand ExportGroup { get; }

    [IgnoreDataMember]
    public ICommand AddShape { get; }

    [IgnoreDataMember]
    public ICommand RemoveShape { get; }

    [IgnoreDataMember]
    public ICommand AddLayer { get; }

    [IgnoreDataMember]
    public ICommand RemoveLayer { get; }

    [IgnoreDataMember]
    public ICommand AddPage { get; }

    [IgnoreDataMember]
    public ICommand InsertPageBefore { get; }

    [IgnoreDataMember]
    public ICommand InsertPageAfter { get; }

    [IgnoreDataMember]
    public ICommand AddDocument { get; }

    [IgnoreDataMember]
    public ICommand InsertDocumentBefore { get; }

    [IgnoreDataMember]
    public ICommand InsertDocumentAfter { get; }

    [IgnoreDataMember]
    public ICommand AddDatabase { get; }

    [IgnoreDataMember]
    public ICommand RemoveDatabase { get; }

    [IgnoreDataMember]
    public ICommand AddColumn { get; }
        
    [IgnoreDataMember]
    public ICommand RemoveColumn { get; }
        
    [IgnoreDataMember]
    public ICommand AddRecord { get; }
        
    [IgnoreDataMember]
    public ICommand RemoveRecord { get; }
        
    [IgnoreDataMember]
    public ICommand ApplyRecord { get; }
        
    [IgnoreDataMember]
    public ICommand ResetRecord { get; }

    [IgnoreDataMember]
    public ICommand AddProperty { get; }
        
    [IgnoreDataMember]
    public ICommand RemoveProperty { get; }

    [IgnoreDataMember]
    public ICommand AddImageKey { get; }

    [IgnoreDataMember]
    public ICommand RemoveImageKey { get; }

    [IgnoreDataMember]
    public ICommand ResetRepl { get; }

    [IgnoreDataMember]
    public ICommand ExecuteRepl { get; }

    [IgnoreDataMember]
    public ICommand ExecuteCode { get; }

    [IgnoreDataMember]
    public ICommand ExecuteScript { get; }

    [IgnoreDataMember]
    public ICommand AddScript { get; }

    [IgnoreDataMember]
    public ICommand RemoveScript { get; }

    [IgnoreDataMember]
    public ICommand ExportScript { get; }

    public void OnNew(object? item)
    {
        ServiceProvider.GetService<ProjectEditorViewModel>()?.OnNew(item);
    }
        
    public void OnAddStyleLibrary()
    {
        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
        var sl = viewModelFactory?.CreateLibrary(ProjectEditorConfiguration.DefaultStyleLibraryName);
        if (sl is null)
        {
            return;
        }
            
        this.AddStyleLibrary(sl);
        SetCurrentStyleLibrary(sl);
    }

    public void OnRemoveStyleLibrary(LibraryViewModel? libraryViewModel)
    {
        this.RemoveStyleLibrary(libraryViewModel);
        SetCurrentStyleLibrary(StyleLibraries.FirstOrDefault());
    }

    public void OnApplyStyle(ShapeStyleViewModel? style)
    {
        if (style is null)
        {
            return;
        }

        if (!(SelectedShapes?.Count > 0))
        {
            return;
        }
            
        foreach (var shape in SelectedShapes)
        {
            this.ApplyStyle(shape, style);
        }
    }

    public void OnAddStyle()
    {
        if (SelectedShapes is { })
        {
            foreach (var shape in SelectedShapes)
            {
                if (shape.Style is { })
                {
                    var style = (ShapeStyleViewModel)shape.Style.Copy(null);
                    this.AddStyle(CurrentStyleLibrary, style);
                }
            }
        }
        else
        {
            var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
            var style = viewModelFactory?.CreateShapeStyle(ProjectEditorConfiguration.DefaultStyleName);
            if (style is { })
            {
                this.AddStyle(CurrentStyleLibrary, style);
            }
        }
    }

    public void OnRemoveStyle(ShapeStyleViewModel? style)
    {
        var library = this.RemoveStyle(style);
        library?.SetSelected(library.Items.FirstOrDefault());
    }

    public void OnExportStyle(ShapeStyleViewModel? style)
    {
        if (style is null)
        {
            return;
        }

        ServiceProvider.GetService<IProjectEditorPlatform>()?.OnExportJson(style);
    }

    public void OnApplyTemplate(TemplateContainerViewModel? template)
    {
        var container = CurrentContainer;
        if (container is PageContainerViewModel page)
        {
            this.ApplyTemplate(page, template);
            CurrentContainer?.InvalidateLayer();
        }
    }

    public void OnEditTemplate(TemplateContainerViewModel? template)
    {
        if (template is null)
        {
            return;
        }

        SetCurrentTemplate(template);

        var editor = ServiceProvider.GetService<ProjectEditorViewModel>();
        if (editor is { })
        {
            editor.OpenTemplate(template);
        }
        else
        {
            SetCurrentContainer(template);
            CurrentContainer?.InvalidateLayer();
        }
    }

    public void OnAddTemplate()
    {
        var containerFactory = ServiceProvider.GetService<IContainerFactory>();
        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
        var template = containerFactory?.GetTemplate(this, "Empty") 
                       ?? viewModelFactory?.CreateTemplateContainer(ProjectEditorConfiguration.DefaultTemplateName);
        if (template is { })
        {
            this.AddTemplate(template);
        }
    }

    public void OnRemoveTemplate(TemplateContainerViewModel? template)
    {
        if (template is null)
        {
            return;
        }
        this.RemoveTemplate(template);
        SetCurrentTemplate(Templates.FirstOrDefault());
    }

    public void OnExportTemplate(FrameContainerViewModel? template)
    {
        if (template is null)
        {
            return;
        }
            
        ServiceProvider.GetService<IProjectEditorPlatform>()?.OnExportJson(template);
    }

    public void OnAddGroupLibrary()
    {
        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
        var gl = viewModelFactory?.CreateLibrary(ProjectEditorConfiguration.DefaultGroupLibraryName);
        if (gl is null)
        {
            return;
        }

        this.AddGroupLibrary(gl);
        SetCurrentGroupLibrary(gl);
    }

    public void OnRemoveGroupLibrary(LibraryViewModel? libraryViewModel)
    {
        this.RemoveGroupLibrary(libraryViewModel);
        SetCurrentGroupLibrary(GroupLibraries.FirstOrDefault());
    }

    public void OnAddGroup()
    {
        if (SelectedShapes?.Count == 1 && SelectedShapes?.FirstOrDefault() is BlockShapeViewModel group)
        {
            var clone = group.CopyShared(new Dictionary<object, object>());
            if (clone is { })
            {
                this.AddGroup(CurrentGroupLibrary, clone);
            }
        }
    }

    public void OnRemoveGroup(BlockShapeViewModel? group)
    {
        if (group is null)
        {
            return;
        }
            
        var library = this.RemoveGroup(@group);
        library?.SetSelected(library.Items.FirstOrDefault());
    }

    public void OnEditGroup(BlockShapeViewModel? group)
    {
        if (group is null)
        {
            return;
        }

        var library = GroupLibraries.FirstOrDefault(l => l.Items.Contains(group));
        if (library is { })
        {
            SetCurrentGroupLibrary(library);
            library.SetSelected(group);
        }

        Selected = group;
        ServiceProvider.GetService<ProjectEditorViewModel>()?.OpenGroup(group);
    }

    public void OnInsertGroup(BlockShapeViewModel? group)
    {
        if (group is null)
        {
            return;
        }

        if (CurrentContainer is { })
        {
            ServiceProvider.GetService<ProjectEditorViewModel>()?.OnDropShapeAsClone(group, 0.0, 0.0);
        }
    }

    public void OnExportGroup(BlockShapeViewModel? group)
    {
        if (group is null)
        {
            return;
        }

        ServiceProvider.GetService<IProjectEditorPlatform>()?.OnExportJson(group);
    }

    public void OnAddShape(BaseShapeViewModel? shape)
    {
        var layer = CurrentContainer?.CurrentLayer;
        if (layer is { } && shape is { })
        {
            this.AddShape(layer, shape);
        }
    }

    public void OnRemoveShape(BaseShapeViewModel? shape)
    {
        var layer = CurrentContainer?.CurrentLayer;
        if (layer is null || shape is null)
        {
            return;
        }
        this.RemoveShape(layer, shape);
        if (CurrentContainer is { })
        {
            CurrentContainer.CurrentShape = layer.Shapes.FirstOrDefault();
        }
    }

    public void OnAddLayer(FrameContainerViewModel? container)
    {
        if (container is null)
        {
            return;
        }

        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
        this.AddLayer(container, viewModelFactory?.CreateLayerContainer(ProjectEditorConfiguration.DefaultLayerName, container));
    }

    public void OnRemoveLayer(LayerContainerViewModel? layer)
    {
        if (layer is null)
        {
            return;
        }
            
        this.RemoveLayer(layer);
        if (layer.Owner is FrameContainerViewModel owner)
        {
            owner.SetCurrentLayer(owner.Layers.FirstOrDefault());
        }
    }

    public void OnAddPage(object? item)
    {
        if (CurrentDocument is null)
        {
            return;
        }
            
        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
        var containerFactory = ServiceProvider.GetService<IContainerFactory>();
        var page =
            containerFactory?.GetPage(this, ProjectEditorConfiguration.DefaultPageName)
            ?? viewModelFactory?.CreatePageContainer(ProjectEditorConfiguration.DefaultPageName);
        if (page is null)
        {
            return;
        }

        this.AddPage(CurrentDocument, page);
        SetCurrentContainer(page);
    }

    public void OnInsertPageBefore(object? item)
    {
        if (CurrentDocument is null)
        {
            return;
        }

        if (item is not PageContainerViewModel selected)
        {
            return;
        }
        var index = CurrentDocument.Pages.IndexOf(selected);

        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
        var containerFactory = ServiceProvider.GetService<IContainerFactory>();
        var page =
            containerFactory?.GetPage(this, ProjectEditorConfiguration.DefaultPageName)
            ?? viewModelFactory?.CreatePageContainer(ProjectEditorConfiguration.DefaultPageName);
        if (page is null)
        {
            return;
        }

        this.AddPageAt(CurrentDocument, page, index);
        SetCurrentContainer(page);
    }

    public void OnInsertPageAfter(object? item)
    {
        if (CurrentDocument is null)
        {
            return;
        }

        if (item is not PageContainerViewModel selected)
        {
            return;
        }
        var index = CurrentDocument.Pages.IndexOf(selected);
            

        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
        var containerFactory = ServiceProvider.GetService<IContainerFactory>();
        var page =
            containerFactory?.GetPage(this, ProjectEditorConfiguration.DefaultPageName)
            ?? viewModelFactory?.CreatePageContainer(ProjectEditorConfiguration.DefaultPageName);
        if (page is null)
        {
            return;
        }

        this.AddPageAt(CurrentDocument, page, index + 1);
        SetCurrentContainer(page);
    }

    public void OnAddDocument(object? item)
    {
        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
        var containerFactory = ServiceProvider.GetService<IContainerFactory>();
        var document =
            containerFactory?.GetDocument(this, ProjectEditorConfiguration.DefaultDocumentName)
            ?? viewModelFactory?.CreateDocumentContainer(ProjectEditorConfiguration.DefaultDocumentName);
        if (document is null)
        {
            return;
        }
            
        this.AddDocument(document);
        SetCurrentDocument(document);
        SetCurrentContainer(document.Pages.FirstOrDefault());
    }

    public void OnInsertDocumentBefore(object? item)
    {
        if (item is not DocumentContainerViewModel selected)
        {
            return;
        }
            
        var index = Documents.IndexOf(selected);

        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
        var containerFactory = ServiceProvider.GetService<IContainerFactory>();
        var document =
            containerFactory?.GetDocument(this, ProjectEditorConfiguration.DefaultDocumentName)
            ?? viewModelFactory?.CreateDocumentContainer(ProjectEditorConfiguration.DefaultDocumentName);
        if (document is null)
        {
            return;
        }

        this.AddDocumentAt(document, index);
        SetCurrentDocument(document);
        SetCurrentContainer(document.Pages.FirstOrDefault());
    }

    public void OnInsertDocumentAfter(object? item)
    {
        if (item is not DocumentContainerViewModel selected)
        {
            return;
        }
        var index = Documents.IndexOf(selected);

        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
        var containerFactory = ServiceProvider.GetService<IContainerFactory>();
        var document =
            containerFactory?.GetDocument(this, ProjectEditorConfiguration.DefaultDocumentName)
            ?? viewModelFactory?.CreateDocumentContainer(ProjectEditorConfiguration.DefaultDocumentName);
        if (document is null)
        {
            return;
        }

        this.AddDocumentAt(document, index + 1);
        SetCurrentDocument(document);
        SetCurrentContainer(document.Pages.FirstOrDefault());
    }

    public void OnAddDatabase()
    {
        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
        var db = viewModelFactory?.CreateDatabase(ProjectEditorConfiguration.DefaultDatabaseName);
        if (db is null)
        {
            return;
        }
        this.AddDatabase(db);
        SetCurrentDatabase(db);
    }

    public void OnRemoveDatabase(DatabaseViewModel? db)
    {
        if (db is null)
        {
            return;
        }
        this.RemoveDatabase(db);
        SetCurrentDatabase(Databases.FirstOrDefault());
    }

    public void OnAddColumn(DatabaseViewModel? db)
    {
        if (db is null)
        {
            return;
        }
        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
        this.AddColumn(db, viewModelFactory?.CreateColumn(db, ProjectEditorConfiguration.DefaultColumnName));
    }

    public void OnRemoveColumn(ColumnViewModel? column)
    {
        this.RemoveColumn(column);
    }

    public void OnAddRecord(DatabaseViewModel? db)
    {
        if (db is null)
        {
            return;
        }
        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
        this.AddRecord(db, viewModelFactory?.CreateRecord(db, ProjectEditorConfiguration.DefaultValue));
    }

    public void OnRemoveRecord(RecordViewModel? record)
    {
        this.RemoveRecord(record);
    }

    public void OnApplyRecord(RecordViewModel? record)
    {
        if (record is null)
        {
            return;
        }
            
        if (SelectedShapes?.Count > 0)
        {
            foreach (var shape in SelectedShapes)
            {
                this.ApplyRecord(shape, record);
            }
        }

        if (SelectedShapes is null)
        {
            var container = CurrentContainer;
            if (container is { })
            {
                this.ApplyRecord(container, record);
            }
        }
    }

    public void OnResetRecord(IDataObject? data)
    {
        this.ResetRecord(data);
    }

    public void OnAddProperty(ViewModelBase? owner)
    {
        if (owner is not IDataObject data)
        {
            return;
        }

        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
        this.AddProperty(data, viewModelFactory?.CreateProperty(owner, ProjectEditorConfiguration.DefaultPropertyName, ProjectEditorConfiguration.DefaultValue));
    }

    public void OnRemoveProperty(PropertyViewModel? property)
    {
        this.RemoveProperty(property);
    }

    public async Task<string?> OnAddImageKey(string? path)
    {
        var imageImporter = ServiceProvider.GetService<IImageImporter>();
        var fileSystem = ServiceProvider.GetService<IFileSystem>();
            
        if (path is null || string.IsNullOrEmpty(path))
        {
            var key = await (imageImporter?.GetImageKeyAsync() ?? Task.FromResult(default(string)));
            if (key is null || string.IsNullOrEmpty(key))
            {
                return default;
            }

            return key;
        }

        using var stream = fileSystem?.Open(path);
        if (stream is null)
        {
            return default;
        }

        var bytes = fileSystem?.ReadBinary(stream);
        if (bytes is null)
        {
            return default;
        }

        return AddImageFromFile(path, bytes);
    }

    public void OnRemoveImageKey(string? key)
    {
        if (key is null)
        {
            return;
        }

        RemoveImage(key);
    }

    public void OnResetRepl()
    {
        ScriptState = null;
    }
        
    public async Task OnExecuteRepl(string? code)
    {
        var scriptRunner = ServiceProvider.GetService<IScriptRunner>();
        if (scriptRunner is null)
        {
            return;
        }

        try
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                ScriptState = await scriptRunner.Execute(code, ScriptState);
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public async Task OnExecuteCode(string? code)
    {
        var scriptRunner = ServiceProvider.GetService<IScriptRunner>();
        if (scriptRunner is null)
        {
            return;
        }
            
        try
        {
            if (!string.IsNullOrWhiteSpace(code))
            {
                await scriptRunner.Execute(code, null);
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public async Task OnExecuteScript(ScriptViewModel? script)
    {
        try
        {
            var code = script?.Code;
            if (!string.IsNullOrWhiteSpace(code))
            {
                await OnExecuteRepl(code);
            }
        }
        catch (Exception ex)
        {
            ServiceProvider.GetService<ILog>()?.LogException(ex);
        }
    }

    public void OnAddScript()
    {
        var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
        var script = viewModelFactory?.CreateScript(ProjectEditorConfiguration.DefaultScriptName);
        this.AddScript(script);
    }

    public void OnRemoveScript(ScriptViewModel? script)
    {
        if (script is null)
        {
            return;
        }

        this.RemoveScript(script);
        SetCurrentScript(Scripts.FirstOrDefault());
    }

    public void OnExportScript(ScriptViewModel? script)
    {
        if (script is null)
        {
            return;
        }

        ServiceProvider.GetService<IProjectEditorPlatform>()?.OnExportJson(script);
    }
}
