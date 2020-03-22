using Autofac;
using Core2D.Data;
using Core2D.FileSystem.DotNet;
using Core2D.FileWriter.Dxf;
using Core2D.FileWriter.Emf;
using Core2D.FileWriter.PdfSharp;
using Core2D.FileWriter.SkiaSharpJpeg;
using Core2D.FileWriter.SkiaSharpPdf;
using Core2D.FileWriter.SkiaSharpPng;
using Core2D.FileWriter.SkiaSharpSvg;
using Core2D.FileWriter.SkiaSharpWebp;
using Core2D.Interfaces;
using Core2D.Log.Trace;
using Core2D.Renderer;
using Core2D.Renderer.Avalonia;
#if !_CORERT
using Core2D.ScriptRunner.Roslyn;
#endif
using Core2D.Serializer.Newtonsoft;
using Core2D.Serializer.Xaml;
using Core2D.TextFieldReader.CsvHelper;
using Core2D.TextFieldWriter.CsvHelper;
using Core2D.UI.Avalonia.Utilities;

namespace Core2D.UI.Avalonia.Modules
{
    /// <summary>
    /// Dependencies components module.
    /// </summary>
    public class DependenciesModule : Module
    {
        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder)
        {
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
            builder.RegisterType<CsvHelperReader>().As<ITextFieldReader<IDatabase>>().InstancePerLifetimeScope();
            builder.RegisterType<CsvHelperWriter>().As<ITextFieldWriter<IDatabase>>().InstancePerLifetimeScope();
        }
    }
}
