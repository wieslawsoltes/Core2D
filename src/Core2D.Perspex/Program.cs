// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Perspex;
using Perspex.Logging.Serilog;
using Serilog;
using System;

namespace Core2D.Perspex
{
    /// <summary>
    /// Encapsulates a Core2D Prespex program.
    /// </summary>
    internal class Program
    {
        /// <summary>
        /// Program entry point.
        /// </summary>
        /// <param name="args">The program arguments.</param>
        private static void Main(string[] args)
        {
            InitializeLogging();

            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.MacOSX:
                case PlatformID.Unix:
                    {
                        new App()
                            .UseGtk()
                            .UseCairo()
                            .LoadFromXaml()
                            .Start();
                    }
                    break;
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                    {
                        new App()
                            .UseWin32()
                            .UseDirect2D()
                            .LoadFromXaml()
                            .Start();
                    }
                    break;
            }
        }

        /// <summary>
        /// Initialize the Serilog logger.
        /// </summary>
        private static void InitializeLogging()
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
