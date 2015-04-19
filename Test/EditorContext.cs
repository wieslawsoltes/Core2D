// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Test.Util;
using Test2d;
using TestWPF;

namespace Test
{
    public class EditorContext : ObservableObject
    {
        private EditorCommands _commands;
        private Editor _editor;

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

        public void Initialize(IView view)
        {
            _commands = new EditorCommands();
            _editor = Editor.Create(Container.Create(), WpfRenderer.Create());

            (_editor.Renderer as WpfRenderer).PropertyChanged +=
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

            _commands.OpenCommand = new DelegateCommand(
                () =>
                {
                    Open();
                });

            _commands.SaveAsCommand = new DelegateCommand(
                () =>
                {
                    SaveAs();
                });

            _commands.ExportCommand = new DelegateCommand(
                () =>
                {
                    Export();
                });

            _commands.ExitCommand = new DelegateCommand(
                () =>
                {
                    view.Close();
                });

            _commands.ClearCommand = new DelegateCommand(
                () =>
                {
                    Clear();
                });

            _commands.ToolNoneCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.None;
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

            _commands.DrawPointsCommand = new DelegateCommand(
                () =>
                {
                    _editor.Renderer.DrawPoints = !_editor.Renderer.DrawPoints;
                    _editor.Container.Invalidate();
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
                    _editor.Container.Styles.Add(ShapeStyle.Create("New"));
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

        public void Export(string path)
        {
            var renderer = new PdfRenderer() { DrawPoints = _editor.Renderer.DrawPoints };
            renderer.Save(path, _editor.Container);
            System.Diagnostics.Process.Start(path);
        }

        public void Open()
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "Json Files (*.json)|*.json|All Files (*.*)|*.*",
                FilterIndex = 0,
                FileName = ""
            };

            if (dlg.ShowDialog() == true)
            {
                Open(dlg.FileName);
            }
        }

        public void SaveAs()
        {
            var dlg = new SaveFileDialog()
            {
                Filter = "Json Files (*.json)|*.json|All Files (*.*)|*.*",
                FilterIndex = 0,
                FileName = "container"
            };

            if (dlg.ShowDialog() == true)
            {
                Save(dlg.FileName);
            }
        }

        public void Export()
        {
            var dlg = new SaveFileDialog()
            {
                Filter = "Pdf Files (*.pdf)|*.pdf|All Files (*.*)|*.*",
                FilterIndex = 0,
                FileName = "container"
            };

            if (dlg.ShowDialog() == true)
            {
                Export(dlg.FileName);
            }
        }

        public void Clear()
        {
            _editor.Container.Clear();
            _editor.Container.Invalidate();
        }
    }
}
