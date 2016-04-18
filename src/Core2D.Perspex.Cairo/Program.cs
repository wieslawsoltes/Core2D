// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Interfaces;
using FileSystem.DotNetFx;
using Log.Trace;
using Perspex;
using Perspex.Logging.Serilog;
using Serilog;

namespace Core2D.Perspex.Cairo
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

            using (ILog log = new TraceLog())
            {
                IFileSystem fileIO = new DotNetFxFileSystem();
                new App().UseGtk().UseCairo().LoadFromXaml().Start(fileIO, log);
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
