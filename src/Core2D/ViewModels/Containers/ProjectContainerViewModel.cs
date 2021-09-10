#nullable enable
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Disposables;
using System.Threading.Tasks;
using System.Windows.Input;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.History;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Scripting;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;

namespace Core2D.ViewModels.Containers
{
    public partial class ProjectContainerViewModel : BaseContainerViewModel, ISelection
    {
        [AutoNotify] private OptionsViewModel? _options;
        [AutoNotify] private IHistory? _history;
        [AutoNotify] private ImmutableArray<LibraryViewModel> _styleLibraries;
        [AutoNotify] private ImmutableArray<LibraryViewModel> _groupLibraries;
        [AutoNotify] private ImmutableArray<DatabaseViewModel> _databases;
        [AutoNotify] private ImmutableArray<TemplateContainerViewModel> _templates;
        [AutoNotify] private ImmutableArray<ScriptViewModel> _scripts;
        [AutoNotify] private ImmutableArray<DocumentContainerViewModel> _documents;
        [AutoNotify] private LibraryViewModel? _currentStyleLibrary;
        [AutoNotify] private LibraryViewModel? _currentGroupLibrary;
        [AutoNotify] private DatabaseViewModel? _currentDatabase;
        [AutoNotify] private TemplateContainerViewModel? _currentTemplate;
        [AutoNotify] private ScriptViewModel? _currentScript;
        [AutoNotify] private DocumentContainerViewModel? _currentDocument;
        [AutoNotify] private FrameContainerViewModel? _currentContainer;
        [AutoNotify] private ViewModelBase? _selected;
        [AutoNotify(IgnoreDataMember = true)] private ISet<BaseShapeViewModel>? _selectedShapes;

        public ProjectContainerViewModel(IServiceProvider serviceProvider) : base(serviceProvider)
        {
            AddStyleLibrary = new Command(OnAddStyleLibrary);

            RemoveStyleLibrary = new Command<LibraryViewModel?>(OnRemoveStyleLibrary);

            ApplyStyle = new Command<ShapeStyleViewModel?>(OnApplyStyle);

            AddStyle = new Command(OnAddStyle);

            ExportStyle = new Command<ShapeStyleViewModel?>(OnExportStyle);

            RemoveStyle = new Command<ShapeStyleViewModel?>(OnRemoveStyle);

            ApplyTemplate = new Command<TemplateContainerViewModel?>(OnApplyTemplate);

            EditTemplate = new Command<TemplateContainerViewModel?>(OnEditTemplate);

            AddTemplate = new Command(OnAddTemplate);

            RemoveTemplate = new Command<TemplateContainerViewModel?>(OnRemoveTemplate);

            ExportTemplate = new Command<TemplateContainerViewModel?>(OnExportTemplate);

            AddGroupLibrary = new Command(OnAddGroupLibrary);

            RemoveGroupLibrary = new Command<LibraryViewModel?>(OnRemoveGroupLibrary);

            AddGroup = new Command(OnAddGroup);

            RemoveGroup = new Command<GroupShapeViewModel?>(OnRemoveGroup);

            InsertGroup = new Command<GroupShapeViewModel?>(OnInsertGroup);

            ExportGroup = new Command<GroupShapeViewModel?>(OnExportGroup);

            AddDatabase = new Command(OnAddDatabase);

            RemoveDatabase = new Command<DatabaseViewModel?>(OnRemoveDatabase);

            AddImageKey = new Command<string?>(async path => await OnAddImageKey(path));

            RemoveImageKey = new Command<string?>(OnRemoveImageKey);

            ResetRepl = new Command(OnResetRepl);

            ExecuteRepl = new Command<string?>(async code => await OnExecuteRepl(code));

            ExecuteCode = new Command<string?>(async code => await OnExecuteCode(code));

            ExecuteScript = new Command<ScriptViewModel?>(async script => await OnExecuteScript(script));

            AddScript = new Command(OnAddScript);

            RemoveScript = new Command<ScriptViewModel?>(OnRemoveScript);

            ExportScript = new Command<ScriptViewModel?>(OnExportScript);

            PropertyChanged += (_, e) =>
            {
                if (e.PropertyName == nameof(Selected))
                {
                    SetSelected(Selected);
                }
            };
        }

        private object? ScriptState { get; set; }

        public ICommand AddStyleLibrary { get; }

        public ICommand RemoveStyleLibrary { get; }

        public ICommand ApplyStyle { get; }

        public ICommand AddStyle { get; }

        public ICommand RemoveStyle { get; }

        public ICommand ExportStyle { get; }

        public ICommand ApplyTemplate { get; }

        public ICommand EditTemplate { get; }

        public ICommand AddTemplate { get; }

        public ICommand RemoveTemplate { get; }

        public ICommand ExportTemplate { get; }

        public ICommand AddGroupLibrary { get; }

        public ICommand RemoveGroupLibrary { get; }

        public ICommand AddGroup { get; }

        public ICommand RemoveGroup { get; }

        public ICommand InsertGroup { get; }

        public ICommand ExportGroup { get; }

        public ICommand AddDatabase { get; }

        public ICommand RemoveDatabase { get; }

        public ICommand AddImageKey { get; }

        public ICommand RemoveImageKey { get; }

        public ICommand ResetRepl { get; }

        public ICommand ExecuteRepl { get; }

        public ICommand ExecuteCode { get; }

        public ICommand ExecuteScript { get; }

        public ICommand AddScript { get; }

        public ICommand RemoveScript { get; }

        public ICommand ExportScript { get; }

        public override object Copy(IDictionary<object, object>? shared)
        {
            var styleLibraries = _styleLibraries.CopyShared(shared).ToImmutable();
            var groupLibraries = _groupLibraries.CopyShared(shared).ToImmutable();
            var databases = _databases.CopyShared(shared).ToImmutable();
            var templates = _templates.CopyShared(shared).ToImmutable();
            var scripts = _scripts.CopyShared(shared).ToImmutable();
            var documents = _documents.CopyShared(shared).ToImmutable();

            var currentStyleLibrary = _currentStyleLibrary.GetCurrentItem(ref _styleLibraries, ref styleLibraries);  
            var currentGroupLibrary = _currentGroupLibrary.GetCurrentItem(ref _styleLibraries, ref styleLibraries); 
            var currentDatabase = _currentDatabase.GetCurrentItem(ref _databases, ref databases); 
            var currentTemplate = _currentTemplate.GetCurrentItem(ref _templates, ref templates); 
            var currentScript = _currentScript.GetCurrentItem(ref _scripts, ref scripts); 
            var currentDocument = _currentDocument.GetCurrentItem(ref _documents, ref documents);
            var currentContainer = _currentContainer switch
            {
                PageContainerViewModel page => page.GetCurrentItem(ref _documents, ref documents, x => x.Pages),
                TemplateContainerViewModel template => template.GetCurrentItem(ref _templates, ref templates),
                _ => default(FrameContainerViewModel?)
            };

            var copy = new ProjectContainerViewModel(ServiceProvider)
            {
                Options = _options?.CopyShared(shared),
                StyleLibraries = styleLibraries,
                GroupLibraries = groupLibraries,
                Databases = databases,
                Templates = templates,
                Scripts = scripts,
                Documents = documents,
                CurrentStyleLibrary = currentStyleLibrary,
                CurrentGroupLibrary = currentGroupLibrary,
                CurrentDatabase = currentDatabase,
                CurrentTemplate = currentTemplate,
                CurrentScript = currentScript,
                CurrentDocument = currentDocument,
                CurrentContainer = currentContainer
            };

            if (_selected is { } && shared is { })
            {
                if (shared.TryGetValue(_selected, out var selected))
                {
                    copy.Selected = selected as ViewModelBase;
                }
            }

            copy.History = null;

            return copy;
        }

        private void SetSelected(ViewModelBase? value)
        {
            Debug.WriteLine($"[SetSelected] {value?.Name} ({value?.GetType()})");
            if (value is BaseShapeViewModel shape)
            {
                var layer = _documents
                    .SelectMany(x => x.Pages)
                    .SelectMany(x => x.Layers)
                    .FirstOrDefault(l => l.Shapes.Contains(shape));

                var container = _documents
                    .SelectMany(x => x.Pages)
                    .FirstOrDefault(c => layer is not null && c.Layers.Contains(layer));

                if (container is { } && layer is { })
                {
                    if (container.CurrentLayer != layer)
                    {
                        Debug.WriteLine($"  [CurrentLayer] {layer.Name}");
                        container.CurrentLayer = layer;
                    }

                    if (CurrentContainer != container)
                    {
                        Debug.WriteLine($"  [CurrentContainer] {container.Name}");
                        CurrentContainer = container;
                        CurrentContainer.InvalidateLayer();
                    }
                }

                // SelectedShapes = new HashSet<BaseShapeViewModel>() { shape };
            }
            else if (value is LayerContainerViewModel layer)
            {
                var container = _documents
                    .SelectMany(x => x.Pages)
                    .FirstOrDefault(c => c.Layers.Contains(layer));

                if (container is { })
                {
                    if (container.CurrentLayer != layer)
                    {
                        Debug.WriteLine($"  [CurrentLayer] {layer.Name}");
                        container.CurrentLayer = layer;
                    }

                    if (CurrentContainer != container)
                    {
                        Debug.WriteLine($"  [CurrentContainer] {container.Name}");
                        CurrentContainer = container;
                        CurrentContainer.InvalidateLayer();
                    }
                }
            }
            else if (value is FrameContainerViewModel container)
            {
                var document = _documents.FirstOrDefault(d => d.Pages.Contains(container));
                if (document is { })
                {
                    if (CurrentDocument != document)
                    {
                        Debug.WriteLine($"  [CurrentDocument] {document.Name}");
                        CurrentDocument = document;
                    }

                    if (CurrentContainer != container)
                    {
                        Debug.WriteLine($"  [CurrentContainer] {container.Name}");
                        CurrentContainer = container;
                        CurrentContainer.InvalidateLayer();
                    }
                }
            }
            else if (value is DocumentContainerViewModel document)
            {
                if (CurrentDocument != document)
                {
                    Debug.WriteLine($"  [CurrentDocument] {document.Name}");
                    CurrentDocument = document;
                    if (!CurrentDocument.Pages.Contains(CurrentContainer))
                    {
                        var current = CurrentDocument.Pages.FirstOrDefault();
                        if (CurrentContainer != current)
                        {
                            Debug.WriteLine($"  [CurrentContainer] {current?.Name}");
                            CurrentContainer = current;
                            CurrentContainer?.InvalidateLayer();
                        }
                    }
                }
            }
        }

        public void SetCurrentDocument(DocumentContainerViewModel? document)
        {
            CurrentDocument = document;
            Selected = document;
        }

        public void SetCurrentContainer(FrameContainerViewModel? container)
        {
            CurrentContainer = container;
            Selected = container;
        }

        public void SetCurrentTemplate(TemplateContainerViewModel? template) => CurrentTemplate = template;

        public void SetCurrentScript(ScriptViewModel? script) => CurrentScript = script;

        public void SetCurrentDatabase(DatabaseViewModel? db) => CurrentDatabase = db;

        public void SetCurrentGroupLibrary(LibraryViewModel? libraryViewModel) => CurrentGroupLibrary = libraryViewModel;

        public void SetCurrentStyleLibrary(LibraryViewModel? libraryViewModel) => CurrentStyleLibrary = libraryViewModel;

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            if (_options != null)
            {
                isDirty |= _options.IsDirty();
            }

            foreach (var styleLibrary in _styleLibraries)
            {
                isDirty |= styleLibrary.IsDirty();
            }

            foreach (var groupLibrary in _groupLibraries)
            {
                isDirty |= groupLibrary.IsDirty();
            }

            foreach (var database in _databases)
            {
                isDirty |= database.IsDirty();
            }

            foreach (var template in _templates)
            {
                isDirty |= template.IsDirty();
            }

            foreach (var script in _scripts)
            {
                isDirty |= script.IsDirty();
            }

            foreach (var document in _documents)
            {
                isDirty |= document.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            _options?.Invalidate();

            foreach (var styleLibrary in _styleLibraries)
            {
                styleLibrary.Invalidate();
            }

            foreach (var groupLibrary in _groupLibraries)
            {
                groupLibrary.Invalidate();
            }

            foreach (var database in _databases)
            {
                database.Invalidate();
            }

            foreach (var template in _templates)
            {
                template.Invalidate();
            }

            foreach (var script in _scripts)
            {
                script.Invalidate();
            }

            foreach (var document in _documents)
            {
                document.Invalidate();
            }
        }

        public override IDisposable Subscribe(IObserver<(object? sender, PropertyChangedEventArgs e)> observer)
        {
            var mainDisposable = new CompositeDisposable();
            var disposablePropertyChanged = default(IDisposable);
            var disposableOptions = default(IDisposable);
            var disposableStyleLibraries = default(CompositeDisposable);
            var disposableGroupLibraries = default(CompositeDisposable);
            var disposableDatabases = default(CompositeDisposable);
            var disposableTemplates = default(CompositeDisposable);
            var disposableScripts = default(CompositeDisposable);
            var disposableDocuments = default(CompositeDisposable);

            ObserveSelf(Handler, ref disposablePropertyChanged, mainDisposable);
            ObserveObject(_options, ref disposableOptions, mainDisposable, observer);
            ObserveList(_styleLibraries, ref disposableStyleLibraries, mainDisposable, observer);
            ObserveList(_groupLibraries, ref disposableGroupLibraries, mainDisposable, observer);
            ObserveList(_databases, ref disposableDatabases, mainDisposable, observer);
            ObserveList(_templates, ref disposableTemplates, mainDisposable, observer);
            ObserveList(_scripts, ref disposableScripts, mainDisposable, observer);
            ObserveList(_documents, ref disposableDocuments, mainDisposable, observer);

            void Handler(object? sender, PropertyChangedEventArgs e)
            {
                if (e.PropertyName == nameof(Options))
                {
                    ObserveObject(_options, ref disposableOptions, mainDisposable, observer);
                }

                if (e.PropertyName == nameof(StyleLibraries))
                {
                    ObserveList(_styleLibraries, ref disposableStyleLibraries, mainDisposable, observer);
                }

                if (e.PropertyName == nameof(GroupLibraries))
                {
                    ObserveList(_groupLibraries, ref disposableGroupLibraries, mainDisposable, observer);
                }

                if (e.PropertyName == nameof(Databases))
                {
                    ObserveList(_databases, ref disposableDatabases, mainDisposable, observer);
                }

                if (e.PropertyName == nameof(Templates))
                {
                    ObserveList(_templates, ref disposableTemplates, mainDisposable, observer);
                }

                if (e.PropertyName == nameof(Scripts))
                {
                    ObserveList(_scripts, ref disposableScripts, mainDisposable, observer);
                }

                if (e.PropertyName == nameof(Documents))
                {
                    ObserveList(_documents, ref disposableDocuments, mainDisposable, observer);
                }

                observer.OnNext((sender, e));
            }

            return mainDisposable;
        }

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
