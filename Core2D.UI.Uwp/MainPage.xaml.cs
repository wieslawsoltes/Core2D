// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using System;
using System.Collections.Generic;
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

namespace Test.Uwp
{
    /// <summary>
    /// 
    /// </summary>
    public sealed partial class MainPage : Page, Core2D.IView
    {
        private Core2D.EditorContext _context;
        private Core2D.ZoomState _state;
        private Win2dRenderer _renderer;
        private PointerPressType _pressed;
        private string _imagePath;

        /// <summary>
        /// 
        /// </summary>
        public MainPage()
        {
            InitializeComponent();
            InitializeContext();
            InitializeCanvas();
            InitializePage();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Close()
        {
            this.Close();
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeContext()
        {
            _renderer = new Win2dRenderer();

            _context = new Core2D.EditorContext()
            {
                View = this,
                Renderers = new Core2D.IRenderer[] { _renderer },
                ProjectFactory = new Core2D.ProjectFactory(),
                TextClipboard = new TextClipboard(),
                Serializer = new Dependencies.NewtonsoftSerializer(),
                //PdfWriter = new Dependencies.PdfWriter(),
                //DxfWriter = new Dependencies.DxfWriter(),
                //CsvReader = new Dependencies.CsvHelperReader(),
                //CsvWriter = new Dependencies.CsvHelperWriter()
            };

            _context.Renderers[0].State.EnableAutofit = true;
            _context.InitializeEditor(null/*new Dependencies.TraceLog()*/);
            _context.InitializeCommands();
            _context.Editor.Renderers[0].State.DrawShapeState.Flags = Core2D.ShapeStateFlags.Visible;
            _context.Editor.GetImageKey = async () => await Task.Run(() => _imagePath);
            _context.Editor.Invalidate = () => canvas.Invalidate();

            _context.Commands.OpenCommand =
                Core2D.Command<object>.Create(
                    async (parameter) => await OnOpen(),
                    (parameter) => _context.IsEditMode());

            _context.Commands.SaveAsCommand =
                Core2D.Command.Create(
                    async () => await OnSaveAs(),
                    () => _context.IsEditMode());

            _state = new Core2D.ZoomState(_context.Editor);

            DataContext = _context;
        }

        /// <summary>
        /// 
        /// </summary>
        public void ResetZoom()
        {
            if (_context != null && _context.Editor.Project != null)
            {
                var container = _context.Editor.Project.CurrentContainer;
                _state.ResetZoom(
                    canvas.ActualWidth,
                    canvas.ActualHeight,
                    container.Width,
                    container.Height);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public void AutoFit()
        {
            if (_context != null && _context.Editor.Project != null)
            {
                var container = _context.Editor.Project.CurrentContainer;
                _state.AutoFit(
                    canvas.ActualWidth,
                    canvas.ActualHeight,
                    container.Width,
                    container.Height);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void InitializeCanvas()
        {
            canvas.Draw += CanvasControl_Draw;
            canvas.PointerPressed += CanvasControl_PointerPressed;
            canvas.PointerReleased += CanvasControl_PointerReleased;
            canvas.PointerMoved += CanvasControl_PointerMoved;
            canvas.PointerWheelChanged += CanvasControl_PointerWheelChanged;

            canvas.SizeChanged += Canvas_SizeChanged;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (_context != null && _context.Editor.Project != null)
            {
                if (_context.Renderers[0].State.EnableAutofit)
                {
                    AutoFit();
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            Draw(args.DrawingSession);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                                if (_context.Editor.CurrentTool == Core2D.Tool.Image && _imagePath == null)
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

                                _state.LeftDown(pos.X, pos.Y);
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
                                _state.RightDown(pos.X, pos.Y);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                                        _state.LeftUp(pos.X, pos.Y);
                                    }
                                }
                                break;
                            case PointerPressType.Middle:
                                break;
                            case PointerPressType.Right:
                                {
                                    if (_context.Editor.IsRightUpAvailable())
                                    {
                                        _state.RightUp(pos.X, pos.Y);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasControl_PointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(sender as CanvasControl);
            var pos = p.Position;

            if (_context.Editor.IsMoveAvailable())
            {
                _state.Move(pos.X, pos.Y);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CanvasControl_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(sender as CanvasControl);
            var pos = p.Position;
            var delta = p.Properties.MouseWheelDelta;

            if (_context.Editor.IsMoveAvailable())
            {
                _state.Wheel(pos.X, pos.Y, delta);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ds"></param>
        /// <param name="c"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        private void DrawBackground(CanvasDrawingSession ds, Core2D.ArgbColor c, double width, double height)
        {
            var color = Color.FromArgb(
                (byte)c.A,
                (byte)c.R,
                (byte)c.G,
                (byte)c.B);
            var rect = Core2D.Rect2.Create(0, 0, width, height);
            ds.FillRectangle(
                (float)rect.X,
                (float)rect.Y,
                (float)rect.Width,
                (float)rect.Height,
                color);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ds"></param>
        private void Draw(CanvasDrawingSession ds)
        {
            ds.Antialiasing = CanvasAntialiasing.Aliased;
            ds.TextAntialiasing = CanvasTextAntialiasing.Auto;
            ds.Clear(Colors.Transparent);

            var renderer = _context.Editor.Renderers[0];
            var container = _context.Editor.Project.CurrentContainer;
            if (container == null)
                return;

            var t = Matrix3x2.CreateTranslation((float)_state.PanX, (float)_state.PanY);
            var s = Matrix3x2.CreateScale((float)_state.Zoom);
            var old = ds.Transform;
            ds.Transform = s * t;

            if (container.Template != null)
            {
                DrawBackground(
                    ds,
                    container.Template.Background,
                    container.Template.Width,
                    container.Template.Height);

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

            ds.Transform = old;
        }

        private void InitializePage()
        {
            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Loaded += (sender, e) => canvas.Focus(FocusState.Programmatic);
            Unloaded += (sender, e) => _context.Dispose();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
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
                        ResetZoom();
                        _context.Editor.Invalidate();
                        break;
                    case VirtualKey.X:
                        AutoFit();
                        _context.Editor.Invalidate();
                        break;
                    case VirtualKey.Delete:
                        _context.Commands.DeleteCommand.Execute(null);
                        break;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        private void OnNew()
        {
            _context.Commands.NewCommand.Execute(null);
            _context.Editor.Invalidate();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task OnOpen()
        {
            var file = await GetOpenProjectPathAsync();
            if (file != null)
            {
                var project = default(Core2D.Project);
                using (var stream = await file.OpenStreamForReadAsync())
                {
                    project = await Task.Run(() =>
                    {
                        return Core2D.Project.Open(stream, _context.Serializer);
                    });
                }

                _context.Editor.History.Reset();
                _context.Editor.Unload();
                _context.Editor.Load(project);

                await CacheImages(project);

                _context.Editor.Invalidate();
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        private async Task OnSaveAs()
        {
            var file = await GetSaveProjectPathAsync(_context.Editor.Project.Name);
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);

                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    await Task.Run(() =>
                    {
                        Core2D.Project.Save(_context.Editor.Project, stream, _context.Serializer);
                    });
                }

                await CachedFileManager.CompleteUpdatesAsync(file);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        private async Task CacheImage(string key)
        {
            var bytes = _context.Renderers[0].State.ImageCache.GetImage(key);
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="project"></param>
        /// <returns></returns>
        private async Task CacheImages(Core2D.Project project)
        {
            var images = Core2D.Editor.GetAllShapes<Core2D.XImage>(project);
            if (images != null)
            {
                foreach (var image in images)
                {
                    await CacheImage(image.Key);
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        private async Task<string> GetImageKey(IStorageFile file)
        {
            var key = default(string);

            using (var fileStream = await file.OpenStreamForReadAsync())
            {
                var bytes = Core2D.Project.ReadBinary(fileStream);
                key = _context.Editor.Project.AddImageFromFile(file.Path, bytes);
            }

            return key;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ContainersListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _context.Editor.Invalidate();
        }
    }
}
