using System.Reflection;
using Autofac;
using Core2D.UI.Avalonia.Dock.Factories;
using Core2D.UI.Avalonia.Dock.Windows;
using Core2D.UI.Avalonia.Windows;
using DM = Dock.Model;

namespace Core2D.UI.Avalonia.Modules
{
    /// <summary>
    /// View components module.
    /// </summary>
    public class ViewModule : Autofac.Module
    {
        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(App).GetTypeInfo().Assembly).As<DM.IDock>().InstancePerLifetimeScope();
            builder.RegisterType<EditorDockFactory>().As<DM.IFactory>().InstancePerDependency();
            builder.Register(c => new ThemedHostWindow()).As<DM.IHostWindow>().InstancePerDependency();
            builder.RegisterType<MainWindow>().As<MainWindow>().InstancePerLifetimeScope();
        }
    }
}
