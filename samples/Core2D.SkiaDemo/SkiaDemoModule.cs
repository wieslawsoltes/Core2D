// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows.Input;
using Autofac;
using Core2D.Data;
using Core2D.Editor;
using Core2D.Editor.Bounds;
using Core2D.Editor.Factories;
using Core2D.Editor.Views.Interfaces;
using Core2D.FileSystem.DotNet;
using Core2D.FileWriter.Dxf;
using Core2D.FileWriter.Emf;
using Core2D.FileWriter.PdfSharp;
using Core2D.FileWriter.SkiaSharpBmp;
using Core2D.FileWriter.SkiaSharpGif;
using Core2D.FileWriter.SkiaSharpIco;
using Core2D.FileWriter.SkiaSharpJpeg;
using Core2D.FileWriter.SkiaSharpKtx;
using Core2D.FileWriter.SkiaSharpPdf;
using Core2D.FileWriter.SkiaSharpPng;
using Core2D.FileWriter.SkiaSharpSvg;
using Core2D.FileWriter.SkiaSharpWbmp;
using Core2D.FileWriter.SkiaSharpWebp;
using Core2D.Interfaces;
using Core2D.Renderer;
using Core2D.Renderer.Presenters;
using Core2D.Renderer.SkiaSharp;
using Core2D.Serializer.Newtonsoft;
using Core2D.Serializer.Xaml;
using Core2D.ServiceProvider.Autofac;
using Core2D.TextFieldReader.CsvHelper;
using Core2D.TextFieldWriter.CsvHelper;
using Core2D.Utilities.Wpf;

namespace Core2D.SkiaDemo
{
    public class SkiaDemoModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Locator
            builder.RegisterType<AutofacServiceProvider>().As<IServiceProvider>().InstancePerLifetimeScope();
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
            builder.RegisterType<CsvHelperReader>().As<ITextFieldReader<Database>>().InstancePerLifetimeScope();
            builder.RegisterType<CsvHelperWriter>().As<ITextFieldWriter<Database>>().InstancePerLifetimeScope();
            builder.Register<ShapeRenderer>((c) => new SkiaSharpRenderer(true, 96.0)).InstancePerDependency();
            builder.RegisterType<EditorPresenter>().As<ContainerPresenter>().InstancePerLifetimeScope();
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
