// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using Avalonia.Controls;
using Core2D.Editor;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.Modules.FileSystem.DotNet;
using Core2D.Modules.FileWriter.Dwg;
using Core2D.Modules.FileWriter.Dxf;
using Core2D.Modules.FileWriter.Emf;
using Core2D.Modules.FileWriter.OpenXml;
using Core2D.Modules.FileWriter.PdfSharp;
using Core2D.Modules.FileWriter.SkiaSharp;
using Core2D.Modules.FileWriter.Svg;
using Core2D.Modules.FileWriter.Wmf;
using Core2D.Modules.FileWriter.Xaml;
using Core2D.Modules.Log.Trace;
using Core2D.Modules.Renderer.Avalonia;
using Core2D.Modules.Renderer.Dwg;
using Core2D.Modules.Renderer.OpenXml;
using Core2D.Modules.Renderer.PdfSharp;
using Core2D.Modules.Renderer.SkiaSharp;
using Core2D.Modules.Renderer.Wmf;
using Core2D.Modules.ScriptRunner.Roslyn;
using Core2D.Modules.Serializer.Newtonsoft;
using Core2D.Modules.SvgExporter.Svg;
using Core2D.Modules.TextFieldReader.CsvHelper;
using Core2D.Modules.TextFieldReader.OpenXml;
using Core2D.Modules.TextFieldWriter.CsvHelper;
using Core2D.Modules.TextFieldWriter.OpenXml;
using Core2D.Modules.XamlExporter.Avalonia;
using Core2D.Rendering;
using Core2D.Services;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Editor.Bounds;
using Core2D.ViewModels.Editor.Bounds.Shapes;
using Core2D.ViewModels.Editor.Tools;
using Core2D.ViewModels.Editor.Tools.Path;
using Core2D.ViewModels.Editor.Factories;
using Core2D.ViewModels.Export;
using Core2D.ViewModels.Path;
using Core2D.ViewModels.Path.Segments;
using Core2D.ViewModels.Scripting;
using Core2D.ViewModels.Shapes;
using Core2D.ViewModels.Style;
using Core2D.ViewModels.Wizard.Export;
using Core2D.ViewModels.Wizard.Export.Execution;
using Core2D.ViewModels.Wizard.Export.Steps;
using Core2D.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Core2D;

public static class AppModule
{
    public static ServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        ConfigureServices(services);
        return services.BuildServiceProvider(new ServiceProviderOptions
        {
            ValidateScopes = true,
            ValidateOnBuild = true
        });
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        RegisterViewModels(services);
        RegisterEditorServices(services);
        RegisterDependencies(services);
        RegisterAvaloniaServices(services);
        RegisterViews(services);
    }

    private static void RegisterViewModels(IServiceCollection services)
    {
        // Containers
        services.AddTransient(sp => new DocumentContainerViewModel(sp));
        services.AddTransient(sp => new LayerContainerViewModel(sp));
        services.AddTransient(sp => new LibraryViewModel(sp));
        services.AddTransient(sp => new OptionsViewModel(sp));
        services.AddTransient(sp => new PageContainerViewModel(sp));
        services.AddTransient(sp => new ProjectContainerViewModel(sp));
        services.AddTransient(sp => new TemplateContainerViewModel(sp));

        // Data
        services.AddTransient(sp => new ColumnViewModel(sp));
        services.AddTransient(sp => new DatabaseViewModel(sp));
        services.AddTransient(sp => new PropertyViewModel(sp));
        services.AddTransient(sp => new RecordViewModel(sp));
        services.AddTransient(sp => new ValueViewModel(sp));

        // Path
        services.AddTransient(sp => new PathFigureViewModel(sp));
        services.AddTransient(sp => new PathSizeViewModel(sp));
        services.AddTransient(sp => new ArcSegmentViewModel(sp));
        services.AddTransient(sp => new CubicBezierSegmentViewModel(sp));
        services.AddTransient(sp => new LineSegmentViewModel(sp));
        services.AddTransient(sp => new QuadraticBezierSegmentViewModel(sp));

        // Scripting
        services.AddTransient(sp => new ScriptViewModel(sp));

        // Shapes
        services.AddTransient(sp => new ArcShapeViewModel(sp));
        services.AddTransient(sp => new BlockShapeViewModel(sp));
        services.AddTransient(sp => new CubicBezierShapeViewModel(sp));
        services.AddTransient(sp => new EllipseShapeViewModel(sp));
        services.AddTransient(sp => new ImageShapeViewModel(sp));
        services.AddTransient(sp => new InsertShapeViewModel(sp));
        services.AddTransient(sp => new LineShapeViewModel(sp));
        services.AddTransient(sp => new PathShapeViewModel(sp));
        services.AddTransient(sp => new PointShapeViewModel(sp));
        services.AddTransient(sp => new QuadraticBezierShapeViewModel(sp));
        services.AddTransient(sp => new RectangleShapeViewModel(sp));
        services.AddTransient(sp => new TextShapeViewModel(sp));
        services.AddTransient(sp => new WireShapeViewModel(sp));

        // Style
        services.AddTransient(sp => new ArgbColorViewModel(sp));
        services.AddTransient(sp => new ArrowStyleViewModel(sp));
        services.AddTransient(sp => new FillStyleViewModel(sp));
        services.AddTransient(sp => new ShapeStyleViewModel(sp));
        services.AddTransient(sp => new StrokeStyleViewModel(sp));
        services.AddTransient(sp => new TextStyleViewModel(sp));
    }

    private static void RegisterEditorServices(IServiceCollection services)
    {
        services.AddSingleton<AboutInfoViewModel>();
        services.AddSingleton<StyleEditorViewModel>();
        services.AddTransient<DialogViewModel>();

        services.AddSingleton<IShapeEditor, ShapeEditor>();
        services.AddSingleton<ISelectionService, SelectionServiceViewModel>();
        services.AddSingleton<IClipboardService, ClipboardServiceViewModel>();
        services.AddSingleton<IShapeService, ShapeServiceViewModel>();
        services.AddSingleton<IGraphLayoutService, GraphLayoutServiceViewModel>();
        services.AddSingleton<IWaveFunctionCollapseService, WaveFunctionCollapseServiceViewModel>();
        services.AddSingleton<IRendererProvider, RendererProvider>();
        services.AddSingleton<IRendererSelectionService, RendererSelectionService>();

        services.AddSingleton<ProjectEditorViewModel>();
        services.AddSingleton<IDialogPresenter>(sp => sp.GetRequiredService<ProjectEditorViewModel>());

        services.AddSingleton<IViewModelFactory, ViewModelFactory>();
        services.AddSingleton<IContainerFactory, ContainerFactory>();
        services.AddSingleton<IShapeFactory, ShapeFactory>();

        RegisterEditorTools(services);
        RegisterPathTools(services);

        services.AddSingleton<IHitTest, HitTest>();

        RegisterBounds(services);

        services.AddSingleton<DataFlow>();
    }

    private static void RegisterEditorTools(IServiceCollection services)
    {
        RegisterEditorTool(services, sp => new NoneToolViewModel(sp));
        RegisterEditorTool(services, sp => new SelectionToolViewModel(sp));
        RegisterEditorTool(services, sp => new PointToolViewModel(sp));
        RegisterEditorTool(services, sp => new LineToolViewModel(sp));
        RegisterEditorTool(services, sp => new ArcToolViewModel(sp));
        RegisterEditorTool(services, sp => new CubicBezierToolViewModel(sp));
        RegisterEditorTool(services, sp => new QuadraticBezierToolViewModel(sp));
        RegisterEditorTool(services, sp => new PathToolViewModel(sp));
        RegisterEditorTool(services, sp => new RectangleToolViewModel(sp));
        RegisterEditorTool(services, sp => new EllipseToolViewModel(sp));
        RegisterEditorTool(services, sp => new TextToolViewModel(sp));
        RegisterEditorTool(services, sp => new ImageToolViewModel(sp));
        RegisterEditorTool(services, sp => new WireToolViewModel(sp));
    }

    private static void RegisterPathTools(IServiceCollection services)
    {
        RegisterPathTool(services, sp => new LinePathToolViewModel(sp));
        RegisterPathTool(services, sp => new ArcPathToolViewModel(sp));
        RegisterPathTool(services, sp => new CubicBezierPathToolViewModel(sp));
        RegisterPathTool(services, sp => new QuadraticBezierPathToolViewModel(sp));
        RegisterPathTool(services, sp => new MovePathToolViewModel(sp));
    }

    private static void RegisterBounds(IServiceCollection services)
    {
        RegisterBoundsType(services, _ => new ArcBounds());
        RegisterBoundsType(services, _ => new BlockBounds());
        RegisterBoundsType(services, _ => new CubicBezierBounds());
        RegisterBoundsType(services, _ => new EllipseBounds());
        RegisterBoundsType(services, _ => new ImageBounds());
        RegisterBoundsType(services, _ => new InsertBounds());
        RegisterBoundsType(services, _ => new LineBounds());
        RegisterBoundsType(services, _ => new PathBounds());
        RegisterBoundsType(services, _ => new PointBounds());
        RegisterBoundsType(services, _ => new QuadraticBezierBounds());
        RegisterBoundsType(services, _ => new RectangleBounds());
        RegisterBoundsType(services, _ => new TextBounds());
        RegisterBoundsType(services, _ => new WireBounds());
    }

    private static void RegisterDependencies(IServiceCollection services)
    {
        services.AddTransient<IShapeRenderer, AvaloniaRendererViewModel>();
        services.AddSingleton<ITextClipboard, AvaloniaTextClipboard>();
        services.AddSingleton<ILog, TraceLog>();
        services.AddSingleton<IFileSystem, DotNetFileSystem>();
        services.AddSingleton<IScriptRunner, RoslynScriptRunner>();
        services.AddSingleton<IJsonSerializer, NewtonsoftJsonSerializer>();

        services.AddSingleton<IFileWriter, PdfSharpWriter>();
        services.AddSingleton<IFileWriter, SvgSvgWriter>();
        services.AddSingleton<IFileWriter, DrawingGroupXamlWriter>();
        services.AddSingleton<IFileWriter, PdfSkiaSharpWriter>();
        services.AddSingleton<IFileWriter, DxfWriter>();
        services.AddSingleton<IFileWriter, DxfACadSharpWriter>();
        services.AddSingleton<IFileWriter, DwgWriter>();
        services.AddSingleton<IFileWriter, SvgSkiaSharpWriter>();
        services.AddSingleton<IFileWriter, PngSkiaSharpWriter>();
        services.AddSingleton<IFileWriter, SkpSkiaSharpWriter>();
        services.AddSingleton<IFileWriter, EmfWriter>();
        services.AddSingleton<IMetafileExporter, EmfWriter>();
        services.AddSingleton<IFileWriter, WmfWriter>();
        services.AddSingleton<IFileWriter, JpegSkiaSharpWriter>();
        services.AddSingleton<IFileWriter, WebpSkiaSharpWriter>();
        services.AddSingleton<IFileWriter, ExcelOpenXmlWriter>();
        services.AddSingleton<IFileWriter, WordOpenXmlWriter>();
        services.AddSingleton<IFileWriter, PowerPointOpenXmlWriter>();
        services.AddSingleton<IFileWriter, VisioOpenXmlWriter>();

        services.AddSingleton<ITextFieldReader<DatabaseViewModel>, OpenXmlReader>();
        services.AddSingleton<ITextFieldReader<DatabaseViewModel>, CsvHelperReader>();
        services.AddSingleton<ITextFieldWriter<DatabaseViewModel>, OpenXmlWriter>();
        services.AddSingleton<ITextFieldWriter<DatabaseViewModel>, CsvHelperWriter>();

        services.AddSingleton<IPathConverter, SkiaSharpPathConverter>();
        services.AddSingleton<ISvgConverter, SkiaSharpSvgConverter>();
        services.AddSingleton<IDwgImporter, DwgImporter>();
        services.AddSingleton<IPdfImporter, PdfImporter>();
        services.AddSingleton<IWmfImporter, WmfImporter>();
        services.AddSingleton<IVisioImporter, VisioImporter>();

        services.AddSingleton<IXamlExporter, DrawingGroupXamlExporter>();
        services.AddSingleton<ISvgExporter, SvgSvgExporter>();

        services.AddSingleton<IExportOptionsCatalog, ExportOptionsCatalog>();
        services.AddSingleton<IExportWizardTelemetry, ExportWizardTelemetry>();
        services.AddSingleton<ExportWizardContext>();
        services.AddSingleton<WizardNavigationService>();
        RegisterWizardStep(services, sp => new ScopeWizardStepViewModel(sp));
        RegisterWizardStep(services, sp => new ExporterWizardStepViewModel(
            sp,
            sp.GetRequiredService<IExportOptionsCatalog>()));
        RegisterWizardStep(services, sp => new SettingsWizardStepViewModel(sp));
        RegisterWizardStep(services, sp => new DestinationWizardStepViewModel(sp));
        RegisterWizardStep(services, sp => new ExecutionWizardStepViewModel(
            sp,
            sp.GetRequiredService<ExportJobRunner>()));
        RegisterWizardStep(services, sp => new SummaryWizardStepViewModel(sp));
        services.AddTransient<ExportWizardViewModel>();
        services.AddTransient<ExportJobRunner>();
    }

    private static void RegisterAvaloniaServices(IServiceCollection services)
    {
        services.AddSingleton<IImageImporter, AvaloniaImageImporter>();
        services.AddSingleton<IProjectEditorPlatform, AvaloniaProjectEditorPlatform>();
        services.AddSingleton<IEditorCanvasPlatform, AvaloniaEditorCanvasPlatform>();
    }

    private static void RegisterViews(IServiceCollection services)
    {
        services.AddSingleton<MainWindow>();
        services.AddSingleton<Window>(sp => sp.GetRequiredService<MainWindow>());
    }

    private static void RegisterWizardStep<TStep>(
        IServiceCollection services,
        Func<IServiceProvider, TStep> factory)
        where TStep : class, IWizardStepViewModel
    {
        services.AddTransient(factory);
        services.AddTransient<IWizardStepViewModel>(sp => sp.GetRequiredService<TStep>());
    }

    private static void RegisterEditorTool<TTool>(
        IServiceCollection services,
        Func<IServiceProvider, TTool> factory)
        where TTool : class, IEditorTool
    {
        services.AddSingleton(factory);
        services.AddSingleton<IEditorTool>(sp => factory(sp));
    }

    private static void RegisterPathTool<TTool>(
        IServiceCollection services,
        Func<IServiceProvider, TTool> factory)
        where TTool : class, IPathTool
    {
        services.AddSingleton(factory);
        services.AddSingleton<IPathTool>(sp => factory(sp));
    }

    private static void RegisterBoundsType<TBounds>(
        IServiceCollection services,
        Func<IServiceProvider, TBounds> factory)
        where TBounds : class, IBounds
    {
        services.AddSingleton(factory);
        services.AddSingleton<IBounds>(sp => factory(sp));
    }
}
