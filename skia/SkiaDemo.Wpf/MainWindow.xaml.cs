using System;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Core2D.Editor;
using Core2D.Editor.Input;
using Core2D.Renderer.Presenters;
using Utilities.Wpf;

namespace SkiaDemo.Wpf
{
    public partial class MainWindow : Window
    {
        private readonly IServiceProvider _serviceProvider;
        private ProjectEditor _projectEditor;
        private InputProcessor _inputProcessor;

        public MainWindow(IServiceProvider serviceProvider)
        {
            InitializeComponent();

            _serviceProvider = serviceProvider;
            _projectEditor = _serviceProvider.GetService<ProjectEditor>();

            Loaded += (seender, e) =>
            {

                _projectEditor.CurrentTool = _projectEditor.Tools.FirstOrDefault(t => t.Name == "Selection");
                _projectEditor.CurrentPathTool = _projectEditor.PathTools.FirstOrDefault(t => t.Name == "Line");

                _projectEditor.OnNewProject();
                _projectEditor.Invalidate = () => skiaView.InvalidateVisual();

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

            Closing += (sender, e) => _inputProcessor.Dispose();

            KeyDown += (sender, e) =>
            {
                bool isControl = Keyboard.Modifiers == ModifierKeys.Control;

                if (isControl)
                {
                    switch (e.Key)
                    {
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
    }
}
