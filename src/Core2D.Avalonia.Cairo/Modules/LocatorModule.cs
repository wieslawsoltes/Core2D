// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Autofac;
using Core2D.Avalonia.Cairo.Locator;

namespace Core2D.Avalonia.Cairo.Modules
{
    /// <summary>
    /// Locator components module.
    /// </summary>
    public class LocatorModule : Module
    {
        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<ServiceProvider>().As<IServiceProvider>().InstancePerLifetimeScope();
        }
    }
}
