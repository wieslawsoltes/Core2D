// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Core2D.Attributes;
using Core2D.Data.Database;
using Core2D.History;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Project
{
    /// <summary>
    /// Project model.
    /// </summary>
    public partial class XProject : XSelectable
    {
        private string _name;
        private XOptions _options;
        private IHistory _history;
        private ImmutableArray<XLibrary<ShapeStyle>> _styleLibraries;
        private ImmutableArray<XLibrary<XGroup>> _groupLibraries;
        private ImmutableArray<XDatabase> _databases;
        private ImmutableArray<XContainer> _templates;
        private ImmutableArray<XDocument> _documents;
        private XLibrary<ShapeStyle> _currentStyleLibrary;
        private XLibrary<XGroup> _currentGroupLibrary;
        private XDatabase _currentDatabase;
        private XContainer _currentTemplate;
        private XDocument _currentDocument;
        private XContainer _currentContainer;
        private XSelectable _selected;

        /// <summary>
        /// Gets or sets project name.
        /// </summary>
        [Name]
        public string Name
        {
            get => _name;
            set => Update(ref _name, value);
        }

        /// <summary>
        /// Gets or sets project options.
        /// </summary>
        public XOptions Options
        {
            get => _options;
            set => Update(ref _options, value);
        }

        /// <summary>
        /// Gets or sets undo/redo history handler.
        /// </summary>
        public IHistory History
        {
            get => _history;
            set => Update(ref _history, value);
        }

        /// <summary>
        /// Gets or sets project style libraries.
        /// </summary>
        public ImmutableArray<XLibrary<ShapeStyle>> StyleLibraries
        {
            get => _styleLibraries;
            set => Update(ref _styleLibraries, value);
        }

        /// <summary>
        /// Gets or sets project group libraries.
        /// </summary>
        public ImmutableArray<XLibrary<XGroup>> GroupLibraries
        {
            get => _groupLibraries;
            set => Update(ref _groupLibraries, value);
        }

        /// <summary>
        /// Gets or sets project databases.
        /// </summary>
        public ImmutableArray<XDatabase> Databases
        {
            get => _databases;
            set => Update(ref _databases, value);
        }

        /// <summary>
        /// Gets or sets project templates.
        /// </summary>
        public ImmutableArray<XContainer> Templates
        {
            get => _templates;
            set => Update(ref _templates, value);
        }

        /// <summary>
        /// Gets or sets project documents.
        /// </summary>
        [Content]
        public ImmutableArray<XDocument> Documents
        {
            get => _documents;
            set => Update(ref _documents, value);
        }

        /// <summary>
        /// Gets or sets project current style library.
        /// </summary>
        public XLibrary<ShapeStyle> CurrentStyleLibrary
        {
            get => _currentStyleLibrary;
            set => Update(ref _currentStyleLibrary, value);
        }

        /// <summary>
        /// Gets or sets project current group library.
        /// </summary>
        public XLibrary<XGroup> CurrentGroupLibrary
        {
            get => _currentGroupLibrary;
            set => Update(ref _currentGroupLibrary, value);
        }

        /// <summary>
        /// Gets or sets project current database.
        /// </summary>
        public XDatabase CurrentDatabase
        {
            get => _currentDatabase;
            set => Update(ref _currentDatabase, value);
        }

        /// <summary>
        /// Gets or sets project current template.
        /// </summary>
        public XContainer CurrentTemplate
        {
            get => _currentTemplate;
            set => Update(ref _currentTemplate, value);
        }

        /// <summary>
        /// Gets or sets project current document.
        /// </summary>
        public XDocument CurrentDocument
        {
            get { return _currentDocument; }
            set
            {
                Update(ref _currentDocument, value);
                if (value != _selected)
                {
                    Selected = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets project current container.
        /// </summary>
        public XContainer CurrentContainer
        {
            get { return _currentContainer; }
            set
            {
                Update(ref _currentContainer, value);
                if (value != _selected)
                {
                    Selected = value;
                }
            }
        }

        /// <summary>
        /// Gets or sets currently selected object.
        /// </summary>
        public XSelectable Selected
        {
            get { return _selected; }
            set
            {
                SetSelected(value);
                Update(ref _selected, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="XProject"/> class.
        /// </summary>
        public XProject()
            : base()
        {
            _options = XOptions.Create();
            _styleLibraries = ImmutableArray.Create<XLibrary<ShapeStyle>>();
            _groupLibraries = ImmutableArray.Create<XLibrary<XGroup>>();
            _databases = ImmutableArray.Create<XDatabase>();
            _templates = ImmutableArray.Create<XContainer>();
            _documents = ImmutableArray.Create<XDocument>();
        }

        /// <summary>
        /// Set current document.
        /// </summary>
        /// <param name="document">The document instance.</param>
        public void SetCurrentDocument(XDocument document) => CurrentDocument = document;

        /// <summary>
        /// Set current container.
        /// </summary>
        /// <param name="container">The container instance.</param>
        public void SetCurrentContainer(XContainer container) => CurrentContainer = container;

        /// <summary>
        /// Set current template.
        /// </summary>
        /// <param name="template">The template instance.</param>
        public void SetCurrentTemplate(XContainer template) => CurrentTemplate = template;

        /// <summary>
        /// Set current database.
        /// </summary>
        /// <param name="db">The database instance.</param>
        public void SetCurrentDatabase(XDatabase db) => CurrentDatabase = db;

        /// <summary>
        /// Set current group library.
        /// </summary>
        /// <param name="library">The group library instance.</param>
        public void SetCurrentGroupLibrary(XLibrary<XGroup> library) => CurrentGroupLibrary = library;

        /// <summary>
        /// Set current group.
        /// </summary>
        /// <param name="library">The style library instance.</param>
        public void SetCurrentStyleLibrary(XLibrary<ShapeStyle> library) => CurrentStyleLibrary = library;

        /// <summary>
        /// Set selected value.
        /// </summary>
        /// <param name="value">The value instance.</param>
        public void SetSelected(XSelectable value)
        {
            if (value != null)
            {
                if (value is XLayer)
                {
                    var layer = value as XLayer;
                    var owner = layer?.Owner;
                    if (owner != null)
                    {
                        if (owner.CurrentLayer != layer)
                        {
                            owner.CurrentLayer = layer;
                        }
                    }
                }
                else if (value is XContainer && _documents != null)
                {
                    var container = value as XContainer;
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
                            CurrentContainer.Invalidate();
                        }
                    }
                }
                else if (value is XDocument)
                {
                    var document = value as XDocument;
                    if (CurrentDocument != document)
                    {
                        CurrentDocument = document;
                        if (!CurrentDocument?.Pages.Contains(CurrentContainer) ?? false)
                        {
                            var container = CurrentDocument.Pages.FirstOrDefault();
                            if (CurrentContainer != container)
                            {
                                CurrentContainer = container;
                            }
                        }
                    }
                }
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
                yield break;

            foreach (var shape in shapes)
            {
                if (shape is XGroup)
                {
                    foreach (var s in GetAllShapes((shape as XGroup)?.Shapes))
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
        public static IEnumerable<T> GetAllShapes<T>(XProject project)
        {
            var shapes = project?.Documents
                .SelectMany(d => d.Pages)
                .SelectMany(c => c.Layers)
                .SelectMany(l => l.Shapes);

            return GetAllShapes(shapes)?.Where(s => s is T).Cast<T>();
        }

        /// <summary>
        /// Creates a new <see cref="XProject"/> instance.
        /// </summary>
        /// <param name="name">The project name.</param>
        /// <returns>The new instance of the <see cref="XProject"/> class.</returns>
        public static XProject Create(string name = "Project")
        {
            return new XProject()
            {
                Name = name
            };
        }
    }
}
