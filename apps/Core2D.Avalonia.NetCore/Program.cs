// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Linq;
using Autofac;
using Avalonia;
using Avalonia.Logging.Serilog;
using Core2D.Avalonia.Modules;
using Core2D.Avalonia.NetCore.Modules;
using Core2D.Avalonia.Views;
using Core2D.Interfaces;
using Serilog;

namespace Core2D.Avalonia.NetCore
{
    /// <summary>
    /// Encapsulates an Avalonia program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">The program arguments.</param>
        static void Main(string[] args)
        {
            InitializeLogging();

            var fbdev = args.Contains("--fbdev");
            var builder = new ContainerBuilder();

            builder.RegisterModule<LocatorModule>();
            builder.RegisterModule<CoreModule>();
            builder.RegisterModule<DependenciesModule>();
            builder.RegisterModule<AppModule>();
            builder.RegisterModule<ViewModule>();

            if (fbdev == true)
            {
                IContainer container = builder.Build();
                var app = new App();
                AppBuilder.Configure(app)
                    .InitializeWithLinuxFramebuffer(tl =>
                    {
                        tl.Content = app.CreateView(container.Resolve<IServiceProvider>());
                        System.Threading.ThreadPool.QueueUserWorkItem(_ => ConsoleSilencer());
                    });
            }
            else
            {
                using (IContainer container = builder.Build())
                {
                    using (ILog log = container.Resolve<ILog>())
                    {
                        var app = new App();
                        AppBuilder.Configure(app)
                            .UsePlatformDetect()
                            .SetupWithoutStarting();
                        app.Start(container.Resolve<IServiceProvider>());
                    }
                }
            }
        }

        /// <summary>
        /// Silence the console.
        /// </summary>
        static void ConsoleSilencer()
        {
            Console.CursorVisible = false;
            while (true)
                Console.ReadKey(true);
        }

        /// <summary>
        /// Initialize the Serilog logger.
        /// </summary>
        static void InitializeLogging()
        {
#if DEBUG
            SerilogLogger.Initialize(new LoggerConfiguration()
                .MinimumLevel.Warning()
                .WriteTo.Trace(outputTemplate: "{Area}: {Message}")
                .CreateLogger());
#endif
        }
    }
}
