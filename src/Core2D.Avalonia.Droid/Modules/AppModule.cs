// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Windows.Input;
using Autofac;
using Core2D.Editor.Views.Interfaces;
using Core2D.Interfaces;
using Core2D.Avalonia.Importers;
using Core2D.Avalonia.Windows;

namespace Core2D.Avalonia.Droid.Modules
{
    /// <summary>
    /// Application components module.
    /// </summary>
    public class AppModule : Module
    {
        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<FileImageImporter>().As<IImageImporter>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(App).Assembly).AssignableTo<ICommand>().AsImplementedInterfaces().AsSelf().PropertiesAutowired().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(App).Assembly).As<IView>().InstancePerLifetimeScope();
            builder.RegisterType<MainWindow>().As<MainWindow>().InstancePerLifetimeScope();
        }
    }
}
