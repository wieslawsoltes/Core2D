// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.IO;
using Avalonia;
using Avalonia.Logging.Serilog;
using Core2D.UI.Avalonia.Renderers;
using Core2D.ViewModels;
using Core2D.ViewModels.Containers;

namespace Core2D.UI.Avalonia
{
    class Program
    {
        static void Print(Exception ex)
        {
            Console.WriteLine(ex.Message);
            Console.WriteLine(ex.StackTrace);
            if (ex.InnerException != null)
            {
                Print(ex.InnerException);
            }
        }

        static void Main(string[] args)
        {
            try
            {
                var builder = BuildAvaloniaApp();
                if (args.Length > 0)
                {
                    foreach (var arg in args)
                    {
                        switch (arg)
                        {
                            case "--d2d":
                                builder.UseDirect2D1();
                                break;
                            case "--skia":
                                builder.UseSkia();
                                break;
                            case "--win32":
                                builder.UseWin32();
                                break;
                            case "--gtk3":
                                builder.UseGtk3();
                                break;
                            case "--native":
                                builder.UseAvaloniaNative();
                                break;

                        }
                    }
                }

                //var file = "vm.json";
                var bootstrapper = new Bootstrapper();

                LayerContainerViewModel vm;

                //if (File.Exists(file))
                //    vm = LayerContainerViewModel.Load(file);
                //else
                    vm = bootstrapper.CreateDemoViewModel();

                bootstrapper.CreateDemoContainer(vm);

                vm.Renderer = new AvaloniaShapeRenderer();

                builder.Start<MainWindow>(() => vm);

                //LayerContainerViewModel.Save(file, vm);
            }
            catch (Exception ex)
            {
                Print(ex);
            }
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .LogToDebug();
    }
}
