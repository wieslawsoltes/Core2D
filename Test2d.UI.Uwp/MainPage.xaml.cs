// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.Storage.Provider;
using Windows.Storage.Streams;
using Windows.System;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Text;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using T2d = Test2d;

namespace Test.Uwp
{
    public sealed partial class MainPage : Page, T2d.IView
    {
        private T2d.EditorContext _context;
        private Win2dRenderer _renderer;
        private PointerPressType _pressed;
        private Uri _imagePath;

        public MainPage()
        {
            InitializeComponent();
            InitializeContext();
            InitializeCanvas();
            InitializePage();
        }

        public void Close()
        {
            this.Close();
        }

        private void InitializeContext()
        {
            _renderer = new Win2dRenderer();

            _context = new T2d.EditorContext()
            {
                View = this,
                Renderers = new T2d.IRenderer[] { _renderer },
                ProjectFactory = new T2d.ProjectFactory(),
                TextClipboard = new TextClipboard(),
                Serializer = new T2d.NewtonsoftSerializer(),
                //PdfWriter = new T2d.PdfWriter(),
                //DxfWriter = new T2d.DxfWriter(),
                //CsvReader = new T2d.CsvHelperReader(),
                //CsvWriter = new T2d.CsvHelperWriter()
            };
            _context.InitializeEditor(null/*new T2d.TraceLog()*/);
            _context.Editor.Renderers[0].State.DrawShapeState = T2d.ShapeState.Visible;
            _context.Editor.GetImagePath = () => _imagePath;

            _context.Commands.OpenCommand =
                T2d.Command<object>.Create(
                    async (parameter) => await OnOpen(),
                    (parameter) => _context.IsEditMode());

            _context.Commands.SaveAsCommand =
                T2d.Command.Create(
                    async () => await OnSaveAs(),
                    () => _context.IsEditMode());

            DataContext = _context;
        }

        private void InitializeCanvas()
        {
            canvas.Draw += CanvasControl_Draw;
            canvas.PointerPressed += CanvasControl_PointerPressed;
            canvas.PointerReleased += CanvasControl_PointerReleased;
            canvas.PointerMoved += CanvasControl_PointerMoved;
            canvas.PointerWheelChanged += CanvasControl_PointerWheelChanged;

            SetContainerInvalidation();
            SetCanvasSize();
        }

        private void SetCanvasSize()
        {
            if (_context.Editor.Project == null)
                return;

            var container = _context.Editor.Project.CurrentContainer;
            if (container == null)
                return;

            canvas.Width = container.Width;
            canvas.Height = container.Height;
        }

        private void SetContainerInvalidation()
        {
            if (_context.Editor.Project == null)
                return;

            var container = _context.Editor.Project.CurrentContainer;
            if (container == null)
                return;

            foreach (var layer in container.Layers)
            {
                layer.InvalidateLayer +=
                    (s, e) =>
                    {
                        canvas.Invalidate();
                    };
            }

            if (container.WorkingLayer != null)
            {
                container.WorkingLayer.InvalidateLayer +=
                    (s, e) =>
                    {
                        canvas.Invalidate();
                    };
            }

            if (container.HelperLayer != null)
            {
                container.HelperLayer.InvalidateLayer +=
                    (s, e) =>
                    {
                        canvas.Invalidate();
                    };
            }
        }

        private void InvalidateContainer()
        {
            SetContainerInvalidation();
            SetCanvasSize();

            if (_context.Editor.Project == null)
                return;

            var container = _context.Editor.Project.CurrentContainer;
            if (_context == null)
                return;

            container.Invalidate();
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
                            if (_context.Editor.IsLeftDownAvailable())
                            {
                                if (_context.Editor.CurrentTool == T2d.Tool.Image && _imagePath == null)
                                {
                                    var file = await GetImagePathAsync();
                                    if (file == null)
                                        return;

                                    var bi = await LoadImage(file, canvas);
                                    if (bi == null)
                                        return;

                                    var uri = new Uri(file.Path);
                                    _renderer.CacheImage(uri, bi);
                                    _imagePath = uri;
                                }
                                else
                                {
                                    _imagePath = null;
                                }

                                _context.Editor.LeftDown(pos.X, pos.Y);
                                _pressed = PointerPressType.Left;
                            }
                        }
                        //else if (p.Properties.IsMiddleButtonPressed)
                        //{
                        //    _pressed = PointerPressType.Middle;
                        //}
                        else if (p.Properties.IsRightButtonPressed)
                        {
                            if (_context.Editor.IsRightDownAvailable())
                            {
                                _context.Editor.RightDown(pos.X, pos.Y);
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
                        // TODO:
                        //_pressed = PointerPressType.Pen;
                    }
                    break;
                case PointerDeviceType.Touch:
                    {
                        // TODO:
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
                                    if (_context.Editor.IsLeftUpAvailable())
                                    {
                                        _context.Editor.LeftUp(pos.X, pos.Y);
                                    }
                                }
                                break;
                            case PointerPressType.Middle:
                                break;
                            case PointerPressType.Right:
                                {
                                    if (_context.Editor.IsRightUpAvailable())
                                    {
                                        _context.Editor.RightUp(pos.X, pos.Y);
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
                                    // TODO:
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
                                    // TODO:
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

            if (_context.Editor.IsMoveAvailable())
            {
                _context.Editor.Move(pos.X, pos.Y);
            }
        }

        private void CanvasControl_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            // TODO: 
            //var p = e.GetCurrentPoint(sender as CanvasControl);
            //var delta = p.Properties.MouseWheelDelta;
            //Debug.WriteLine("Delta: {0}", p.Properties.MouseWheelDelta);
        }

        private void DrawBackground(CanvasDrawingSession ds, T2d.ArgbColor c, double width, double height)
        {
            var color = Color.FromArgb(c.A, c.R, c.G, c.B);
            var rect = T2d.Rect2.Create(0, 0, width, height);
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
            ds.Clear(Colors.Transparent);

            var renderer = _context.Editor.Renderers[0];
            var container = _context.Editor.Project.CurrentContainer;

            if (container.Template != null)
            {
                DrawBackground(
                    ds,
                    container.Template.Background,
                    container.Width,
                    container.Height);

                renderer.Draw(
                    ds,
                    container.Template,
                    container.Properties,
                    null);
            }

            DrawBackground(
                ds,
                container.Background,
                container.Width,
                container.Height);

            renderer.Draw(
                ds,
                container,
                container.Properties,
                null);

            if (container.WorkingLayer != null)
            {
                renderer.Draw(
                    ds,
                    container.WorkingLayer,
                    container.Properties,
                    null);
            }

            if (container.HelperLayer != null)
            {
                renderer.Draw(
                    ds,
                    container.HelperLayer,
                    container.Properties,
                    null);
            }
        }

        private void InitializePage()
        {
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Loaded += (sender, e) => canvas.Focus(FocusState.Programmatic);
            Unloaded += (sender, e) => _context.Dispose();
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
                        {
                            OnNew();
                        }
                        break;
                    case VirtualKey.O:
                        {
                            await OnOpen();
                        }
                        break;
                    case VirtualKey.S:
                        {
                            await OnSaveAs();
                        }
                        break;
                    case VirtualKey.Z:
                        {
                            _context.Commands.UndoCommand.Execute(null);
                        }
                        break;
                    case VirtualKey.Y:
                        {
                            _context.Commands.RedoCommand.Execute(null);
                        }
                        break;
                    case VirtualKey.X:
                        {
                            _context.Commands.CutCommand.Execute(null);
                        }
                        break;
                    case VirtualKey.C:
                        {
                            _context.Commands.CopyCommand.Execute(null);
                        }
                        break;
                    case VirtualKey.V:
                        {
                            _context.Commands.PasteCommand.Execute(null);
                        }
                        break;
                    case VirtualKey.A:
                        {
                            _context.Commands.SelectAllCommand.Execute(null);
                        }
                        break;
                    case VirtualKey.G:
                        {
                            _context.Commands.GroupCommand.Execute(null);
                        }
                        break;
                    case VirtualKey.U:
                        {
                            _context.Commands.UngroupCommand.Execute(null);
                        }
                        break;
                }
            }
            else
            {
                switch (args.VirtualKey)
                {
                    case VirtualKey.N:
                        _context.Commands.ToolNoneCommand.Execute(null);
                        break;
                    case VirtualKey.S:
                        _context.Commands.ToolSelectionCommand.Execute(null);
                        break;
                    case VirtualKey.P:
                        _context.Commands.ToolPointCommand.Execute(null);
                        break;
                    case VirtualKey.L:
                        _context.Commands.ToolLineCommand.Execute(null);
                        break;
                    case VirtualKey.R:
                        _context.Commands.ToolRectangleCommand.Execute(null);
                        break;
                    case VirtualKey.E:
                        _context.Commands.ToolEllipseCommand.Execute(null);
                        break;
                    case VirtualKey.A:
                        _context.Commands.ToolArcCommand.Execute(null);
                        break;
                    case VirtualKey.B:
                        _context.Commands.ToolBezierCommand.Execute(null);
                        break;
                    case VirtualKey.Q:
                        _context.Commands.ToolQBezierCommand.Execute(null);
                        break;
                    case VirtualKey.T:
                        _context.Commands.ToolTextCommand.Execute(null);
                        break;
                    case VirtualKey.I:
                        _context.Commands.ToolImageCommand.Execute(null);
                        break;
                    case VirtualKey.H:
                        _context.Commands.ToolPathCommand.Execute(null);
                        break;
                    case VirtualKey.M:
                        _context.Commands.ToolMoveCommand.Execute(null);
                        break;
                    case VirtualKey.F:
                        _context.Commands.DefaultIsFilledCommand.Execute(null);
                        break;
                    case VirtualKey.G:
                        _context.Commands.SnapToGridCommand.Execute(null);
                        break;
                    case VirtualKey.C:
                        _context.Commands.TryToConnectCommand.Execute(null);
                        break;
                    case VirtualKey.Z:
                        // TODO: Zoom canvas.
                        //InvalidateContainer();
                        break;
                    case VirtualKey.X:
                        // TODO: Autofit canvas.
                        //InvalidateContainer();
                        break;
                    case VirtualKey.Delete:
                        _context.Commands.DeleteCommand.Execute(null);
                        break;
                }
            }
        }

        private void OnNew()
        {
            _context.Commands.NewCommand.Execute(null);
            InvalidateContainer();
        }

        private async Task OnOpen()
        {
            var file = await GetOpenProjectPathAsync();
            if (file != null)
            {
                var buffer = await FileIO.ReadBufferAsync(file);

                string json = await Task.Run(() =>
                {
                    using (var fs = buffer.AsStream())
                    {
                        return T2d.Utf8TextFile.Decompress(fs);
                    }
                });

                var project = _context.Serializer.FromJson<T2d.Project>(json);
                _context.Editor.History.Reset();
                _context.Editor.Unload();

                await CacheImages(project);

                _context.Editor.Load(project);

                InvalidateContainer();
            }
        }

        private async Task OnSaveAs()
        {
            var file = await GetSaveProjectPathAsync(_context.Editor.Project.Name);
            if (file != null)
            {
                var json = await Task.Run(() => _context.Serializer.ToJson(_context.Editor.Project));

                CachedFileManager.DeferUpdates(file);

                var fs = await file.OpenStreamForWriteAsync();
                await Task.Run(() => T2d.Utf8TextFile.Compress(fs, json));
                fs.Dispose();

                await CachedFileManager.CompleteUpdatesAsync(file);
            }
        }

        private async Task<CanvasBitmap> LoadImage(IStorageFile file, ICanvasResourceCreator resourceCreator)
        {
            var stream = await file.OpenReadAsync();
            var bi = await CanvasBitmap.LoadAsync(resourceCreator, stream);
            return bi;
        }

        private async Task CacheImages(T2d.Project project)
        {
            var images = T2d.Editor.GetAllShapes<T2d.XImage>(project);
            if (images != null)
            {
                foreach (var image in images)
                {
                    var file = await StorageFile.GetFileFromPathAsync(image.Path.LocalPath);
                    var bi = await LoadImage(file, canvas);
                    if (bi != null)
                    {
                        _renderer.CacheImage(image.Path, bi);
                    }
                }
            }
        }

        private async Task<IStorageFile> GetImagePathAsync()
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
            InvalidateContainer();
        }
    }
}
