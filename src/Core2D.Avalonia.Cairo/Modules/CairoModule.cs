// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Autofac;
using Core2D.Data.Database;
using Core2D.Interfaces;
using FileSystem.DotNetFx;
using FileWriter.Pdf_core;
using Log.Trace;
using Serializer.Newtonsoft;
using Serializer.Xaml;
using TextFieldReader.CsvHelper;
using TextFieldWriter.CsvHelper;

namespace Core2D.Avalonia.Cairo.Modules
{
    /// <summary>
    /// Dependencies components module.
    /// </summary>
    public class CairoModule : Module
    {
        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<TraceLog>().As<ILog>().SingleInstance();
            builder.RegisterType<DotNetFxFileSystem>().As<IFileSystem>().InstancePerLifetimeScope();
            builder.RegisterType<NewtonsoftJsonSerializer>().As<IJsonSerializer>().InstancePerLifetimeScope();
            builder.RegisterType<PortableXamlSerializer>().As<IXamlSerializer>().InstancePerLifetimeScope();
            builder.RegisterType<PdfWriter>().As<IFileWriter>().InstancePerLifetimeScope();
            builder.RegisterType<CsvHelperReader>().As<ITextFieldReader<XDatabase>>().InstancePerLifetimeScope();
            builder.RegisterType<CsvHelperWriter>().As<ITextFieldWriter<XDatabase>>().InstancePerLifetimeScope();
        }
    }
}
