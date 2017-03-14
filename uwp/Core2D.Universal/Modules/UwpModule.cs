// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Reflection;
using System.Windows.Input;
using Autofac;
using Core2D.Data.Database;
using Core2D.Editor;
using Core2D.Editor.Bounds;
using Core2D.Editor.Factories;
using Core2D.Editor.Views.Interfaces;
using Core2D.Interfaces;
using Core2D.Renderer;
using Core2D.Universal.Importers;
using Core2D.Universal.Locator;
using FileSystem.DotNet;
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
using Log.Trace;
using Renderer.Win2D;
//using ScriptRunner.Roslyn;
using Serializer.Newtonsoft;
using Serializer.Xaml;
using TextFieldReader.CsvHelper;
using TextFieldWriter.CsvHelper;
using Utilities.Uwp;

namespace Core2D.Universal.Modules
{
    public class UwpModule : Autofac.Module
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
            builder.RegisterType<HitTest>().As<HitTest>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(HitTestBase).GetTypeInfo().Assembly).As<HitTestBase>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ProjectEditorCommands>().As<ProjectEditorCommands>().InstancePerLifetimeScope();
            builder.RegisterType<ProjectEditorCommands>().AutoActivate().PropertiesAutowired().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(ProjectEditorCommands).GetTypeInfo().Assembly).AssignableTo<ICommand>().AsImplementedInterfaces().AsSelf().PropertiesAutowired().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(IView).GetTypeInfo().Assembly).As<IView>().InstancePerLifetimeScope();
            // Dependencies
            builder.RegisterType<TraceLog>().As<ILog>().SingleInstance();
            builder.RegisterType<DotNetFileSystem>().As<IFileSystem>().InstancePerLifetimeScope();
            //builder.RegisterType<RoslynScriptRunner>().As<IScriptRunner>().InstancePerLifetimeScope();
            builder.RegisterType<NewtonsoftJsonSerializer>().As<IJsonSerializer>().InstancePerLifetimeScope();
            builder.RegisterType<PortableXamlSerializer>().As<IXamlSerializer>().InstancePerLifetimeScope();
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
            builder.RegisterType<CsvHelperReader>().As<ITextFieldReader<XDatabase>>().InstancePerLifetimeScope();
            builder.RegisterType<CsvHelperWriter>().As<ITextFieldWriter<XDatabase>>().InstancePerLifetimeScope();
            builder.Register<ShapeRenderer>((c) => new Win2dRenderer()).InstancePerDependency();
            builder.RegisterType<UwpTextClipboard>().As<ITextClipboard>().InstancePerLifetimeScope();
            // App
            builder.RegisterType<UwpImageImporter>().As<IImageImporter>().InstancePerLifetimeScope();
        }
    }
}
