// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Core2D.Data.Database;
using Core2D.Editor;
using Core2D.Editor.Factories;
using Core2D.Editor.Input;
using Core2D.Editor.Interfaces;
using Core2D.Interfaces;
using Core2D.Renderer;
using FileSystem.DotNetFx;
using Log.Trace;
using Renderer.Avalonia;
using Serializer.Newtonsoft;
using Serializer.Xaml;
using TextFieldReader.CsvHelper;
using TextFieldWriter.CsvHelper;
using Utilities.Avalonia;
using A = Avalonia;
using AAPS = Avalonia.Android.Platform.Specific;

namespace Core2D.Avalonia.Droid
{
    [Activity
        (Label = "Core2D",
        MainLauncher = true,
        Icon = "@drawable/icon",
        LaunchMode = LaunchMode.SingleInstance,
        ScreenOrientation = ScreenOrientation.Landscape)]
    public class MainActivity : AAPS.AvaloniaActivity
    {
        public MainActivity()
            : base(typeof(App))
        {
        }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            ServiceLocator.Instance.RegisterSingleton<ITextFieldReader<ProjectEditor>>(() => new ProjectEditor());
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
            ServiceLocator.Instance.RegisterSingleton<ImmutableArray<IFileWriter>>(() => new IFileWriter[] { }.ToImmutableArray());
            ServiceLocator.Instance.RegisterSingleton<ITextFieldReader<XDatabase>>(() => new CsvHelperReader());
            ServiceLocator.Instance.RegisterSingleton<ITextFieldWriter<XDatabase>>(() => new CsvHelperWriter());
            ServiceLocator.Instance.RegisterSingleton<Windows.MainWindow>(() => new Windows.MainWindow());

            using (var log = ServiceLocator.Instance.Resolve<ILog>())
            {
                var app = A.Application.Current as App ?? new App();
                ServiceLocator.Instance.RegisterSingleton<IEditorApplication>(() => app);

                app.Start();
            }
        }
    }
}
