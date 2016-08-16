// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Logging.Serilog;
using Core2D.Data.Database;
using Core2D.Editor;
using Core2D.Editor.Factories;
using Core2D.Editor.Input;
using Core2D.Editor.Interfaces;
using Core2D.Interfaces;
using Core2D.Renderer;
using FileSystem.DotNetFx;
using FileWriter.Pdf_core;
using Log.Trace;
using Renderer.Avalonia;
using Serializer.Newtonsoft;
using Serializer.Xaml;
using Serilog;
using TextFieldReader.CsvHelper;
using TextFieldWriter.CsvHelper;
using Utilities.Avalonia;

namespace Core2D.Avalonia.Cairo
{
    /// <summary>
    /// Encapsulates a Avalonia program.
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

            RegisterServices();

            using (var log = ServiceLocator.Instance.Resolve<ILog>())
            {
                var app = new App();
                ServiceLocator.Instance.RegisterSingleton<IEditorApplication>(() => app);

                AppBuilder.Configure(app)
                    .UseGtk()
                    .UseCairo()
                    .SetupWithoutStarting();
                app.Start();
            }
        }

        /// <summary>
        /// Register application services.
        /// </summary>
        private static void RegisterServices()
        {
            ServiceLocator.Instance.RegisterSingleton<ProjectEditor>(() => new ProjectEditor());
            ServiceLocator.Instance.RegisterSingleton<ILog>(() => new TraceLog());
            ServiceLocator.Instance.RegisterSingleton<CommandManager>(() => new CommandManager());
            ServiceLocator.Instance.RegisterSingleton<ShapeRenderer[]>(
                () =>
                {
                    return new[]
                    {
                        new AvaloniaRenderer(),
                        new AvaloniaRenderer()
                    };
                });
            ServiceLocator.Instance.RegisterSingleton<IFileSystem>(() => new DotNetFxFileSystem());
            ServiceLocator.Instance.RegisterSingleton<IProjectFactory>(() => new ProjectFactory());
            ServiceLocator.Instance.RegisterSingleton<ITextClipboard>(() => new AvaloniaTextClipboard());
            ServiceLocator.Instance.RegisterSingleton<IJsonSerializer>(() => new NewtonsoftJsonSerializer());
            ServiceLocator.Instance.RegisterSingleton<IXamlSerializer>(() => new PortableXamlSerializer());
            ServiceLocator.Instance.RegisterSingleton<ImmutableArray<IFileWriter>>(
                () =>
                {
                    return new IFileWriter[]
                    {
                        new PdfWriter()
                    }.ToImmutableArray();
                });
            ServiceLocator.Instance.RegisterSingleton<ITextFieldReader<XDatabase>>(() => new CsvHelperReader());
            ServiceLocator.Instance.RegisterSingleton<ITextFieldWriter<XDatabase>>(() => new CsvHelperWriter());
            ServiceLocator.Instance.RegisterSingleton<Windows.MainWindow>(() => new Windows.MainWindow());
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
