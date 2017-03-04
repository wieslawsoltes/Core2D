// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using Autofac;
using Core2D.Data.Database;
using Core2D.Editor;
using Core2D.Editor.Factories;
using Core2D.Editor.Tools;
using Core2D.Editor.Views.Interfaces;
using Core2D.Interfaces;
using Core2D.Project;
using Core2D.Renderer;
using Core2D.Renderer.Presenters;
using Core2D.Shapes;
using FileSystem.Uwp;
using FileWriter.PdfSkiaSharp;
using FileWriter.SvgSkiaSharp;
using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.Graphics.Canvas.UI.Xaml;
using Renderer.Win2D;
using Serializer.Newtonsoft;
using Serializer.Xaml;
using TextFieldReader.CsvHelper;
using TextFieldWriter.CsvHelper;
using Utilities.Uwp;
using Windows.Devices.Input;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.System;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace Core2D.Uwp
{
    class ServiceProvider : IServiceProvider
    {
        private readonly ILifetimeScope _scope;

        public ServiceProvider(ILifetimeScope scope)
        {
            _scope = scope;
        }

        object IServiceProvider.GetService(Type serviceType)
        {
            return _scope.Resolve(serviceType);
        }
    }

    class UwpImageImporter : IImageImporter
    {
        private readonly IServiceProvider _serviceProvider;

        public UwpImageImporter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<string> GetImageKeyAsync()
        {
            return await Task.Run<string>(() => string.Empty);
        }
    }

    class UwpModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Locator
            builder.RegisterType<ServiceProvider>().As<IServiceProvider>().InstancePerLifetimeScope();
            // Core
            builder.RegisterType<ProjectEditor>().As<ProjectEditor>().InstancePerLifetimeScope();
            builder.RegisterType<ProjectFactory>().As<IProjectFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ShapeFactory>().As<IShapeFactory>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(ToolBase).GetTypeInfo().Assembly).As<ToolBase>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(PathToolBase).GetTypeInfo().Assembly).As<PathToolBase>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ProjectEditorCommands>().As<ProjectEditorCommands>().InstancePerLifetimeScope();
            builder.RegisterType<ProjectEditorCommands>().AutoActivate().PropertiesAutowired().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(ProjectEditorCommands).GetTypeInfo().Assembly).AssignableTo<ICommand>().AsImplementedInterfaces().AsSelf().PropertiesAutowired().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(IView).GetTypeInfo().Assembly).As<IView>().InstancePerLifetimeScope();
            // Dependencies
            builder.RegisterType<UwpFileSystem>().As<IFileSystem>().InstancePerLifetimeScope();
            builder.RegisterType<NewtonsoftJsonSerializer>().As<IJsonSerializer>().InstancePerLifetimeScope();
            builder.RegisterType<PortableXamlSerializer>().As<IXamlSerializer>().InstancePerLifetimeScope();
            builder.RegisterType<PdfSkiaSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<SvgSkiaSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<CsvHelperReader>().As<ITextFieldReader<XDatabase>>().InstancePerLifetimeScope();
            builder.RegisterType<CsvHelperWriter>().As<ITextFieldWriter<XDatabase>>().InstancePerLifetimeScope();
            builder.Register<ShapeRenderer>((c) => new Win2dRenderer()).InstancePerDependency();
            builder.RegisterType<UwpTextClipboard>().As<ITextClipboard>().InstancePerLifetimeScope();
            // App
            builder.RegisterType<UwpImageImporter>().As<IImageImporter>().InstancePerLifetimeScope();
        }
    }

    enum PointerPressType
    {
        None,
        Left,
        Middle,
        Right,
        Pen,
        Touch
    }

    public sealed partial class MainPage : Page
    {
        private IContainer _componentContainer;
        private IServiceProvider _serviceProvider;
        private ContainerPresenter _presenter;
        private ProjectEditor _projectEditor;
        private PointerPressType _pressed;
        private string _imagePath;

        public MainPage()
        {
            InitializeComponent();

            var builder = new ContainerBuilder();
            builder.RegisterAssemblyModules(typeof(MainPage).GetTypeInfo().Assembly);

            // View
            builder.RegisterAssemblyTypes(typeof(App).GetTypeInfo().Assembly).AssignableTo<ICommand>().AsImplementedInterfaces().AsSelf().PropertiesAutowired().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(App).GetTypeInfo().Assembly).As<IView>().InstancePerLifetimeScope();
            builder.Register(c => this).As<MainPage>().InstancePerLifetimeScope();

            _componentContainer = builder.Build();

            _serviceProvider = _componentContainer.Resolve<IServiceProvider>();
            _projectEditor = _serviceProvider.GetService<ProjectEditor>();

            DataContext = _projectEditor;

            Window.Current.CoreWindow.KeyDown += CoreWindow_KeyDown;
            Loaded += (sender, e) => canvas.Focus(FocusState.Programmatic);
            Unloaded += (sender, e) =>
            {
                _projectEditor.Log?.Dispose();
                _componentContainer?.Dispose();
            };

            _projectEditor.CurrentTool = _projectEditor.Tools.FirstOrDefault(t => t.Name == "Selection");
            _projectEditor.CurrentPathTool = _projectEditor.PathTools.FirstOrDefault(t => t.Name == "Line");
            _projectEditor.OnNewProject();
            _projectEditor.Invalidate = () => canvas.Invalidate();

            _presenter = new EditorPresenter();

            canvas.Draw += CanvasControl_Draw;
            canvas.PointerPressed += CanvasControl_PointerPressed;
            canvas.PointerReleased += CanvasControl_PointerReleased;
            canvas.PointerMoved += CanvasControl_PointerMoved;
            canvas.PointerWheelChanged += CanvasControl_PointerWheelChanged;
            canvas.SizeChanged += Canvas_SizeChanged;
        }

        public Point FixPointOffset(Point point)
        {
            var container = _projectEditor.Project.CurrentContainer;
            double offsetX = (this.canvas.ActualWidth * this.canvas.DpiScale - container.Width) / 2.0;
            double offsetY = (this.canvas.ActualHeight * this.canvas.DpiScale - container.Height) / 2.0;
            return new Point(point.X - offsetX, point.Y - offsetY);
        }

        private void CanvasControl_Draw(CanvasControl sender, CanvasDrawEventArgs args)
        {
            Draw(args.DrawingSession);
        }

        private async void CanvasControl_PointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var p = e.GetCurrentPoint(sender as CanvasControl);
            var pos = FixPointOffset(p.Position);
            var type = p.PointerDevice.PointerDeviceType;
            switch (type)
            {
                case PointerDeviceType.Mouse:
                    {
                        if (p.Properties.IsLeftButtonPressed)
                        {
                            if (_projectEditor.IsLeftDownAvailable())
                            {
                                if (_projectEditor.CurrentTool.GetType() == typeof(ToolImage) && _imagePath == null)
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

                                _projectEditor.LeftDown(pos.X, pos.Y);
                                _pressed = PointerPressType.Left;
                            }
                        }
                        //else if (p.Properties.IsMiddleButtonPressed)
                        //{
                        //    _pressed = PointerPressType.Middle;
                        //}
                        else if (p.Properties.IsRightButtonPressed)
                        {
                            if (_projectEditor.IsRightDownAvailable())
                            {
                                _projectEditor.RightDown(pos.X, pos.Y);
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
                                    if (_projectEditor.IsLeftUpAvailable())
                                    {
                                        _projectEditor.LeftUp(pos.X, pos.Y);
                                    }
                                }
                                break;
                            case PointerPressType.Middle:
                                break;
                            case PointerPressType.Right:
                                {
                                    if (_projectEditor.IsRightUpAvailable())
                                    {
                                        _projectEditor.RightUp(pos.X, pos.Y);
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
            var pos = FixPointOffset(p.Position);

            if (_projectEditor.IsMoveAvailable())
            {
                _projectEditor.Move(pos.X, pos.Y);
            }
        }

        private void CanvasControl_PointerWheelChanged(object sender, PointerRoutedEventArgs e)
        {
        }

        private void Canvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
        }

        private void Draw(CanvasDrawingSession ds)
        {
            ds.Antialiasing = CanvasAntialiasing.Aliased;
            ds.TextAntialiasing = CanvasTextAntialiasing.Auto;
            ds.Clear(Windows.UI.Colors.White);

            var renderer = _projectEditor.Renderers[0];
            var container = _projectEditor.Project.CurrentContainer;
            if (container != null)
            {
                var t = Matrix3x2.CreateTranslation(0.0f, 0.0f);
                var s = Matrix3x2.CreateScale(1.0f);
                var old = ds.Transform;
                ds.Transform = s * t;

                double offsetX = (this.canvas.ActualWidth * ds.Transform.M11 - container?.Width ?? 0) / 2.0;
                double offsetY = (this.canvas.ActualHeight * ds.Transform.M22 - container?.Height ?? 0) / 2.0;
                _presenter?.Render(ds, _projectEditor.Renderers[0], container, offsetX, offsetY);

                ds.Transform = old;
            }
        }

        private async void CoreWindow_KeyDown(CoreWindow sender, KeyEventArgs args)
        {
            var state = Window.Current.CoreWindow.GetKeyState(VirtualKey.Control);
            bool isControl = state.HasFlag(CoreVirtualKeyStates.Down);
            if (isControl)
            {
                switch (args.VirtualKey)
                {
                    case VirtualKey.N:
                        NewProject();
                        break;
                    case VirtualKey.O:
                        await OpenProject();
                        break;
                    case VirtualKey.S:
                        await SaveProject();
                        break;
                    case VirtualKey.Z:
                        _projectEditor.OnUndo();
                        break;
                    case VirtualKey.Y:
                        _projectEditor.OnRedo();
                        break;
                    case VirtualKey.X:
                        _projectEditor.OnCut();
                        break;
                    case VirtualKey.C:
                        _projectEditor.OnCopy();
                        break;
                    case VirtualKey.V:
                        _projectEditor.OnPaste();
                        break;
                    case VirtualKey.A:
                        _projectEditor.OnSelectAll();
                        break;
                    case VirtualKey.G:
                        _projectEditor.OnGroupSelected();
                        break;
                    case VirtualKey.U:
                        _projectEditor.OnUngroupSelected();
                        break;
                }
            }
            else
            {
                switch (args.VirtualKey)
                {
                    case VirtualKey.N:
                        _projectEditor.OnToolNone();
                        break;
                    case VirtualKey.S:
                        _projectEditor.OnToolSelection();
                        break;
                    case VirtualKey.P:
                        _projectEditor.OnToolPoint();
                        break;
                    case VirtualKey.L:
                        _projectEditor.OnToolLine();
                        break;
                    case VirtualKey.A:
                        _projectEditor.OnToolArc();
                        break;
                    case VirtualKey.B:
                        _projectEditor.OnToolCubicBezier();
                        break;
                    case VirtualKey.Q:
                        _projectEditor.OnToolQuadraticBezier();
                        break;
                    case VirtualKey.H:
                        _projectEditor.OnToolPath();
                        break;
                    case VirtualKey.M:
                        _projectEditor.OnToolMove();
                        break;
                    case VirtualKey.R:
                        _projectEditor.OnToolRectangle();
                        break;
                    case VirtualKey.E:
                        _projectEditor.OnToolEllipse();
                        break;
                    case VirtualKey.T:
                        _projectEditor.OnToolText();
                        break;
                    case VirtualKey.I:
                        _projectEditor.OnToolImage();
                        break;
                    case VirtualKey.K:
                        _projectEditor.OnToggleDefaultIsStroked();
                        break;
                    case VirtualKey.F:
                        _projectEditor.OnToggleDefaultIsFilled();
                        break;
                    case VirtualKey.D:
                        _projectEditor.OnToggleDefaultIsClosed();
                        break;
                    case VirtualKey.J:
                        _projectEditor.OnToggleDefaultIsSmoothJoin();
                        break;
                    case VirtualKey.G:
                        _projectEditor.OnToggleSnapToGrid();
                        break;
                    case VirtualKey.C:
                        _projectEditor.OnToggleTryToConnect();
                        break;
                    case VirtualKey.Y:
                        _projectEditor.OnToggleCloneStyle();
                        break;
                    case VirtualKey.Delete:
                        _projectEditor.OnDeleteSelected();
                        break;
                    case VirtualKey.Escape:
                        _projectEditor.OnDeselectAll();
                        break;
                }
            }
        }

        private async Task CacheImage(string key)
        {
            var bytes = _projectEditor.Renderers[0].State.ImageCache.GetImage(key);
            if (bytes != null)
            {
                using (var ms = new MemoryStream(bytes))
                {
                    using (var ras = ms.AsRandomAccessStream())
                    {
                        var bi = await CanvasBitmap.LoadAsync(canvas, ras);
                        (_projectEditor.Renderers[0] as Win2dRenderer).CacheImage(key, bi);
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
                var bytes = _projectEditor.FileIO.ReadBinary(fileStream);
                key = _projectEditor.Project.AddImageFromFile(file.Path, bytes);
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
            _projectEditor.Invalidate();
        }

        private void NewProject()
        {
            _projectEditor.OnNewProject();
            _projectEditor.Invalidate();
        }

        public async Task OpenProject()
        {
            var file = await GetOpenProjectPathAsync();
            if (file != null)
            {
                var project = default(XProject);
                using (var stream = await file.OpenStreamForReadAsync())
                {
                    project = await Task.Run(() =>
                    {
                        return XProject.Open(stream, _projectEditor.FileIO, _projectEditor.JsonSerializer);
                    });
                }

                _projectEditor.OnOpen(project, file.Path);
                await CacheImages(project);
                _projectEditor.Invalidate();
            }

            await Task.Run(() => { });
        }

        public async Task SaveProject()
        {
            var file = await GetSaveProjectPathAsync(_projectEditor.Project.Name);
            if (file != null)
            {
                CachedFileManager.DeferUpdates(file);

                using (var stream = await file.OpenStreamForWriteAsync())
                {
                    await Task.Run(() =>
                    {
                        XProject.Save(_projectEditor.Project, stream, _projectEditor.FileIO, _projectEditor.JsonSerializer);
                    });
                }

                await CachedFileManager.CompleteUpdatesAsync(file);
            }

            await Task.Run(() => { });
        }
    }
}
