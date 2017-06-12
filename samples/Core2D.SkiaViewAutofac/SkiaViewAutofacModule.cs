// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Autofac;
using Core2D.FileSystem.DotNet;
using Core2D.Interfaces;
using Core2D.Renderer;
using Core2D.Renderer.Presenters;
using Core2D.Renderer.SkiaSharp;
using Core2D.Serializer.Newtonsoft;
using Core2D.ServiceProvider.Autofac;

namespace Core2D.SkiaViewAutofac
{
    public class SkiaViewAutofacModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Locator
            builder.RegisterType<AutofacServiceProvider>().As<IServiceProvider>().InstancePerLifetimeScope();
            // Dependencies
            builder.RegisterType<DotNetFileSystem>().As<IFileSystem>().InstancePerLifetimeScope();
            builder.RegisterType<NewtonsoftJsonSerializer>().As<IJsonSerializer>().InstancePerLifetimeScope();
            builder.Register<ShapeRenderer>((c) => new SkiaSharpRenderer(true, 96.0)).InstancePerDependency();
            builder.RegisterType<EditorPresenter>().As<ContainerPresenter>().InstancePerLifetimeScope();
        }
    }
}
