// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
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
