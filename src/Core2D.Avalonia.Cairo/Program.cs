// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using Core2D.Interfaces;
using FileSystem.DotNetFx;
using FileWriter.Dxf;
using FileWriter.Pdf_core;
using Log.Trace;
using Avalonia;
using Avalonia.Logging.Serilog;
using Serilog;
using System.Collections.Immutable;

namespace Core2D.Avalonia.Cairo
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
                ImmutableArray<IFileWriter> writers = 
                    new IFileWriter[] 
                    {
                        new PdfWriter(),
                        new DxfWriter()
                    }.ToImmutableArray();

                new App().UseGtk().UseCairo().LoadFromXaml().Start(fileIO, log, writers);
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
