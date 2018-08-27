// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Autofac;
using Core2D.Interfaces;
using Core2D.Renderer;
using Core2D.Renderer.Avalonia;
using Core2D.Utilities.Avalonia;

namespace Core2D.UI.Avalonia.Modules
{
    /// <summary>
    /// Designer dependencies components module.
    /// </summary>
    public class DesignerModule : Module
    {
        /// <inheritdoc/>
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<AvaloniaRenderer>().As<IShapeRenderer>().InstancePerDependency();
            builder.RegisterType<AvaloniaTextClipboard>().As<ITextClipboard>().InstancePerLifetimeScope();
        }
    }
}
