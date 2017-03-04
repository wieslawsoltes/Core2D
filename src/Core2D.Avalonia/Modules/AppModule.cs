// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Autofac;
using Core2D.Avalonia.Importers;
using Core2D.Interfaces;

namespace Core2D.Avalonia.Modules
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
        }
    }
}
