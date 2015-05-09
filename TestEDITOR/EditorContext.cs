// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.CodeAnalysis.Scripting.CSharp;
using Microsoft.Practices.Prism.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Test2d;
using TestDXF;
using TestEMF;
using TestJSON;
using TestPDF;
using TestSIM;

namespace TestEDITOR
{
    public class EditorContext : ObservableObject, IDisposable
    {
        private EditorCommands _commands;
        private Editor _editor;
        private ITextClipboard _textClipboard;
        private string _rootScriptsPath;
        private IList<ScriptDirectory> _scriptDirectories;
        private bool _isSimulationPaused;
        private System.IO.FileSystemWatcher _watcher = null;
        private System.Threading.Timer _timer = null;
        private BoolSimulationFactory _simulationFactory = null;
        private IDictionary<XGroup, BoolSimulation> _simulations;
        private Clock _clock = null;

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

        public Action<Action> Execute { get; set; }

        public Container EmptyTemplate(Project project)
        {
            var container = Container.Create("Empty");

            return container;
        }
        
        public Container GridTemplate(Project project)
        {
            var container = Container.Create("Grid");

            var gs = project
                .StyleGroups.FirstOrDefault(g => g.Name == "Template")
                .Styles.FirstOrDefault(s => s.Name == "Grid");
            var settings = LineGrid.Settings.Create(0, 0, container.Width, container.Height, 30, 30);
            var shapes = LineGrid.Create(gs, settings, project.PointShape);
            var layer = container.Layers.FirstOrDefault();
            
            foreach (var shape in shapes) 
            {
                layer.Shapes.Add(shape);
            }

            return container;
        }
        
        public Container DefaultContainer(Project project)
        {
            var container = Container.Create();
            container.Template = project.CurrentTemplate;
            return container;
        }
        
        public Document DefaultDocument(Project project)
        {
             var document = Document.Create();
      
             return document;
        }
        
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

        public void Initialize(IView view, IRenderer renderer, ITextClipboard clipboard)
        {
            _commands = new EditorCommands();

            _editor = Editor.Create(DefaultProject(), renderer);
            _textClipboard = clipboard;

            (_editor.Renderer as ObservableObject).PropertyChanged +=
                (s, e) =>
                {
                    if (e.PropertyName == "Zoom")
                    {
                        Invalidate();
                    }
                };

            _commands.NewCommand = new DelegateCommand(
                () =>
                {
                    _editor.Load(DefaultProject());
                },
                () => IsEditMode());

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
                () => IsEditMode() && CanUndo());

            _commands.RedoCommand = new DelegateCommand(
                () =>
                {
                    Redo();
                },
                () => IsEditMode() && CanRedo());

            _commands.CutCommand = new DelegateCommand(
                () =>
                {
                    Cut();
                },
                () => IsEditMode() /* && CanCopy() */);

            _commands.CopyCommand = new DelegateCommand(
                () =>
                {
                    Copy();
                },
                () => IsEditMode() /* && CanCopy() */);

            _commands.PasteCommand = new DelegateCommand(
                () =>
                {
                    Paste();
                },
                () => IsEditMode() /* && CanPaste() */);

            _commands.DeleteCommand = new DelegateCommand(
                () =>
                {
                    _editor.DeleteSelected();
                },
                () => IsEditMode() /* && _editor.IsSelectionAvailable() */);

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
                    _editor.DefaultIsFilled = !_editor.DefaultIsFilled;
                },
                () => IsEditMode());

            _commands.SnapToGridCommand = new DelegateCommand(
                () =>
                {
                    _editor.SnapToGrid = !_editor.SnapToGrid;
                },
                () => IsEditMode());

            _commands.TryToConnectCommand = new DelegateCommand(
                () =>
                {
                    _editor.TryToConnect = !_editor.TryToConnect;
                },
                () => IsEditMode());

            
            _commands.AddPropertyCommand = new DelegateCommand(
                () =>
                {
                    if (_editor.Renderer.SelectedShape != null)
                    {
                        _editor.Renderer.SelectedShape.Properties.Add(ShapeProperty.Create("New", ""));
                    }
                },
                () => IsEditMode());
            
            _commands.RemovePropertyCommand = new DelegateCommand<object>(
                (property) =>
                {
                    if (property is ShapeProperty)
                    {
                        _editor.Renderer.SelectedShape.Properties.Remove(property as ShapeProperty);
                    }
                },
                (property) => IsEditMode());
            
            _commands.AddGroupLibraryCommand = new DelegateCommand(
                () =>
                {
                    var gl = GroupLibrary.Create("New");
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
                    _editor.Container.Layers.Add(Layer.Create("New"));
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
                    sg.Styles.Add(ShapeStyle.Create("New"));
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
                    _editor.Project.Templates.Add(EmptyTemplate(_editor.Project));
                },
                () => IsEditMode());

            _commands.RemoveTemplateCommand = new DelegateCommand(
                () =>
                {
                    if (_editor.Project.CurrentTemplate != null)
                    {
                        _editor.Project.Templates.Remove(_editor.Project.CurrentTemplate);
                    }
                    
                    var template = _editor.Project.CurrentTemplate;
                    if (template != null)
                    {
                        _editor.Project.Templates.Remove(template);
                        _editor.Project.CurrentTemplate = _editor.Project.Templates.FirstOrDefault();
                    }
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
                        
                        _editor.Project.CurrentContainer.Template = template;
                        System.Diagnostics.Debug.Print("Template: " + template.Name);
                    }
                },
                (item) => IsEditMode());
            
            _commands.SelectedItemChangedCommand = new DelegateCommand<object>(
                (item) =>
                {
                    if (item is Container)
                    {
                        var container = item as Container;
                        var document = _editor.Project.Documents.FirstOrDefault(d => d.Containers.Contains(container));
                        if (document != null)
                        {
                            _editor.Project.CurrentDocument = document;
                            _editor.Project.CurrentContainer = container;
                        }
                    }
                    else if (item is Document)
                    {
                        var document = item as Document;
              
                        _editor.Project.CurrentDocument = document;
                        _editor.Project.CurrentContainer = document.Containers.FirstOrDefault();
                    }
                },
                (item) => IsEditMode());

            _commands.AddContainerCommand = new DelegateCommand<object>(
                (item) =>
                {
                    var container = DefaultContainer(_editor.Project);
                    _editor.Project.CurrentDocument.Containers.Add(container);
                    _editor.Project.CurrentContainer = container;
                },
                (item) => IsEditMode());

            _commands.InsertContainerBeforeCommand = new DelegateCommand<object>(
                (item) =>
                {
                    // TODO:
                },
                (item) => IsEditMode());

            _commands.InsertContainerAfterCommand = new DelegateCommand<object>(
                (item) =>
                {
                    // TODO:
                },
                (item) => IsEditMode());

            _commands.CutContainerCommand = new DelegateCommand<object>(
                (item) =>
                {
                    // TODO:
                },
                (item) => IsEditMode());

            _commands.CopyContainerCommand = new DelegateCommand<object>(
                (item) =>
                {
                    // TODO:
                },
                (item) => IsEditMode());

            _commands.PasteContainerCommand = new DelegateCommand<object>(
                (item) =>
                {
                    // TODO:
                },
                (item) => IsEditMode());

            _commands.DeleteContainerCommand = new DelegateCommand<object>(
                (item) =>
                {
                    // TODO:
                },
                (item) => IsEditMode());

            _commands.AddDocumentCommand = new DelegateCommand<object>(
                (item) =>
                {
                    var document = DefaultDocument(_editor.Project);
                    _editor.Project.Documents.Add(document);
                    _editor.Project.CurrentDocument = document;
                    _editor.Project.CurrentContainer = document.Containers.FirstOrDefault();
                },
                (item) => IsEditMode());

            _commands.InsertDocumentBeforeCommand = new DelegateCommand<object>(
                (item) =>
                {
                    // TODO:
                },
                (item) => IsEditMode());

            _commands.InsertDocumentAfterCommand = new DelegateCommand<object>(
                (item) =>
                {
                    // TODO:
                },
                (item) => IsEditMode());

            _commands.CutDocumentCommand = new DelegateCommand<object>(
                (item) =>
                {
                    // TODO:
                },
                (item) => IsEditMode());

            _commands.CopyDocumentCommand = new DelegateCommand<object>(
                (item) =>
                {
                    // TODO:
                },
                (item) => IsEditMode());

            _commands.PasteDocumentCommand = new DelegateCommand<object>(
                (item) =>
                {
                    // TODO:
                },
                (item) => IsEditMode());

            _commands.DeleteDocumentCommand = new DelegateCommand<object>(
                (item) =>
                {
                    // TODO:
                },
                (item) => IsEditMode());

            WarmUpCSharpScript();
        }

        public void Invalidate()
        {
            _editor.Renderer.ClearCache();
            _editor.Container.Invalidate();
        }

        public void Eval(string code, EditorContext context, Action<Action> execute)
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
                .AddReferences(Assembly.GetAssembly(typeof(Dxf.DxfObject)))
                .AddNamespaces("TestDXF")
                .AddReferences(Assembly.GetAssembly(typeof(Emf)))
                .AddNamespaces("TestEMF")
                .AddReferences(Assembly.GetAssembly(typeof(ContainerSerializer)))
                .AddNamespaces("TestJSON")
                .AddReferences(Assembly.GetAssembly(typeof(EditorContext)))
                .AddNamespaces("TestEDITOR")
                .AddNamespaces("TestPDF")
                .AddNamespaces("TestSIM");

            CSharpScript.Eval(
                code, 
                options, 
                new ScriptGlobals() 
                { 
                    Context = context,
                    Execute = execute
                });
        }

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

        public void Open(string path)
        {
            var json = System.IO.File.ReadAllText(path, Encoding.UTF8);
            var project = ContainerSerializer.Deserialize<Project>(json);

            var root = new Uri(path);
            var images = Editor.GetAllShapes<XImage>(project);

            _editor.ToAbsoluteUri(root, images);

            _editor.Load(project);
        }

        public void Save(string path)
        {
            var root = new Uri(path);
            var images = Editor.GetAllShapes<XImage>(_editor.Project);

            _editor.ToRelativeUri(root, images);

            var json = ContainerSerializer.Serialize(_editor.Project);
            System.IO.File.WriteAllText(path, json, Encoding.UTF8);

            _editor.ToAbsoluteUri(root, images);
        }

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
  
        public void ExportAsEmf(string path)
        {
            try
            {
                Emf.Save(path, _editor.Container);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }
        
        public void ExportAsDxf(string path, Dxf.DxfAcadVer version)
        {
            try
            { 
                var renderer = new DxfRenderer()
                {
                    DrawShapeState = ShapeState.Printable
                };
                renderer.Create(path, _editor.Container, version);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        public bool CanCopy()
        {
            return _editor.IsSelectionAvailable();
        }

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

        private void Copy(IList<BaseShape> shapes)
        {
            try
            {
                var json = ContainerSerializer.Serialize(shapes);
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

        public void Paste(string json)
        {
            try
            {
                var shapes = ContainerSerializer.Deserialize<IList<BaseShape>>(json);
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

        private void TryToRestoreStyles(IEnumerable<BaseShape> shapes)
        {
            var styles = _editor.Project.StyleGroups
                .SelectMany(sg => sg.Styles)
                .ToDictionary(s => s.Name);

            // reset point shape to container default
            foreach (var point in Editor.GetAllPoints(shapes))
            {
                point.Shape = _editor.Project.PointShape;
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
                    // add missing style
                    _editor.Project.CurrentStyleGroup.Styles.Add(shape.Style);
                    styles = _editor.Project.StyleGroups
                        .SelectMany(sg => sg.Styles)
                        .ToDictionary(s => s.Name);
                }
            }
        }

        public void Paste(IEnumerable<BaseShape> shapes)
        {
            _editor.Deselect(_editor.Container);

            TryToRestoreStyles(shapes);

            foreach (var shape in shapes)
            {
                _editor.Container.CurrentLayer.Shapes.Add(shape);
            }

            _editor.Select(_editor.Container, new HashSet<BaseShape>(shapes));
        }

        public XGroup Clone(XGroup group)
        {
            try
            {
                var json = ContainerSerializer.Serialize(group);
                if (!string.IsNullOrEmpty(json))
                {
                    var clone = ContainerSerializer.Deserialize<XGroup>(json);
                    if (clone != null)
                    {
                        TryToRestoreStyles(Enumerable.Repeat(clone, 1).ToList());
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

        public void Drop(XGroup group, double x, double y)
        {
            try
            {
                double sx = _editor.SnapToGrid ? Editor.Snap(x, _editor.SnapX) : x;
                double sy = _editor.SnapToGrid ? Editor.Snap(y, _editor.SnapY) : y;

                var clone = Clone(group);
                if (clone != null)
                {
                    _editor.Deselect(_editor.Container);
                    clone.Move(sx, sy);
                    _editor.Container.CurrentLayer.Shapes.Add(clone);
                    _editor.Select(_editor.Container, clone);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        public bool CanUndo()
        {
            return false;
        }

        public bool CanRedo()
        {
            return false;
        }

        public void Undo()
        {
            // TODO:
        }

        public void Redo()
        {
            // TODO:
        }

        public void Cut()
        {
            if (CanCopy())
            {
                Copy();

                _editor.DeleteSelected();
            }
        }

        public void Copy()
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

        public void SelectAll()
        {
            _editor.Deselect(_editor.Container);
            _editor.Select(
                _editor.Container,
                new HashSet<BaseShape>(_editor.Container.CurrentLayer.Shapes));
        }

        public void ClearAll()
        {
            _editor.Container.Clear();
            _editor.Container.Invalidate();
        }

        private void WarmUpCSharpScript()
        {
            // NOTE: Warmup Roslyn script engine.
            try
            {
                Task.Run(() => Eval("Action a = () => { };", this, Execute));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        public void InitializeSctipts()
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

        public void InitializeSimulation()
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

        public bool IsEditMode()
        {
            return _timer == null;
        }

        public bool IsSimulationMode()
        {
            return _timer != null;
        }

        public void StartSimulation()
        {
            try
            {
                if (IsSimulationMode())
                {
                    return;
                }

                var graph = ContainerGraph.Create(Editor.Container);
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

        public void StopSimulation()
        {
            try
            {
                // TODO: Reset Working layer simulation state.

                if (IsSimulationMode())
                {
                    _timer.Dispose();
                    _timer = null;
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

        public void RestartSimulation()
        {
            StopSimulation();
            StartSimulation();
        }

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

        private void UpdateCanExecuteState()
        {
            (_commands.NewCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.OpenCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.SaveAsCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.ExportCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.ExitCommand as DelegateCommand).RaiseCanExecuteChanged();

            (_commands.CopyAsEmfCommand as DelegateCommand).RaiseCanExecuteChanged();
            (_commands.DeleteCommand as DelegateCommand).RaiseCanExecuteChanged();
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

            (_commands.AddPropertyCommand as DelegateCommand).RaiseCanExecuteChanged();
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
            (_commands.CutContainerCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.CopyContainerCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.PasteContainerCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.DeleteContainerCommand as DelegateCommand<object>).RaiseCanExecuteChanged();

            (_commands.AddDocumentCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.InsertDocumentBeforeCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.InsertDocumentAfterCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.CutDocumentCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.CopyDocumentCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.PasteDocumentCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
            (_commands.DeleteDocumentCommand as DelegateCommand<object>).RaiseCanExecuteChanged();
        }

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
