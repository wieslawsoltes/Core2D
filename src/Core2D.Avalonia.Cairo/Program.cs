// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging.Serilog;
using Core2D.Interfaces;
using FileSystem.DotNetFx;
using FileWriter.Pdf_core;
using Log.Trace;
using Serilog;

namespace Core2D.Avalonia.Cairo
{
    /// <summary>
    /// Encapsulates a Core2D Avalonia program.
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
                ImmutableArray<IFileWriter> writers = 
                    new IFileWriter[] 
                    {
                        new PdfWriter()
                    }.ToImmutableArray();

                var app = new App();
                AppBuilder.Configure(app)
                    .UseGtk()
                    .UseCairo()
                    .SetupWithoutStarting();
                app.Start(fileIO, log, writers);
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
