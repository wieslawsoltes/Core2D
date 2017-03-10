// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.ComponentModel;
using System.Windows;
using Autofac;
using Core2D.Editor.Designer;
using Core2D.Wpf.Modules;

namespace Core2D.Wpf.Designer
{
    /// <summary>
    /// The design time DataContext helper class.
    /// </summary>
    public class DesignerHelper : DesignerContext
    {
        /// <summary>
        /// Initializes static data.
        /// </summary>
        static DesignerHelper()
        {
            if (DesignerProperties.GetIsInDesignMode(new DependencyObject()))
            {
                var builder = new ContainerBuilder();

                builder.RegisterModule<LocatorModule>();
                builder.RegisterModule<CoreModule>();
                builder.RegisterModule<DependenciesModule>();
                builder.RegisterModule<AppModule>();
                builder.RegisterModule<ViewModule>();

                var container = builder.Build();

                InitializeContext(container.Resolve<IServiceProvider>());
            }
        }
    }
}
