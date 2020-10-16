using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Data;
using Core2D.History;
using Core2D.Scripting;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Containers
{
    public partial class ProjectContainer : BaseContainer
    {
        private Options _options;
        private IHistory _history;
        private ImmutableArray<Library<ShapeStyle>> _styleLibraries;
        private ImmutableArray<Library<GroupShape>> _groupLibraries;
        private ImmutableArray<Database> _databases;
        private ImmutableArray<PageContainer> _templates;
        private ImmutableArray<Script> _scripts;
        private ImmutableArray<DocumentContainer> _documents;
        private Library<ShapeStyle> _currentStyleLibrary;
        private Library<GroupShape> _currentGroupLibrary;
        private Database _currentDatabase;
        private PageContainer _currentTemplate;
        private Script _currentScript;
        private DocumentContainer _currentDocument;
        private PageContainer _currentContainer;
        private ObservableObject _selected;

        public Options Options
        {
            get => _options;
            set => RaiseAndSetIfChanged(ref _options, value);
        }

        public IHistory History
        {
            get => _history;
            set => RaiseAndSetIfChanged(ref _history, value);
        }

        public ImmutableArray<Library<ShapeStyle>> StyleLibraries
        {
            get => _styleLibraries;
            set => RaiseAndSetIfChanged(ref _styleLibraries, value);
        }

        public ImmutableArray<Library<GroupShape>> GroupLibraries
        {
            get => _groupLibraries;
            set => RaiseAndSetIfChanged(ref _groupLibraries, value);
        }

        public ImmutableArray<Database> Databases
        {
            get => _databases;
            set => RaiseAndSetIfChanged(ref _databases, value);
        }

        public ImmutableArray<PageContainer> Templates
        {
            get => _templates;
            set => RaiseAndSetIfChanged(ref _templates, value);
        }

        public ImmutableArray<Script> Scripts
        {
            get => _scripts;
            set => RaiseAndSetIfChanged(ref _scripts, value);
        }

        public ImmutableArray<DocumentContainer> Documents
        {
            get => _documents;
            set => RaiseAndSetIfChanged(ref _documents, value);
        }

        public Library<ShapeStyle> CurrentStyleLibrary
        {
            get => _currentStyleLibrary;
            set => RaiseAndSetIfChanged(ref _currentStyleLibrary, value);
        }

        public Library<GroupShape> CurrentGroupLibrary
        {
            get => _currentGroupLibrary;
            set => RaiseAndSetIfChanged(ref _currentGroupLibrary, value);
        }

        public Database CurrentDatabase
        {
            get => _currentDatabase;
            set => RaiseAndSetIfChanged(ref _currentDatabase, value);
        }

        public PageContainer CurrentTemplate
        {
            get => _currentTemplate;
            set => RaiseAndSetIfChanged(ref _currentTemplate, value);
        }

        public Script CurrentScript
        {
            get => _currentScript;
            set => RaiseAndSetIfChanged(ref _currentScript, value);
        }

        public DocumentContainer CurrentDocument
        {
            get => _currentDocument;
            set => RaiseAndSetIfChanged(ref _currentDocument, value);
        }

        public PageContainer CurrentContainer
        {
            get => _currentContainer;
            set => RaiseAndSetIfChanged(ref _currentContainer, value);
        }

        public ObservableObject Selected
        {
            get => _selected;
            set
            {
                SetSelected(value);
                RaiseAndSetIfChanged(ref _selected, value);
            }
        }

        public static IEnumerable<BaseShape> GetAllShapes(IEnumerable<BaseShape> shapes)
        {
            if (shapes == null)
            {
                yield break;
            }

            foreach (var shape in shapes)
            {
                if (shape is GroupShape groupShape)
                {
                    foreach (var s in GetAllShapes(groupShape.Shapes))
                    {
                        yield return s;
                    }

                    yield return shape;
                }
                else
                {
                    yield return shape;
                }
            }
        }

        public static IEnumerable<T> GetAllShapes<T>(IEnumerable<BaseShape> shapes)
        {
            return GetAllShapes(shapes)?.Where(s => s is T).Cast<T>();
        }

        public static IEnumerable<T> GetAllShapes<T>(ProjectContainer project)
        {
            var shapes = project?.Documents
                .SelectMany(d => d.Pages)
                .SelectMany(c => c.Layers)
                .SelectMany(l => l.Shapes);

            return GetAllShapes(shapes)?.Where(s => s is T).Cast<T>();
        }

        public void SetCurrentDocument(DocumentContainer document)
        {
            CurrentDocument = document;
            Selected = document;
        }

        public void SetCurrentContainer(PageContainer container)
        {
            CurrentContainer = container;
            Selected = container;
        }

        public void SetCurrentTemplate(PageContainer template) => CurrentTemplate = template;

        public void SetCurrentScript(Script script) => CurrentScript = script;

        public void SetCurrentDatabase(Database db) => CurrentDatabase = db;

        public void SetCurrentGroupLibrary(Library<GroupShape> library) => CurrentGroupLibrary = library;

        public void SetCurrentStyleLibrary(Library<ShapeStyle> library) => CurrentStyleLibrary = library;

        public void SetSelected(ObservableObject value)
        {
            if (value is LayerContainer layer)
            {
                if (layer.Owner is PageContainer owner)
                {
                    if (owner.CurrentLayer != layer)
                    {
                        owner.CurrentLayer = layer;
                    }
                }
            }
            else if (value is PageContainer container && _documents != null)
            {
                var document = _documents.FirstOrDefault(d => d.Pages.Contains(container));
                if (document != null)
                {
                    if (CurrentDocument != document)
                    {
                        CurrentDocument = document;
                    }

                    if (CurrentContainer != container)
                    {
                        CurrentContainer = container;
                        CurrentContainer.InvalidateLayer();
                    }
                }
            }
            else if (value is DocumentContainer document)
            {
                if (CurrentDocument != document)
                {
                    CurrentDocument = document;
                    if (!CurrentDocument?.Pages.Contains(CurrentContainer) ?? false)
                    {
                        var current = CurrentDocument.Pages.FirstOrDefault();
                        if (CurrentContainer != current)
                        {
                            CurrentContainer = current;
                        }
                    }
                }
            }
        }

        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        public override bool IsDirty()
        {
            var isDirty = base.IsDirty();

            isDirty |= Options.IsDirty();

            foreach (var styleLibrary in StyleLibraries)
            {
                isDirty |= styleLibrary.IsDirty();
            }

            foreach (var groupLibrary in GroupLibraries)
            {
                isDirty |= groupLibrary.IsDirty();
            }

            foreach (var database in Databases)
            {
                isDirty |= database.IsDirty();
            }

            foreach (var template in Templates)
            {
                isDirty |= template.IsDirty();
            }

            foreach (var script in Scripts)
            {
                isDirty |= script.IsDirty();
            }

            foreach (var document in Documents)
            {
                isDirty |= document.IsDirty();
            }

            return isDirty;
        }

        public override void Invalidate()
        {
            base.Invalidate();

            Options.Invalidate();

            foreach (var styleLibrary in StyleLibraries)
            {
                styleLibrary.Invalidate();
            }

            foreach (var groupLibrary in GroupLibraries)
            {
                groupLibrary.Invalidate();
            }

            foreach (var database in Databases)
            {
                database.Invalidate();
            }

            foreach (var template in Templates)
            {
                template.Invalidate();
            }

            foreach (var script in Scripts)
            {
                script.Invalidate();
            }

            foreach (var document in Documents)
            {
                document.Invalidate();
            }
        }

        public virtual bool ShouldSerializeOptions() => _options != null;

        public virtual bool ShouldSerializeHistory() => false;

        public virtual bool ShouldSerializeStyleLibraries() => true;

        public virtual bool ShouldSerializeGroupLibraries() => true;

        public virtual bool ShouldSerializeDatabases() => true;

        public virtual bool ShouldSerializeTemplates() => true;

        public virtual bool ShouldSerializeScripts() => true;

        public virtual bool ShouldSerializeDocuments() => true;

        public virtual bool ShouldSerializeCurrentStyleLibrary() => _currentStyleLibrary != null;

        public virtual bool ShouldSerializeCurrentGroupLibrary() => _currentGroupLibrary != null;

        public virtual bool ShouldSerializeCurrentDatabase() => _currentDatabase != null;

        public virtual bool ShouldSerializeCurrentTemplate() => _currentTemplate != null;

        public virtual bool ShouldSerializeCurrentScript() => _currentScript != null;

        public virtual bool ShouldSerializeCurrentDocument() => _currentDocument != null;

        public virtual bool ShouldSerializeCurrentContainer() => _currentContainer != null;

        public virtual bool ShouldSerializeSelected() => _selected != null;
    }
}
