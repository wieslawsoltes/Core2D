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
    /// <summary>
    /// Project container.
    /// </summary>
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

        /// <summary>
        /// Gets or sets project options.
        /// </summary>
        public Options Options
        {
            get => _options;
            set => RaiseAndSetIfChanged(ref _options, value);
        }

        /// <summary>
        /// Gets or sets undo/redo history handler.
        /// </summary>
        public IHistory History
        {
            get => _history;
            set => RaiseAndSetIfChanged(ref _history, value);
        }

        /// <summary>
        /// Gets or sets project style libraries.
        /// </summary>
        public ImmutableArray<Library<ShapeStyle>> StyleLibraries
        {
            get => _styleLibraries;
            set => RaiseAndSetIfChanged(ref _styleLibraries, value);
        }

        /// <summary>
        /// Gets or sets project group libraries.
        /// </summary>
        public ImmutableArray<Library<GroupShape>> GroupLibraries
        {
            get => _groupLibraries;
            set => RaiseAndSetIfChanged(ref _groupLibraries, value);
        }

        /// <summary>
        /// Gets or sets project databases.
        /// </summary>
        public ImmutableArray<Database> Databases
        {
            get => _databases;
            set => RaiseAndSetIfChanged(ref _databases, value);
        }

        /// <summary>
        /// Gets or sets project templates.
        /// </summary>
        public ImmutableArray<PageContainer> Templates
        {
            get => _templates;
            set => RaiseAndSetIfChanged(ref _templates, value);
        }

        /// <summary>
        /// Gets or sets project scripts.
        /// </summary>
        public ImmutableArray<Script> Scripts
        {
            get => _scripts;
            set => RaiseAndSetIfChanged(ref _scripts, value);
        }

        /// <summary>
        /// Gets or sets project documents.
        /// </summary>
        public ImmutableArray<DocumentContainer> Documents
        {
            get => _documents;
            set => RaiseAndSetIfChanged(ref _documents, value);
        }

        /// <summary>
        /// Gets or sets project current style library.
        /// </summary>
        public Library<ShapeStyle> CurrentStyleLibrary
        {
            get => _currentStyleLibrary;
            set => RaiseAndSetIfChanged(ref _currentStyleLibrary, value);
        }

        /// <summary>
        /// Gets or sets project current group library.
        /// </summary>
        public Library<GroupShape> CurrentGroupLibrary
        {
            get => _currentGroupLibrary;
            set => RaiseAndSetIfChanged(ref _currentGroupLibrary, value);
        }

        /// <summary>
        /// Gets or sets project current database.
        /// </summary>
        public Database CurrentDatabase
        {
            get => _currentDatabase;
            set => RaiseAndSetIfChanged(ref _currentDatabase, value);
        }

        /// <summary>
        /// Gets or sets project current template.
        /// </summary>
        public PageContainer CurrentTemplate
        {
            get => _currentTemplate;
            set => RaiseAndSetIfChanged(ref _currentTemplate, value);
        }

        /// <summary>
        /// Gets or sets project current script.
        /// </summary>
        public Script CurrentScript
        {
            get => _currentScript;
            set => RaiseAndSetIfChanged(ref _currentScript, value);
        }

        /// <summary>
        /// Gets or sets project current document.
        /// </summary>
        public DocumentContainer CurrentDocument
        {
            get => _currentDocument;
            set => RaiseAndSetIfChanged(ref _currentDocument, value);
        }

        /// <summary>
        /// Gets or sets project current container.
        /// </summary>
        public PageContainer CurrentContainer
        {
            get => _currentContainer;
            set => RaiseAndSetIfChanged(ref _currentContainer, value);
        }

        /// <summary>
        /// Gets or sets currently selected object.
        /// </summary>
        public ObservableObject Selected
        {
            get => _selected;
            set
            {
                SetSelected(value);
                RaiseAndSetIfChanged(ref _selected, value);
            }
        }

        /// <summary>
        /// Get all shapes including grouped shapes.
        /// </summary>
        /// <param name="shapes">The shapes collection.</param>
        /// <returns>All shapes including grouped shapes.</returns>
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

        /// <summary>
        /// Get all shapes including grouped shapes of specified type.
        /// </summary>
        /// <typeparam name="T">The type of shape to include.</typeparam>
        /// <param name="shapes">The shapes collection.</param>
        /// <returns>All shapes including grouped shapes of specified type.</returns>
        public static IEnumerable<T> GetAllShapes<T>(IEnumerable<BaseShape> shapes)
        {
            return GetAllShapes(shapes)?.Where(s => s is T).Cast<T>();
        }

        /// <summary>
        /// Get all shapes including grouped shapes of specified type.
        /// </summary>
        /// <typeparam name="T">The type of shapes to include.</typeparam>
        /// <param name="project">The project object.</param>
        /// <returns>All shapes including grouped shapes of specified type.</returns>
        public static IEnumerable<T> GetAllShapes<T>(ProjectContainer project)
        {
            var shapes = project?.Documents
                .SelectMany(d => d.Pages)
                .SelectMany(c => c.Layers)
                .SelectMany(l => l.Shapes);

            return GetAllShapes(shapes)?.Where(s => s is T).Cast<T>();
        }

        /// <summary>
        /// Set current document.
        /// </summary>
        /// <param name="document">The document instance.</param>
        public void SetCurrentDocument(DocumentContainer document)
        {
            CurrentDocument = document;
            Selected = document;
        }

        /// <summary>
        /// Set current container.
        /// </summary>
        /// <param name="container">The container instance.</param>
        public void SetCurrentContainer(PageContainer container)
        {
            CurrentContainer = container;
            Selected = container;
        }

        /// <summary>
        /// Set current template.
        /// </summary>
        /// <param name="template">The template instance.</param>
        public void SetCurrentTemplate(PageContainer template) => CurrentTemplate = template;

        /// <summary>
        /// Set current script.
        /// </summary>
        /// <param name="script">The script instance.</param>
        public void SetCurrentScript(Script script) => CurrentScript = script;

        /// <summary>
        /// Set current database.
        /// </summary>
        /// <param name="db">The database instance.</param>
        public void SetCurrentDatabase(Database db) => CurrentDatabase = db;

        /// <summary>
        /// Set current group library.
        /// </summary>
        /// <param name="library">The group library instance.</param>
        public void SetCurrentGroupLibrary(Library<GroupShape> library) => CurrentGroupLibrary = library;

        /// <summary>
        /// Set current group.
        /// </summary>
        /// <param name="library">The style library instance.</param>
        public void SetCurrentStyleLibrary(Library<ShapeStyle> library) => CurrentStyleLibrary = library;

        /// <summary>
        /// Set selected value.
        /// </summary>
        /// <param name="value">The value instance.</param>
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

        /// <inheritdoc/>
        public override object Copy(IDictionary<object, object> shared)
        {
            throw new NotImplementedException();
        }

        /// <inheritdoc/>
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

        /// <inheritdoc/>
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

        /// <summary>
        /// Check whether the <see cref="Options"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeOptions() => _options != null;

        /// <summary>
        /// Check whether the <see cref="History"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeHistory() => false;

        /// <summary>
        /// Check whether the <see cref="StyleLibraries"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeStyleLibraries() => true;

        /// <summary>
        /// Check whether the <see cref="GroupLibraries"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeGroupLibraries() => true;

        /// <summary>
        /// Check whether the <see cref="Databases"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeDatabases() => true;

        /// <summary>
        /// Check whether the <see cref="Templates"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeTemplates() => true;

        /// <summary>
        /// Check whether the <see cref="Scripts"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeScripts() => true;

        /// <summary>
        /// Check whether the <see cref="Documents"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeDocuments() => true;

        /// <summary>
        /// Check whether the <see cref="CurrentStyleLibrary"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCurrentStyleLibrary() => _currentStyleLibrary != null;

        /// <summary>
        /// Check whether the <see cref="CurrentGroupLibrary"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCurrentGroupLibrary() => _currentGroupLibrary != null;

        /// <summary>
        /// Check whether the <see cref="CurrentDatabase"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCurrentDatabase() => _currentDatabase != null;

        /// <summary>
        /// Check whether the <see cref="CurrentTemplate"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCurrentTemplate() => _currentTemplate != null;

        /// <summary>
        /// Check whether the <see cref="CurrentScript"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCurrentScript() => _currentScript != null;

        /// <summary>
        /// Check whether the <see cref="CurrentDocument"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCurrentDocument() => _currentDocument != null;

        /// <summary>
        /// Check whether the <see cref="CurrentContainer"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeCurrentContainer() => _currentContainer != null;

        /// <summary>
        /// Check whether the <see cref="Selected"/> property has changed from its default value.
        /// </summary>
        /// <returns>Returns true if the property has changed; otherwise, returns false.</returns>
        public virtual bool ShouldSerializeSelected() => _selected != null;
    }
}
