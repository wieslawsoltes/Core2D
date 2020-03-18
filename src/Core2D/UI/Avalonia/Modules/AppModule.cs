using Autofac;
using Core2D.Editor;
using Core2D.Interfaces;
using Core2D.UI.Avalonia.Editor;
using Core2D.UI.Avalonia.Importers;

namespace Core2D.UI.Avalonia.Modules
{
    /// <summary>
    /// Application components module.
    /// </summary>
    public class AppModule : Autofac.Module
    {
        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AvaloniaImageImporter>().As<IImageImporter>().InstancePerLifetimeScope();
            builder.RegisterType<AvaloniaProjectEditorPlatform>().As<IProjectEditorPlatform>().InstancePerLifetimeScope();
            builder.RegisterType<AvaloniaEditorCanvasPlatform>().As<IEditorCanvasPlatform>().InstancePerLifetimeScope();
            builder.RegisterType<AvaloniaEditorLayoutPlatform>().As<IEditorLayoutPlatform>().InstancePerLifetimeScope();
        }
    }
}
