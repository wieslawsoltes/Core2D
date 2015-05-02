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

        public void Initialize(IView view, IRenderer renderer)
        {
            _commands = new EditorCommands();
            _editor = Editor.Create(Container.Create(), renderer);

            (_editor.Renderer as ObservableObject).PropertyChanged +=
                (s, e) =>
                {
                    if (e.PropertyName == "Zoom")
                    {
                        _editor.Renderer.ClearCache();
                        _editor.Container.Invalidate();
                    }
                };

            _commands.NewCommand = new DelegateCommand(
                () =>
                {
                    _editor.Load(Container.Create());
                });

            _commands.ExitCommand = new DelegateCommand(
                () =>
                {
                    view.Close();
                });

            _commands.CopyAsEmfCommand = new DelegateCommand(
                () =>
                {
                    Emf.PutOnClipboard(_editor.Container);
                });

            _commands.DeleteSelectedCommand = new DelegateCommand(
                () =>
                {
                    _editor.DeleteSelected();
                });

            _commands.ClearAllCommand = new DelegateCommand(
                () =>
                {
                    ClearAll();
                });

            _commands.GroupSelectedCommand = new DelegateCommand(
                () =>
                {
                    _editor.GroupSelected();
                });

            _commands.GroupCurrentLayerCommand = new DelegateCommand(
                () =>
                {
                    _editor.GroupCurrentLayer();
                });

            _commands.ToolNoneCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.None;
                });

            _commands.ToolSelectionCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.Selection;
                });

            _commands.ToolPointCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.Point;
                });

            _commands.ToolLineCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.Line;
                });

            _commands.ToolRectangleCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.Rectangle;
                });

            _commands.ToolEllipseCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.Ellipse;
                });

            _commands.ToolArcCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.Arc;
                });

            _commands.ToolBezierCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.Bezier;
                });

            _commands.ToolQBezierCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.QBezier;
                });

            _commands.ToolTextCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.Text;
                });

            _commands.EvalScriptCommand = new DelegateCommand<string>(
                (path) =>
                {
                    Eval(path);
                });

            _commands.DefaultIsFilledCommand = new DelegateCommand(
                () =>
                {
                    _editor.DefaultIsFilled = !_editor.DefaultIsFilled;
                });

            _commands.SnapToGridCommand = new DelegateCommand(
                () =>
                {
                    _editor.SnapToGrid = !_editor.SnapToGrid;
                });

            _commands.TryToConnectCommand = new DelegateCommand(
                () =>
                {
                    _editor.TryToConnect = !_editor.TryToConnect;
                });

            _commands.AddLayerCommand = new DelegateCommand(
                () =>
                {
                    _editor.Container.Layers.Add(Layer.Create("New"));
                });

            _commands.RemoveLayerCommand = new DelegateCommand(
                () =>
                {
                    _editor.RemoveCurrentLayer();
                });

            _commands.AddStyleCommand = new DelegateCommand(
                () =>
                {
                    _editor.Container.CurrentStyleGroup.Styles.Add(ShapeStyle.Create("New"));
                });

            _commands.RemoveStyleCommand = new DelegateCommand(
                () =>
                {
                    _editor.RemoveCurrentStyle();
                });

            _commands.RemoveShapeCommand = new DelegateCommand(
                () =>
                {
                    _editor.RemoveCurrentShape();
                });

            _commands.StartSimulationCommand = new DelegateCommand(
                () =>
                {
                    StartSimulation();
                });

            _commands.StopSimulationCommand = new DelegateCommand(
                () =>
                {
                    StopSimulation(); 
                });

            _commands.RestartSimulationCommand = new DelegateCommand(
                () =>
                {
                    RestartSimulation();
                });

            _commands.PauseSimulationCommand = new DelegateCommand(
                () =>
                {
                    PauseSimulation();
                });

            _commands.TickSimulationCommand = new DelegateCommand(
                () =>
                {
                    TickSimulation(_simulations);
                });

            WarmUpCSharpScript();
        }

        public void Eval(string code, EditorContext context)
        {
            ScriptOptions options = ScriptOptions.Default
                .AddNamespaces("System")
                .AddNamespaces("System.Collections.Generic")
                .AddReferences(Assembly.GetAssembly(typeof(ObservableCollection<>)))
                .AddNamespaces("System.Collections.ObjectModel")
                .AddReferences(Assembly.GetAssembly(typeof(System.Linq.Enumerable)))
                .AddNamespaces("System.Linq")
                .AddReferences(Assembly.GetAssembly(typeof(ObservableObject)))
                .AddNamespaces("Test2d");

            CSharpScript.Eval(code, options, new ScriptGlobals() { Context = context });
        }

        public void Eval(string path)
        {
            try
            {
                var code = System.IO.File.ReadAllText(path);
                var context = this;
                Eval(code, context);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
            }
        }

        public void Open(string path)
        {
            var json = System.IO.File.ReadAllText(path, Encoding.UTF8);
            var container = ContainerSerializer.Deserialize(json);
            _editor.Load(container);
        }

        public void Save(string path)
        {
            var json = ContainerSerializer.Serialize(_editor.Container);
            System.IO.File.WriteAllText(path, json, Encoding.UTF8);
        }

        public void ExportAsPdf(string path)
        {
            var renderer = new PdfRenderer()
            {
                DrawShapeState = ShapeState.Printable
            };
            renderer.Save(path, _editor.Container);
        }
  
        public void ExportAsEmf(string path)
        {
            Emf.Save(path, _editor.Container);
        }
        
        public void ExportAsDxf(string path, Dxf.DxfAcadVer version)
        {
            var renderer = new DxfRenderer()
            {
                DrawShapeState = ShapeState.Printable
            };
            renderer.Create(path, _editor.Container, version);
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
                Task.Run(() => Eval("Action a = () => { };", this));
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
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

        private void StartSimulation(IDictionary<XGroup, BoolSimulation> simulations)
        {
            _clock = new Clock(cycle: 0L, resolution: 100);
            IsSimulationPaused = false;
            _timer = new System.Threading.Timer(
                (state) =>
                {
                    try
                    {
                        if (!IsSimulationPaused)
                        {
                            TickSimulation(simulations);
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
        }

        private void StartSimulation()
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
                    _simulations = _simulationFactory.Create(graph);
                    if (_simulations != null)
                    {
                        // TODO: Use Working layer to show simulation state.
                        StartSimulation(_simulations);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        private void PauseSimulation()
        {
            try
            {
                if (IsSimulationMode())
                {
                    IsSimulationPaused = !IsSimulationPaused;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        private void TickSimulation(IDictionary<XGroup, BoolSimulation> simulations)
        {
            try
            {
                if (IsSimulationMode())
                {
                    _simulationFactory.Run(simulations, _clock);
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

        private void RestartSimulation()
        {
            StopSimulation();
            StartSimulation();
        }

        private void StopSimulation()
        {
            try
            {
                // TODO: Reset Working layer simulation state.

                if (IsSimulationMode())
                {
                    _timer.Dispose();
                    _timer = null;
                    IsSimulationPaused = false;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.Print(ex.Message);
                System.Diagnostics.Debug.Print(ex.StackTrace);
            }
        }

        public void Dispose()
        {
            if (_watcher != null)
            {
                _watcher.Dispose();
            }
        }
    }
}
