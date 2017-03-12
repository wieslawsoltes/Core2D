// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Reflection;
using System.Windows.Input;
using Autofac;
using Core2D.Editor.Views.Interfaces;

namespace Core2D.Avalonia.Android.Modules
{
    /// <summary>
    /// View components module.
    /// </summary>
    public class DroidViewModule : Autofac.Module
    {
        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterAssemblyTypes(typeof(App).GetTypeInfo().Assembly).AssignableTo<ICommand>().AsImplementedInterfaces().AsSelf().PropertiesAutowired().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(App).GetTypeInfo().Assembly).As<IView>().InstancePerLifetimeScope();
        }
    }
}
