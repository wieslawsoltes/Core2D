// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Practices.Prism.Commands;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Test2d;
using Test.Util;
using TestWPF;

namespace Test.Windows
{
    public interface IView
    {
        void Close();
    }

    public class MainViewModel : ObservableObject
    {
        private Editor _editor;

        public Editor Editor 
        {
            get { return _editor; }
            set
            {
                if (value != _editor)
                {
                    _editor = value;
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

            _editor.NewCommand = new DelegateCommand(
                () =>
                {
                    _editor.Load(Container.Create());
                });

            _editor.OpenCommand = new DelegateCommand(
                () =>
                {
                    Open();
                });

            _editor.SaveAsCommand = new DelegateCommand(
                () =>
                {
                    SaveAs();
                });

            _editor.ExportCommand = new DelegateCommand(
                () =>
                {
                    Export();
                });

            _editor.ExitCommand = new DelegateCommand(
                () =>
                {
                    view.Close();
                });

            _editor.ClearCommand = new DelegateCommand(
                () =>
                {
                    Clear();
                });

            _editor.ToolNoneCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.None;
                });

            _editor.ToolLineCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.Line;
                });

            _editor.ToolRectangleCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.Rectangle;
                });

            _editor.ToolEllipseCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.Ellipse;
                });

            _editor.ToolArcCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.Arc;
                });

            _editor.ToolBezierCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.Bezier;
                });

            _editor.ToolQBezierCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.QBezier;
                });

            _editor.ToolTextCommand = new DelegateCommand(
                () =>
                {
                    _editor.CurrentTool = Tool.Text;
                });

            _editor.DefaultIsFilledCommand = new DelegateCommand(
                () =>
                {
                    _editor.DefaultIsFilled = !_editor.DefaultIsFilled;
                });

            _editor.SnapToGridCommand = new DelegateCommand(
                () =>
                {
                    _editor.SnapToGrid = !_editor.SnapToGrid;
                });

            _editor.DrawPointsCommand = new DelegateCommand(
                () =>
                {
                    _editor.Renderer.DrawPoints = !_editor.Renderer.DrawPoints;
                    _editor.Container.Invalidate();
                });

            _editor.AddLayerCommand = new DelegateCommand(
                () =>
                {
                    _editor.Container.Layers.Add(Layer.Create("New"));
                });

            _editor.RemoveLayerCommand = new DelegateCommand(
                () =>
                {
                    _editor.RemoveCurrentLayer();
                });

            _editor.AddStyleCommand = new DelegateCommand(
                () =>
                {
                    _editor.Container.Styles.Add(ShapeStyle.Create("New"));
                });

            _editor.RemoveStyleCommand = new DelegateCommand(
                () =>
                {
                    _editor.RemoveCurrentStyle();
                });

            _editor.RemoveShapeCommand = new DelegateCommand(
                () =>
                {
                    _editor.RemoveCurrentShape();
                });

            _editor.GroupSelectedCommand = new DelegateCommand(
                () =>
                {
                    _editor.GroupSelected();
                });

            _editor.GroupCurrentLayerCommand = new DelegateCommand(
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

    public partial class MainWindow : Window, IView
    {
        private MainViewModel _vm;

        public MainWindow()
        {
            InitializeComponent();

            _vm = new MainViewModel();

            _vm.Initialize(this);

            _vm.Editor.LayersWindowCommand = new DelegateCommand(
                () =>
                {
                    (new LayersWindow() { Owner = this, DataContext = _vm.Editor }).Show();
                });

            _vm.Editor.StyleWindowCommand = new DelegateCommand(
                () =>
                {
                    (new StyleWindow() { Owner = this, DataContext = _vm.Editor }).Show();
                });

            _vm.Editor.StylesWindowCommand = new DelegateCommand(
                () =>
                {
                    (new StylesWindow() { Owner = this, DataContext = _vm.Editor }).Show();
                });

            _vm.Editor.ShapesWindowCommand = new DelegateCommand(
                () =>
                {
                    (new ShapesWindow() { Owner = this, DataContext = _vm.Editor }).Show();
                });

            _vm.Editor.ContainerWindowCommand = new DelegateCommand(
                () =>
                {
                    (new ContainerWindow() { Owner = this, DataContext = _vm.Editor }).Show();
                });

            Loaded +=
                (s, e) =>
                {
                    //Demo.All(_vm.Editor.Container, 10);
                };

            DataContext = _vm.Editor;
        }
    }
}
