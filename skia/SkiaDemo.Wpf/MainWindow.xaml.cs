using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Core2D.Editor;
using Core2D.Editor.Input;
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Renderer.Presenters;
using Core2D.Shape;
using Microsoft.Win32;
using Renderer.SkiaSharp;
using SkiaSharp;
using Utilities.Wpf;

namespace SkiaDemo.Wpf
{
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private ProjectEditor _projectEditor;
        private InputProcessor _inputProcessor;
        private SvgWindow _previewWindow;

        public MainWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            bool isClosing = false;

            _serviceProvider = serviceProvider;
            _projectEditor = _serviceProvider.GetService<ProjectEditor>();

            _previewWindow = new SvgWindow();

            _previewWindow.svgButton.Click += (sender, e) =>
            {
                UpdateSvg();
            };

            _previewWindow.Closing += (sender, e) =>
            {
                if (!isClosing)
                {
                    _previewWindow.Hide();
                    e.Cancel = true;
                }
            };

            Loaded += (seender, e) =>
            {
                _projectEditor.CurrentTool = _projectEditor.Tools.FirstOrDefault(t => t.Name == "Selection");
                _projectEditor.CurrentPathTool = _projectEditor.PathTools.FirstOrDefault(t => t.Name == "Line");
                _projectEditor.OnNewProject();
                _projectEditor.Invalidate = () =>
                {
                    skiaView.InvalidateVisual();
                    if (_previewWindow.svgLive.IsChecked == true)
                    {
                        UpdateSvg();
                    }
                };

                skiaView.Renderer = _projectEditor.Renderers[0];
                skiaView.Container = _projectEditor.Project.CurrentContainer;
                skiaView.Presenter = new ContainerPresenter();
                skiaView.Focusable = true;
                skiaView.Focus();
                skiaView.InvalidateVisual();

                _inputProcessor = new InputProcessor(
                    new WpfInputSource(
                        skiaView,
                        skiaView,
                        skiaView.FixPointOffset),
                    _projectEditor);
            };

            Closing += (sender, e) =>
            {
                _inputProcessor.Dispose();
                isClosing = true;
                _previewWindow.Close();
            };

            KeyDown += (sender, e) =>
            {
                bool isControl = Keyboard.Modifiers == ModifierKeys.Control;

                if (isControl)
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
            };
        }

        private void UpdateSvg()
        {
            Task.Factory.StartNew(() =>
            {
                try
                {
                    IImageCache ic = _projectEditor.Project;
                    var container = _projectEditor.Project.CurrentContainer;

                    var renderer = new SkiaRenderer(true, 96.0);
                    renderer.State.DrawShapeState.Flags = ShapeStateFlags.Printable;
                    renderer.State.ImageCache = _projectEditor.Project;

                    var presenter = new ContainerPresenter();

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
            skiaView.Container = _projectEditor.Project.CurrentContainer;
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
                skiaView.Container = _projectEditor.Project.CurrentContainer;
                skiaView.InvalidateVisual();
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

            if (item is XProject)
            {
                name = (item as XProject).Name;
            }
            else if (item is XDocument)
            {
                name = (item as XDocument).Name;
            }
            else if (item is XContainer)
            {
                name = (item as XContainer).Name;
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
