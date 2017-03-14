// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows.Input;
using Autofac;
using Core2D.Data.Database;
using Core2D.Editor;
using Core2D.Editor.Bounds;
using Core2D.Editor.Factories;
using Core2D.Editor.Views.Interfaces;
using Core2D.Interfaces;
using Core2D.Renderer;
using Core2D.SkiaDemo.Importers;
using Core2D.SkiaDemo.Locator;
using FileSystem.DotNet;
using FileWriter.Dxf;
using FileWriter.Emf;
using FileWriter.PdfSharp;
using FileWriter.SkiaSharpBmp;
using FileWriter.SkiaSharpGif;
using FileWriter.SkiaSharpIco;
using FileWriter.SkiaSharpJpeg;
using FileWriter.SkiaSharpKtx;
using FileWriter.SkiaSharpPdf;
using FileWriter.SkiaSharpPng;
using FileWriter.SkiaSharpSvg;
using FileWriter.SkiaSharpWbmp;
using FileWriter.SkiaSharpWebp;
using Renderer.SkiaSharp;
using Serializer.Newtonsoft;
using Serializer.Xaml;
using TextFieldReader.CsvHelper;
using TextFieldWriter.CsvHelper;
using Utilities.Wpf;

namespace Core2D.SkiaDemo.Modules
{
    public class WpfSkiaModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Locator
            builder.RegisterType<ServiceProvider>().As<IServiceProvider>().InstancePerLifetimeScope();
            // Core
            builder.RegisterType<ProjectEditor>().As<ProjectEditor>().InstancePerLifetimeScope();
            builder.RegisterType<ProjectFactory>().As<IProjectFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ShapeFactory>().As<IShapeFactory>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(ToolBase).Assembly).As<ToolBase>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(PathToolBase).Assembly).As<PathToolBase>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<HitTest>().As<HitTest>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(HitTestBase).Assembly).As<HitTestBase>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ProjectEditorCommands>().AutoActivate().PropertiesAutowired().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(ProjectEditorCommands).Assembly).AssignableTo<ICommand>().AsImplementedInterfaces().AsSelf().PropertiesAutowired().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(IView).Assembly).As<IView>().InstancePerLifetimeScope();
            // Dependencies
            builder.RegisterType<DotNetFileSystem>().As<IFileSystem>().InstancePerLifetimeScope();
            builder.RegisterType<NewtonsoftJsonSerializer>().As<IJsonSerializer>().InstancePerLifetimeScope();
            builder.RegisterType<PortableXamlSerializer>().As<IXamlSerializer>().InstancePerLifetimeScope();
            builder.RegisterType<PdfSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<BmpSkiaSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<GifSkiaSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<IcoSkiaSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<JpegSkiaSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<KtxSkiaSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<PdfSkiaSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<PngSkiaSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<SvgSkiaSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<WbmpSkiaSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<WebpSkiaSharpWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<EmfWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<DxfWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<CsvHelperReader>().As<ITextFieldReader<XDatabase>>().InstancePerLifetimeScope();
            builder.RegisterType<CsvHelperWriter>().As<ITextFieldWriter<XDatabase>>().InstancePerLifetimeScope();
            builder.Register<ShapeRenderer>((c) => new SkiaSharpRenderer(true, 96.0)).InstancePerDependency();
            builder.RegisterType<WpfTextClipboard>().As<ITextClipboard>().InstancePerLifetimeScope();
            // App
            builder.RegisterType<Win32ImageImporter>().As<IImageImporter>().InstancePerLifetimeScope();
            // View
            builder.RegisterAssemblyTypes(typeof(App).Assembly).AssignableTo<ICommand>().AsImplementedInterfaces().AsSelf().PropertiesAutowired().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(App).Assembly).As<IView>().InstancePerLifetimeScope();
            builder.RegisterType<MainWindow>().As<MainWindow>().InstancePerLifetimeScope();
        }
    }
}
