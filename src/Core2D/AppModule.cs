// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT. See LICENSE.TXT file in the project root for details.

#nullable enable
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
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
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Editor.Bounds;
using Core2D.ViewModels.Editor.Factories;
using Core2D.ViewModels.Export;
using Core2D.ViewModels.Wizard.Export;
using Core2D.ViewModels.Wizard.Export.Execution;
using Core2D.ViewModels.Wizard.Export.Steps;
using Core2D.Views;
using Microsoft.Extensions.DependencyInjection;

namespace Core2D;

public static class AppModule
{
    private static readonly string[] s_viewModelNamespaces =
    {
        "Core2D.ViewModels.Containers",
        "Core2D.ViewModels.Data",
        "Core2D.ViewModels.Path",
        "Core2D.ViewModels.Scripting",
        "Core2D.ViewModels.Shapes",
        "Core2D.ViewModels.Style"
    };

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

    [UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Service registration intentionally scans assemblies at runtime.")]
    [UnconditionalSuppressMessage("Trimming", "IL2066", Justification = "View model registration relies on reflection.")]
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
        var assembly = typeof(ViewModelBase).GetTypeInfo().Assembly;
        foreach (var type in assembly.GetTypes())
        {
            if (IsEligibleViewModel(type))
            {
                services.AddTransient(type);
            }
        }
    }

    private static bool IsEligibleViewModel(Type type)
    {
        if (!type.IsClass || type.IsAbstract || !type.IsPublic)
        {
            return false;
        }

        if (!type.Name.EndsWith("ViewModel", StringComparison.Ordinal))
        {
            return false;
        }

        var ns = type.Namespace;
        if (ns is null)
        {
            return false;
        }

        foreach (var prefix in s_viewModelNamespaces)
        {
            if (ns.StartsWith(prefix, StringComparison.Ordinal))
            {
                return true;
            }
        }

        return false;
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

        RegisterAssemblyTypes(
            services,
            typeof(IEditorTool).GetTypeInfo().Assembly,
            type => type.Namespace is not null && type.Namespace.StartsWith("Core2D.ViewModels.Editor.Tools", StringComparison.Ordinal),
            ServiceLifetime.Singleton,
            includeSelf: true,
            typeof(IEditorTool));

        RegisterAssemblyTypes(
            services,
            typeof(IPathTool).GetTypeInfo().Assembly,
            type => type.Namespace is not null && type.Namespace.StartsWith("Core2D.ViewModels.Editor.Tools.Path", StringComparison.Ordinal),
            ServiceLifetime.Singleton,
            includeSelf: true,
            typeof(IPathTool));

        services.AddSingleton<IHitTest, HitTest>();

        RegisterAssemblyTypes(
            services,
            typeof(IBounds).GetTypeInfo().Assembly,
            type => type.Namespace is not null && type.Namespace.StartsWith("Core2D.ViewModels.Editor.Bounds.Shapes", StringComparison.Ordinal),
            ServiceLifetime.Singleton,
            includeSelf: true,
            typeof(IBounds));

        services.AddSingleton<DataFlow>();
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
        RegisterWizardStep<ScopeWizardStepViewModel>(services);
        RegisterWizardStep<ExporterWizardStepViewModel>(services);
        RegisterWizardStep<SettingsWizardStepViewModel>(services);
        RegisterWizardStep<DestinationWizardStepViewModel>(services);
        RegisterWizardStep<ExecutionWizardStepViewModel>(services);
        RegisterWizardStep<SummaryWizardStepViewModel>(services);
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

    private static void RegisterWizardStep<TStep>(IServiceCollection services)
        where TStep : class, IWizardStepViewModel
    {
        services.AddTransient<TStep>();
        services.AddTransient<IWizardStepViewModel, TStep>();
    }

    private static void RegisterAssemblyTypes(
        IServiceCollection services,
        Assembly assembly,
        Func<Type, bool> predicate,
        ServiceLifetime lifetime,
        bool includeSelf,
        params Type[] serviceTypes)
    {
        foreach (var type in assembly.GetTypes())
        {
            if (!type.IsClass || type.IsAbstract || !type.IsPublic || !predicate(type))
            {
                continue;
            }

            RegisterType(services, type, lifetime, includeSelf, serviceTypes);
        }
    }

    private static void RegisterType(
        IServiceCollection services,
        Type implementationType,
        ServiceLifetime lifetime,
        bool includeSelf,
        IReadOnlyCollection<Type> serviceTypes)
    {
        var registered = false;
        foreach (var serviceType in serviceTypes)
        {
            if (serviceType.IsAssignableFrom(implementationType))
            {
                services.Add(new ServiceDescriptor(serviceType, implementationType, lifetime));
                registered = true;
            }
        }

        if (includeSelf)
        {
            services.Add(new ServiceDescriptor(implementationType, implementationType, lifetime));
        }

        if (!registered && serviceTypes.Count > 0)
        {
            // Keep behavior aligned with Autofac: skip interface registrations for incompatible types.
        }
    }
}
