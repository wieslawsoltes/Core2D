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

namespace Core2D.ViewModels.Containers
{
    public partial class ProjectContainerViewModel
    {
        [IgnoreDataMember]
        private object? ScriptState { get; set; }

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
        public ICommand InsertGroup { get; }

        [IgnoreDataMember]
        public ICommand ExportGroup { get; }

        [IgnoreDataMember]
        public ICommand AddDatabase { get; }

        [IgnoreDataMember]
        public ICommand RemoveDatabase { get; }

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

        public void OnAddStyleLibrary()
        {
            var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
            var sl = viewModelFactory?.CreateLibrary(ProjectEditorConfiguration.DefaulStyleLibraryName);
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
                var style = viewModelFactory?.CreateShapeStyle(ProjectEditorConfiguration.DefaulStyleName);
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

            ServiceProvider.GetService<IProjectEditorPlatform>().OnExportObject(style);
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

        public void OnEditTemplate(FrameContainerViewModel? template)
        {
            if (template is null)
            {
                return;
            }

            SetCurrentContainer(template);
            CurrentContainer?.InvalidateLayer();
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
            
            ServiceProvider.GetService<IProjectEditorPlatform>().OnExportObject(template);
        }

        public void OnAddGroupLibrary()
        {
            var viewModelFactory = ServiceProvider.GetService<IViewModelFactory>();
            var gl = viewModelFactory?.CreateLibrary(ProjectEditorConfiguration.DefaulGroupLibraryName);
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
            if (SelectedShapes?.Count == 1 && SelectedShapes?.FirstOrDefault() is GroupShapeViewModel group)
            {
                var clone = group.CopyShared(new Dictionary<object, object>());
                if (clone is { })
                {
                    this.AddGroup(CurrentGroupLibrary, clone);
                }
            }
        }

        public void OnRemoveGroup(GroupShapeViewModel? group)
        {
            if (group is null)
            {
                return;
            }
            
            var library = this.RemoveGroup(@group);
            library?.SetSelected(library.Items.FirstOrDefault());
        }

        public void OnInsertGroup(GroupShapeViewModel? group)
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

        public void OnExportGroup(GroupShapeViewModel? group)
        {
            if (group is null)
            {
                return;
            }

            ServiceProvider.GetService<IProjectEditorPlatform>().OnExportObject(group);
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
            this.RemoveDatabase(db);
            SetCurrentDatabase(Databases.FirstOrDefault());
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

            ServiceProvider.GetService<IProjectEditorPlatform>().OnExportObject(script);
        }
    }
}
