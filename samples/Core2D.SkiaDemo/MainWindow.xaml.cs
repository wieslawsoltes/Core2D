// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Autofac;
using Core2D.Editor;
using Core2D.Editor.Input;
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Renderer.Presenters;
using Core2D.Renderer.SkiaSharp;
using Core2D.Shape;
using Core2D.Utilities.Wpf;
using Microsoft.Win32;
using SkiaSharp;
using SkiaSharp.Views.Desktop;

namespace Core2D.SkiaDemo
{
    public partial class MainWindow : Window
    {
        private IContainer _container;
        private IServiceProvider _serviceProvider;
        private ContainerPresenter _presenter;
        private ProjectEditor _projectEditor;
        private InputProcessor _inputProcessor;
        private SvgWindow _previewWindow;
        private bool _isClosing = false;

        public MainWindow()
        {
            InitializeComponent();

            InitializeContainer();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            KeyDown += MainWindow_KeyDown;

            _previewWindow = new SvgWindow();
            _previewWindow.svgButton.Click += PreviewWindowSvgButton_Click;
            _previewWindow.Closing += PreviewWindow_Closing;

            CanvasElement.PaintSurface += PaintSurface;
        }

        private void InitializeContainer()
        {
            var builder = new ContainerBuilder();

            builder.RegisterAssemblyModules(typeof(MainWindow).Assembly);

            _container = builder.Build();

            _serviceProvider = _container.Resolve<IServiceProvider>();

            _presenter = _serviceProvider.GetService<ContainerPresenter>();
            _projectEditor = _serviceProvider.GetService<ProjectEditor>();

            _projectEditor.CurrentTool = _projectEditor.Tools.FirstOrDefault(t => t.Title == "Selection");
            _projectEditor.CurrentPathTool = _projectEditor.PathTools.FirstOrDefault(t => t.Title == "Line");
            _projectEditor.OnNewProject();

            _projectEditor.Invalidate = () =>
            {
                RefreshRequested(null, null);
                if (_previewWindow.svgLive.IsChecked == true)
                {
                    UpdateSvg();
                }
            };

            _inputProcessor = new InputProcessor(
                new WpfInputSource(
                    CanvasElement,
                    CanvasElement,
                    FiPointShapeOffset),
                _projectEditor);
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            CanvasElement.Focusable = true;
            CanvasElement.Focus();
            RefreshRequested(null, null);
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _inputProcessor.Dispose();
            _isClosing = true;
            _previewWindow.Close();
            _container.Dispose();
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                switch (e.Key)
                {
                    case Key.P:
                        _previewWindow.Show();
                        break;
                    case Key.N:
                        NewProject();
                        break;
                    case Key.O:
                        OpenProject();
                        break;
                    case Key.S:
                        SaveProject();
                        break;
                    case Key.E:
                        Export(_projectEditor.Project.CurrentContainer);
                        break;
                    case Key.Z:
                        _projectEditor.OnUndo();
                        break;
                    case Key.Y:
                        _projectEditor.OnRedo();
                        break;
                    case Key.X:
                        _projectEditor.OnCut();
                        break;
                    case Key.C:
                        _projectEditor.OnCopy();
                        break;
                    case Key.V:
                        _projectEditor.OnPaste();
                        break;
                    case Key.A:
                        _projectEditor.OnSelectAll();
                        break;
                    case Key.G:
                        _projectEditor.OnGroupSelected();
                        break;
                    case Key.U:
                        _projectEditor.OnUngroupSelected();
                        break;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.N:
                        _projectEditor.OnToolNone();
                        break;
                    case Key.S:
                        _projectEditor.OnToolSelection();
                        break;
                    case Key.P:
                        _projectEditor.OnToolPoint();
                        break;
                    case Key.L:
                        _projectEditor.OnToolLine();
                        break;
                    case Key.A:
                        _projectEditor.OnToolArc();
                        break;
                    case Key.B:
                        _projectEditor.OnToolCubicBezier();
                        break;
                    case Key.Q:
                        _projectEditor.OnToolQuadraticBezier();
                        break;
                    case Key.H:
                        _projectEditor.OnToolPath();
                        break;
                    case Key.M:
                        _projectEditor.OnToolMove();
                        break;
                    case Key.R:
                        _projectEditor.OnToolRectangle();
                        break;
                    case Key.E:
                        _projectEditor.OnToolEllipse();
                        break;
                    case Key.T:
                        _projectEditor.OnToolText();
                        break;
                    case Key.I:
                        _projectEditor.OnToolImage();
                        break;
                    case Key.K:
                        _projectEditor.OnToggleDefaultIsStroked();
                        break;
                    case Key.F:
                        _projectEditor.OnToggleDefaultIsFilled();
                        break;
                    case Key.D:
                        _projectEditor.OnToggleDefaultIsClosed();
                        break;
                    case Key.J:
                        _projectEditor.OnToggleDefaultIsSmoothJoin();
                        break;
                    case Key.G:
                        _projectEditor.OnToggleSnapToGrid();
                        break;
                    case Key.C:
                        _projectEditor.OnToggleTryToConnect();
                        break;
                    case Key.Y:
                        _projectEditor.OnToggleCloneStyle();
                        break;
                    case Key.Delete:
                        _projectEditor.OnDeleteSelected();
                        break;
                    case Key.Escape:
                        _projectEditor.OnDeselectAll();
                        break;
                }
            }
        }

        private void PreviewWindowSvgButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateSvg();
        }

        private void PreviewWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!_isClosing)
            {
                _previewWindow.Hide();
                e.Cancel = true;
            }
        }

        public Point FiPointShapeOffset(Point point)
        {
            var container = _projectEditor.Project.CurrentContainer;
            var matrix = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
            double offsetX = (CanvasElement.ActualWidth * matrix.M11 - container.Width) / 2.0;
            double offsetY = (CanvasElement.ActualHeight * matrix.M22 - container.Height) / 2.0;
            return new Point(point.X - offsetX, point.Y - offsetY);
        }

        private void PaintSurface(object sender, SKPaintSurfaceEventArgs e)
        {
            PaintSurface(e.Surface.Canvas, e.Info.Width, e.Info.Height);
        }

        private void RefreshRequested(object sender, EventArgs e)
        {
            CanvasElement.InvalidateVisual();
        }

        private void PaintSurface(SKCanvas canvas, int width, int height)
        {
            var container = _projectEditor?.Project?.CurrentContainer;
            if (container != null)
            {
                var matrix = PresentationSource.FromVisual(this).CompositionTarget.TransformToDevice;
                double offsetX = (CanvasElement.ActualWidth * matrix.M11 - container?.Width ?? 0) / 2.0;
                double offsetY = (CanvasElement.ActualHeight * matrix.M22 - container?.Height ?? 0) / 2.0;

                canvas.Clear(SKColors.White);
                _presenter?.Render(canvas, _projectEditor.Renderers[0], container, offsetX, offsetY);
            }
        }

        private void UpdateSvg()
        {
            bool exportPresenter = _previewWindow.svgExport.IsChecked == true;
            bool showPrintable = _previewWindow.svgPrintable.IsChecked == true;

            Task.Factory.StartNew(() =>
            {
                try
                {
                    IImageCache ic = _projectEditor.Project;
                    var container = _projectEditor.Project.CurrentContainer;

                    var renderer = new SkiaSharpRenderer(true, 96.0);
                    if (!showPrintable)
                    {
                        renderer.State.DrawShapeState.Flags = ShapeStateFlags.Printable;
                    }
                    renderer.State.ImageCache = _projectEditor.Project;

                    var presenter = exportPresenter ?
                        (ContainerPresenter)new ExportPresenter() :
                        (ContainerPresenter)new EditorPresenter();

                    using (var ms = new MemoryStream())
                    {
                        using (var stream = new SKManagedWStream(ms))
                        {
                            using (var writer = new SKXmlStreamWriter(stream))
                            using (var canvas = SKSvgCanvas.Create(SKRect.Create(0, 0, (int)container.Width, (int)container.Height), writer))
                            {
                                presenter.Render(canvas, renderer, container, 0, 0);
                            }
                            stream.Flush();
                        }

                        var svg = Encoding.ASCII.GetString(ms.GetBuffer(), 0, (int)ms.Length);

                        Dispatcher.Invoke(() =>
                        {
                            _previewWindow.svgText.Text = svg;
                        });
                    }
                }
                catch (Exception) { }
            });
        }

        private void NewProject()
        {
            _projectEditor.OnNewProject();
        }

        private void OpenProject()
        {
            var dlg = new OpenFileDialog()
            {
                Filter = "Project (*.project)|*.project|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = ""
            };

            if (dlg.ShowDialog(this) == true)
            {
                _projectEditor.OnOpen(dlg.FileName);
                RefreshRequested(null, null);
            }
        }

        private void SaveProject()
        {
            var dlg = new SaveFileDialog()
            {
                Filter = "Project (*.project)|*.project|All (*.*)|*.*",
                FilterIndex = 0,
                FileName = _projectEditor.Project.Name
            };

            if (dlg.ShowDialog(this) == true)
            {
                _projectEditor.OnSave(dlg.FileName);
            }
        }

        private void Export(object item)
        {
            string name = string.Empty;

            if (item is ProjectContainer)
            {
                name = (item as ProjectContainer).Name;
            }
            else if (item is DocumentContainer)
            {
                name = (item as DocumentContainer).Name;
            }
            else if (item is PageContainer)
            {
                name = (item as PageContainer).Name;
            }

            var sb = new StringBuilder();
            foreach (var writer in _projectEditor.FileWriters)
            {
                sb.Append($"{writer.Name} (*.{writer.Extension})|*.{writer.Extension}|");
            }
            sb.Append("All (*.*)|*.*");

            var dlg = new SaveFileDialog()
            {
                Filter = sb.ToString(),
                FilterIndex = 0,
                FileName = name
            };

            if (dlg.ShowDialog(this) == true)
            {
                string result = dlg.FileName;
                IFileWriter writer = _projectEditor.FileWriters[dlg.FilterIndex - 1];
                if (writer != null)
                {
                    _projectEditor.OnExport(result, item, writer);
                }
            }
        }
    }
}
