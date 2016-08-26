// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.Immutable;
using System.Linq;
using Autofac;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Core2D.Avalonia.Controls.Data;
using Core2D.Avalonia.Controls.Path;
using Core2D.Avalonia.Controls.Project;
using Core2D.Avalonia.Controls.Shapes;
using Core2D.Avalonia.Controls.State;
using Core2D.Avalonia.Controls.Style;
using Core2D.Avalonia.Converters;
using Core2D.Avalonia.Modules;
using Core2D.Avalonia.Presenters;
using Core2D.Avalonia.Views;
using Core2D.Data;
using Core2D.Data.Database;
using Core2D.Editor;
using Core2D.Editor.Designer;
using Core2D.Editor.Views;
using Core2D.Interfaces;
using Core2D.Path;
using Core2D.Path.Segments;
using Core2D.Project;
using Core2D.Shape;
using Core2D.Shapes;
using Core2D.Style;

namespace Core2D.Avalonia
{
    /// <summary>
    /// Encapsulates an Avalonia application.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Initializes static data.
        /// </summary>
        static App()
        {
            InitializePresenters();
            InitializeDesigner();
        }

        /// <summary>
        /// Initializes presenters.
        /// </summary>
        static void InitializePresenters()
        {
            CachedContentPresenter.Register(typeof(ProjectEditor), () => new Grid());
            CachedContentPresenter.Register(typeof(XLibrary<ShapeStyle>), () => new Grid());
            CachedContentPresenter.Register(typeof(XLibrary<XGroup>), () => new Grid());
            CachedContentPresenter.Register(typeof(ImmutableArray<XLibrary<ShapeStyle>>), () => new Grid());
            CachedContentPresenter.Register(typeof(ImmutableArray<XLibrary<XGroup>>), () => new Grid());
            CachedContentPresenter.Register(typeof(ImmutableArray<XDatabase>), () => new Grid());
            CachedContentPresenter.Register(typeof(ImmutableArray<XContainer>), () => new Grid());
            CachedContentPresenter.Register(typeof(ImmutableArray<XDocument>), () => new Grid());

            // Views
            CachedContentPresenter.Register(typeof(BrowserView), () => new BrowserViewControl());
            CachedContentPresenter.Register(typeof(DashboardView), () => new DashboardViewControl());
            CachedContentPresenter.Register(typeof(DocumentView), () => new DocumentViewControl());
            CachedContentPresenter.Register(typeof(EditorView), () => new EditorViewControl());

            // Project
            CachedContentPresenter.Register(typeof(XProject), () => new ProjectControl());
            CachedContentPresenter.Register(typeof(XOptions), () => new OptionsControl());
            CachedContentPresenter.Register(typeof(XDocument), () => new DocumentControl());
            CachedContentPresenter.Register(typeof(XContainer), () => new ContainerControl());
            CachedContentPresenter.Register(typeof(XLayer), () => new LayerControl());

            // Data
            CachedContentPresenter.Register(typeof(ImmutableArray<XColumn>), () => new ColumnsControl());
            CachedContentPresenter.Register(typeof(XDatabase), () => new DatabaseControl());
            CachedContentPresenter.Register(typeof(XContext), () => new DataControl());
            CachedContentPresenter.Register(typeof(ImmutableArray<XProperty>), () => new PropertiesControl());
            CachedContentPresenter.Register(typeof(XRecord), () => new RecordControl());
            CachedContentPresenter.Register(typeof(ImmutableArray<XRecord>), () => new RecordsControl());

            // Path
            CachedContentPresenter.Register(typeof(XArcSegment), () => new ArcSegmentControl());
            CachedContentPresenter.Register(typeof(XCubicBezierSegment), () => new CubicBezierSegmentControl());
            CachedContentPresenter.Register(typeof(XLineSegment), () => new LineSegmentControl());
            CachedContentPresenter.Register(typeof(XPathFigure), () => new PathFigureControl());
            CachedContentPresenter.Register(typeof(XPathGeometry), () => new PathGeometryControl());
            CachedContentPresenter.Register(typeof(XPathSize), () => new PathSizeControl());
            CachedContentPresenter.Register(typeof(XPolyCubicBezierSegment), () => new PolyCubicBezierSegmentControl());
            CachedContentPresenter.Register(typeof(XPolyLineSegment), () => new PolyLineSegmentControl());
            CachedContentPresenter.Register(typeof(XPolyQuadraticBezierSegment), () => new PolyQuadraticBezierSegmentControl());
            CachedContentPresenter.Register(typeof(XQuadraticBezierSegment), () => new QuadraticBezierSegmentControl());

            // Shapes
            CachedContentPresenter.Register(typeof(XArc), () => new ArcControl());
            CachedContentPresenter.Register(typeof(XCubicBezier), () => new CubicBezierControl());
            CachedContentPresenter.Register(typeof(XEllipse), () => new EllipseControl());
            CachedContentPresenter.Register(typeof(XGroup), () => new GroupControl());
            CachedContentPresenter.Register(typeof(XImage), () => new ImageControl());
            CachedContentPresenter.Register(typeof(XLine), () => new LineControl());
            CachedContentPresenter.Register(typeof(XPath), () => new PathControl());
            CachedContentPresenter.Register(typeof(XPoint), () => new PointControl());
            CachedContentPresenter.Register(typeof(XQuadraticBezier), () => new QuadraticBezierControl());
            CachedContentPresenter.Register(typeof(XRectangle), () => new RectangleControl());
            CachedContentPresenter.Register(typeof(XText), () => new TextControl());

            // State
            CachedContentPresenter.Register(typeof(ShapeState), () => new ShapeStateControl());

            // Style
            CachedContentPresenter.Register(typeof(ArgbColor), () => new ArgbColorControl());
            CachedContentPresenter.Register(typeof(ArrowStyle), () => new ArrowStyleControl());
            CachedContentPresenter.Register(typeof(FontStyle), () => new FontStyleControl());
            CachedContentPresenter.Register(typeof(LineFixedLength), () => new LineFixedLengthControl());
            CachedContentPresenter.Register(typeof(LineStyle), () => new LineStyleControl());
            CachedContentPresenter.Register(typeof(ShapeStyle), () => new ShapeStyleControl());
            CachedContentPresenter.Register(typeof(BaseStyle), () => new StyleControl());
            CachedContentPresenter.Register(typeof(TextStyle), () => new TextStyleControl());
        }

        /// <summary>
        /// Initializes designer.
        /// </summary>
        static void InitializeDesigner()
        {
            if (Design.IsDesignMode)
            {
                var builder = new ContainerBuilder();

                builder.RegisterModule<LocatorModule>();
                builder.RegisterModule<CoreModule>();
                builder.RegisterModule<DependenciesModule>();
                builder.RegisterModule<AppModule>();

                var container = builder.Build();

                DesignerContext.InitializeContext(container.Resolve<IServiceProvider>());
            }
        }

        /// <summary>
        /// Initializes converters.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        static void InitializeConverters(IServiceProvider serviceProvider)
        {
            ObjectToXamlStringConverter.XamlSerializer = serviceProvider.GetServiceLazily<IXamlSerializer>();
            ObjectToJsonStringConverter.JsonSerializer = serviceProvider.GetServiceLazily<IJsonSerializer>();
        }

        /// <inheritdoc/>
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        /// <summary>
        /// Initialize application context and displays main window.
        /// </summary>
        /// <param name="serviceProvider">The service provider.</param>
        public void Start(IServiceProvider serviceProvider)
        {
            InitializeConverters(serviceProvider);

            var log = serviceProvider.GetService<ILog>();
            var fileIO = serviceProvider.GetService<IFileSystem>();

            log?.Initialize(System.IO.Path.Combine(fileIO?.GetAssemblyPath(null), "Core2D.log"));

            try
            {
                var editor = serviceProvider.GetService<ProjectEditor>();

                var path = System.IO.Path.Combine(fileIO.GetAssemblyPath(null), "Core2D.recent");
                if (fileIO.Exists(path))
                {
                    editor.OnLoadRecent(path);
                }

                editor.CurrentView = editor.Views.FirstOrDefault(v => v.Name == "Dashboard");
                editor.CurrentTool = editor.Tools.FirstOrDefault(t => t.Name == "Selection");
                editor.CurrentPathTool = editor.PathTools.FirstOrDefault(t => t.Name == "Line");

                var window = serviceProvider.GetService<Windows.MainWindow>();

                window.Closed +=
                    (sender, e) =>
                    {
                        editor.OnSaveRecent(path);
                    };

                window.DataContext = editor;
                window.Show();
                Run(window);
            }
            catch (Exception ex)
            {
                log?.LogError($"{ex.Message}{Environment.NewLine}{ex.StackTrace}");
                if (ex.InnerException != null)
                {
                    log?.LogError($"{ex.InnerException.Message}{Environment.NewLine}{ex.InnerException.StackTrace}");
                }
            }
        }
    }
}
