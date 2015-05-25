// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.CSharp;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
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
        private ITextClipboard _textClipboard;
        private ISerializer _serializer;
        private ICompressor _compressor;
        private string _rootScriptsPath;
        private IList<ScriptDirectory> _scriptDirectories;
        private bool _isSimulationPaused;
        private System.IO.FileSystemWatcher _watcher = default(System.IO.FileSystemWatcher);
        private System.Threading.Timer _timer = default(System.Threading.Timer);
        private BoolSimulationFactory _simulationFactory = default(BoolSimulationFactory);
        private IDictionary<XGroup, BoolSimulation> _simulations;
        private Clock _clock = default(Clock);

        /// <summary>
        /// 
        /// </summary>
        public EditorCommands Commands
        {
            get { return _commands; }
            set
            {
                if (value != _commands)
                {
                    _commands = value;
                    Notify("Commands");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public Editor Editor
        {
            get { return _editor; }
            set
            {
                if (value != _editor)
                {
                    _editor = value;
                    Notify("Editor");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ITextClipboard TextClipboard
        {
            get { return _textClipboard; }
            set
            {
                if (value != _textClipboard)
                {
                    _textClipboard = value;
                    Notify("TextClipboard");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ISerializer Serializer
        {
            get { return _serializer; }
            set
            {
                if (value != _serializer)
                {
                    _serializer = value;
                    Notify("Serializer");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public ICompressor Compressor
        {
            get { return _compressor; }
            set
            {
                if (value != _compressor)
                {
                    _compressor = value;
                    Notify("Compressor");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string RootScriptsPath
        {
            get { return _rootScriptsPath; }
            set
            {
                if (value != _rootScriptsPath)
                {
                    _rootScriptsPath = value;
                    Notify("RootScriptsPath");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IList<ScriptDirectory> ScriptDirectories
        {
            get { return _scriptDirectories; }
            set
            {
                if (value != _scriptDirectories)
                {
                    _scriptDirectories = value;
                    Notify("ScriptDirectories");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsSimulationPaused
        {
            get { return _isSimulationPaused; }
            set
            {
                if (value != _isSimulationPaused)
                {
                    _isSimulationPaused = value;
                    Notify("IsSimulationPaused");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<XGroup, BoolSimulation> Simulations
        {
            get { return _simulations; }
            set
            {
                if (value != _simulations)
                {
                    _simulations = value;
                    Notify("Simulations");
                }
            }
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
            
            foreach (var shape in shapes) 
            {
                layer.Shapes.Add(shape);
            }

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

            project.Templates.Add(EmptyTemplate(project));
            project.Templates.Add(GridTemplate(project));
            project.CurrentTemplate = project.Templates.FirstOrDefault(t => t.Name == "Grid");

            var document1 = DefaultDocument(project);
            var document2 = DefaultDocument(project);

            var container1 = DefaultContainer(project);
            var container2 = DefaultContainer(project);

            document1.Containers.Add(container1);
            document2.Containers.Add(container2);

            project.Documents.Add(document1);
            project.Documents.Add(document2);

            project.CurrentDocument = project.Documents.FirstOrDefault();
            project.CurrentContainer = document1.Containers.FirstOrDefault();

            return project;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="view"></param>
        /// <param name="renderer"></param>
        /// <param name="clipboard"></param>
        /// <param name="serializer"></param>
        /// <param name="compressor"></param>
        public void Initialize(
            IView view, 
            IRenderer renderer, 
            ITextClipboard clipboard, 
            ISerializer serializer,
            ICompressor compressor)
        {
            try
            {
                _commands = new EditorCommands();

                _textClipboard = clipboard;
                _serializer = serializer;
                _compressor = compressor;

                _editor = Editor.Create(
                    DefaultProject(), 
                    renderer,
                    serializer, 
                    compressor);

                (_editor.Renderer as ObservableObject).PropertyChanged +=
                    (s, e) =>
                    {
                        if (e.PropertyName == "Zoom")
                        {
                            Invalidate();
                        }
                    };

                _commands.NewCommand = new DelegateCommand<object>(
                    (item) =>
                    {
                        if (item is Container)
                        {
                            var selected = item as Container;
                            var document = _editor.Project.Documents.FirstOrDefault(d => d.Containers.Contains(selected));
                            if (document != null)
                            {
                                var container = DefaultContainer(_editor.Project);
                                _editor.History.Snapshot(_editor.Project);
                                document.Containers.Add(container);
                                _editor.Project.CurrentContainer = container;
                            }
                        }
                        else if (item is Document)
                        {
                            var selected = item as Document;
                            var container = DefaultContainer(_editor.Project);
                            _editor.History.Snapshot(_editor.Project);
                            selected.Containers.Add(container);
                            _editor.Project.CurrentContainer = container;
                        }
                        else if (item is Project)
                        {
                            var document = DefaultDocument(_editor.Project);
                            _editor.History.Snapshot(_editor.Project);
                            _editor.Project.Documents.Add(document);
                            _editor.Project.CurrentDocument = document;
                            _editor.Project.CurrentContainer = document.Containers.FirstOrDefault();
                        }
                        else if (item is EditorContext || item == null)
                        {
                            _editor.History.Reset();
                            _editor.History.Snapshot(_editor.Project);
                            _editor.Load(DefaultProject());
                        }
                    },
                    (item) => IsEditMode());

                _commands.ExitCommand = new DelegateCommand(
                    () =>
                    {
                        view.Close();
                    },
                    () => true);

                _commands.UndoCommand = new DelegateCommand(
                    () =>
                    {
                        Undo();
                    },
                    () => IsEditMode() /* && CanUndo() */);

                _commands.RedoCommand = new DelegateCommand(
                    () =>
                    {
                        Redo();
                    },
                    () => IsEditMode() /* && CanRedo() */);

                var _containerToCopy = default(Container);
                var _documentToCopy = default(Document);

                _commands.CutCommand = new DelegateCommand<object>(
                    (item) =>
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
                    },
                    (item) => IsEditMode() /* && CanCopy() */);

                _commands.CopyCommand = new DelegateCommand<object>(
                    (item) =>
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
                    },
                    (item) => IsEditMode() /* && CanCopy() */);

                _commands.PasteCommand = new DelegateCommand<object>(
                    (item) =>
                    {
                        if (item is Container)
                        {
                            if (_containerToCopy != null)
                            {
                                var container = item as Container;
                                var document = _editor.Project.Documents.FirstOrDefault(d => d.Containers.Contains(container));
                                if (document != null)
                                {
                                    _editor.History.Snapshot(_editor.Project);
                                    int index = document.Containers.IndexOf(container);
                                    var clone = Clone(_containerToCopy);
                                    document.Containers[index] = clone;
                                    _editor.Project.CurrentContainer = clone;
                                }
                            }
                        }
                        else if (item is Document)
                        {
                            if (_containerToCopy != null)
                            {
                                var document = item as Document;
                                _editor.History.Snapshot(_editor.Project);
                                var clone = Clone(_containerToCopy);
                                document.Containers.Add(clone);
                                _editor.Project.CurrentContainer = clone;
                            }
                            else if (_documentToCopy != null)
                            {
                                var document = item as Document;
                                int index = _editor.Project.Documents.IndexOf(document);
                                var clone = Clone(_documentToCopy);
                                _editor.Project.Documents[index] = clone;
                                _editor.Project.CurrentDocument = clone;
                            }
                        }
                        else if (item is EditorContext || item == null)
                        {
                            Paste();
                        }
                    },
                    (item) => IsEditMode() /* && CanPaste() */);

                _commands.DeleteCommand = new DelegateCommand<object>(
                    (item) =>
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
                    },
                    (item) => IsEditMode() /* && _editor.IsSelectionAvailable() */);

                _commands.SelectAllCommand = new DelegateCommand(
                    () =>
                    {
                        SelectAll();
                    },
                    () => IsEditMode());

                _commands.ClearAllCommand = new DelegateCommand(
                    () =>
                    {
                        ClearAll();
                    },
                    () => IsEditMode());

                _commands.GroupCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.GroupSelected();
                    },
                    () => IsEditMode() /* && _editor.IsSelectionAvailable() */);

                _commands.GroupLayerCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.GroupCurrentLayer();
                    },
                    () => IsEditMode());

                _commands.ToolNoneCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.CurrentTool = Tool.None;
                    },
                    () => IsEditMode());

                _commands.ToolSelectionCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.CurrentTool = Tool.Selection;
                    },
                    () => IsEditMode());

                _commands.ToolPointCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.CurrentTool = Tool.Point;
                    },
                    () => IsEditMode());

                _commands.ToolLineCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.CurrentTool = Tool.Line;
                    },
                    () => IsEditMode());

                _commands.ToolRectangleCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.CurrentTool = Tool.Rectangle;
                    },
                    () => IsEditMode());

                _commands.ToolEllipseCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.CurrentTool = Tool.Ellipse;
                    },
                    () => IsEditMode());

                _commands.ToolArcCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.CurrentTool = Tool.Arc;
                    },
                    () => IsEditMode());

                _commands.ToolBezierCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.CurrentTool = Tool.Bezier;
                    },
                    () => IsEditMode());

                _commands.ToolQBezierCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.CurrentTool = Tool.QBezier;
                    },
                    () => IsEditMode());

                _commands.ToolTextCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.CurrentTool = Tool.Text;
                    },
                    () => IsEditMode());

                _commands.ToolImageCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.CurrentTool = Tool.Image;
                    },
                    () => IsEditMode());

                _commands.EvalScriptCommand = new DelegateCommand<string>(
                    (path) =>
                    {
                        Eval(path);
                    },
                    (path) => IsEditMode());

                _commands.DefaultIsFilledCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.Project.Options.DefaultIsFilled = !_editor.Project.Options.DefaultIsFilled;
                    },
                    () => IsEditMode());

                _commands.SnapToGridCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.Project.Options.SnapToGrid = !_editor.Project.Options.SnapToGrid;
                    },
                    () => IsEditMode());

                _commands.TryToConnectCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.Project.Options.TryToConnect = !_editor.Project.Options.TryToConnect;
                    },
                    () => IsEditMode());


                _commands.AddBindingCommand = new DelegateCommand<object>(
                    (owner) =>
                    {
                        if (owner != null)
                        {
                            if (owner is BaseShape)
                            {
                                var shape = owner as BaseShape;
                                if (shape.Bindings == null)
                                {
                                    shape.Bindings = new ObservableCollection<ShapeBinding>();
                                }

                                _editor.History.Snapshot(_editor.Project);
                                shape.Bindings.Add(ShapeBinding.Create("", ""));
                            }
                        }
                    },
                    (owner) => IsEditMode());

                _commands.RemoveBindingCommand = new DelegateCommand<object>(
                    (parameter) =>
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
                                    _editor.History.Snapshot(_editor.Project);
                                    shape.Bindings.Remove(binding);
                                }
                            }
                        }
                    },
                    (parameter) => IsEditMode());

                _commands.AddPropertyCommand = new DelegateCommand<object>(
                    (owner) =>
                    {
                        if (owner != null)
                        {
                            if (owner is BaseShape)
                            {
                                var shape = owner as BaseShape;
                                if (shape.Properties == null)
                                {
                                    shape.Properties = new ObservableCollection<ShapeProperty>();
                                }

                                _editor.History.Snapshot(_editor.Project);
                                shape.Properties.Add(ShapeProperty.Create("New", ""));
                            }
                            else if (owner is Container)
                            {
                                var container = owner as Container;
                                if (container.Properties == null)
                                {
                                    container.Properties = new ObservableCollection<ShapeProperty>();
                                }

                                _editor.History.Snapshot(_editor.Project);
                                container.Properties.Add(ShapeProperty.Create("New", ""));
                            }
                        }
                    },
                    (owner) => IsEditMode());

                _commands.RemovePropertyCommand = new DelegateCommand<object>(
                    (parameter) =>
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
                                    _editor.History.Snapshot(_editor.Project);
                                    shape.Properties.Remove(property);
                                }
                            }
                            else if (owner is Container)
                            {
                                var container = owner as Container;
                                if (container.Properties != null)
                                {
                                    _editor.History.Snapshot(_editor.Project);
                                    container.Properties.Remove(property);
                                }
                            }
                        }
                    },
                    (parameter) => IsEditMode());

                _commands.AddGroupLibraryCommand = new DelegateCommand(
                    () =>
                    {
                        var gl = GroupLibrary.Create("New");
                        _editor.History.Snapshot(_editor.Project);
                        _editor.Project.GroupLibraries.Add(gl);
                    },
                    () => IsEditMode());

                _commands.RemoveGroupLibraryCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.RemoveCurrentGroupLibrary();
                    },
                    () => IsEditMode());

                _commands.AddGroupCommand = new DelegateCommand(
                    () =>
                    {
                        var group = _editor.Renderer.SelectedShape;
                        if (group != null && group is XGroup)
                        {
                            if (_editor.Project.CurrentGroupLibrary != null)
                            {
                                var clone = Clone(group as XGroup);
                                if (clone != null)
                                {
                                    _editor.History.Snapshot(_editor.Project);
                                    _editor.Project.CurrentGroupLibrary.Groups.Add(clone);
                                }
                            }
                        }
                    },
                    () => IsEditMode());

                _commands.RemoveGroupCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.RemoveCurrentGroup();
                    },
                    () => IsEditMode());

                _commands.AddLayerCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.History.Snapshot(_editor.Project);
                        _editor.Project.CurrentContainer.Layers.Add(Layer.Create("New", _editor.Project.CurrentContainer));
                    },
                    () => IsEditMode());

                _commands.RemoveLayerCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.RemoveCurrentLayer();
                    },
                    () => IsEditMode());

                _commands.AddStyleGroupCommand = new DelegateCommand(
                    () =>
                    {
                        var sg = ShapeStyleGroup.Create("New");
                        _editor.History.Snapshot(_editor.Project);
                        _editor.Project.StyleGroups.Add(sg);
                    },
                    () => IsEditMode());

                _commands.RemoveStyleGroupCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.RemoveCurrentStyleGroup();
                    },
                    () => IsEditMode());

                _commands.AddStyleCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.History.Snapshot(_editor.Project);
                        _editor.Project.CurrentStyleGroup.Styles.Add(ShapeStyle.Create("New"));
                    },
                    () => IsEditMode());

                _commands.RemoveStyleCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.RemoveCurrentStyle();
                    },
                    () => IsEditMode());

                _commands.RemoveShapeCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.RemoveCurrentShape();
                    },
                    () => IsEditMode());

                _commands.StartSimulationCommand = new DelegateCommand(
                    () =>
                    {
                        StartSimulation();
                    },
                    () => IsEditMode());

                _commands.StopSimulationCommand = new DelegateCommand(
                    () =>
                    {
                        StopSimulation();
                    },
                    () => IsSimulationMode());

                _commands.RestartSimulationCommand = new DelegateCommand(
                    () =>
                    {
                        RestartSimulation();
                    },
                    () => IsSimulationMode());

                _commands.PauseSimulationCommand = new DelegateCommand(
                    () =>
                    {
                        PauseSimulation();
                    },
                    () => IsSimulationMode());

                _commands.TickSimulationCommand = new DelegateCommand(
                    () =>
                    {
                        TickSimulation();
                    },
                    () => IsSimulationMode() && IsSimulationPaused);

                _commands.AddTemplateCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.History.Snapshot(_editor.Project);
                        _editor.Project.Templates.Add(EmptyTemplate(_editor.Project));
                    },
                    () => IsEditMode());

                _commands.RemoveTemplateCommand = new DelegateCommand(
                    () =>
                    {
                        _editor.RemoveCurrentTemplate();
                    },
                    () => IsEditMode());

                _commands.EditTemplateCommand = new DelegateCommand(
                    () =>
                    {
                        var template = _editor.Project.CurrentTemplate;
                        if (template != null)
                        {
                            _editor.Project.CurrentContainer = template;
                            _editor.Project.CurrentContainer.Invalidate();
                        }
                    },
                    () => IsEditMode());

                _commands.ApplyTemplateCommand = new DelegateCommand<object>(
                    (item) =>
                    {
                        if (item is Container)
                        {
                            var template = item as Container;
                            _editor.History.Snapshot(_editor.Project);
                            _editor.Project.CurrentContainer.Template = template;
                        }
                    },
                    (item) => IsEditMode());

                _commands.SelectedItemChangedCommand = new DelegateCommand<object>(
                    (item) =>
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
                    },
                    (item) => IsEditMode());

                _commands.AddContainerCommand = new DelegateCommand<object>(
                    (item) =>
                    {
                        var container = DefaultContainer(_editor.Project);
                        _editor.History.Snapshot(_editor.Project);
                        _editor.Project.CurrentDocument.Containers.Add(container);
                        _editor.Project.CurrentContainer = container;
                    },
                    (item) => IsEditMode());

                _commands.InsertContainerBeforeCommand = new DelegateCommand<object>(
                    (item) =>
                    {
                        if (item is Container)
                        {
                            var selected = item as Container;
                            int index = _editor.Project.CurrentDocument.Containers.IndexOf(selected);
                            var container = DefaultContainer(_editor.Project);
                            _editor.History.Snapshot(_editor.Project);
                            _editor.Project.CurrentDocument.Containers.Insert(index, container);
                            _editor.Project.CurrentContainer = container;
                        }
                    },
                    (item) => IsEditMode());

                _commands.InsertContainerAfterCommand = new DelegateCommand<object>(
                    (item) =>
                    {
                        if (item is Container)
                        {
                            var selected = item as Container;
                            int index = _editor.Project.CurrentDocument.Containers.IndexOf(selected);
                            var container = DefaultContainer(_editor.Project);
                            _editor.History.Snapshot(_editor.Project);
                            _editor.Project.CurrentDocument.Containers.Insert(index + 1, container);
                            _editor.Project.CurrentContainer = container;
                        }
                    },
                    (item) => IsEditMode());

                _commands.AddDocumentCommand = new DelegateCommand<object>(
                    (item) =>
                    {
                        var document = DefaultDocument(_editor.Project);
                        _editor.History.Snapshot(_editor.Project);
                        _editor.Project.Documents.Add(document);
                        _editor.Project.CurrentDocument = document;
                        _editor.Project.CurrentContainer = document.Containers.FirstOrDefault();
                    },
                    (item) => IsEditMode());

                _commands.InsertDocumentBeforeCommand = new DelegateCommand<object>(
                    (item) =>
                    {
                        if (item is Document)
                        {
                            var selected = item as Document;
                            int index = _editor.Project.Documents.IndexOf(selected);
                            var document = DefaultDocument(_editor.Project);
                            _editor.History.Snapshot(_editor.Project);
                            _editor.Project.Documents.Insert(index, document);
                            _editor.Project.CurrentDocument = document;
                            _editor.Project.CurrentContainer = document.Containers.FirstOrDefault();
                        }

                    },
                    (item) => IsEditMode());

                _commands.InsertDocumentAfterCommand = new DelegateCommand<object>(
                    (item) =>
                    {
                        if (item is Document)
                        {
                            var selected = item as Document;
                            int index = _editor.Project.Documents.IndexOf(selected);
                            var document = DefaultDocument(_editor.Project);
                            _editor.History.Snapshot(_editor.Project);
                            _editor.Project.Documents.Insert(index + 1, document);
                            _editor.Project.CurrentDocument = document;
                            _editor.Project.CurrentContainer = document.Containers.FirstOrDefault();
                        }
                    },
                    (item) => IsEditMode());

                WarmUpCSharpScript();
                WarmUpHistory();
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
                ScriptOptions options = ScriptOptions.Default
                    .AddNamespaces("System")
                    .AddNamespaces("System.Collections.Generic")
                    .AddNamespaces("System.Text")
                    .AddNamespaces("System.Threading")
                    .AddNamespaces("System.Threading.Tasks")
                    .AddReferences(Assembly.GetAssembly(typeof(ObservableCollection<>)))
                    .AddNamespaces("System.Collections.ObjectModel")
                    .AddReferences(Assembly.GetAssembly(typeof(System.Linq.Enumerable)))
                    .AddNamespaces("System.Linq")
                    .AddReferences(Assembly.GetAssembly(typeof(ObservableObject)))
                    .AddNamespaces("Test2d")
                    .AddReferences(Assembly.GetAssembly(typeof(EditorContext)))
                    .AddNamespaces("TestEDITOR")
                    .AddNamespaces("TestSIM")
                    .AddNamespaces("Dxf");

                CSharpScript.Eval(
                    code,
                    options,
                    new ScriptGlobals()
                    {
                        Context = context,
                        Execute = execute
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
        /// <param name="project"></param>
        public void Validate(Project project)
        {
            try
            {
                if (project == null)
                    return;

                if (project.Options == null)
                {
                    project.Options = Options.Create();
                }

                if (project.StyleGroups == null)
                {
                    project.StyleGroups = new ObservableCollection<ShapeStyleGroup>();
                }

                if (project.GroupLibraries == null)
                {
                    project.GroupLibraries = new ObservableCollection<GroupLibrary>();
                }

                if (project.Templates == null)
                {
                    project.Templates = new ObservableCollection<Container>();
                }

                if (project.Documents == null)
                {
                    project.Documents = new ObservableCollection<Document>();
                }

                foreach (var document in project.Documents)
                {
                    if (document.Containers == null)
                    {
                        document.Containers = new ObservableCollection<Container>();
                    }

                    foreach (var container in document.Containers)
                    {
                        if (container.Layers == null)
                        {
                            container.Layers = new ObservableCollection<Layer>();
                        }

                        if (container.Properties == null)
                        {
                            container.Properties = new ObservableCollection<ShapeProperty>();
                        }

                        if (container.WorkingLayer == null)
                        {
                            container.WorkingLayer = Layer.Create("Working", container);
                        }

                        if (container.HelperLayer == null)
                        {
                            container.HelperLayer = Layer.Create("Helper", container);
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

                Validate(project);

                var root = new Uri(path);
                var images = Editor.GetAllShapes<XImage>(project);
                _editor.ToAbsoluteUri(root, images);

                _editor.History.Snapshot(_editor.Project);
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
            try
            {
                switch (type)
                {
                    case ImportType.Style:
                        {
                            var styles = item as IList<ShapeStyle>;
                            var json = ReadUtf8Text(path, false);
                            var import = Serializer.FromJson<ShapeStyle>(json);
                            _editor.History.Snapshot(_editor.Project);
                            styles.Add(import);
                        }
                        break;
                    case ImportType.Styles:
                        {
                            var styles = item as IList<ShapeStyle>;
                            var json = ReadUtf8Text(path, false);
                            var import = Serializer.FromJson<IList<ShapeStyle>>(json);
                            _editor.History.Snapshot(_editor.Project);
                            foreach (var style in import)
                            {
                                styles.Add(style);
                            }
                        }
                        break;
                    case ImportType.StyleGroup:
                        {
                            var sgs = item as IList<ShapeStyleGroup>;
                            var json = ReadUtf8Text(path, false);
                            var import = Serializer.FromJson<ShapeStyleGroup>(json);
                            _editor.History.Snapshot(_editor.Project);
                            sgs.Add(import);
                        }
                        break;
                    case ImportType.StyleGroups:
                        {
                            var sgs = item as IList<ShapeStyleGroup>;
                            var json = ReadUtf8Text(path, false);
                            var import = Serializer.FromJson<IList<ShapeStyleGroup>>(json);
                            _editor.History.Snapshot(_editor.Project);
                            foreach (var sg in import)
                            {
                                sgs.Add(sg);
                            }
                        }
                        break;
                    case ImportType.Group:
                        {
                            var groups = item as IList<XGroup>;
                            var json = ReadUtf8Text(path, false);
                            var import = Serializer.FromJson<XGroup>(json);

                            var shapes = Enumerable.Repeat(import as XGroup, 1);
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);
                            var root = new Uri(path);
                            var images = Editor.GetAllShapes<XImage>(shapes);
                            _editor.ToAbsoluteUri(root, images);

                            _editor.History.Snapshot(_editor.Project);
                            groups.Add(import);
                        }
                        break;
                    case ImportType.Groups:
                        {
                            var groups = item as IList<XGroup>;
                            var json = ReadUtf8Text(path, false);
                            var import = Serializer.FromJson<IList<XGroup>>(json);

                            var shapes = import;
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);
                            var root = new Uri(path);
                            var images = Editor.GetAllShapes<XImage>(shapes);
                            _editor.ToAbsoluteUri(root, images);

                            _editor.History.Snapshot(_editor.Project);
                            foreach (var group in import)
                            {
                                groups.Add(group);
                            }
                        }
                        break;
                    case ImportType.GroupLibrary:
                        {
                            var gls = item as IList<GroupLibrary>;
                            var json = ReadUtf8Text(path, false);
                            var import = Serializer.FromJson<GroupLibrary>(json);

                            var shapes = import.Groups;
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);
                            var root = new Uri(path);
                            var images = Editor.GetAllShapes<XImage>(shapes);
                            _editor.ToAbsoluteUri(root, images);

                            _editor.History.Snapshot(_editor.Project);
                            gls.Add(import);
                        }
                        break;
                    case ImportType.GroupLibraries:
                        {
                            var gls = item as IList<GroupLibrary>;
                            var json = ReadUtf8Text(path, false);
                            var import = Serializer.FromJson<IList<GroupLibrary>>(json);

                            var shapes = import.SelectMany(x => x.Groups);
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);
                            var root = new Uri(path);
                            var images = Editor.GetAllShapes<XImage>(shapes);
                            _editor.ToAbsoluteUri(root, images);

                            _editor.History.Snapshot(_editor.Project);
                            foreach (var library in import)
                            {
                                gls.Add(library);
                            }
                        }
                        break;
                    case ImportType.Template:
                        {
                            var templates = item as IList<Container>;
                            var json = ReadUtf8Text(path, false);
                            var import = Serializer.FromJson<Container>(json);

                            var shapes = import.Layers.SelectMany(x => x.Shapes);
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);
                            var root = new Uri(path);
                            var images = Editor.GetAllShapes<XImage>(shapes);
                            _editor.ToAbsoluteUri(root, images);

                            _editor.History.Snapshot(_editor.Project);
                            templates.Add(import);
                        }
                        break;
                    case ImportType.Templates:
                        {
                            var templates = item as IList<Container>;
                            var json = ReadUtf8Text(path, false);
                            var import = Serializer.FromJson<IList<Container>>(json);

                            var shapes = import.SelectMany(x => x.Layers).SelectMany(x => x.Shapes);
                            TryToRestoreStyles(shapes);
                            TryToRestoreRecords(shapes);
                            var root = new Uri(path);
                            var images = Editor.GetAllShapes<XImage>(shapes);
                            _editor.ToAbsoluteUri(root, images);

                            _editor.History.Snapshot(_editor.Project);
                            foreach (var template in import)
                            {
                                templates.Add(template);
                            }
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
        public void ImportData(string path)
        {
            try
            {
                var reader = new CsvReader();
                var fields = reader.Read(path);
                var name = System.IO.Path.GetFileNameWithoutExtension(path);

                var db = Database.Create(name);

                var tempColumns = fields.FirstOrDefault().Select(c => Column.Create(c));
                IList<Column> columns = new ObservableCollection<Column>(tempColumns);

                var tempRecords = fields
                    .Skip(1)
                    .Select(v => 
                            Record.Create(
                                columns,
                                v.Select(c => Value.Create(c))));
                var records = new ObservableCollection<Record>(tempRecords);

                db.Columns = columns;
                db.Records = records;

                _editor.Project.Databases.Add(db);
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
                // TODO:
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
                    .Where(sg => sg.Styles != null && sg.Styles.Count > 0)
                    .SelectMany(sg => sg.Styles)
                    .Distinct(new StyleComparer())
                    .ToDictionary(s => s.Name);

                // reset point shape to container default
                foreach (var point in Editor.GetAllPoints(shapes))
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
                            _editor.Project.StyleGroups.Add(sg);
                            _editor.Project.CurrentStyleGroup = sg;
                        }

                        // add missing style
                        _editor.Project.CurrentStyleGroup.Styles.Add(shape.Style);

                        // recreate styles dictionary
                        styles = _editor.Project.StyleGroups
                            .Where(sg => sg.Styles != null && sg.Styles.Count > 0)
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
                    .Where(d => d.Records != null && d.Records.Count > 0)
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
                            var db = Database.Create(
                                "Imported",
                                shape.Record.Columns,
                                new ObservableCollection<Record>());
                            _editor.Project.Databases.Add(db);
                            _editor.Project.CurrentDatabase = db;
                        }

                        // add missing data record
                        _editor.Project.CurrentDatabase.Records.Add(shape.Record);

                        // recreate records dictionary
                        records = _editor.Project.Databases
                            .Where(d => d.Records != null && d.Records.Count > 0)
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

                _editor.History.Snapshot(_editor.Project);

                TryToRestoreStyles(shapes);
                TryToRestoreRecords(shapes);

                foreach (var shape in shapes)
                {
                    _editor.Project.CurrentContainer.CurrentLayer.Shapes.Add(shape);
                }

                _editor.Select(_editor.Project.CurrentContainer, new HashSet<BaseShape>(shapes));
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
                        for (int i = 0; i < clone.Containers.Count; i++)
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
                            ImportEx(path, _editor.Project.CurrentStyleGroup.Styles, ImportType.Style);
                            result = true;
                        }
                        else if (string.Compare(ext, ".styles", true, CultureInfo.InvariantCulture) == 0)
                        {
                            ImportEx(path, _editor.Project.CurrentStyleGroup.Styles, ImportType.Styles);
                            result = true;
                        }
                        else if (string.Compare(ext, ".stylegroup", true, CultureInfo.InvariantCulture) == 0)
                        {
                            ImportEx(path, _editor.Project.StyleGroups, ImportType.StyleGroup);
                            result = true;
                        }
                        else if (string.Compare(ext, ".stylegroups", true, CultureInfo.InvariantCulture) == 0)
                        {
                            ImportEx(path, _editor.Project.StyleGroups, ImportType.StyleGroups);
                            result = true;
                        }
                        else if (string.Compare(ext, ".group", true, CultureInfo.InvariantCulture) == 0)
                        {
                            ImportEx(path, _editor.Project.CurrentGroupLibrary.Groups, ImportType.Group);
                            result = true;
                        }
                        else if (string.Compare(ext, ".groups", true, CultureInfo.InvariantCulture) == 0)
                        {
                            ImportEx(path, _editor.Project.CurrentGroupLibrary.Groups, ImportType.Groups);
                            result = true;
                        }
                        else if (string.Compare(ext, ".grouplibrary", true, CultureInfo.InvariantCulture) == 0)
                        {
                            ImportEx(path, _editor.Project.GroupLibraries, ImportType.GroupLibrary);
                            result = true;
                        }
                        else if (string.Compare(ext, ".grouplibraries", true, CultureInfo.InvariantCulture) == 0)
                        {
                            ImportEx(path, _editor.Project.GroupLibraries, ImportType.GroupLibraries);
                            result = true;
                        }
                        else if (string.Compare(ext, ".template", true, CultureInfo.InvariantCulture) == 0)
                        {
                            ImportEx(path, _editor.Project.Templates, ImportType.Template);
                            result = true;
                        }
                        else if (string.Compare(ext, ".templates", true, CultureInfo.InvariantCulture) == 0)
                        {
                            ImportEx(path, _editor.Project.Templates, ImportType.Templates);
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
                    _editor.History.Snapshot(_editor.Project);
                    _editor.Project.CurrentContainer.CurrentLayer.Shapes.Add(clone);
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
                    _editor.History.Snapshot(_editor.Project);
                    _editor.Renderer.SelectedShape.Record = record;
                }
                else if (_editor.Renderer.SelectedShapes != null && _editor.Renderer.SelectedShapes.Count > 0)
                {
                    _editor.History.Snapshot(_editor.Project);
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
                            _editor.History.Snapshot(_editor.Project);
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

            var count = record.Values.Count;
            double px = sx;
            double py = sy;
            double width = 150;
            double height = 15;

            for (int i = 0; i < count; i++)
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
                    text.Bindings.Add(binding);
                    g.AddShape(text);

                    py += height;
                }
            }

            var rectangle = XRectangle.Create(
                sx, sy,
                sx + width, sy + (double)count * height,
                _editor.Project.CurrentStyleGroup.CurrentStyle,
                _editor.Project.Options.PointShape);
            g.AddShape(rectangle);

            double ptx = sx + width / 2;
            double pty = sy;

            double pbx = sx + width / 2;
            double pby = sy + (double)count * height;

            double plx = sx;
            double ply = sy + ((double)count * height) / 2;

            double prx = sx + width;
            double pry = sy + ((double)count * height) / 2;

            var pt = XPoint.Create(ptx, pty, _editor.Project.Options.PointShape);
            var pb = XPoint.Create(pbx, pby, _editor.Project.Options.PointShape);
            var pl = XPoint.Create(plx, ply, _editor.Project.Options.PointShape);
            var pr = XPoint.Create(prx, pry, _editor.Project.Options.PointShape);

            g.AddConnectorAsNone(pt);
            g.AddConnectorAsNone(pb);
            g.AddConnectorAsNone(pl);
            g.AddConnectorAsNone(pr);

            _editor.History.Snapshot(_editor.Project);
            _editor.Project.CurrentContainer.CurrentLayer.Shapes.Add(g);
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
                    _editor.History.Snapshot(_editor.Project);
                    _editor.Renderer.SelectedShape.Style = style;
                }
                else if (_editor.Renderer.SelectedShapes != null && _editor.Renderer.SelectedShapes.Count > 0)
                {
                    _editor.History.Snapshot(_editor.Project);
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
                            _editor.History.Snapshot(_editor.Project);
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
                    var project = _editor.History.Undo(_editor.Project);
                    _editor.Load(project);
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
                    var project = _editor.History.Redo(_editor.Project);
                    _editor.Load(project);
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
                    new HashSet<BaseShape>(_editor.Project.CurrentContainer.CurrentLayer.Shapes));
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
                _editor.History.Snapshot(_editor.Project);
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
        private void WarmUpHistory()
        {
            // NOTE: Warmup history serializer and compressor.
            try
            {
                Task.Run(
                    () =>
                    {
                        var history = new History<Project>(_serializer, _compressor);
                        var project = DefaultProject();
                        history.Snapshot(project);
                        history.Undo(project);
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
            (_commands.ToolPointCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ToolLineCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ToolRectangleCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ToolEllipseCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ToolArcCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ToolBezierCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ToolQBezierCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ToolTextCommand as DelegateCommand).RaiseCanExecuteChanged();

            (_commands.EvalCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.EvalScriptCommand as DelegateCommand<string>).RaiseCanExecuteChanged();

            (_commands.DefaultIsFilledCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.SnapToGridCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.TryToConnectCommand as DelegateCommand).RaiseCanExecuteChanged();

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
