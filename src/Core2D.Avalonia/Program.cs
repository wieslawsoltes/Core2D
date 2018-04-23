// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Diagnostics;
using System.Linq;
using Autofac;
using Autofac.Core;
using Avalonia;
using Avalonia.Logging.Serilog;
using Core2D.Avalonia.Modules;
using Core2D.Interfaces;

namespace Core2D.Avalonia
{
    /// <summary>
    /// Encapsulates an Core2D avalonia program.
    /// </summary>
    class Program
    {
        static void Print(Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            if (ex.InnerException != null)
            {
                Print(ex.InnerException);
            }
        }

        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">The program arguments.</param>
        static void Main(string[] args)
        {
            try
            {
                var builder = new ContainerBuilder();
                builder.RegisterModule<LocatorModule>();
                builder.RegisterModule<CoreModule>();
                builder.RegisterModule<DependenciesModule>();
                builder.RegisterModule<AppModule>();
                builder.RegisterModule<ViewModule>();
                using (IContainer container = builder.Build())
                {
                    using (ILog log = container.Resolve<ILog>())
                    {
                        var appBuilder = BuildAvaloniaApp();
                        if (args.Length > 0)
                        {
                            foreach (var arg in args)
                            {
                                switch (arg)
                                {
                                    case "--d2d":
                                        appBuilder.UseDirect2D1();
                                        break;
                                    case "--skia":
                                        appBuilder.UseSkia();
                                        break;
                                    case "--win32":
                                        appBuilder.UseWin32();
                                        break;
                                    case "--gtk3":
                                        appBuilder.UseGtk3();
                                        break;
                                    case "--mac":
                                        appBuilder.UseMonoMac();
                                        break;
                                }
                            }
                        }
                        appBuilder.SetupWithoutStarting();
                        var app = appBuilder.Instance as App;
                        var aboutInfo = app.CreateAboutInfo(
                            appBuilder.RuntimePlatform.GetRuntimeInfo(),
                            appBuilder.WindowingSubsystemName,
                            appBuilder.RenderingSubsystemName);
                        Debug.Write(aboutInfo);
                        app.Start(container.Resolve<IServiceProvider>(), aboutInfo);
                    }
                }
            }
            catch (Exception ex)
            {
                Print(ex);
            }
        }

        /// <summary>
        /// Builds Avalonia app.
        /// </summary>
        /// <returns>The Avalonia app builder.</returns>
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect()
                         .LogToDebug();

    }
}
