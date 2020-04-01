using Autofac;
using Core2D.Editor;
using Core2D.Interfaces;
using Core2D.ServiceProvider.Autofac;
using System;
using System.Reflection;
using Core2D.Data;
using Core2D.Editor.Bounds;
using Core2D.Editor.Factories;
using Core2D.FileSystem.DotNet;
using Core2D.FileWriter.Dxf;
using Core2D.FileWriter.Emf;
using Core2D.FileWriter.PdfSharp;
using Core2D.FileWriter.SkiaSharpJpeg;
using Core2D.FileWriter.SkiaSharpPdf;
using Core2D.FileWriter.SkiaSharpPng;
using Core2D.FileWriter.SkiaSharpSvg;
using Core2D.FileWriter.SkiaSharpWebp;
using Core2D.Log.Trace;
using Core2D.Renderer;
using Core2D.Renderer.Avalonia;
#if !_CORERT
using Core2D.ScriptRunner.Roslyn;
#endif
using Core2D.Serializer.Newtonsoft;
using Core2D.Serializer.Xaml;
using Core2D.TextFieldReader.CsvHelper;
using Core2D.TextFieldReader.OpenXml;
using Core2D.TextFieldWriter.CsvHelper;
using Core2D.TextFieldWriter.OpenXml;
using Core2D.UI.Avalonia.Utilities;
using Core2D.UI.Avalonia.Editor;
using Core2D.UI.Avalonia.Importers;
using Core2D.UI.Avalonia.Dock.Factories;
using Core2D.UI.Avalonia.Dock.Windows;
using Core2D.UI.Avalonia.Views;
using DM = Dock.Model;
using Core2D.Renderer.SkiaSharp;

namespace Core2D.UI.Avalonia.Modules
{
    /// <summary>
    /// Avalonia module.
    /// </summary>
    public class AvaloniaModule : Autofac.Module
    {
        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder)
        {
            // Locator

            builder.RegisterType<AutofacServiceProvider>().As<IServiceProvider>().InstancePerLifetimeScope();

            // Core

            builder.RegisterType<ProjectEditor>().As<IProjectEditor>().InstancePerLifetimeScope();
            builder.RegisterType<StyleEditor>().As<IStyleEditor>().InstancePerLifetimeScope();
            builder.RegisterType<Factory>().As<IFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ContainerFactory>().As<IContainerFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ShapeFactory>().As<IShapeFactory>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(IEditorTool).GetTypeInfo().Assembly).As<IEditorTool>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(IPathTool).GetTypeInfo().Assembly).As<IPathTool>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<HitTest>().As<IHitTest>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(IBounds).GetTypeInfo().Assembly).As<IBounds>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<DataFlow>().As<IDataFlow>().InstancePerLifetimeScope();

            // Dependencies

            builder.RegisterType<AvaloniaRenderer>().As<IShapeRenderer>().InstancePerDependency();
            builder.RegisterType<AvaloniaTextClipboard>().As<ITextClipboard>().InstancePerLifetimeScope();
            builder.RegisterType<TraceLog>().As<ILog>().SingleInstance();
            builder.RegisterType<DotNetFileSystem>().As<IFileSystem>().InstancePerLifetimeScope();
#if !_CORERT
            builder.RegisterType<RoslynScriptRunner>().As<IScriptRunner>().InstancePerLifetimeScope();
#endif
            builder.RegisterType<NewtonsoftJsonSerializer>().As<IJsonSerializer>().InstancePerLifetimeScope();
            builder.RegisterType<PortableXamlSerializer>().As<IXamlSerializer>().InstancePerLifetimeScope();
            builder.RegisterType<PdfSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<PdfSkiaSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<DxfWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<SvgSkiaSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<PngSkiaSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<EmfWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<JpegSkiaSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<WebpSkiaSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<OpenXmlReader>().As<ITextFieldReader<IDatabase>>().InstancePerLifetimeScope();
            builder.RegisterType<CsvHelperReader>().As<ITextFieldReader<IDatabase>>().InstancePerLifetimeScope();
            builder.RegisterType<OpenXmlWriter>().As<ITextFieldWriter<IDatabase>>().InstancePerLifetimeScope();
            builder.RegisterType<CsvHelperWriter>().As<ITextFieldWriter<IDatabase>>().InstancePerLifetimeScope();
            builder.RegisterType<SkiaSharpPathConverter>().As<IPathConverter>().InstancePerLifetimeScope();

            // App

            builder.RegisterType<AvaloniaImageImporter>().As<IImageImporter>().InstancePerLifetimeScope();
            builder.RegisterType<AvaloniaProjectEditorPlatform>().As<IProjectEditorPlatform>().InstancePerLifetimeScope();
            builder.RegisterType<AvaloniaEditorCanvasPlatform>().As<IEditorCanvasPlatform>().InstancePerLifetimeScope();
            builder.RegisterType<AvaloniaEditorLayoutPlatform>().As<IEditorLayoutPlatform>().InstancePerLifetimeScope();

            // View

            builder.RegisterAssemblyTypes(typeof(App).GetTypeInfo().Assembly).As<DM.IDock>().InstancePerLifetimeScope();
            builder.RegisterType<EditorDockFactory>().As<DM.IFactory>().InstancePerDependency();
            builder.Register(c => new ThemedHostWindow()).As<DM.IHostWindow>().InstancePerDependency();
            builder.RegisterType<MainWindow>().As<MainWindow>().InstancePerLifetimeScope();
        }
    }
}
