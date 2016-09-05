// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Reflection;
using System.Windows.Input;
using Autofac;
using Core2D.Editor;
using Core2D.Editor.Factories;
using Core2D.Editor.Views.Interfaces;

namespace Core2D.Avalonia.Modules
{
    /// <summary>
    /// Core components module.
    /// </summary>
    public class CoreModule : Autofac.Module
    {
        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProjectEditor>().As<ProjectEditor>().InstancePerLifetimeScope();
            builder.RegisterType<ProjectFactory>().As<IProjectFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ShapeFactory>().As<IShapeFactory>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(ToolBase).GetTypeInfo().Assembly).As<ToolBase>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(PathToolBase).GetTypeInfo().Assembly).As<PathToolBase>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<ProjectEditorCommands>().AutoActivate().PropertiesAutowired().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(ProjectEditorCommands).GetTypeInfo().Assembly).AssignableTo<ICommand>().AsImplementedInterfaces().AsSelf().PropertiesAutowired().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(IView).GetTypeInfo().Assembly).As<IView>().InstancePerLifetimeScope();
        }
    }
}
