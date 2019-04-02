// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Reflection;
using Autofac;
using Core2D.Data;
using Core2D.Editor;
using Core2D.Editor.Bounds;
using Core2D.Editor.Factories;
using Core2D.Interfaces;

namespace Core2D.UI.Avalonia.Modules
{
    /// <summary>
    /// Core components module.
    /// </summary>
    public class CoreModule : Autofac.Module
    {
        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ProjectEditor>().As<IProjectEditor>().InstancePerLifetimeScope();
            builder.RegisterType<Factory>().As<IFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ContainerFactory>().As<IContainerFactory>().InstancePerLifetimeScope();
            builder.RegisterType<ShapeFactory>().As<IShapeFactory>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(IEditorTool).GetTypeInfo().Assembly).As<IEditorTool>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(IPathTool).GetTypeInfo().Assembly).As<IPathTool>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<HitTest>().As<IHitTest>().InstancePerLifetimeScope();
            builder.RegisterAssemblyTypes(typeof(IBounds).GetTypeInfo().Assembly).As<IBounds>().AsSelf().InstancePerLifetimeScope();
            builder.RegisterType<DataFlow>().As<IDataFlow>().InstancePerLifetimeScope();
        }
    }
}
