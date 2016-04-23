// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Attributes;
using Core2D.Data.Database;
using Core2D.History;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

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
        private ImmutableArray<XTemplate> _templates;
        private ImmutableArray<XDocument> _documents;
        private XLibrary<ShapeStyle> _currentStyleLibrary;
        private XLibrary<XGroup> _currentGroupLibrary;
        private XDatabase _currentDatabase;
        private XTemplate _currentTemplate;
        private XDocument _currentDocument;
        private XContainer _currentContainer;
        private XSelectable _selected;

        /// <summary>
        /// Gets or sets project name.
        /// </summary>
        [Name]
        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        /// <summary>
        /// Gets or sets project options.
        /// </summary>
        public XOptions Options
        {
            get { return _options; }
            set { Update(ref _options, value); }
        }

        /// <summary>
        /// Gets or sets undo/redo history handler.
        /// </summary>
        public IHistory History
        {
            get { return _history; }
            set { Update(ref _history, value); }
        }

        /// <summary>
        /// Gets or sets project style libraries.
        /// </summary>
        public ImmutableArray<XLibrary<ShapeStyle>> StyleLibraries
        {
            get { return _styleLibraries; }
            set { Update(ref _styleLibraries, value); }
        }

        /// <summary>
        /// Gets or sets project group libraries.
        /// </summary>
        public ImmutableArray<XLibrary<XGroup>> GroupLibraries
        {
            get { return _groupLibraries; }
            set { Update(ref _groupLibraries, value); }
        }

        /// <summary>
        /// Gets or sets project databases.
        /// </summary>
        public ImmutableArray<XDatabase> Databases
        {
            get { return _databases; }
            set { Update(ref _databases, value); }
        }

        /// <summary>
        /// Gets or sets project templates.
        /// </summary>
        public ImmutableArray<XTemplate> Templates
        {
            get { return _templates; }
            set { Update(ref _templates, value); }
        }

        /// <summary>
        /// Gets or sets project documents.
        /// </summary>
        [Content]
        public ImmutableArray<XDocument> Documents
        {
            get { return _documents; }
            set { Update(ref _documents, value); }
        }

        /// <summary>
        /// Gets or sets project current style library.
        /// </summary>
        public XLibrary<ShapeStyle> CurrentStyleLibrary
        {
            get { return _currentStyleLibrary; }
            set { Update(ref _currentStyleLibrary, value); }
        }

        /// <summary>
        /// Gets or sets project current group library.
        /// </summary>
        public XLibrary<XGroup> CurrentGroupLibrary
        {
            get { return _currentGroupLibrary; }
            set { Update(ref _currentGroupLibrary, value); }
        }

        /// <summary>
        /// Gets or sets project current database.
        /// </summary>
        public XDatabase CurrentDatabase
        {
            get { return _currentDatabase; }
            set { Update(ref _currentDatabase, value); }
        }

        /// <summary>
        /// Gets or sets project current template.
        /// </summary>
        public XTemplate CurrentTemplate
        {
            get { return _currentTemplate; }
            set { Update(ref _currentTemplate, value); }
        }

        /// <summary>
        /// Gets or sets project current document.
        /// </summary>
        public XDocument CurrentDocument
        {
            get { return _currentDocument; }
            set { Update(ref _currentDocument, value); }
        }

        /// <summary>
        /// Gets or sets project current container.
        /// </summary>
        public XContainer CurrentContainer
        {
            get { return _currentContainer; }
            set { Update(ref _currentContainer, value); }
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
            _templates = ImmutableArray.Create<XTemplate>();
            _documents = ImmutableArray.Create<XDocument>();
        }

        /// <summary>
        /// Set current document.
        /// </summary>
        /// <param name="document">The document instance.</param>
        public void SetCurrentDocument(XDocument document)
        {
            CurrentDocument = document;
        }

        /// <summary>
        /// Set current container.
        /// </summary>
        /// <param name="container">The container instance.</param>
        public void SetCurrentContainer(XContainer container)
        {
            CurrentContainer = container;
            Selected = container;
        }

        /// <summary>
        /// Set current template.
        /// </summary>
        /// <param name="template">The template instance.</param>
        public void SetCurrentTemplate(XTemplate template)
        {
            CurrentTemplate = template;
        }

        /// <summary>
        /// Set current database.
        /// </summary>
        /// <param name="db">The database instance.</param>
        public void SetCurrentDatabase(XDatabase db)
        {
            CurrentDatabase = db;
        }

        /// <summary>
        /// Set current group library.
        /// </summary>
        /// <param name="library">The group library instance.</param>
        public void SetCurrentGroupLibrary(XLibrary<XGroup> library)
        {
            CurrentGroupLibrary = library;
        }

        /// <summary>
        /// Set current group.
        /// </summary>
        /// <param name="library">The style library instance.</param>
        public void SetCurrentStyleLibrary(XLibrary<ShapeStyle> library)
        {
            CurrentStyleLibrary = library;
        }

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
                    if (layer?.Owner != null)
                    {
                        layer.Owner.CurrentLayer = value as XLayer;
                    }
                }
                else if (value is XContainer && _documents != null)
                {
                    var document = _documents.FirstOrDefault(d => d.Pages.Contains(value as XContainer));
                    if (document != null)
                    {
                        CurrentDocument = document;
                        CurrentContainer = value as XContainer;
                        CurrentContainer.Invalidate();
                    }
                }
                else if (value is XDocument)
                {
                    CurrentDocument = value as XDocument;
                    if (!CurrentDocument?.Pages.Contains(CurrentContainer) ?? false)
                    {
                        CurrentContainer = CurrentDocument.Pages.FirstOrDefault();
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
