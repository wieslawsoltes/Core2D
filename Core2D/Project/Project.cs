// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using System.Linq;
using Portable.Xaml.Markup;

namespace Core2D
{
    /// <summary>
    /// The documents container.
    /// </summary>
    [ContentProperty(nameof(Documents))]
    [RuntimeNameProperty(nameof(Name))]
    public partial class Project : ObservableResource
    {
        private string _name;
        private Options _options;
        private History _history;
        private ImmutableArray<Library<ShapeStyle>> _styleLibraries;
        private ImmutableArray<Library<XGroup>> _groupLibraries;
        private ImmutableArray<Database> _databases;
        private ImmutableArray<Template> _templates;
        private ImmutableArray<Document> _documents;
        private Library<ShapeStyle> _currentStyleLibrary;
        private Library<XGroup> _currentGroupLibrary;
        private Database _currentDatabase;
        private Template _currentTemplate;
        private Document _currentDocument;
        private Container _currentContainer;
        private object _selected;

        /// <summary>
        /// Gets or sets project name.
        /// </summary>
        public string Name
        {
            get { return _name; }
            set { Update(ref _name, value); }
        }

        /// <summary>
        /// Gets or sets project options.
        /// </summary>
        public Options Options
        {
            get { return _options; }
            set { Update(ref _options, value); }
        }

        /// <summary>
        /// Gets or sets undo/redo history handler.
        /// </summary>
        public History History
        {
            get { return _history; }
            set { Update(ref _history, value); }
        }

        /// <summary>
        /// Gets or sets project style libraries.
        /// </summary>
        public ImmutableArray<Library<ShapeStyle>> StyleLibraries
        {
            get { return _styleLibraries; }
            set { Update(ref _styleLibraries, value); }
        }

        /// <summary>
        /// Gets or sets project group libraries.
        /// </summary>
        public ImmutableArray<Library<XGroup>> GroupLibraries
        {
            get { return _groupLibraries; }
            set { Update(ref _groupLibraries, value); }
        }

        /// <summary>
        /// Gets or sets project databases.
        /// </summary>
        public ImmutableArray<Database> Databases
        {
            get { return _databases; }
            set { Update(ref _databases, value); }
        }

        /// <summary>
        /// Gets or sets project templates.
        /// </summary>
        public ImmutableArray<Template> Templates
        {
            get { return _templates; }
            set { Update(ref _templates, value); }
        }

        /// <summary>
        /// Gets or sets project documents.
        /// </summary>
        public ImmutableArray<Document> Documents
        {
            get { return _documents; }
            set { Update(ref _documents, value); }
        }

        /// <summary>
        /// Gets or sets project current style library.
        /// </summary>
        public Library<ShapeStyle> CurrentStyleLibrary
        {
            get { return _currentStyleLibrary; }
            set { Update(ref _currentStyleLibrary, value); }
        }

        /// <summary>
        /// Gets or sets project current group library.
        /// </summary>
        public Library<XGroup> CurrentGroupLibrary
        {
            get { return _currentGroupLibrary; }
            set { Update(ref _currentGroupLibrary, value); }
        }

        /// <summary>
        /// Gets or sets project current database.
        /// </summary>
        public Database CurrentDatabase
        {
            get { return _currentDatabase; }
            set { Update(ref _currentDatabase, value); }
        }

        /// <summary>
        /// Gets or sets project current template.
        /// </summary>
        public Template CurrentTemplate
        {
            get { return _currentTemplate; }
            set { Update(ref _currentTemplate, value); }
        }

        /// <summary>
        /// Gets or sets project current document.
        /// </summary>
        public Document CurrentDocument
        {
            get { return _currentDocument; }
            set { Update(ref _currentDocument, value); }
        }

        /// <summary>
        /// Gets or sets project current container.
        /// </summary>
        public Container CurrentContainer
        {
            get { return _currentContainer; }
            set { Update(ref _currentContainer, value); }
        }

        /// <summary>
        /// Gets or sets currently selected object.
        /// </summary>
        public object Selected
        {
            get { return _selected; }
            set
            {
                SetSelected(value);
                Update(ref _selected, value);
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Project"/> class.
        /// </summary>
        public Project()
            : base()
        {
            _options = Options.Create();
            _styleLibraries = ImmutableArray.Create<Library<ShapeStyle>>();
            _groupLibraries = ImmutableArray.Create<Library<XGroup>>();
            _databases = ImmutableArray.Create<Database>();
            _templates = ImmutableArray.Create<Template>();
            _documents = ImmutableArray.Create<Document>();
        }

        /// <summary>
        /// Set current document.
        /// </summary>
        /// <param name="document">The document instance.</param>
        public void SetCurrentDocument(Document document)
        {
            CurrentDocument = document;
        }

        /// <summary>
        /// Set current container.
        /// </summary>
        /// <param name="container">The container instance.</param>
        public void SetCurrentContainer(Container container)
        {
            CurrentContainer = container;
            Selected = container;
        }

        /// <summary>
        /// Set current template.
        /// </summary>
        /// <param name="template">The template instance.</param>
        public void SetCurrentTemplate(Template template)
        {
            CurrentTemplate = template;
        }

        /// <summary>
        /// Set current database.
        /// </summary>
        /// <param name="db">The database instance.</param>
        public void SetCurrentDatabase(Database db)
        {
            CurrentDatabase = db;
        }

        /// <summary>
        /// Set current group library.
        /// </summary>
        /// <param name="library">The group library instance.</param>
        public void SetCurrentGroupLibrary(Library<XGroup> library)
        {
            CurrentGroupLibrary = library;
        }

        /// <summary>
        /// Set current group.
        /// </summary>
        /// <param name="library">The style library instance.</param>
        public void SetCurrentStyleLibrary(Library<ShapeStyle> library)
        {
            CurrentStyleLibrary = library;
        }

        /// <summary>
        /// Set selected value.
        /// </summary>
        /// <param name="value">The value instance.</param>
        public void SetSelected(object value)
        {
            if (value != null)
            {
                if (value is Container && _documents != null)
                {
                    var document = _documents.FirstOrDefault(d => d.Pages.Contains(value as Container));
                    if (document != null)
                    {
                        CurrentDocument = document;
                        CurrentContainer = value as Container;
                        CurrentContainer.Invalidate();
                    }
                }
                else if (value is Document)
                {
                    CurrentDocument = value as Document;
                }
            }
        }

        /// <summary>
        /// Creates a new <see cref="Project"/> instance.
        /// </summary>
        /// <param name="name">The project name.</param>
        /// <returns>The new instance of the <see cref="Project"/> class.</returns>
        public static Project Create(string name = "Project")
        {
            return new Project()
            {
                Name = name
            };
        }
    }
}
