// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Numerics;
using System.Threading.Tasks;
using Core2D.Editor;
using Core2D.Editor.Factories;
using Core2D.Editor.Input;
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Shapes;
using Core2D.Style;
using FileSystem.Uwp;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Renderer.Win2D;
using Serializer.Xaml;
using TextFieldReader.CsvHelper;
using TextFieldWriter.CsvHelper;
using Windows.Devices.Input;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Core2D.Uwp
{
    /// <summary>
    /// Interaction logic for <see cref="MainPage"/> xaml.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private IFileSystem _fileIO;
        private ILog _log;
        private ImmutableArray<IFileWriter> _writers;
        private ProjectEditor _editor;
        private Win2dRenderer _renderer;
        private PointerPressType _pressed;
        private string _imagePath;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainPage"/> class.
        /// </summary>
        public MainPage()
        {
            InitializeComponent();

            _log = default(ILog);
            _fileIO = new UwpFileSystem();
            _writers = new IFileWriter[] { }.ToImmutableArray();

            InitializeEditor(_fileIO, _log, _writers);

            InitializeCanvas();
            InitializePage();

            OnNew();
            //Commands.ToolLineCommand.Execute(null);
        }

        /// <summary>
        /// Initialize <see cref="ProjectEditor"/> object.
        /// </summary>
        /// <param name="fileIO">The file system instance.</param>
        /// <param name="log">The log instance.</param>
        /// <param name="writers">The file writers.</param>
        private void InitializeEditor(IFileSystem fileIO, ILog log, ImmutableArray<IFileWriter> writers)
        {
            _renderer = new Win2dRenderer();

            _editor = new ProjectEditor()
            {
                CurrentTool = Tool.Selection,
                CurrentPathTool = PathTool.Line,
                //Application = this,
                Log = log,
                FileIO = fileIO,
                CommandManager = new UwpCommandManager(),
                Renderers = new ShapeRenderer[] { _renderer },
                ProjectFactory = new ProjectFactory(),
                TextClipboard = new UwpTextClipboard(),
                JsonSerializer = new NewtonsoftTextSerializer(),
                XamlSerializer = new PortableXamlSerializer(),
                FileWriters = writers,
                CsvReader = new CsvHelperReader(),
                CsvWriter = new CsvHelperWriter(),
                //GetImageKey = async () => await (this as IEditorApplication).OnGetImageKeyAsync()
            }.Defaults();

            _editor.InitializeCommands();

            _editor.GetImageKey = async () => await Task.Run(() => _imagePath);
            _editor.Invalidate = () => canvas.Invalidate();

            Commands.OpenCommand =
                Command<string>.Create(
                    async (parameter) => await OnOpen(),
                    (parameter) => _editor.IsEditMode());

            Commands.SaveAsCommand =
                Command.Create(
                    async () => await OnSaveAs(),
                    () => _editor.IsEditMode());

            _editor.CommandManager.RegisterCommands();

            DataContext = _editor;
        }

        private void InitializeCanvas()
        {
            canvas.Draw += CanvasControl_Draw;
            canvas.PointerPressed += CanvasControl_PointerPressed;
            canvas.PointerReleased += CanvasControl_PointerReleased;
            canvas.PointerMoved += CanvasControl_PointerMoved;
            canvas.PointerWheelChanged += CanvasControl_PointerWheelChanged;
            canvas.SizeChanged += Canvas_SizeChanged;
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            /*
            if (_editor != null && _editor.Project != null)
            {
                if (_editor.Renderers[0].State.EnableAutofit)
                {
                    AutoFit();
                }
            }
            */
        }

        private void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            Draw(args.DrawingSession);
        }

        private async void CanvasControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(sender as CanvasControl);
            var pos = p.Position;
            var type = p.PointerDevice.PointerDeviceType;
            switch (type)
            {
                case PointerDeviceType.Mouse:
                    {
                        if (p.Properties.IsLeftButtonPressed)
                        {
                            if (_editor.IsLeftDownAvailable())
                            {
                                if (_editor.CurrentTool == Tool.Image && _imagePath == null)
                                {
                                    var file = await GetImageKeyAsync();
                                    if (file == null)
                                        return;

                                    string key = await GetImageKey(file);

                                    await CacheImage(key);

                                    _imagePath = key;
                                }
                                else
                                {
                                    _imagePath = null;
                                }

                                _editor.LeftDown(pos.X, pos.Y);
                                _pressed = PointerPressType.Left;
                            }
                        }
                        //else if (p.Properties.IsMiddleButtonPressed)
                        //{
                        //    _pressed = PointerPressType.Middle;
                        //}
                        else if (p.Properties.IsRightButtonPressed)
                        {
                            if (_editor.IsRightDownAvailable())
                            {
                                _editor.RightDown(pos.X, pos.Y);
                                _pressed = PointerPressType.Right;
                            }
                        }
                        else
                        {
                            _pressed = PointerPressType.None;
                        }
                    }
                    break;
                case PointerDeviceType.Pen:
                    {
                        // TODO: Add pen support.
                        //_pressed = PointerPressType.Pen;
                    }
                    break;
                case PointerDeviceType.Touch:
                    {
                        // TODO: Add touch support.
                        //_pressed = PointerPressType.Touch;
                    }
                    break;
            }
        }

        private void CanvasControl_PointerReleased(object sender, PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(sender as CanvasControl);
            var pos = p.Position;
            var type = p.PointerDevice.PointerDeviceType;
            switch (type)
            {
                case PointerDeviceType.Mouse:
                    {
                        switch (_pressed)
                        {
                            case PointerPressType.None:
                                break;
                            case PointerPressType.Left:
                                {
                                    if (_editor.IsLeftUpAvailable())
                                    {
                                        _editor.LeftUp(pos.X, pos.Y);
                                    }
                                }
                                break;
                            case PointerPressType.Middle:
                                break;
                            case PointerPressType.Right:
                                {
                                    if (_editor.IsRightUpAvailable())
                                    {
                                        _editor.RightUp(pos.X, pos.Y);
                                    }
                                }
                                break;
                            case PointerPressType.Pen:
                            case PointerPressType.Touch:
                                break;
                        }
                    }
                    break;
                case PointerDeviceType.Pen:
                    {
                        switch (_pressed)
                        {
                            case PointerPressType.None:
                            case PointerPressType.Left:
                            case PointerPressType.Middle:
                            case PointerPressType.Right:
                                break;
                            case PointerPressType.Pen:
                                {
                                    // TODO: Add pen support.
                                }
                                break;
                            case PointerPressType.Touch:
                                break;
                        }
                    }
                    break;
                case PointerDeviceType.Touch:
                    {
                        switch (_pressed)
                        {
                            case PointerPressType.None:
                            case PointerPressType.Left:
                            case PointerPressType.Middle:
                            case PointerPressType.Right:
                            case PointerPressType.Pen:
                            case PointerPressType.Touch:
                                {
                                    // TODO: Add touch support.
                                }
                                break;
                        }
                    }
                    break;
            }

            _pressed = PointerPressType.None;
        }

        private void CanvasControl_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(sender as CanvasControl);
            var pos = p.Position;

            if (_editor.IsMoveAvailable())
            {
                _editor.Move(pos.X, pos.Y);
            }
        }

        private void CanvasControl_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            /*
            var p = e.GetCurrentPoint(sender as CanvasControl);
            var pos = p.Position;
            var delta = p.Properties.MouseWheelDelta;

            if (_editor.IsMoveAvailable())
            {
                if (_editor != null && _editor.Project != null)
                {
                    var container = _editor.Project.CurrentContainer;
                    _editor.Wheel(
                        pos.X,
                        pos.Y,
                        delta,
                        canvas.ActualWidth,
                        canvas.ActualHeight,
                        container.Template.Width,
                        container.Template.Height);
                }
            }
            */
        }

        private void DrawBackground(CanvasDrawingSession ds, ArgbColor c, double width, double height)
        {
            var color = Color.FromArgb(c.A, c.R, c.G, c.B);
            var rect = Rect2.Create(0, 0, width, height);
            ds.FillRectangle(
                (float)rect.X,
                (float)rect.Y,
                (float)rect.Width,
                (float)rect.Height,
                color);
        }

        private void Draw(CanvasDrawingSession ds)
        {
            ds.Antialiasing = CanvasAntialiasing.Aliased;
            ds.TextAntialiasing = CanvasTextAntialiasing.Auto;
            ds.Clear(Windows.UI.Colors.Transparent);

            var renderer = _editor.Renderers[0];
            var container = _editor.Project.CurrentContainer;
            if (container == null)
                return;

            var t = Matrix3x2.CreateTranslation((float)_renderer.State.PanX, (float)_renderer.State.PanY);
            var s = Matrix3x2.CreateScale((float)_renderer.State.ZoomX);
            var old = ds.Transform;
            ds.Transform = s * t;

            var template = container.Template;
            if (template != null)
            {
                DrawBackground(
                    ds,
                    template.Background,
                    template.Width,
                    template.Height);

                renderer.Draw(
                    ds,
                    template,
                    container.Data.Properties,
                    null);
            }
            else
            {
                DrawBackground(
                    ds,
                    container.Background,
                    container.Width,
                    container.Height);
            }

            renderer.Draw(
                ds,
                container,
                container.Data.Properties,
                null);

            if (container.WorkingLayer != null)
            {
                renderer.Draw(
                    ds,
                    container.WorkingLayer,
                    container.Data.Properties,
                    null);
            }

            if (container.HelperLayer != null)
            {
                renderer.Draw(
                    ds,
                    container.HelperLayer,
                    container.Data.Properties,
                    null);
            }

            ds.Transform = old;
        }

        private void InitializePage()
        {
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Loaded += (sender, e) => canvas.Focus(FocusState.Programmatic);
            Unloaded += (sender, e) => _log?.Dispose();
        }

        private async void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            var state = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
            bool control = state.HasFlag(CoreVirtualKeyStates.Down);

            if (control)
            {
                switch (args.VirtualKey)
                {
                    case VirtualKey.N:
                        OnNew();
                        break;
                    case VirtualKey.O:
                        await OnOpen();
                        break;
                    case VirtualKey.S:
                        await OnSaveAs();
                        break;
                    case VirtualKey.Z:
                        Commands.UndoCommand.Execute(null);
                        break;
                    case VirtualKey.Y:
                        Commands.RedoCommand.Execute(null);
                        break;
                    case VirtualKey.X:
                        Commands.CutCommand.Execute(null);
                        break;
                    case VirtualKey.C:
                        Commands.CopyCommand.Execute(null);
                        break;
                    case VirtualKey.V:
                        Commands.PasteCommand.Execute(null);
                        break;
                    case VirtualKey.A:
                        Commands.SelectAllCommand.Execute(null);
                        break;
                    case VirtualKey.G:
                        Commands.GroupCommand.Execute(null);
                        break;
                    case VirtualKey.U:
                        Commands.UngroupCommand.Execute(null);
                        break;
                }
            }
            else
            {
                switch (args.VirtualKey)
                {
                    case VirtualKey.N:
                        Commands.ToolNoneCommand.Execute(null);
                        break;
                    case VirtualKey.S:
                        Commands.ToolSelectionCommand.Execute(null);
                        break;
                    case VirtualKey.P:
                        Commands.ToolPointCommand.Execute(null);
                        break;
                    case VirtualKey.L:
                        Commands.ToolLineCommand.Execute(null);
                        break;
                    case VirtualKey.R:
                        Commands.ToolRectangleCommand.Execute(null);
                        break;
                    case VirtualKey.E:
                        Commands.ToolEllipseCommand.Execute(null);
                        break;
                    case VirtualKey.A:
                        Commands.ToolArcCommand.Execute(null);
                        break;
                    case VirtualKey.B:
                        Commands.ToolCubicBezierCommand.Execute(null);
                        break;
                    case VirtualKey.Q:
                        Commands.ToolQuadraticBezierCommand.Execute(null);
                        break;
                    case VirtualKey.T:
                        Commands.ToolTextCommand.Execute(null);
                        break;
                    case VirtualKey.I:
                        Commands.ToolImageCommand.Execute(null);
                        break;
                    case VirtualKey.H:
                        Commands.ToolPathCommand.Execute(null);
                        break;
                    case VirtualKey.M:
                        Commands.ToolMoveCommand.Execute(null);
                        break;
                    case VirtualKey.F:
                        Commands.DefaultIsFilledCommand.Execute(null);
                        break;
                    case VirtualKey.G:
                        Commands.SnapToGridCommand.Execute(null);
                        break;
                    case VirtualKey.C:
                        Commands.TryToConnectCommand.Execute(null);
                        break;
                    case VirtualKey.Z:
                        // Reset Zoom
                        //_editor.Invalidate();
                        break;
                    case VirtualKey.X:
                        // Auto Fit Zoom
                        //_editor.Invalidate();
                        break;
                    case VirtualKey.Delete:
                        Commands.DeleteCommand.Execute(null);
                        break;
                }
            }
        }

        private void OnNew()
        {
            Commands.NewCommand.Execute(null);
            _editor.Invalidate();
        }

        private async Task OnOpen()
        {
            var file = await GetOpenProjectPathAsync();
            if (file != null)
            {
                var project = default(XProject);
                using (var stream = await file.OpenStreamForReadAsync())
                {
                    project = await Task.Run(() =>
                    {
                        return XProject.Open(stream, _fileIO, _editor.JsonSerializer);
                    });
                }

                _editor.OnOpen(project, file.Path);
                await CacheImages(project);
                _editor.Invalidate();
            }

            await Task.Run(() => { });
        }

        private async Task OnSaveAs()
        {
            var file = await GetSaveProjectPathAsync(_editor.Project.Name);
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);

                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    await Task.Run(() =>
                    {
                        XProject.Save(_editor.Project, stream, _fileIO, _editor.JsonSerializer);
                    });
                }

                await CachedFileManager.CompleteUpdatesAsync(file);
            }

            await Task.Run(() => { });
        }

        private async Task CacheImage(string key)
        {
            var bytes = _editor.Renderers[0].State.ImageCache.GetImage(key);
            if (bytes != null)
            {
                using (var ms = new MemoryStream(bytes))
                {
                    using (var ras = ms.AsRandomAccessStream())
                    {
                        var bi = await CanvasBitmap.LoadAsync(canvas, ras);
                        _renderer.CacheImage(key, bi);
                        ras.Dispose();
                    }
                }
            }
        }

        private async Task CacheImages(XProject project)
        {
            var images = XProject.GetAllShapes<XImage>(project);
            if (images != null)
            {
                foreach (var image in images)
                {
                    await CacheImage(image.Key);
                }
            }
        }

        private async Task<string> GetImageKey(IStorageFile file)
        {
            var key = default(string);

            using (var fileStream = await file.OpenStreamForReadAsync())
            {
                var bytes = _fileIO.ReadBinary(fileStream);
                key = _editor.Project.AddImageFromFile(file.Path, bytes);
            }

            return key;
        }

        private async Task<IStorageFile> GetImageKeyAsync()
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.Thumbnail;
            picker.SuggestedStartLocation = PickerLocationId.PicturesLibrary;
            picker.FileTypeFilter.Add(".jpg");
            picker.FileTypeFilter.Add(".jpeg");
            picker.FileTypeFilter.Add(".png");
            picker.FileTypeFilter.Add(".bmp");
            picker.FileTypeFilter.Add(".gif");
            picker.FileTypeFilter.Add(".tiff");
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                return file;
            }
            return null;
        }

        private async Task<IStorageFile> GetOpenProjectPathAsync()
        {
            var picker = new FileOpenPicker();
            picker.ViewMode = PickerViewMode.List;
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeFilter.Add(".project");
            var file = await picker.PickSingleFileAsync();
            if (file != null)
            {
                return file;
            }
            return null;
        }

        private async Task<IStorageFile> GetSaveProjectPathAsync(string name)
        {
            var picker = new FileSavePicker();
            picker.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
            picker.FileTypeChoices.Add("Project", new List<string>() { ".project" });
            picker.SuggestedFileName = name;
            var file = await picker.PickSaveFileAsync();
            if (file != null)
            {
                return file;
            }
            return null;
        }

        private void ContainersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _editor.Invalidate();
        }
    }
}
