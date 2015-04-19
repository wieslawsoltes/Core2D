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

namespace Test.Windows
{
    public class EditorContext : ObservableObject
    {
        public ICommand NewCommand { get; set; }
        public ICommand OpenCommand { get; set; }
        public ICommand SaveAsCommand { get; set; }
        public ICommand ExportCommand { get; set; }
        public ICommand ExitCommand { get; set; }

        public ICommand ClearCommand { get; set; }

        public ICommand ToolNoneCommand { get; set; }
        public ICommand ToolLineCommand { get; set; }
        public ICommand ToolRectangleCommand { get; set; }
        public ICommand ToolEllipseCommand { get; set; }
        public ICommand ToolArcCommand { get; set; }
        public ICommand ToolBezierCommand { get; set; }
        public ICommand ToolQBezierCommand { get; set; }
        public ICommand ToolTextCommand { get; set; }

        public ICommand DefaultIsFilledCommand { get; set; }
        public ICommand SnapToGridCommand { get; set; }
        public ICommand DrawPointsCommand { get; set; }

        public ICommand AddLayerCommand { get; set; }
        public ICommand RemoveLayerCommand { get; set; }

        public ICommand AddStyleCommand { get; set; }
        public ICommand RemoveStyleCommand { get; set; }

        public ICommand RemoveShapeCommand { get; set; }

        public ICommand GroupSelectedCommand { get; set; }
        public ICommand GroupCurrentLayerCommand { get; set; }

        public ICommand LayersWindowCommand { get; set; }
        public ICommand StyleWindowCommand { get; set; }
        public ICommand StylesWindowCommand { get; set; }
        public ICommand ShapesWindowCommand { get; set; }
        public ICommand ContainerWindowCommand { get; set; }

        private Editor _editor;

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

            NewCommand = new DelegateCommand(
                () =>
                {
                    _editor.Load(Container.Create());
                });

            OpenCommand = new DelegateCommand(
                () =>
                {
                    Open();
                });

            SaveAsCommand = new DelegateCommand(
                () =>
                {
                    SaveAs();
                });

            ExportCommand = new DelegateCommand(
                () =>
                {
                    Export();
                });

            ExitCommand = new DelegateCommand(
                () =>
                {
                    view.Close();
                });

            ClearCommand = new DelegateCommand(
                () =>
                {
                    Clear();
                });

            ToolNoneCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.None;
                });

            ToolLineCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.Line;
                });

            ToolRectangleCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.Rectangle;
                });

            ToolEllipseCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.Ellipse;
                });

            ToolArcCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.Arc;
                });

            ToolBezierCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.Bezier;
                });

            ToolQBezierCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.QBezier;
                });

            ToolTextCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.Text;
                });

            DefaultIsFilledCommand = new DelegateCommand(
                () =>
                {
                    _editor.DefaultIsFilled = !_editor.DefaultIsFilled;
                });

            SnapToGridCommand = new DelegateCommand(
                () =>
                {
                    _editor.SnapToGrid = !_editor.SnapToGrid;
                });

            DrawPointsCommand = new DelegateCommand(
                () =>
                {
                    _editor.Renderer.DrawPoints = !_editor.Renderer.DrawPoints;
                    _editor.Container.Invalidate();
                });

            AddLayerCommand = new DelegateCommand(
                () =>
                {
                    _editor.Container.Layers.Add(Layer.Create("New"));
                });

            RemoveLayerCommand = new DelegateCommand(
                () =>
                {
                    _editor.RemoveCurrentLayer();
                });

            AddStyleCommand = new DelegateCommand(
                () =>
                {
                    _editor.Container.Styles.Add(ShapeStyle.Create("New"));
                });

            RemoveStyleCommand = new DelegateCommand(
                () =>
                {
                    _editor.RemoveCurrentStyle();
                });

            RemoveShapeCommand = new DelegateCommand(
                () =>
                {
                    _editor.RemoveCurrentShape();
                });

            GroupSelectedCommand = new DelegateCommand(
                () =>
                {
                    _editor.GroupSelected();
                });

            GroupCurrentLayerCommand = new DelegateCommand(
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
