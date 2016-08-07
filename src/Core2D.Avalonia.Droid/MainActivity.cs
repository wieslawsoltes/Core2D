// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System.Collections.Immutable;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Core2D.Interfaces;
using FileSystem.DotNetFx;
using Log.Trace;
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

            using (ILog log = new TraceLog())
            {
                IFileSystem fileIO = new DotNetFxFileSystem();
                ImmutableArray<IFileWriter> writers =
                    new IFileWriter[]
                    {
                    }.ToImmutableArray();

                var app = A.Application.Current as App ?? new App();
                app.Start(fileIO, log, writers);
            }
        }
    }
}
