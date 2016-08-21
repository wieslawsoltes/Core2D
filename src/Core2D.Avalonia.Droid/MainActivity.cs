// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Autofac;
using Core2D.Avalonia.Droid.Modules;
using Core2D.Avalonia.Modules;
using Core2D.Interfaces;
using A = Avalonia;
using AAPS = Avalonia.Android.Platform.Specific;
using AC = Avalonia.Controls;

namespace Core2D.Avalonia.Droid
{
    [Activity(
        Label = "Core2D",
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

            var builder = new ContainerBuilder();

            builder.RegisterModule<LocatorModule>();
            builder.RegisterModule<CoreModule>();
            builder.RegisterModule<DependenciesModule>();
            builder.RegisterModule<AppModule>();
            builder.RegisterModule<DroidModule>();

            using (IContainer container = builder.Build())
            {
                using (var log = container.Resolve<ILog>())
                {
                    var app = A.Application.Current as App ?? new App();
                    AC.AppBuilder.Configure(app)
                        .UsePlatformDetect()
                        .SetupWithoutStarting();
                    app.Start(container.Resolve<IServiceProvider>());
                }
            }
        }
    }
}
