// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Autofac;
using Core2D.Data.Database;
using Core2D.FileSystem.DotNet;
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
using Core2D.Log.Trace;
using Core2D.Renderer;
using Core2D.Renderer.Avalonia;
using Core2D.ScriptRunner.Roslyn;
using Core2D.Serializer.Newtonsoft;
using Core2D.Serializer.Xaml;
using Core2D.TextFieldReader.CsvHelper;
using Core2D.TextFieldWriter.CsvHelper;
using Core2D.Utilities.Avalonia;

namespace Core2D.Avalonia.Skia.Modules
{
    /// <summary>
    /// Dependencies components module.
    /// </summary>
    public class DependenciesModule : Module
    {
        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AvaloniaRenderer>().As<ShapeRenderer>().InstancePerDependency();
            builder.RegisterType<AvaloniaTextClipboard>().As<ITextClipboard>().InstancePerLifetimeScope();
            builder.RegisterType<TraceLog>().As<ILog>().SingleInstance();
            builder.RegisterType<DotNetFileSystem>().As<IFileSystem>().InstancePerLifetimeScope();
            builder.RegisterType<RoslynScriptRunner>().As<IScriptRunner>().InstancePerLifetimeScope();
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
            builder.RegisterType<CsvHelperReader>().As<ITextFieldReader<Database>>().InstancePerLifetimeScope();
            builder.RegisterType<CsvHelperWriter>().As<ITextFieldWriter<Database>>().InstancePerLifetimeScope();
        }
    }
}
