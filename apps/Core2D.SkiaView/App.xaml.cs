// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Windows;
using Autofac;

namespace Core2D.SkiaView
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var builder = new ContainerBuilder();

            builder.RegisterAssemblyModules(typeof(MainWindow).Assembly);

            using (IContainer container = builder.Build())
            {
                new MainWindow(container.Resolve<IServiceProvider>()).ShowDialog();
            }
        }
    }
}
