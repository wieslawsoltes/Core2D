using System;
using Autofac;
using Core2D.Interfaces;
using SkiaDemo.Wpf.Importers;

namespace SkiaDemo.Wpf.Modules
{
    public class AppModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Win32ImageImporter>().As<IImageImporter>().InstancePerLifetimeScope();
        }
    }
}
