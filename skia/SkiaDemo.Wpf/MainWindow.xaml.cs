using System.Collections.Immutable;
using System.Windows;
using System.Windows.Input;
using Core2D.Editor;
using Core2D.Editor.Factories;
using Core2D.Editor.Input;
using Core2D.Interfaces;
using Core2D.Renderer;
using Core2D.Renderer.Presenters;
using FileSystem.DotNetFx;
using FileWriter.PdfSkiaSharp;
using Renderer.SkiaSharp;
using Serializer.Newtonsoft;
using Serializer.Xaml;
using TextFieldReader.CsvHelper;
using TextFieldWriter.CsvHelper;
using Utilities.Wpf;

namespace SkiaDemo.Wpf
{
    public partial class MainWindow : Window
    {
        public ProjectEditor Editor { get; set; }
        public InputProcessor Input { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            Loaded += (seender, e) => Initialize();
        }

        public void Initialize()
        {
            Editor = new ProjectEditor()
            {
                CurrentTool = Tool.Selection,
                CurrentPathTool = PathTool.Line,
                FileIO = new DotNetFxFileSystem(),
                CommandManager = new WpfCommandManager(),
                Renderers = new ShapeRenderer[] { new SkiaRenderer() },
                ProjectFactory = new ProjectFactory(),
                TextClipboard = new WpfTextClipboard(),
                JsonSerializer = new NewtonsoftTextSerializer(),
                XamlSerializer = new PortableXamlSerializer(),
                FileWriters = new[] { new PdfWriter() }.ToImmutableArray<IFileWriter>(),
                CsvReader = new CsvHelperReader(),
                CsvWriter = new CsvHelperWriter()
            };

            Editor.Defaults();
            Editor.OnNewProject();
            Editor.Invalidate = () => skiaView.InvalidateVisual();

            skiaView.Renderer = Editor.Renderers[0];
            skiaView.Container = Editor.Project.CurrentContainer;
            skiaView.Presenter = new ContainerPresenter();

            skiaView.Focusable = true;
            skiaView.Focus();
            skiaView.InvalidateVisual();

            Input = new InputProcessor(
                new WpfInputSource(
                    skiaView,
                    skiaView,
                    skiaView.FixPointOffset),
                Editor);

            Closing += (sender, e) => Input.Dispose();
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            bool isControl = Keyboard.Modifiers == ModifierKeys.Control;

            if (isControl)
            {
                switch (e.Key)
                {
                    case Key.Z:
                        Editor.OnUndo();
                        break;
                    case Key.Y:
                        Editor.OnRedo();
                        break;
                    case Key.X:
                        Editor.OnCut();
                        break;
                    case Key.C:
                        Editor.OnCopy();
                        break;
                    case Key.V:
                        Editor.OnPaste();
                        break;
                    case Key.A:
                        Editor.OnSelectAll();
                        break;
                    case Key.G:
                        Editor.OnGroupSelected();
                        break;
                    case Key.U:
                        Editor.OnUngroupSelected();
                        break;
                }
            }
            else
            {
                switch (e.Key)
                {
                    case Key.N:
                        Editor.OnToolNone();
                        break;
                    case Key.S:
                        Editor.OnToolSelection();
                        break;
                    case Key.P:
                        Editor.OnToolPoint();
                        break;
                    case Key.L:
                        Editor.OnToolLine();
                        break;
                    case Key.A:
                        Editor.OnToolArc();
                        break;
                    case Key.B:
                        Editor.OnToolCubicBezier();
                        break;
                    case Key.Q:
                        Editor.OnToolQuadraticBezier();
                        break;
                    case Key.H:
                        Editor.OnToolPath();
                        break;
                    case Key.M:
                        Editor.OnToolMove();
                        break;
                    case Key.R:
                        Editor.OnToolRectangle();
                        break;
                    case Key.E:
                        Editor.OnToolEllipse();
                        break;
                    case Key.T:
                        Editor.OnToolText();
                        break;
                    case Key.I:
                        Editor.OnToolImage();
                        break;
                    case Key.K:
                        Editor.OnToggleDefaultIsStroked();
                        break;
                    case Key.F:
                        Editor.OnToggleDefaultIsFilled();
                        break;
                    case Key.D:
                        Editor.OnToggleDefaultIsClosed();
                        break;
                    case Key.J:
                        Editor.OnToggleDefaultIsSmoothJoin();
                        break;
                    case Key.G:
                        Editor.OnToggleSnapToGrid();
                        break;
                    case Key.C:
                        Editor.OnToggleTryToConnect();
                        break;
                    case Key.Y:
                        Editor.OnToggleCloneStyle();
                        break;
                    case Key.Delete:
                        Editor.OnDeleteSelected();
                        break;
                    case Key.Escape:
                        Editor.OnDeselectAll();
                        break;
                }
            }
        }
    }
}
