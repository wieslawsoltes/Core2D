using System;
using Autofac;
using Core2D.ServiceProvider.Autofac;

namespace Core2D.UI.Avalonia.Modules
{
    /// <summary>
    /// Locator components module.
    /// </summary>
    public class LocatorModule : Module
    {
        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AutofacServiceProvider>().As<IServiceProvider>().InstancePerLifetimeScope();
        }
    }
}
