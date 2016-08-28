using Autofac;
using Core2D.Data.Database;
using Core2D.Interfaces;
using Core2D.Renderer;
using FileSystem.DotNetFx;
using FileWriter.PdfSkiaSharp;
using FileWriter.SvgSkiaSharp;
using Renderer.SkiaSharp;
using Serializer.Newtonsoft;
using Serializer.Xaml;
using TextFieldReader.CsvHelper;
using TextFieldWriter.CsvHelper;
using Utilities.Wpf;

namespace SkiaDemo.Modules
{
    class DependenciesModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<DotNetFxFileSystem>().As<IFileSystem>().InstancePerLifetimeScope();
            builder.RegisterType<NewtonsoftJsonSerializer>().As<IJsonSerializer>().InstancePerLifetimeScope();
            builder.RegisterType<PortableXamlSerializer>().As<IXamlSerializer>().InstancePerLifetimeScope();
            builder.RegisterType<PdfWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<SvgWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<CsvHelperReader>().As<ITextFieldReader<XDatabase>>().InstancePerLifetimeScope();
            builder.RegisterType<CsvHelperWriter>().As<ITextFieldWriter<XDatabase>>().InstancePerLifetimeScope();
            builder.Register<ShapeRenderer>((c) => new SkiaRenderer(true, 96.0)).InstancePerDependency();
            builder.RegisterType<WpfTextClipboard>().As<ITextClipboard>().InstancePerLifetimeScope();
        }
    }
}
