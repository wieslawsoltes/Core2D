// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Autofac;
using Core2D.Editor;
using Core2D.Interfaces;
using Core2D.Wpf.Editor;
using Core2D.Wpf.Importers;

namespace Core2D.Wpf.Modules
{
    /// <summary>
    /// Application components module.
    /// </summary>
    public class AppModule : Module
    {
        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<Win32ImageImporter>().As<IImageImporter>().InstancePerLifetimeScope();
            builder.RegisterType<WpfProjectEditorPlatform>().As<IProjectEditorPlatform>().InstancePerLifetimeScope();
            builder.RegisterType<WpfEditorCanvasPlatform>().As<IEditorCanvasPlatform>().InstancePerLifetimeScope();
        }
    }
}
