// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Immutable;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Test2d;
using TestSIM;

namespace TestEDITOR
{
    /// <summary>
    ///
    /// </summary>
    public class EditorContext : ObservableObject, IDisposable
    {
        private EditorCommands _commands;
        private Editor _editor;
        private IView _view;
        private IRenderer _renderer;
        private ITextClipboard _textClipboard;
        private ISerializer _serializer;
        private ICompressor _compressor;
        private IScriptEngine _scriptEngine;
        private string _rootScriptsPath;
        private ImmutableArray<ScriptDirectory> _scriptDirectories;
        private bool _isSimulationPaused;
        private System.IO.FileSystemWatcher _watcher = default(System.IO.FileSystemWatcher);
        private System.Threading.Timer _timer = default(System.Threading.Timer);
        private BoolSimulationFactory _simulationFactory = default(BoolSimulationFactory);
        private IDictionary<XGroup, BoolSimulation> _simulations;
        private Clock _clock = default(Clock);
        private Container _containerToCopy = default(Container);
        private Document _documentToCopy = default(Document);
        
        /// <summary>
        ///
        /// </summary>
        public EditorCommands Commands
        {
            get { return _commands; }
            set { Update(ref _commands, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public Editor Editor
        {
            get { return _editor; }
            set { Update(ref _editor, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public IView View
        {
            get { return _view; }
            set { Update(ref _view, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public IRenderer Renderer
        {
            get { return _renderer; }
            set { Update(ref _renderer, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public ITextClipboard TextClipboard
        {
            get { return _textClipboard; }
            set { Update(ref _textClipboard, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public ISerializer Serializer
        {
            get { return _serializer; }
            set { Update(ref _serializer, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public ICompressor Compressor
        {
            get { return _compressor; }
            set { Update(ref _compressor, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public IScriptEngine ScriptEngine
        {
            get { return _scriptEngine; }
            set { Update(ref _scriptEngine, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public string RootScriptsPath
        {
            get { return _rootScriptsPath; }
            set { Update(ref _rootScriptsPath, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public ImmutableArray<ScriptDirectory> ScriptDirectories
        {
            get { return _scriptDirectories; }
            set { Update(ref _scriptDirectories, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public bool IsSimulationPaused
        {
            get { return _isSimulationPaused; }
            set { Update(ref _isSimulationPaused, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public IDictionary<XGroup, BoolSimulation> Simulations
        {
            get { return _simulations; }
            set { Update(ref _simulations, value); }
        }

        /// <summary>
        ///
        /// </summary>
        public Action<Action> Execute { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="container"></param>
        private void RenameTemplateLayers(Container container)
        {
            foreach (var layer in container.Layers)
            {
                layer.Name = string.Concat("Template", layer.Name);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public Container EmptyTemplate(Project project)
        {
            var container = Container.Create("Empty");

            RenameTemplateLayers(container);

            return container;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public Container GridTemplate(Project project)
        {
            var container = Container.Create("Grid");

            RenameTemplateLayers(container);

            var gs = project
                .StyleGroups.FirstOrDefault(g => g.Name == "Template")
                .Styles.FirstOrDefault(s => s.Name == "Grid");
            var settings = LineGrid.Settings.Create(0, 0, container.Width, container.Height, 30, 30);
            var shapes = LineGrid.Create(gs, settings, project.Options.PointShape);
            var layer = container.Layers.FirstOrDefault();

            var builder = layer.Shapes.ToBuilder();
            foreach (var shape in shapes)
            {
                builder.Add(shape);
            }
            layer.Shapes = builder.ToImmutable();

            return container;
        }
        
        /// <summary>
        ///
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public Container DefaultContainer(Project project)
        {
            var container = Container.Create();
            container.Template = project.CurrentTemplate;
            return container;
        }
        
        /// <summary>
        ///
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        public Document DefaultDocument(Project project)
        {
             var document = Document.Create();
      
             return document;
        }
        
        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public Project DefaultProject()
        {
            var project = Project.Create();

            var templateBuilder = project.Templates.ToBuilder();
            templateBuilder.Add(EmptyTemplate(project));
            templateBuilder.Add(GridTemplate(project));
            project.Templates = templateBuilder.ToImmutable();

            project.CurrentTemplate = project.Templates.FirstOrDefault(t => t.Name == "Grid");

            var document1 = DefaultDocument(project);
            var document2 = DefaultDocument(project);

            var container1 = DefaultContainer(project);
            var container2 = DefaultContainer(project);

            var document1Builder = document1.Containers.ToBuilder();
            document1Builder.Add(container1);
            document1.Containers = document1Builder.ToImmutable();

            var document2Builder = document2.Containers.ToBuilder();
            document2Builder.Add(container2);
            document2.Containers = document2Builder.ToImmutable();

            var documentBuilder = project.Documents.ToBuilder();
            documentBuilder.Add(document1);
            documentBuilder.Add(document2);
            project.Documents = documentBuilder.ToImmutable();

            project.CurrentDocument = project.Documents.FirstOrDefault();
            project.CurrentContainer = document1.Containers.FirstOrDefault();

            return project;
        }
 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void NewCommandHandler(object item)
        {
            if (item is Container)
            {
                var selected = item as Container;
                var document = _editor.Project.Documents.FirstOrDefault(d => d.Containers.Contains(selected));
                if (document != null)
                {
                    var container = DefaultContainer(_editor.Project);

                    var previous = document.Containers;
                    var next = document.Containers.Add(container);
                    _editor.History.Snapshot(previous, next, (p) => document.Containers = p);
                    document.Containers = next;

                    _editor.Project.CurrentContainer = container;
                }
            }
            else if (item is Document)
            {
                var selected = item as Document;
                var container = DefaultContainer(_editor.Project);

                var previous = selected.Containers;
                var next = selected.Containers.Add(container);
                _editor.History.Snapshot(previous, next, (p) => selected.Containers = p);
                selected.Containers = next;

                _editor.Project.CurrentContainer = container;
            }
            else if (item is Project)
            {
                var document = DefaultDocument(_editor.Project);

                var previous = _editor.Project.Documents;
                var next = _editor.Project.Documents.Add(document);
                _editor.History.Snapshot(previous, next, (p) => _editor.Project.Documents = p);
                _editor.Project.Documents = next;

                _editor.Project.CurrentDocument = document;
                _editor.Project.CurrentContainer = document.Containers.FirstOrDefault();
            }
            else if (item is EditorContext || item == null)
            {
                _editor.History.Reset();

                _editor.Load(DefaultProject());
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void ExitCommandHandler()
        {
            _view.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        private void UndoCommandHandler()
        {
            Undo();
        }

        /// <summary>
        /// 
        /// </summary>
        private void RedoCommandHandler()
        {
            Redo();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void CutCommandHandler(object item)
        {
            if (item is Container)
            {
                var container = item as Container;
                _containerToCopy = container;
                _documentToCopy = default(Document);
                _editor.Delete(container);
            }
            else if (item is Document)
            {
                var document = item as Document;
                _containerToCopy = default(Container);
                _documentToCopy = document;
                _editor.Delete(document);
            }
            else if (item is EditorContext || item == null)
            {
                Cut();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void CopyCommandHandler(object item)
        {
            if (item is Container)
            {
                var container = item as Container;
                _containerToCopy = container;
                _documentToCopy = default(Document);
            }
            else if (item is Document)
            {
                var document = item as Document;
                _containerToCopy = default(Container);
                _documentToCopy = document;
            }
            else if (item is EditorContext || item == null)
            {
                Copy();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void PasteCommandHandler(object item)
        {
            if (item is Container)
            {
                if (_containerToCopy != null)
                {
                    var container = item as Container;
                    var document = _editor.Project.Documents.FirstOrDefault(d => d.Containers.Contains(container));
                    if (document != null)
                    {
                        int index = document.Containers.IndexOf(container);
                        var clone = Clone(_containerToCopy);

                        var builder = document.Containers.ToBuilder();
                        builder[index] = clone;
                        document.Containers = builder.ToImmutable();

                        var previous = document.Containers;
                        var next = builder.ToImmutable();
                        _editor.History.Snapshot(previous, next, (p) => document.Containers = p);
                        document.Containers = next;

                        _editor.Project.CurrentContainer = clone;
                    }
                }
            }
            else if (item is Document)
            {
                if (_containerToCopy != null)
                {
                    var document = item as Document;
                    var clone = Clone(_containerToCopy);

                    var previous = document.Containers;
                    var next = document.Containers.Add(clone);
                    _editor.History.Snapshot(previous, next, (p) => document.Containers = p);
                    document.Containers = next;

                    _editor.Project.CurrentContainer = clone;
                }
                else if (_documentToCopy != null)
                {
                    var document = item as Document;
                    int index = _editor.Project.Documents.IndexOf(document);
                    var clone = Clone(_documentToCopy);

                    var builder = _editor.Project.Documents.ToBuilder();
                    builder[index] = clone;

                    var previous = _editor.Project.Documents;
                    var next = builder.ToImmutable();
                    _editor.History.Snapshot(previous, next, (p) => _editor.Project.Documents = p);
                    _editor.Project.Documents = next;

                    _editor.Project.CurrentDocument = clone;
                }
            }
            else if (item is EditorContext || item == null)
            {
                Paste();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void DeleteCommandHandler(object item)
        {
            if (item is Container)
            {
                var container = item as Container;
                _editor.Delete(container);
            }
            else if (item is Document)
            {
                var document = item as Document;
                _editor.Delete(document);
            }
            else if (item is EditorContext || item == null)
            {
                _editor.DeleteSelected();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void AddDatabaseCommandHandler()
        {
            var builder = ImmutableArray.CreateBuilder<Column>();
            for (int i = 0; i < 4; i++)
            {
                builder.Add(Column.Create("Column" + i));
            }
      
            var db = Database.Create(
                "New",
                builder.ToImmutable());

            var previous = _editor.Project.Databases;
            var next = _editor.Project.Databases.Add(db);
            _editor.History.Snapshot(previous, next, (p) => _editor.Project.Databases = p);
            _editor.Project.Databases = next;

            _editor.Project.CurrentDatabase = db;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="db"></param>
        private void RemoveDatabaseCommandHandler(object db)
        {
            if (db != null && db is Database)
            {
                var previous = _editor.Project.Databases;
                var next = _editor.Project.Databases.Remove(db as Database);
                _editor.History.Snapshot(previous, next, (p) => _editor.Project.Databases = p);
                _editor.Project.Databases = next;

                _editor.Project.CurrentDatabase = _editor.Project.Databases.FirstOrDefault();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void AddRecordCommandHandler()
        {
            if (_editor.Project.CurrentDatabase != null)
            {
                var db = _editor.Project.CurrentDatabase;

                var values = Enumerable.Repeat("<empty>", db.Columns.Length).Select(c => Value.Create(c));
                var record = Record.Create(
                    db.Columns,
                    ImmutableArray.CreateRange<Value>(values));

                var previous = db.Records;
                var next = db.Records.Add(record);
                _editor.History.Snapshot(previous, next, (p) => db.Records = p);
                db.Records = next;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RemoveRecordCommandHandler()
        {
            if (_editor.Project.CurrentDatabase != null)
            {
                var db = _editor.Project.CurrentDatabase;
                if (db.CurrentRecord != null)
                {
                    var record = db.CurrentRecord;

                    var previous = db.Records;
                    var next = db.Records.Remove(record);
                    _editor.History.Snapshot(previous, next, (p) => db.Records = p);
                    db.Records = next;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        private void AddBindingCommandHandler(object owner)
        {
            if (owner != null)
            {
                if (owner is BaseShape)
                {
                    var shape = owner as BaseShape;
                    if (shape.Bindings == null)
                    {
                        shape.Bindings = ImmutableArray.Create<ShapeBinding>();
                    }

                    var previous = shape.Bindings;
                    var next = shape.Bindings.Add(ShapeBinding.Create("", ""));
                    _editor.History.Snapshot(previous, next, (p) => shape.Bindings = p);
                    shape.Bindings = next;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        private void RemoveBindingCommandHandler(object parameter)
        {
            if (parameter != null && parameter is ShapeBindingParameter)
            {
                var owner = (parameter as ShapeBindingParameter).Owner;
                var binding = (parameter as ShapeBindingParameter).Binding;

                if (owner is BaseShape)
                {
                    var shape = owner as BaseShape;
                    if (shape.Bindings != null)
                    {
                        var previous = shape.Bindings;
                        var next = shape.Bindings.Remove(binding);
                        _editor.History.Snapshot(previous, next, (p) => shape.Bindings = p);
                        shape.Bindings = next;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="owner"></param>
        private void AddPropertyCommandHandler(object owner)
        {
            if (owner != null)
            {
                if (owner is BaseShape)
                {
                    var shape = owner as BaseShape;
                    if (shape.Properties == null)
                    {
                        shape.Properties = ImmutableArray.Create<ShapeProperty>();
                    }

                    var previous = shape.Properties;
                    var next = shape.Properties.Add(ShapeProperty.Create("New", ""));
                    _editor.History.Snapshot(previous, next, (p) => shape.Properties = p);
                    shape.Properties = next;
                }
                else if (owner is Container)
                {
                    var container = owner as Container;
                    if (container.Properties == null)
                    {
                        container.Properties = ImmutableArray.Create<ShapeProperty>();
                    }

                    var previous = container.Properties;
                    var next = container.Properties.Add(ShapeProperty.Create("New", ""));
                    _editor.History.Snapshot(previous, next, (p) => container.Properties = p);
                    container.Properties = next;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="parameter"></param>
        private void RemovePropertyCommandHandler(object parameter)
        {
            if (parameter != null && parameter is ShapePropertyParameter)
            {
                var owner = (parameter as ShapePropertyParameter).Owner;
                var property = (parameter as ShapePropertyParameter).Property;

                if (owner is BaseShape)
                {
                    var shape = owner as BaseShape;
                    if (shape.Properties != null)
                    {
                        var previous = shape.Properties;
                        var next = shape.Properties.Remove(property);
                        _editor.History.Snapshot(previous, next, (p) => shape.Properties = p);
                        shape.Properties = next;
                    }
                }
                else if (owner is Container)
                {
                    var container = owner as Container;
                    if (container.Properties != null)
                    {
                        var previous = container.Properties;
                        var next = container.Properties.Remove(property);
                        _editor.History.Snapshot(previous, next, (p) => container.Properties = p);
                        container.Properties = next;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void AddGroupLibraryCommandHandler()
        {
            var gl = GroupLibrary.Create("New");

            var previous = _editor.Project.GroupLibraries;
            var next = _editor.Project.GroupLibraries.Add(gl);
            _editor.History.Snapshot(previous, next, (p) => _editor.Project.GroupLibraries = p);
            _editor.Project.GroupLibraries = next;
        }

        /// <summary>
        /// 
        /// </summary>
        private void RemoveGroupLibraryCommandHandler()
        {
            _editor.RemoveCurrentGroupLibrary();
        }
 
        /// <summary>
        /// 
        /// </summary>
        private void AddGroupCommandHandler()
        {
            var group = _editor.Renderer.SelectedShape;
            if (group != null && group is XGroup)
            {
                if (_editor.Project.CurrentGroupLibrary != null)
                {
                    var clone = Clone(group as XGroup);
                    if (clone != null)
                    {
                        var gl = _editor.Project.CurrentGroupLibrary;
                        var previous = gl.Groups;
                        var next = gl.Groups.Add(clone);
                        _editor.History.Snapshot(previous, next, (p) => gl.Groups = p);
                        gl.Groups = next;
                    }
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void RemoveGroupCommandHandler()
        {
            _editor.RemoveCurrentGroup();
        }

        /// <summary>
        /// 
        /// </summary>
        private void AddLayerCommandHandler()
        {
            var container = _editor.Project.CurrentContainer;
            var previous = container.Layers;
            var next = container.Layers.Add(Layer.Create("New", container));
            _editor.History.Snapshot(previous, next, (p) => container.Layers = p);
            container.Layers = next;
        }

        /// <summary>
        /// 
        /// </summary>
        private void RemoveLayerCommandHandler()
        {
            _editor.RemoveCurrentLayer();
        }

        /// <summary>
        /// 
        /// </summary>
        private void AddStyleGroupCommandHandler()
        {
            var sg = ShapeStyleGroup.Create("New");

            var previous = _editor.Project.StyleGroups;
            var next = _editor.Project.StyleGroups.Add(sg);
            _editor.History.Snapshot(previous, next, (p) => _editor.Project.StyleGroups = p);
            _editor.Project.StyleGroups = next;
        }
 
        /// <summary>
        /// 
        /// </summary>
        private void RemoveStyleGroupCommandHandler()
        {
            _editor.RemoveCurrentStyleGroup();
        }

        /// <summary>
        /// 
        /// </summary>
        private void AddStyleCommandHandler()
        {
            var sg = _editor.Project.CurrentStyleGroup;
            var previous = sg.Styles;
            var next = sg.Styles.Add(ShapeStyle.Create("New"));
            _editor.History.Snapshot(previous, next, (p) => sg.Styles = p);
            sg.Styles = next;
        }

        /// <summary>
        /// 
        /// </summary>
        private void RemoveStyleCommandHandler()
        {
            _editor.RemoveCurrentStyle();
        }

        /// <summary>
        /// 
        /// </summary>
        private void RemoveShapeCommandHandler()
        {
            _editor.RemoveCurrentShape();
        }

        /// <summary>
        /// 
        /// </summary>
        private void AddTemplateCommandHandler()
        {
            var previous = _editor.Project.Templates;
            var next = _editor.Project.Templates.Add(EmptyTemplate(_editor.Project));
            _editor.History.Snapshot(previous, next, (p) => _editor.Project.Templates = p);
            _editor.Project.Templates = next;
        }

        /// <summary>
        /// 
        /// </summary>
        private void RemoveTemplateCommandHandler()
        {
            _editor.RemoveCurrentTemplate();
        }

        /// <summary>
        /// 
        /// </summary>
        private void EditTemplateCommandHandler()
        {
            var template = _editor.Project.CurrentTemplate;
            if (template != null)
            {
                _editor.Project.CurrentContainer = template;
                _editor.Project.CurrentContainer.Invalidate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void ApplyTemplateCommandHandler(object item)
        {
            if (item is Container)
            {
                var template = item as Container;
                
                // TODO: Add history snapshot.
                _editor.Project.CurrentContainer.Template = template;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void SelectedItemChangedCommandHandler(object item)
        {
            if (item is Container)
            {
                var selected = item as Container;
                var document = _editor.Project.Documents.FirstOrDefault(d => d.Containers.Contains(selected));
                if (document != null)
                {
                    _editor.Project.CurrentDocument = document;
                    _editor.Project.CurrentContainer = selected;
                    _editor.Project.CurrentContainer.Invalidate();
                }
            }
            else if (item is Document)
            {
                var selected = item as Document;
                _editor.Project.CurrentDocument = selected;
            }
        }
  
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void AddContainerCommandHandler(object item)
        {
            var container = DefaultContainer(_editor.Project);

            var document = _editor.Project.CurrentDocument;
            var previous = document.Containers;
            var next = document.Containers.Add(container);
            _editor.History.Snapshot(previous, next, (p) => document.Containers = p);
            document.Containers = next;

            _editor.Project.CurrentContainer = container;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void InsertContainerBeforeCommandHandler(object item)
        {
            if (item is Container)
            {
                var selected = item as Container;
                int index = _editor.Project.CurrentDocument.Containers.IndexOf(selected);
                var container = DefaultContainer(_editor.Project);

                var document = _editor.Project.CurrentDocument;
                var previous = document.Containers;
                var next = document.Containers.Insert(index, container);
                _editor.History.Snapshot(previous, next, (p) => document.Containers = p);
                document.Containers = next;

                _editor.Project.CurrentContainer = container;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void InsertContainerAfterCommandHandler(object item)
        {
            if (item is Container)
            {
                var selected = item as Container;
                int index = _editor.Project.CurrentDocument.Containers.IndexOf(selected);
                var container = DefaultContainer(_editor.Project);

                var document = _editor.Project.CurrentDocument;
                var previous = document.Containers;
                var next = document.Containers.Insert(index + 1, container);
                _editor.History.Snapshot(previous, next, (p) => document.Containers = p);
                document.Containers = next;

                _editor.Project.CurrentContainer = container;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void AddDocumentCommandHandler(object item)
        {
            var document = DefaultDocument(_editor.Project);

            var previous = _editor.Project.Documents;
            var next = _editor.Project.Documents.Add(document);
            _editor.History.Snapshot(previous, next, (p) => _editor.Project.Documents = p);
            _editor.Project.Documents = next;

            _editor.Project.CurrentDocument = document;
            _editor.Project.CurrentContainer = document.Containers.FirstOrDefault();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void InsertDocumentBeforeCommandHandler(object item)
        {
            if (item is Document)
            {
                var selected = item as Document;
                int index = _editor.Project.Documents.IndexOf(selected);
                var document = DefaultDocument(_editor.Project);

                var previous = _editor.Project.Documents;
                var next = _editor.Project.Documents.Insert(index, document);
                _editor.History.Snapshot(previous, next, (p) => _editor.Project.Documents = p);
                _editor.Project.Documents = next;

                _editor.Project.CurrentDocument = document;
                _editor.Project.CurrentContainer = document.Containers.FirstOrDefault();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        private void InsertDocumentAfterCommandHandler(object item)
        {
            if (item is Document)
            {
                var selected = item as Document;
                int index = _editor.Project.Documents.IndexOf(selected);
                var document = DefaultDocument(_editor.Project);

                var previous = _editor.Project.Documents;
                var next = _editor.Project.Documents.Insert(index + 1, document);
                _editor.History.Snapshot(previous, next, (p) => _editor.Project.Documents = p);
                _editor.Project.Documents = next;

                _editor.Project.CurrentDocument = document;
                _editor.Project.CurrentContainer = document.Containers.FirstOrDefault();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void InitializeEditor()
        {
            try
            {
                _editor = Editor.Create(
                    DefaultProject(),
                    _renderer,
                    _serializer,
                    _compressor);

                (_editor.Renderer as ObservableObject).PropertyChanged +=
                    (s, e) =>
                    {
                        if (e.PropertyName == "Zoom")
                        {
                            Invalidate();
                        }
                    };

                _commands = new EditorCommands();

                _commands.NewCommand = 
                    new DelegateCommand<object>(
                        (item) => NewCommandHandler(item),
                        (item) => IsEditMode());

                _commands.ExitCommand = 
                    new DelegateCommand(
                        () => ExitCommandHandler(),
                        () => true);

                _commands.UndoCommand = 
                    new DelegateCommand(
                        () => UndoCommandHandler(),
                        () => IsEditMode() /* && CanUndo() */);

                _commands.RedoCommand = 
                    new DelegateCommand(
                        () => RedoCommandHandler(),
                        () => IsEditMode() /* && CanRedo() */);

                _commands.CutCommand = 
                    new DelegateCommand<object>(
                        (item) => CutCommandHandler(item),
                        (item) => IsEditMode() /* && CanCopy() */);

                _commands.CopyCommand = 
                    new DelegateCommand<object>(
                        (item) => CopyCommandHandler(item),
                        (item) => IsEditMode() /* && CanCopy() */);

                _commands.PasteCommand = 
                    new DelegateCommand<object>(
                        (item) => PasteCommandHandler(item),
                        (item) => IsEditMode() /* && CanPaste() */);

                _commands.DeleteCommand = 
                    new DelegateCommand<object>(
                        (item) => DeleteCommandHandler(item),
                        (item) => IsEditMode() /* && _editor.IsSelectionAvailable() */);

                _commands.SelectAllCommand = 
                    new DelegateCommand(
                        () => SelectAll(),
                        () => IsEditMode());

                _commands.ClearAllCommand = 
                    new DelegateCommand(
                        () => ClearAll(),
                        () => IsEditMode());

                _commands.GroupCommand = 
                    new DelegateCommand(
                        () => _editor.GroupSelected(),
                        () => IsEditMode() /* && _editor.IsSelectionAvailable() */);

                _commands.GroupLayerCommand = 
                    new DelegateCommand(
                        () => _editor.GroupCurrentLayer(),
                        () => IsEditMode());

                _commands.ToolNoneCommand = 
                    new DelegateCommand(
                        () => _editor.CurrentTool = Tool.None,
                        () => IsEditMode());

                _commands.ToolSelectionCommand = 
                    new DelegateCommand(
                        () => _editor.CurrentTool = Tool.Selection,
                        () => IsEditMode());

                _commands.ToolGroupCommand = 
                    new DelegateCommand(
                        () => _editor.CurrentTool = Tool.Group,
                        () => IsEditMode());

                _commands.ToolPointCommand = 
                    new DelegateCommand(
                        () => _editor.CurrentTool = Tool.Point,
                        () => IsEditMode());

                _commands.ToolLineCommand = 
                    new DelegateCommand(
                        () => _editor.CurrentTool = Tool.Line,
                        () => IsEditMode());

                _commands.ToolRectangleCommand = 
                    new DelegateCommand(
                        () => _editor.CurrentTool = Tool.Rectangle,
                        () => IsEditMode());

                _commands.ToolEllipseCommand = 
                    new DelegateCommand(
                        () => _editor.CurrentTool = Tool.Ellipse,
                        () => IsEditMode());

                _commands.ToolArcCommand = 
                    new DelegateCommand(
                        () => _editor.CurrentTool = Tool.Arc,
                        () => IsEditMode());

                _commands.ToolBezierCommand = 
                    new DelegateCommand(
                        () => _editor.CurrentTool = Tool.Bezier,
                        () => IsEditMode());

                _commands.ToolQBezierCommand = 
                    new DelegateCommand(
                        () => _editor.CurrentTool = Tool.QBezier,
                        () => IsEditMode());

                _commands.ToolTextCommand = 
                    new DelegateCommand(
                        () => _editor.CurrentTool = Tool.Text,
                        () => IsEditMode());

                _commands.ToolImageCommand = 
                    new DelegateCommand(
                        () => _editor.CurrentTool = Tool.Image,
                        () => IsEditMode());

                _commands.ToolPathCommand = 
                    new DelegateCommand(
                        () => _editor.CurrentTool = Tool.Path,
                        () => IsEditMode());

                _commands.EvalScriptCommand = 
                    new DelegateCommand<string>(
                        (path) => Eval(path),
                        (path) => IsEditMode());

                _commands.DefaultIsFilledCommand = 
                    new DelegateCommand(
                        () => _editor.Project.Options.DefaultIsFilled = !_editor.Project.Options.DefaultIsFilled,
                        () => IsEditMode());

                _commands.SnapToGridCommand = 
                    new DelegateCommand(
                        () => _editor.Project.Options.SnapToGrid = !_editor.Project.Options.SnapToGrid,
                        () => IsEditMode());

                _commands.TryToConnectCommand = 
                    new DelegateCommand(
                        () => _editor.Project.Options.TryToConnect = !_editor.Project.Options.TryToConnect,
                        () => IsEditMode());

                _commands.AddDatabaseCommand = 
                    new DelegateCommand(
                        () => AddDatabaseCommandHandler(),
                        () => IsEditMode());

                _commands.RemoveDatabaseCommand = 
                    new DelegateCommand<object>(
                        (db) => RemoveDatabaseCommandHandler(db),
                        (db) => IsEditMode());

                _commands.AddRecordCommand = 
                    new DelegateCommand(
                        () => AddRecordCommandHandler(),
                        () => IsEditMode());

                _commands.RemoveRecordCommand = 
                    new DelegateCommand(
                        () => RemoveRecordCommandHandler(),
                        () => IsEditMode());

                _commands.AddBindingCommand = 
                    new DelegateCommand<object>(
                        (owner) => AddBindingCommandHandler(owner),
                        (owner) => IsEditMode());

                _commands.RemoveBindingCommand = 
                    new DelegateCommand<object>(
                        (parameter) => RemoveBindingCommandHandler(parameter),
                        (parameter) => IsEditMode());

                _commands.AddPropertyCommand = 
                    new DelegateCommand<object>(
                        (owner) => AddPropertyCommandHandler(owner),
                        (owner) => IsEditMode());

                _commands.RemovePropertyCommand = 
                    new DelegateCommand<object>(
                        (parameter) => RemovePropertyCommandHandler(parameter),
                        (parameter) => IsEditMode());
                
                _commands.AddGroupLibraryCommand = 
                    new DelegateCommand(
                        () => AddGroupLibraryCommandHandler(),
                        () => IsEditMode());

                _commands.RemoveGroupLibraryCommand = 
                    new DelegateCommand(
                        () => RemoveGroupLibraryCommandHandler(),
                        () => IsEditMode());

                _commands.AddGroupCommand = 
                    new DelegateCommand(
                        () => AddGroupCommandHandler(),
                        () => IsEditMode());

                _commands.RemoveGroupCommand = 
                    new DelegateCommand(
                        () => RemoveGroupCommandHandler(),
                        () => IsEditMode());

                _commands.AddLayerCommand = 
                    new DelegateCommand(
                        () => AddLayerCommandHandler(),
                        () => IsEditMode());

                _commands.RemoveLayerCommand = 
                    new DelegateCommand(
                        () => RemoveLayerCommandHandler(),
                        () => IsEditMode());

                _commands.AddStyleGroupCommand = 
                    new DelegateCommand(
                        () => AddStyleGroupCommandHandler(),
                        () => IsEditMode());

                _commands.RemoveStyleGroupCommand = 
                    new DelegateCommand(
                        () => RemoveStyleGroupCommandHandler(),
                        () => IsEditMode());

                _commands.AddStyleCommand = 
                    new DelegateCommand(
                        () => AddStyleCommandHandler(),
                        () => IsEditMode());

                _commands.RemoveStyleCommand = 
                    new DelegateCommand(
                        () => RemoveStyleCommandHandler(),
                        () => IsEditMode());

                _commands.RemoveShapeCommand = 
                    new DelegateCommand(
                        () => RemoveShapeCommandHandler(),
                        () => IsEditMode());

                _commands.StartSimulationCommand = 
                    new DelegateCommand(
                        () => StartSimulation(),
                        () => IsEditMode());

                _commands.StopSimulationCommand = 
                    new DelegateCommand(
                        () => StopSimulation(),
                        () => IsSimulationMode());

                _commands.RestartSimulationCommand = 
                    new DelegateCommand(
                        () => RestartSimulation(),
                        () => IsSimulationMode());

                _commands.PauseSimulationCommand = 
                    new DelegateCommand(
                        () => PauseSimulation(),
                        () => IsSimulationMode());

                _commands.TickSimulationCommand = 
                    new DelegateCommand(
                        () => TickSimulation(),
                        () => IsSimulationMode() && IsSimulationPaused);

                _commands.AddTemplateCommand = 
                    new DelegateCommand(
                        () => AddTemplateCommandHandler(),
                        () => IsEditMode());

                _commands.RemoveTemplateCommand = 
                    new DelegateCommand(
                        () => RemoveTemplateCommandHandler(),
                        () => IsEditMode());

                _commands.EditTemplateCommand = 
                    new DelegateCommand(
                        () => EditTemplateCommandHandler(),
                        () => IsEditMode());

                _commands.ApplyTemplateCommand = 
                    new DelegateCommand<object>(
                        (item) => ApplyTemplateCommandHandler(item),
                        (item) => IsEditMode());

                _commands.SelectedItemChangedCommand = 
                    new DelegateCommand<object>(
                        (item) => SelectedItemChangedCommandHandler(item),
                        (item) => IsEditMode());

                _commands.AddContainerCommand = 
                    new DelegateCommand<object>(
                        (item) => AddContainerCommandHandler(item),
                        (item) => IsEditMode());

                _commands.InsertContainerBeforeCommand = 
                    new DelegateCommand<object>(
                        (item) => InsertContainerBeforeCommandHandler(item),
                        (item) => IsEditMode());

                _commands.InsertContainerAfterCommand = 
                    new DelegateCommand<object>(
                        (item) => InsertContainerAfterCommandHandler(item),
                        (item) => IsEditMode());

                _commands.AddDocumentCommand = 
                    new DelegateCommand<object>(
                        (item) => AddDocumentCommandHandler(item),
                        (item) => IsEditMode());

                _commands.InsertDocumentBeforeCommand = 
                    new DelegateCommand<object>(
                        (item) => InsertDocumentBeforeCommandHandler(item),
                        (item) => IsEditMode());

                _commands.InsertDocumentAfterCommand = 
                    new DelegateCommand<object>(
                        (item) => InsertDocumentAfterCommandHandler(item),
                        (item) => IsEditMode());

                WarmUpCSharpScript();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        private void WarmUpCSharpScript()
        {
            // NOTE: Warmup Roslyn script engine.
            try
            {
                Task.Run(
                    () =>
                    {
                        Eval("Action a = () => { };", this, Execute);
                    });
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void InitializeSctipts()
        {
            try
            {
#if DEBUG
                _rootScriptsPath = "../../../Scripts";
#else
                _rootScriptsPath = "Scripts";
#endif
                ScriptDirectories = ImmutableArray.Create<ScriptDirectory>();

                Action update = () =>
                {
                    try
                    {
                        ScriptDirectories =
                            ScriptDirectory.CreateScriptDirectories(_rootScriptsPath);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.Print(ex.Message);
                        System.Diagnostics.Debug.Print(ex.StackTrace);
                    }
                };

                if (System.IO.Directory.Exists(_rootScriptsPath))
                {
                    update();

                    _watcher = new System.IO.FileSystemWatcher();
                    _watcher.Path = _rootScriptsPath;
                    _watcher.Filter = "*.*";
                    _watcher.NotifyFilter =
                        System.IO.NotifyFilters.LastAccess
                        | System.IO.NotifyFilters.LastWrite
                        | System.IO.NotifyFilters.FileName
                        | System.IO.NotifyFilters.DirectoryName;
                    _watcher.IncludeSubdirectories = true;
                    _watcher.Filter = "*.*";
                    _watcher.Changed += (s, e) => update();
                    _watcher.Created += (s, e) => update();
                    _watcher.Deleted += (s, e) => update();
                    _watcher.Renamed += (s, e) => update();
                    _watcher.EnableRaisingEvents = true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void InitializeSimulation()
        {
            try
            {
                _simulationFactory = new BoolSimulationFactory();
                _simulationFactory.Register(new SignalSimulation());
                _simulationFactory.Register(new InputSimulation());
                _simulationFactory.Register(new OutputSimulation());
                _simulationFactory.Register(new ShortcutSimulation());
                _simulationFactory.Register(new AndSimulation());
                _simulationFactory.Register(new OrSimulation());
                _simulationFactory.Register(new InverterSimulation());
                _simulationFactory.Register(new XorSimulation());
                _simulationFactory.Register(new TimerOnSimulation());
                _simulationFactory.Register(new TimerOffSimulation());
                _simulationFactory.Register(new TimerPulseSimulation());
                _simulationFactory.Register(new MemoryResetPriorityVSimulation());
                _simulationFactory.Register(new MemorySetPriorityVSimulation());
                _simulationFactory.Register(new MemoryResetPrioritySimulation());
                _simulationFactory.Register(new MemorySetPrioritySimulation());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void Invalidate()
        {
            try
            {
                _editor.Renderer.ClearCache();
                _editor.Project.CurrentContainer.Invalidate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="code"></param>
        /// <param name="context"></param>
        /// <param name="execute"></param>
        public void Eval(string code, EditorContext context, Action<Action> execute)
        {
            try
            {
                _scriptEngine.Eval(code, context, execute);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        public void Eval(string path)
        {
            try
            {
                var code = System.IO.File.ReadAllText(path);
                var context = this;
                Eval(code, context, Execute);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        /// <param name="compress"></param>
        public string ReadUtf8Text(string path, bool compress = true)
        {
            try
            {
                if (compress)
                {
                    using (var fs = System.IO.File.OpenRead(path))
                    {
                        using (var cs = new System.IO.Compression.GZipStream(fs, System.IO.Compression.CompressionMode.Decompress))
                        {
                            using (var sr = new System.IO.StreamReader(cs, Encoding.UTF8))
                            {
                                return sr.ReadToEnd();
                            }
                        }
                    }
                }
                else
                {
                    using (var fs = System.IO.File.OpenRead(path))
                    {
                        using (var sr = new System.IO.StreamReader(fs, Encoding.UTF8))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
            return null;
        }
        
        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="text"></param>
        /// <param name="compress"></param>
        public void WriteUtf8Text(string path, string text, bool compress = true)
        {
            try
            {
                if (compress)
                {
                    using (var fs = System.IO.File.Create(path))
                    {
                        using (var cs = new System.IO.Compression.GZipStream(fs, System.IO.Compression.CompressionMode.Compress))
                        {
                            using (var sw = new System.IO.StreamWriter(cs, Encoding.UTF8))
                            {
                                sw.Write(text);
                            }
                        }
                    }
                }
                else
                {
                    using (var fs = System.IO.File.Create(path))
                    {
                        using (var sw = new System.IO.StreamWriter(fs, Encoding.UTF8))
                        {
                            sw.Write(text);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        public void Open(string path)
        {
            try
            {
                var json = ReadUtf8Text(path);
                var project = Serializer.FromJson<Project>(json);

                var root = new Uri(path);
                var images = Editor.GetAllShapes<XImage>(project);
                _editor.ToAbsoluteUri(root, images);

                _editor.History.Reset();
                _editor.Load(project);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            try
            {
                var root = new Uri(path);
                var images = Editor.GetAllShapes<XImage>(_editor.Project);
                _editor.ToRelativeUri(root, images);

                var json = Serializer.ToJson(_editor.Project);
                WriteUtf8Text(path, json);

                _editor.ToAbsoluteUri(root, images);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="item"></param>
        public void ExportAsPdf(string path, object item)
        {
            try
            {
                var renderer = new PdfRenderer()
                {
                    DrawShapeState = ShapeState.Printable
                };

                if (item is Container)
                {
                    renderer.Save(path, item as Container);
                }
                else if (item is Document)
                {
                    renderer.Save(path, item as Document);
                }
                else if (item is Project)
                {
                    renderer.Save(path, item as Project);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }
  
        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        public void ExportAsEmf(string path)
        {
            try
            {
                Emf.Save(path, _editor.Project.CurrentContainer);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }
        
        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="version"></param>
        public void ExportAsDxf(string path, Dxf.DxfAcadVer version)
        {
            try
            {
                var renderer = new DxfRenderer()
                {
                    DrawShapeState = ShapeState.Printable
                };
                renderer.Create(path, _editor.Project.CurrentContainer, version);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="item"></param>
        /// <param name="type"></param>
        public void ImportEx(string path, object item, ImportType type)
        {
            // TODO: Add ImmutableArray support.

            try
            {
                switch (type)
                {
                    case ImportType.Style:
                        {
                            var sg = item as ShapeStyleGroup;
                            var json = ReadUtf8Text(path, false);
                            var import = Serializer.FromJson<ShapeStyle>(json);

                            var previous = sg.Styles;
                            var next = sg.Styles.Add(import);
                            _editor.History.Snapshot(previous, next, (p) => sg.Styles = p);
                            sg.Styles = next;
                        }
                        break;
                    case ImportType.Styles:
                        {
                            var sg = item as ShapeStyleGroup;
                            var json = ReadUtf8Text(path, false);
                            var import = Serializer.FromJson<IList<ShapeStyle>>(json);

                            var builder = sg.Styles.ToBuilder();
                            foreach (var style in import)
                            {
                                builder.Add(style);
                            }

                            var previous = sg.Styles;
                            var next = builder.ToImmutable();
                            _editor.History.Snapshot(previous, next, (p) => sg.Styles = p);
                            sg.Styles = next;
                        }
                        break;
                    case ImportType.StyleGroup:
                        {
                            var project = item as Project;
                            var json = ReadUtf8Text(path, false);
                            var import = Serializer.FromJson<ShapeStyleGroup>(json);

                            var previous = project.StyleGroups;
                            var next = project.StyleGroups.Add(import);
                            _editor.History.Snapshot(previous, next, (p) => project.StyleGroups = p);
                            project.StyleGroups = next;
                        }
                        break;
                    case ImportType.StyleGroups:
                        {
                            var project = item as Project;
                            var json = ReadUtf8Text(path, false);
                            var import = Serializer.FromJson<IList<ShapeStyleGroup>>(json);

                            var builder = project.StyleGroups.ToBuilder();
                            foreach (var sg in import)
                            {
                                builder.Add(sg);
                            }

                            var previous = project.StyleGroups;
                            var next = builder.ToImmutable();
                            _editor.History.Snapshot(previous, next, (p) => project.StyleGroups = p);
                            project.StyleGroups = next;
                        }
                        break;
                    case ImportType.Group:
                        {
                            var gl = item as GroupLibrary;
                            var json = ReadUtf8Text(path, false);
                            var import = Serializer.FromJson<XGroup>(json);

                            var shapes = Enumerable.Repeat(import as XGroup, 1);
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);
                            var root = new Uri(path);
                            var images = Editor.GetAllShapes<XImage>(shapes);
                            _editor.ToAbsoluteUri(root, images);

                            var previous = gl.Groups;
                            var next = gl.Groups.Add(import);
                            _editor.History.Snapshot(previous, next, (p) => gl.Groups = p);
                            gl.Groups = next;
                        }
                        break;
                    case ImportType.Groups:
                        {
                            var gl = item as GroupLibrary;
                            var json = ReadUtf8Text(path, false);
                            var import = Serializer.FromJson<IList<XGroup>>(json);

                            var shapes = import;
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);
                            var root = new Uri(path);
                            var images = Editor.GetAllShapes<XImage>(shapes);
                            _editor.ToAbsoluteUri(root, images);

                            var builder = gl.Groups.ToBuilder();
                            foreach (var group in import)
                            {
                                builder.Add(group);
                            }

                            var previous = gl.Groups;
                            var next = builder.ToImmutable();
                            _editor.History.Snapshot(previous, next, (p) => gl.Groups = p);
                            gl.Groups = next;
                        }
                        break;
                    case ImportType.GroupLibrary:
                        {
                            var project = item as Project;
                            var json = ReadUtf8Text(path, false);
                            var import = Serializer.FromJson<GroupLibrary>(json);

                            var shapes = import.Groups;
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);
                            var root = new Uri(path);
                            var images = Editor.GetAllShapes<XImage>(shapes);
                            _editor.ToAbsoluteUri(root, images);

                            var previous = project.GroupLibraries;
                            var next = project.GroupLibraries.Add(import);
                            _editor.History.Snapshot(previous, next, (p) => project.GroupLibraries = p);
                            project.GroupLibraries = next;
                        }
                        break;
                    case ImportType.GroupLibraries:
                        {
                            var project = item as Project;
                            var json = ReadUtf8Text(path, false);
                            var import = Serializer.FromJson<IList<GroupLibrary>>(json);

                            var shapes = import.SelectMany(x => x.Groups);
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);
                            var root = new Uri(path);
                            var images = Editor.GetAllShapes<XImage>(shapes);
                            _editor.ToAbsoluteUri(root, images);

                            var builder = project.GroupLibraries.ToBuilder();
                            foreach (var library in import)
                            {
                                builder.Add(library);
                            }

                            var previous = project.GroupLibraries;
                            var next = builder.ToImmutable();
                            _editor.History.Snapshot(previous, next, (p) => project.GroupLibraries = p);
                            project.GroupLibraries = next;
                        }
                        break;
                    case ImportType.Template:
                        {
                            var project = item as Project;
                            var json = ReadUtf8Text(path, false);
                            var import = Serializer.FromJson<Container>(json);

                            var shapes = import.Layers.SelectMany(x => x.Shapes);
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);
                            var root = new Uri(path);
                            var images = Editor.GetAllShapes<XImage>(shapes);
                            _editor.ToAbsoluteUri(root, images);

                            var previous = project.Templates;
                            var next = project.Templates.Add(import);
                            _editor.History.Snapshot(previous, next, (p) => project.Templates = p);
                            project.Templates = next;
                        }
                        break;
                    case ImportType.Templates:
                        {
                            var project = item as Project;
                            var json = ReadUtf8Text(path, false);
                            var import = Serializer.FromJson<IList<Container>>(json);

                            var shapes = import.SelectMany(x => x.Layers).SelectMany(x => x.Shapes);
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);
                            var root = new Uri(path);
                            var images = Editor.GetAllShapes<XImage>(shapes);
                            _editor.ToAbsoluteUri(root, images);

                            var builder = project.Templates.ToBuilder();
                            foreach (var template in import)
                            {
                                builder.Add(template);
                            }

                            var previous = project.Templates;
                            var next = builder.ToImmutable();
                            _editor.History.Snapshot(previous, next, (p) => project.Templates = p);
                            project.Templates = next;
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="item"></param>
        /// <param name="type"></param>
        public void ExportEx(string path, object item, ExportType type)
        {
            try
            {
                switch (type)
                {
                    case ExportType.Style:
                        {
                            var json = Serializer.ToJson(item as ShapeStyle);
                            WriteUtf8Text(path, json, false);
                        }
                        break;
                    case ExportType.Styles:
                        {
                            var json = Serializer.ToJson((item as ShapeStyleGroup).Styles);
                            WriteUtf8Text(path, json, false);
                        }
                        break;
                    case ExportType.StyleGroup:
                        {
                            var json = Serializer.ToJson((item as ShapeStyleGroup));
                            WriteUtf8Text(path, json, false);
                        }
                        break;
                    case ExportType.StyleGroups:
                        {
                            var json = Serializer.ToJson((item as Project).StyleGroups);
                            WriteUtf8Text(path, json, false);
                        }
                        break;
                    case ExportType.Group:
                        {
                            var shapes = Enumerable.Repeat(item as XGroup, 1);
                            var root = new Uri(path);
                            var images = Editor.GetAllShapes<XImage>(shapes);
                            _editor.ToRelativeUri(root, images);

                            var json = Serializer.ToJson(item as XGroup);
                            WriteUtf8Text(path, json, false);

                            _editor.ToAbsoluteUri(root, images);
                        }
                        break;
                    case ExportType.Groups:
                        {
                            var shapes = (item as GroupLibrary).Groups;
                            var root = new Uri(path);
                            var images = Editor.GetAllShapes<XImage>(shapes);
                            _editor.ToRelativeUri(root, images);

                            var json = Serializer.ToJson((item as GroupLibrary).Groups);
                            WriteUtf8Text(path, json, false);

                            _editor.ToAbsoluteUri(root, images);
                        }
                        break;
                    case ExportType.GroupLibrary:
                        {
                            var shapes = (item as GroupLibrary).Groups;
                            var root = new Uri(path);
                            var images = Editor.GetAllShapes<XImage>(shapes);
                            _editor.ToRelativeUri(root, images);

                            var json = Serializer.ToJson(item as GroupLibrary);
                            WriteUtf8Text(path, json, false);

                            _editor.ToAbsoluteUri(root, images);
                        }
                        break;
                    case ExportType.GroupLibraries:
                        {
                            var shapes = (item as Project).GroupLibraries.SelectMany(x => x.Groups);
                            var root = new Uri(path);
                            var images = Editor.GetAllShapes<XImage>(shapes);
                            _editor.ToRelativeUri(root, images);

                            var json = Serializer.ToJson((item as Project).GroupLibraries);
                            WriteUtf8Text(path, json, false);

                            _editor.ToAbsoluteUri(root, images);
                        }
                        break;
                    case ExportType.Template:
                        {
                            var shapes = (item as Container).Layers.SelectMany(x => x.Shapes);
                            var root = new Uri(path);
                            var images = Editor.GetAllShapes<XImage>(shapes);
                            _editor.ToRelativeUri(root, images);

                            var json = Serializer.ToJson(item as Container);
                            WriteUtf8Text(path, json, false);

                            _editor.ToAbsoluteUri(root, images);
                        }
                        break;
                    case ExportType.Templates:
                        {
                            var shapes = (item as Project).Templates.SelectMany(x => x.Layers).SelectMany(x => x.Shapes);
                            var root = new Uri(path);
                            var images = Editor.GetAllShapes<XImage>(shapes);
                            _editor.ToRelativeUri(root, images);

                            var json = Serializer.ToJson((item as Project).Templates);
                            WriteUtf8Text(path, json, false);

                            _editor.ToAbsoluteUri(root, images);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public Database OpenDatabase(string path)
        {
            var reader = new CsvReader();
            var fields = reader.Read(path);
            var name = System.IO.Path.GetFileNameWithoutExtension(path);

            var db = Database.Create(name);

            var tempColumns = fields.FirstOrDefault().Select(c => Column.Create(c));
            var columns = ImmutableArray.CreateRange<Column>(tempColumns);

            if (columns.Length >= 1 && columns[0].Name == "Id")
            {
                // use existing record Id's
                var tempRecords = fields
                    .Skip(1)
                    .Select(v =>
                            Record.Create(
                                v.FirstOrDefault(),
                                columns,
                                ImmutableArray.CreateRange<Value>(v.Select(c => Value.Create(c)))));
                var records = ImmutableArray.CreateRange<Record>(tempRecords);

                db.Columns = columns;
                db.Records = records;
            }
            else
            {
                // create records with new Id's
                var tempRecords = fields
                    .Skip(1)
                    .Select(v =>
                            Record.Create(
                                columns,
                                ImmutableArray.CreateRange<Value>(v.Select(c => Value.Create(c)))));
                var records = ImmutableArray.CreateRange<Record>(tempRecords);

                db.Columns = columns;
                db.Records = records;
            }

            return db;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        public void ImportData(string path)
        {
            try
            {
                var db = OpenDatabase(path);

                var previous = _editor.Project.Databases;
                var next = _editor.Project.Databases.Add(db);
                _editor.History.Snapshot(previous, next, (p) => _editor.Project.Databases = p);
                _editor.Project.Databases = next;

                _editor.Project.CurrentDatabase = db;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="database"></param>
        public void ExportData(string path, Database database)
        {
            try
            {
                using (var writer = new System.IO.StringWriter())
                {
                    var configuration = new CsvHelper.Configuration.CsvConfiguration();

                    configuration.Delimiter = System.Globalization.CultureInfo.CurrentCulture.TextInfo.ListSeparator;
                    configuration.CultureInfo = System.Globalization.CultureInfo.CurrentCulture;

                    using (var csv = new CsvHelper.CsvWriter(writer, configuration))
                    {
                        // columns
                        csv.WriteField("Id");
                        foreach (var column in database.Columns)
                        {
                            csv.WriteField(column.Name);
                        }
                        csv.NextRecord();

                        // records
                        foreach (var record in database.Records)
                        {
                            csv.WriteField(record.Id.ToString());
                            foreach (var value in record.Values)
                            {
                                csv.WriteField(value.Content);
                            }
                            csv.NextRecord();
                        }
                    }

                    using (var file = System.IO.File.CreateText(path))
                    {
                        file.Write(writer.ToString());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="path"></param>
        /// <param name="database"></param>
        public void UpdateData(string path, Database database)
        {
            try
            {
                var db = OpenDatabase(path);

                if (db.Columns.Length <= 1)
                    return;

                // check for the Id column
                if (db.Columns[0].Name != "Id")
                    return;

                // skip Id column for update
                if (db.Columns.Length - 1 != database.Columns.Length)
                    return;

                // check column names
                for (int i = 1; i < db.Columns.Length; i++)
                {
                    if (db.Columns[i].Name != database.Columns[i - 1].Name)
                        return;
                }

                bool isDirty = false;
                var recordsBuilder = database.Records.ToBuilder();

                for (int i = 0; i < database.Records.Length; i++)
                {
                    var record = database.Records[i];
                    
                    var result = db.Records.FirstOrDefault(r => r.Id == record.Id);
                    if (result != null)
                    {
                        // update existing record
                        for (int j = 1; j < result.Values.Length; j++)
                        {
                            var valuesBuilder = record.Values.ToBuilder();
                            valuesBuilder[j - 1] = result.Values[j];
                            record.Values = valuesBuilder.ToImmutable();
                        }
                        isDirty = true;
                    }
                    else
                    {
                        var r = db.Records[i];

                        // use existing columns
                        r.Columns = database.Columns;

                        // skip Id column
                        r.Values = r.Values.Skip(1).ToImmutableArray();

                        recordsBuilder.Add(r);
                        isDirty = true;
                    }
                }

                if (isDirty)
                {
                    //var previous = database.Records;
                    //var next = builder.ToImmutable();
                    //_editor.History.Snapshot(previous, next, (p) => database.Records = p);
                    //database.Records = next;

                    var builder = _editor.Project.Databases.ToBuilder();
                    var index = builder.IndexOf(database);
                    database.Records = recordsBuilder.ToImmutable();
                    builder[index] = database;

                    var previous = _editor.Project.Databases;
                    var next = builder.ToImmutable();
                    _editor.History.Snapshot(previous, next, (p) => _editor.Project.Databases = p);
                    _editor.Project.Databases = next;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public bool CanCopy()
        {
            return _editor.IsSelectionAvailable();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public bool CanPaste()
        {
            try
            {
                return _textClipboard.ContainsText();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="shapes"></param>
        private void Copy(IList<BaseShape> shapes)
        {
            try
            {
                var json = Serializer.ToJson(shapes);
                if (!string.IsNullOrEmpty(json))
                {
                    _textClipboard.SetText(json);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="json"></param>
        public void Paste(string json)
        {
            try
            {
                var shapes = Serializer.FromJson<IList<BaseShape>>(json);
                if (shapes != null && shapes.Count() > 0)
                {
                    Paste(shapes);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="shapes"></param>
        private void TryToRestoreStyles(IEnumerable<BaseShape> shapes)
        {
            try
            {
                if (_editor.Project.StyleGroups == null)
                    return;

                var styles = _editor.Project.StyleGroups
                    .Where(sg => sg.Styles != null && sg.Styles.Length > 0)
                    .SelectMany(sg => sg.Styles)
                    .Distinct(new StyleComparer())
                    .ToDictionary(s => s.Name);

                // reset point shape to container default
                foreach (var point in Editor.GetAllPoints(shapes, ShapeState.Connector))
                {
                    point.Shape = _editor.Project.Options.PointShape;
                }

                // try to restore shape styles
                foreach (var shape in Editor.GetAllShapes(shapes))
                {
                    if (shape.Style == null)
                        continue;

                    ShapeStyle style;
                    if (styles.TryGetValue(shape.Style.Name, out style))
                    {
                        // use existing style
                        shape.Style = style;
                    }
                    else
                    {
                        // create Imported style group
                        if (_editor.Project.CurrentStyleGroup == null)
                        {
                            var sg = ShapeStyleGroup.Create("Imported");
                            _editor.Project.StyleGroups = _editor.Project.StyleGroups.Add(sg);
                            _editor.Project.CurrentStyleGroup = sg;
                        }

                        // add missing style
                        _editor.Project.CurrentStyleGroup.Styles = _editor.Project.CurrentStyleGroup.Styles.Add(shape.Style);

                        // recreate styles dictionary
                        styles = _editor.Project.StyleGroups
                            .Where(sg => sg.Styles != null && sg.Styles.Length > 0)
                            .SelectMany(sg => sg.Styles)
                            .Distinct(new StyleComparer())
                            .ToDictionary(s => s.Name);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="shapes"></param>
        private void TryToRestoreRecords(IEnumerable<BaseShape> shapes)
        {
            try
            {
                if (_editor.Project.Databases == null)
                    return;

                var records = _editor.Project.Databases
                    .Where(d => d.Records != null && d.Records.Length > 0)
                    .SelectMany(d => d.Records)
                    .ToDictionary(s => s.Id);

                // try to restore shape record
                foreach (var shape in Editor.GetAllShapes(shapes))
                {
                    if (shape.Record == null)
                        continue;

                    Record record;
                    if (records.TryGetValue(shape.Record.Id, out record))
                    {
                        // use existing record
                        shape.Record = record;
                    }
                    else
                    {
                        // create Imported database
                        if (_editor.Project.CurrentDatabase == null)
                        {
                            var db = Database.Create("Imported", shape.Record.Columns);
                            _editor.Project.Databases = _editor.Project.Databases.Add(db);
                            _editor.Project.CurrentDatabase = db;
                        }

                        // add missing data record
                        _editor.Project.CurrentDatabase.Records = _editor.Project.CurrentDatabase.Records.Add(shape.Record);

                        // recreate records dictionary
                        records = _editor.Project.Databases
                            .Where(d => d.Records != null && d.Records.Length > 0)
                            .SelectMany(d => d.Records)
                            .ToDictionary(s => s.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="shapes"></param>
        public void Paste(IEnumerable<BaseShape> shapes)
        {
            try
            {
                _editor.Deselect(_editor.Project.CurrentContainer);

                TryToRestoreStyles(shapes);
                TryToRestoreRecords(shapes);

                var layer = _editor.Project.CurrentContainer.CurrentLayer;

                var builder = layer.Shapes.ToBuilder();
                foreach (var shape in shapes)
                {
                    builder.Add(shape);
                }

                var previous = layer.Shapes;
                var next = builder.ToImmutable();
                _editor.History.Snapshot(previous, next, (p) => layer.Shapes = p);
                layer.Shapes = next;

                _editor.Select(_editor.Project.CurrentContainer, ImmutableHashSet.CreateRange<BaseShape>(shapes));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public XGroup Clone(XGroup group)
        {
            try
            {
                var json = Serializer.ToJson(group);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = Serializer.FromJson<XGroup>(json);
                    if (clone != null)
                    {
                        var shapes = Enumerable.Repeat(clone, 1).ToList();
                        TryToRestoreStyles(shapes);
                        TryToRestoreRecords(shapes);
                        return clone;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }

            return null;
        }
        
        /// <summary>
        ///
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public Container Clone(Container container)
        {
            try
            {
                var template = container.Template;
                var json = Serializer.ToJson(container);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = Serializer.FromJson<Container>(json);
                    if (clone != null)
                    {
                        var shapes = clone.Layers.SelectMany(l => l.Shapes);
                        TryToRestoreStyles(shapes);
                        TryToRestoreRecords(shapes);
                        clone.Template = template;
                        return clone;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }

            return null;
        }
        
        /// <summary>
        ///
        /// </summary>
        /// <param name="document"></param>
        /// <returns></returns>
        public Document Clone(Document document)
        {
            try
            {
                var templates = document.Containers.Select(c => c.Template).ToArray();
                var json = Serializer.ToJson(document);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = Serializer.FromJson<Document>(json);
                    if (clone != null)
                    {
                        for (int i = 0; i < clone.Containers.Length; i++)
                        {
                            var container = clone.Containers[i];
                            var shapes = container.Layers.SelectMany(l => l.Shapes);
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);
                            container.Template = templates[i];
                        }
                        return clone;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }

            return null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="files"></param>
        /// <returns></returns>
        public bool Drop(string[] files)
        {
            try
            {
                if (files != null && files.Length >= 1)
                {
                    bool result = false;
                    foreach (var path in files)
                    {
                        if (string.IsNullOrEmpty(path))
                            continue;

                        string ext = System.IO.Path.GetExtension(path);

                        if (string.Compare(ext, ".project", true, CultureInfo.InvariantCulture) == 0)
                        {
                            Open(path);
                            result = true;
                        }
                        else if (string.Compare(ext, ".cs", true, CultureInfo.InvariantCulture) == 0)
                        {
                            Eval(path);
                            result = true;
                        }
                        else if (string.Compare(ext, ".csv", true, CultureInfo.InvariantCulture) == 0)
                        {
                            ImportData(path);
                            result = true;
                        }
                        else if (string.Compare(ext, ".style", true, CultureInfo.InvariantCulture) == 0)
                        {
                            ImportEx(path, _editor.Project.CurrentStyleGroup, ImportType.Style);
                            result = true;
                        }
                        else if (string.Compare(ext, ".styles", true, CultureInfo.InvariantCulture) == 0)
                        {
                            ImportEx(path, _editor.Project.CurrentStyleGroup, ImportType.Styles);
                            result = true;
                        }
                        else if (string.Compare(ext, ".stylegroup", true, CultureInfo.InvariantCulture) == 0)
                        {
                            ImportEx(path, _editor.Project, ImportType.StyleGroup);
                            result = true;
                        }
                        else if (string.Compare(ext, ".stylegroups", true, CultureInfo.InvariantCulture) == 0)
                        {
                            ImportEx(path, _editor.Project, ImportType.StyleGroups);
                            result = true;
                        }
                        else if (string.Compare(ext, ".group", true, CultureInfo.InvariantCulture) == 0)
                        {
                            ImportEx(path, _editor.Project.CurrentGroupLibrary, ImportType.Group);
                            result = true;
                        }
                        else if (string.Compare(ext, ".groups", true, CultureInfo.InvariantCulture) == 0)
                        {
                            ImportEx(path, _editor.Project.CurrentGroupLibrary, ImportType.Groups);
                            result = true;
                        }
                        else if (string.Compare(ext, ".grouplibrary", true, CultureInfo.InvariantCulture) == 0)
                        {
                            ImportEx(path, _editor.Project, ImportType.GroupLibrary);
                            result = true;
                        }
                        else if (string.Compare(ext, ".grouplibraries", true, CultureInfo.InvariantCulture) == 0)
                        {
                            ImportEx(path, _editor.Project, ImportType.GroupLibraries);
                            result = true;
                        }
                        else if (string.Compare(ext, ".template", true, CultureInfo.InvariantCulture) == 0)
                        {
                            ImportEx(path, _editor.Project, ImportType.Template);
                            result = true;
                        }
                        else if (string.Compare(ext, ".templates", true, CultureInfo.InvariantCulture) == 0)
                        {
                            ImportEx(path, _editor.Project, ImportType.Templates);
                            result = true;
                        }
                    }

                    return result;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }

            return false;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="group"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Drop(XGroup group, double x, double y)
        {
            try
            {
                double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
                double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;

                var clone = Clone(group);
                if (clone != null)
                {
                    _editor.Deselect(_editor.Project.CurrentContainer);
                    clone.Move(sx, sy);

                    var layer = _editor.Project.CurrentContainer.CurrentLayer;
  
                    var previous = layer.Shapes;
                    var next = layer.Shapes.Add(clone);
                    _editor.History.Snapshot(previous, next, (p) => layer.Shapes = p);
                    layer.Shapes = next;

                    _editor.Select(_editor.Project.CurrentContainer, clone);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="record"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Drop(Record record, double x, double y)
        {
            try
            {
                if (_editor.Renderer.SelectedShape != null)
                {
                    // TODO: Add history snapshot.
                    _editor.Renderer.SelectedShape.Record = record;
                }
                else if (_editor.Renderer.SelectedShapes != null && _editor.Renderer.SelectedShapes.Count > 0)
                {
                    // TODO: Add history snapshot.
                    foreach (var shape in _editor.Renderer.SelectedShapes)
                    {
                        shape.Record = record;
                    }
                }
                else
                {
                    var container = _editor.Project.CurrentContainer;
                    if (container != null)
                    {
                        var result = ShapeBounds.HitTest(container, new Vector2(x, y), _editor.Project.Options.HitTreshold);
                        if (result != null)
                        {
                            // TODO: Add history snapshot.
                            result.Record = record;
                        }
                        else
                        {
                            DropAsGroup(record, x, y);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="record"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void DropAsGroup(Record record, double x, double y)
        {
            var g = XGroup.Create("g");
            g.Record = record;

            double sx = _editor.Project.Options.SnapToGrid ? Editor.Snap(x, _editor.Project.Options.SnapX) : x;
            double sy = _editor.Project.Options.SnapToGrid ? Editor.Snap(y, _editor.Project.Options.SnapY) : y;

            var length = record.Values.Length;
            double px = sx;
            double py = sy;
            double width = 150;
            double height = 15;

            for (int i = 0; i < length; i++)
            {
                var column = record.Columns[i];
                if (column.IsVisible)
                {
                    var text = XText.Create(
                        px, py,
                        px + width, py + height,
                        _editor.Project.CurrentStyleGroup.CurrentStyle,
                        _editor.Project.Options.PointShape, "");
                    var binding = ShapeBinding.Create("Text", record.Columns[i].Name);
                    text.Bindings = text.Bindings.Add(binding);
                    g.AddShape(text);

                    py += height;
                }
            }

            var rectangle = XRectangle.Create(
                sx, sy,
                sx + width, sy + (double)length * height,
                _editor.Project.CurrentStyleGroup.CurrentStyle,
                _editor.Project.Options.PointShape);
            g.AddShape(rectangle);

            double ptx = sx + width / 2;
            double pty = sy;

            double pbx = sx + width / 2;
            double pby = sy + (double)length * height;

            double plx = sx;
            double ply = sy + ((double)length * height) / 2;

            double prx = sx + width;
            double pry = sy + ((double)length * height) / 2;

            var pt = XPoint.Create(ptx, pty, _editor.Project.Options.PointShape);
            var pb = XPoint.Create(pbx, pby, _editor.Project.Options.PointShape);
            var pl = XPoint.Create(plx, ply, _editor.Project.Options.PointShape);
            var pr = XPoint.Create(prx, pry, _editor.Project.Options.PointShape);

            g.AddConnectorAsNone(pt);
            g.AddConnectorAsNone(pb);
            g.AddConnectorAsNone(pl);
            g.AddConnectorAsNone(pr);

            var layer = _editor.Project.CurrentContainer.CurrentLayer;

            var previous = layer.Shapes;
            var next = layer.Shapes.Add(g);
            _editor.History.Snapshot(previous, next, (p) => layer.Shapes = p);
            layer.Shapes = next;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="style"></param>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Drop(ShapeStyle style, double x, double y)
        {
            try
            {
                if (_editor.Renderer.SelectedShape != null)
                {
                    // TODO: Add history snapshot.
                    _editor.Renderer.SelectedShape.Style = style;
                }
                else if (_editor.Renderer.SelectedShapes != null && _editor.Renderer.SelectedShapes.Count > 0)
                {
                    // TODO: Add history snapshot.
                    foreach (var shape in _editor.Renderer.SelectedShapes)
                    {
                        shape.Style = style;
                    }
                }
                else
                {
                    var container = _editor.Project.CurrentContainer;
                    if (container != null)
                    {
                        var result = ShapeBounds.HitTest(container, new Vector2(x, y), _editor.Project.Options.HitTreshold);
                        if (result != null)
                        {
                            // Add history snapshot.
                            result.Style = style;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public bool CanUndo()
        {
            return _editor.History.CanUndo();
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public bool CanRedo()
        {
            return _editor.History.CanRedo();
        }

        /// <summary>
        ///
        /// </summary>
        public void Undo()
        {
            try
            {
                if (_editor.History.CanUndo())
                {
                    _editor.Deselect();
                    _editor.History.Undo();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void Redo()
        {
            try
            {
                if (_editor.History.CanRedo())
                {
                    _editor.Deselect();
                    _editor.History.Redo();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void Cut()
        {
            try
            {
                if (CanCopy())
                {
                    Copy();

                    _editor.DeleteSelected();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void Copy()
        {
            try
            {
                if (CanCopy())
                {
                    if (_editor.Renderer.SelectedShape != null)
                    {
                        Copy(Enumerable.Repeat(_editor.Renderer.SelectedShape, 1).ToList());
                    }

                    if (_editor.Renderer.SelectedShapes != null)
                    {
                        Copy(_editor.Renderer.SelectedShapes.ToList());
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void Paste()
        {
            try
            {
                if (CanPaste())
                {
                    var json = _textClipboard.GetText();
                    if (!string.IsNullOrEmpty(json))
                    {
                        Paste(json);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void SelectAll()
        {
            try
            {
                _editor.Deselect(_editor.Project.CurrentContainer);
                _editor.Select(
                    _editor.Project.CurrentContainer,
                    ImmutableHashSet.CreateRange<BaseShape>(_editor.Project.CurrentContainer.CurrentLayer.Shapes));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void ClearAll()
        {
            try
            {
                // TODO: Add history snapshot.
                _editor.Project.CurrentContainer.Clear();
                _editor.Project.CurrentContainer.Invalidate();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public bool IsEditMode()
        {
            return _timer == null;
        }

        /// <summary>
        ///
        /// </summary>
        /// <returns></returns>
        public bool IsSimulationMode()
        {
            return _timer != null;
        }

        /// <summary>
        ///
        /// </summary>
        public void StartSimulation()
        {
            try
            {
                if (IsSimulationMode())
                {
                    return;
                }

                var graph = ContainerGraph.Create(Editor.Project.CurrentContainer);
                if (graph != null)
                {
                    Simulations = _simulationFactory.Create(graph);
                    if (Simulations != null)
                    {
                        // TODO: Use Working layer to show simulation state.
                        _clock = new Clock(cycle: 0L, resolution: 100);
                        IsSimulationPaused = false;
                        _timer = new System.Threading.Timer(
                            (state) =>
                            {
                                try
                                {
                                    if (!IsSimulationPaused)
                                    {
                                        TickSimulation();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.Print(ex.Message);
                                    System.Diagnostics.Debug.Print(ex.StackTrace);

                                    if (IsSimulationMode())
                                    {
                                        StopSimulation();
                                    }
                                }
                            },
                            null, 0, _clock.Resolution);

                        UpdateCanExecuteState();
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void StopSimulation()
        {
            try
            {
                // TODO: Reset Working layer simulation state.

                if (IsSimulationMode())
                {
                    _timer.Dispose();
                    _timer = default(System.Threading.Timer);
                    IsSimulationPaused = false;
                    UpdateCanExecuteState();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void RestartSimulation()
        {
            StopSimulation();
            StartSimulation();
        }

        /// <summary>
        ///
        /// </summary>
        public void PauseSimulation()
        {
            try
            {
                if (IsSimulationMode())
                {
                    IsSimulationPaused = !IsSimulationPaused;
                    UpdateCanExecuteState();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        public void TickSimulation()
        {
            try
            {
                if (IsSimulationMode())
                {
                    _simulationFactory.Run(Simulations, _clock);
                    _clock.Tick();
                    // TODO: Update Working layer simulation state.
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        /// <summary>
        ///
        /// </summary>
        private void UpdateCanExecuteState()
        {
            (_commands.NewCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.OpenCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.SaveAsCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ExportCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.ExitCommand as DelegateCommand).RaiseCanExecuteChanged();

            (_commands.ImportDataCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.ExportDataCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.UpdateDataCommand as DelegateCommand<object>).RaiseCanExecuteChanged();

            (_commands.ImportStyleCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.ImportStylesCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.ImportStyleGroupCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.ImportStyleGroupsCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.ImportGroupCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.ImportGroupsCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.ImportGroupLibraryCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.ImportGroupLibrariesCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.ImportTemplateCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.ImportTemplatesCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.ExportStyleCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.ExportStylesCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.ExportStyleGroupCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.ExportStyleGroupsCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.ExportGroupCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.ExportGroupsCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.ExportGroupLibraryCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.ExportGroupLibrariesCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.ExportTemplateCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.ExportTemplatesCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
 
            (_commands.UndoCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.RedoCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.CopyAsEmfCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.CutCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.CopyCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.PasteCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.DeleteCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.SelectAllCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ClearAllCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.GroupCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.GroupLayerCommand as DelegateCommand).RaiseCanExecuteChanged();

            (_commands.ToolNoneCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ToolSelectionCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ToolGroupCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ToolPointCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ToolLineCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ToolRectangleCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ToolEllipseCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ToolArcCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ToolBezierCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ToolQBezierCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ToolTextCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ToolImageCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ToolPathCommand as DelegateCommand).RaiseCanExecuteChanged();

            (_commands.EvalCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.EvalScriptCommand as DelegateCommand<string>).RaiseCanExecuteChanged();

            (_commands.DefaultIsFilledCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.SnapToGridCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.TryToConnectCommand as DelegateCommand).RaiseCanExecuteChanged();

            (_commands.AddDatabaseCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.RemoveDatabaseCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.AddRecordCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.RemoveRecordCommand as DelegateCommand).RaiseCanExecuteChanged();

            (_commands.AddBindingCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.RemoveBindingCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            
            (_commands.AddPropertyCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.RemovePropertyCommand as DelegateCommand<object>).RaiseCanExecuteChanged();

            (_commands.AddGroupLibraryCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.RemoveGroupLibraryCommand as DelegateCommand).RaiseCanExecuteChanged();

            (_commands.AddGroupCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.RemoveGroupCommand as DelegateCommand).RaiseCanExecuteChanged();

            (_commands.AddLayerCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.RemoveLayerCommand as DelegateCommand).RaiseCanExecuteChanged();

            (_commands.AddStyleCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.RemoveStyleCommand as DelegateCommand).RaiseCanExecuteChanged();

            (_commands.AddStyleGroupCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.RemoveStyleGroupCommand as DelegateCommand).RaiseCanExecuteChanged();

            (_commands.RemoveShapeCommand as DelegateCommand).RaiseCanExecuteChanged();

            (_commands.StartSimulationCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.StopSimulationCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.RestartSimulationCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.PauseSimulationCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.TickSimulationCommand as DelegateCommand).RaiseCanExecuteChanged();

            (_commands.ZoomResetCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ZoomExtentCommand as DelegateCommand).RaiseCanExecuteChanged();

            (_commands.DatabasesWindowCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.LayersWindowCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.StyleWindowCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.StylesWindowCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ShapesWindowCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ContainerWindowCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ScriptWindowCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.PropertiesWindowCommand as DelegateCommand).RaiseCanExecuteChanged();

            (_commands.AddTemplateCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.RemoveTemplateCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.EditTemplateCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ApplyTemplateCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            
            (_commands.SelectedItemChangedCommand as DelegateCommand<object>).RaiseCanExecuteChanged();

            (_commands.AddContainerCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.InsertContainerBeforeCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.InsertContainerAfterCommand as DelegateCommand<object>).RaiseCanExecuteChanged();

            (_commands.AddDocumentCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.InsertDocumentBeforeCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.InsertDocumentAfterCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
        }

        /// <summary>
        ///
        /// </summary>
        public void Dispose()
        {
            if (_watcher != null)
            {
                _watcher.Dispose();
            }

            if (IsSimulationMode())
            {
                StopSimulation();
            }
        }
    }
}
