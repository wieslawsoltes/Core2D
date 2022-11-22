﻿#nullable enable
using System;
using System.Reflection;
using Autofac;
using Avalonia.Controls;
using Core2D.Editor;
using Core2D.Model;
using Core2D.Model.Editor;
using Core2D.Model.Renderer;
using Core2D.Modules.FileSystem.DotNet;
using Core2D.Modules.FileWriter.Dxf;
using Core2D.Modules.FileWriter.Emf;
using Core2D.Modules.FileWriter.SkiaSharp;
using Core2D.Modules.FileWriter.Svg;
using Core2D.Modules.FileWriter.Xaml;
using Core2D.Modules.Log.Trace;
using Core2D.Modules.Renderer.Avalonia;
using Core2D.Modules.Renderer.SkiaSharp;
using Core2D.Modules.ScriptRunner.Roslyn;
using Core2D.Modules.Serializer.Newtonsoft;
using Core2D.Modules.ServiceProvider.Autofac;
using Core2D.Modules.TextFieldReader.CsvHelper;
using Core2D.Modules.TextFieldReader.OpenXml;
using Core2D.Modules.TextFieldWriter.CsvHelper;
using Core2D.Modules.TextFieldWriter.OpenXml;
using Core2D.ViewModels;
using Core2D.ViewModels.Data;
using Core2D.ViewModels.Editor;
using Core2D.ViewModels.Editor.Bounds;
using Core2D.ViewModels.Editor.Factories;
using Core2D.Views;
using Core2D.Modules.FileWriter.PdfSharp;
using Core2D.Modules.SvgExporter.Svg;
using Core2D.Modules.XamlExporter.Avalonia;

namespace Core2D;

public class AppModule : Autofac.Module
{
    protected override void Load(ContainerBuilder builder)
    {
        // Container

        ILifetimeScope lifetimeScope = null!;
        builder.Register(_ => lifetimeScope).AsSelf().SingleInstance();
        builder.RegisterBuildCallback(x => lifetimeScope = x);

        // Locator

        builder.RegisterType<AutofacServiceProvider>().As<IServiceProvider>().InstancePerLifetimeScope();

        // ViewModels

        builder.RegisterAssemblyTypes(typeof(ViewModelBase).GetTypeInfo().Assembly)
            .PublicOnly()
            .Where(t =>
            {
                if (t.Namespace is null)
                {
                    return false;
                }
                if ((
                        t.Namespace.StartsWith("Core2D.ViewModels.Containers")
                        || t.Namespace.StartsWith("Core2D.ViewModels.Data")
                        || t.Namespace.StartsWith("Core2D.ViewModels.Path")
                        || t.Namespace.StartsWith("Core2D.ViewModels.Scripting")
                        || t.Namespace.StartsWith("Core2D.ViewModels.Shapes")
                        || t.Namespace.StartsWith("Core2D.ViewModels.Style")
                    )
                    && t.Name.EndsWith("ViewModel"))
                {
                    return true;
                }
                return false;
            })
            .AsSelf()
            .InstancePerDependency();

        // Editor

        builder.RegisterType<AboutInfoViewModel>().As<AboutInfoViewModel>().InstancePerLifetimeScope();
        builder.RegisterType<StyleEditorViewModel>().As<StyleEditorViewModel>().InstancePerLifetimeScope();
        builder.RegisterType<DialogViewModel>().As<DialogViewModel>().InstancePerDependency();

        builder.RegisterType<ShapeEditor>().As<IShapeEditor>().InstancePerLifetimeScope();
        builder.RegisterType<SelectionServiceViewModel>().As<ISelectionService>().InstancePerLifetimeScope();
        builder.RegisterType<ClipboardServiceViewModel>().As<IClipboardService>().InstancePerLifetimeScope();
        builder.RegisterType<ShapeServiceViewModel>().As<IShapeService>().InstancePerLifetimeScope();

        builder.RegisterType<ProjectEditorViewModel>().As<ProjectEditorViewModel>().InstancePerLifetimeScope();

        builder.RegisterType<ViewModelFactory>().As<IViewModelFactory>().InstancePerLifetimeScope();
        builder.RegisterType<ContainerFactory>().As<IContainerFactory>().InstancePerLifetimeScope();
        builder.RegisterType<ShapeFactory>().As<IShapeFactory>().InstancePerLifetimeScope();

        builder.RegisterAssemblyTypes(typeof(IEditorTool).GetTypeInfo().Assembly)
            .PublicOnly()
            .Where(t => t.Namespace is not null && t.Namespace.StartsWith("Core2D.ViewModels.Editor.Tools"))
            .As<IEditorTool>()
            .AsSelf()
            .InstancePerLifetimeScope();
            
        builder.RegisterAssemblyTypes(typeof(IPathTool).GetTypeInfo().Assembly)
            .PublicOnly()
            .Where(t => t.Namespace is not null && t.Namespace.StartsWith("Core2D.ViewModels.Editor.Tools.Path"))
            .As<IPathTool>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<HitTest>().As<IHitTest>().InstancePerLifetimeScope();
            
        builder.RegisterAssemblyTypes(typeof(IBounds).GetTypeInfo().Assembly)
            .PublicOnly()
            .Where(t => t.Namespace is not null && t.Namespace.StartsWith("Core2D.ViewModels.Editor.Bounds.Shapes"))
            .As<IBounds>()
            .AsSelf()
            .InstancePerLifetimeScope();

        builder.RegisterType<DataFlow>().As<DataFlow>().InstancePerLifetimeScope();

        // Dependencies

        builder.RegisterType<AvaloniaRendererViewModel>().As<IShapeRenderer>().InstancePerDependency();
        builder.RegisterType<AvaloniaTextClipboard>().As<ITextClipboard>().InstancePerLifetimeScope();
        builder.RegisterType<TraceLog>().As<ILog>().SingleInstance();
        builder.RegisterType<DotNetFileSystem>().As<IFileSystem>().InstancePerLifetimeScope();
        builder.RegisterType<RoslynScriptRunner>().As<IScriptRunner>().InstancePerLifetimeScope();
        builder.RegisterType<NewtonsoftJsonSerializer>().As<IJsonSerializer>().InstancePerLifetimeScope();
        builder.RegisterType<PdfSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
        builder.RegisterType<SvgSvgWriter>().As<IFileWriter>().InstancePerLifetimeScope();
        builder.RegisterType<DrawingGroupXamlWriter>().As<IFileWriter>().InstancePerLifetimeScope();
        builder.RegisterType<PdfSkiaSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
        builder.RegisterType<DxfWriter>().As<IFileWriter>().InstancePerLifetimeScope();
        builder.RegisterType<SvgSkiaSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
        builder.RegisterType<PngSkiaSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
        builder.RegisterType<SkpSkiaSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
        builder.RegisterType<EmfWriter>().As<IFileWriter>().InstancePerLifetimeScope();
        builder.RegisterType<EmfWriter>().As<IMetafileExporter>().InstancePerLifetimeScope();
        builder.RegisterType<JpegSkiaSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
        builder.RegisterType<WebpSkiaSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
        builder.RegisterType<OpenXmlReader>().As<ITextFieldReader<DatabaseViewModel>>().InstancePerLifetimeScope();
        builder.RegisterType<CsvHelperReader>().As<ITextFieldReader<DatabaseViewModel>>().InstancePerLifetimeScope();
        builder.RegisterType<OpenXmlWriter>().As<ITextFieldWriter<DatabaseViewModel>>().InstancePerLifetimeScope();
        builder.RegisterType<CsvHelperWriter>().As<ITextFieldWriter<DatabaseViewModel>>().InstancePerLifetimeScope();
        builder.RegisterType<SkiaSharpPathConverter>().As<IPathConverter>().InstancePerLifetimeScope();
        builder.RegisterType<SkiaSharpSvgConverter>().As<ISvgConverter>().InstancePerLifetimeScope();

        builder.RegisterType<DrawingGroupXamlExporter>().As<IXamlExporter>().InstancePerLifetimeScope();
        builder.RegisterType<SvgSvgExporter>().As<ISvgExporter>().InstancePerLifetimeScope();
        
        // Avalonia

        builder.RegisterType<AvaloniaImageImporter>().As<IImageImporter>().InstancePerLifetimeScope();
        builder.RegisterType<AvaloniaProjectEditorPlatform>().As<IProjectEditorPlatform>().InstancePerLifetimeScope();
        builder.RegisterType<AvaloniaEditorCanvasPlatform>().As<IEditorCanvasPlatform>().InstancePerLifetimeScope();

        // Views

        builder.RegisterType<MainWindow>().As<Window>().InstancePerLifetimeScope();
    }
}
