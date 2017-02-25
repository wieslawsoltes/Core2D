// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Autofac;
using Core2D.Data.Database;
using Core2D.Interfaces;
using Core2D.Renderer;
using FileSystem.DotNetFx;
using FileWriter.Emf;
using FileWriter.Pdf_core;
using Log.Trace;
using Renderer.Wpf;
using ScriptRunner.Roslyn;
using Serializer.Newtonsoft;
using Serializer.Xaml;
using TextFieldReader.CsvHelper;
using TextFieldWriter.CsvHelper;
using Utilities.Wpf;

namespace Core2D.Wpf.Modules
{
    /// <summary>
    /// Dependencies components module.
    /// </summary>
    public class DependenciesModule : Module
    {
        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TraceLog>().As<ILog>().SingleInstance();
            builder.RegisterType<DotNetFxFileSystem>().As<IFileSystem>().InstancePerLifetimeScope();
            builder.RegisterType<RoslynScriptRunner>().As<IScriptRunner>().InstancePerLifetimeScope();
            builder.RegisterType<NewtonsoftJsonSerializer>().As<IJsonSerializer>().InstancePerLifetimeScope();
            builder.RegisterType<PortableXamlSerializer>().As<IXamlSerializer>().InstancePerLifetimeScope();
            builder.RegisterType<PdfWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<EmfWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<CsvHelperReader>().As<ITextFieldReader<XDatabase>>().InstancePerLifetimeScope();
            builder.RegisterType<CsvHelperWriter>().As<ITextFieldWriter<XDatabase>>().InstancePerLifetimeScope();
            builder.RegisterType<WpfRenderer>().As<ShapeRenderer>().InstancePerDependency();
            builder.RegisterType<WpfTextClipboard>().As<ITextClipboard>().InstancePerLifetimeScope();
        }
    }
}
