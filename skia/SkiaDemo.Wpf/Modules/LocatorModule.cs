using System;
using Autofac;
using SkiaDemo.Locator;

namespace SkiaDemo.Modules
{
    class LocatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ServiceProvider>().As<IServiceProvider>().InstancePerLifetimeScope();
        }
    }
}
