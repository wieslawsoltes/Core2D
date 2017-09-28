// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Diagnostics;
using Autofac;
using Avalonia;
using Avalonia.Logging.Serilog;
using Core2D.Avalonia.Modules;
using Core2D.Interfaces;
using Serilog;

namespace Core2D.Avalonia
{
    /// <summary>
    /// Encapsulates an Core2D avalonia program.
    /// </summary>
    class Program
    {
        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">The program arguments.</param>
        static void Main(string[] args)
        {
            try
            {
                InitializeLogging();

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
                        var app = new App();
                        var appBuilder = AppBuilder.Configure(app).UsePlatformDetect();

                        appBuilder.SetupWithoutStarting();

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
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }
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
