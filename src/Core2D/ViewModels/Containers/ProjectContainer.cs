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
        [AutoNotify] private Options _options;
        [AutoNotify] private IHistory _history;
        [AutoNotify] private ImmutableArray<Library<ShapeStyle>> _styleLibraries;
        [AutoNotify] private ImmutableArray<Library<GroupShape>> _groupLibraries;
        [AutoNotify] private ImmutableArray<Database> _databases;
        [AutoNotify] private ImmutableArray<PageContainer> _templates;
        [AutoNotify] private ImmutableArray<Script> _scripts;
        [AutoNotify] private ImmutableArray<DocumentContainer> _documents;
        [AutoNotify] private Library<ShapeStyle> _currentStyleLibrary;
        [AutoNotify] private Library<GroupShape> _currentGroupLibrary;
        [AutoNotify] private Database _currentDatabase;
        [AutoNotify] private PageContainer _currentTemplate;
        [AutoNotify] private Script _currentScript;
        [AutoNotify] private DocumentContainer _currentDocument;
        [AutoNotify] private PageContainer _currentContainer;
        [AutoNotify] private ViewModelBase _selected;

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

        public void SetSelected(ViewModelBase value)
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
    }
}
