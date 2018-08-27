// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Reflection;
using Autofac;
using Core2D.UI.Avalonia.Dock.Factories;
using Core2D.UI.Avalonia.Windows;
using Dock.Avalonia.Controls;
using Dock.Model;

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
            builder.RegisterAssemblyTypes(typeof(App).GetTypeInfo().Assembly).As<IDock>().InstancePerLifetimeScope();
            builder.RegisterType<EditorDockFactory>().As<IDockFactory>().InstancePerDependency();
            builder.RegisterType<HostWindow>().As<IDockHost>().InstancePerDependency();
            builder.RegisterType<MainWindow>().As<MainWindow>().InstancePerLifetimeScope();
        }
    }
}
