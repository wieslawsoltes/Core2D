// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.using System;
using System;
using System.Linq;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Autofac;
using Avalonia;
using Avalonia.Android;
using Core2D.Avalonia.Android.Modules;
using Core2D.Avalonia.Modules;
using Core2D.Avalonia.Views;
using Core2D.Editor;
using Core2D.Interfaces;

namespace Core2D.Avalonia.Android
{
    [Activity(Label = "Core2D.Avalonia.Android", MainLauncher = true, Icon = "@drawable/icon", LaunchMode = LaunchMode.SingleInstance)]
    public class MainActivity : AvaloniaActivity
    {
        private IContainer container;
        private IServiceProvider serviceProvider;
        private ILog log;
        private IFileSystem fileIO;
        private ProjectEditor editor;
        private MainControl view;

        protected override void OnCreate(Bundle bundle)
        {
            if (global::Avalonia.Application.Current == null)
            {
                var builder = new ContainerBuilder();

                builder.RegisterModule<LocatorModule>();
                builder.RegisterModule<CoreModule>();
                builder.RegisterModule<DroidDependenciesModule>();
                builder.RegisterModule<AppModule>();
                builder.RegisterModule<DroidViewModule>();

                container = builder.Build();

                serviceProvider = container.Resolve<IServiceProvider>();

                App.InitializeConverters(serviceProvider);

                log = serviceProvider.GetService<ILog>();
                fileIO = serviceProvider.GetService<IFileSystem>();

                var app = new App();
                AppBuilder.Configure(app)
                    .UseAndroid()
                    .SetupWithoutStarting();

                log.Initialize(System.IO.Path.Combine(fileIO?.GetAssemblyPath(null), "Core2D.log"));

                editor = serviceProvider.GetService<ProjectEditor>();

                var path = System.IO.Path.Combine(fileIO.GetAssemblyPath(null), "Core2D.recent");
                if (fileIO.Exists(path))
                {
                    editor.OnLoadRecent(path);
                }

                editor.CurrentView = editor.Views.FirstOrDefault(v => v.Name == "Dashboard");
                editor.CurrentTool = editor.Tools.FirstOrDefault(t => t.Name == "Line");
                editor.CurrentPathTool = editor.PathTools.FirstOrDefault(t => t.Name == "Line");

                view = new MainControl();
                view.DataContext = editor;

                Content = view;
            }

            base.OnCreate(bundle);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            var path = System.IO.Path.Combine(fileIO.GetAssemblyPath(null), "Core2D.recent");
            editor.OnSaveRecent(path);

            log?.Dispose();
            container?.Dispose();
        }
    }
}
