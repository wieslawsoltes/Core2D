// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Reflection;
using Avalonia.Data;
using Autofac;
using Core2D.UI.Avalonia.Dock.Factories;
using Core2D.UI.Avalonia.Windows;
using DM=Dock.Model;
using DAC=Dock.Avalonia.Controls;

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
            builder.Register(c => new DAC.HostWindow()).As<DM.IHostWindow>().InstancePerDependency();
            builder.RegisterType<MainWindow>().As<MainWindow>().InstancePerLifetimeScope();
        }
    }
}
