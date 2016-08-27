using System;
using System.Threading.Tasks;
using System.Windows;
using Autofac;
using Core2D.Editor;
using Core2D.Interfaces;
using FileWriter.SvgSkiaSharp;
using Microsoft.Win32;

namespace SkiaDemo.Wpf
{
    public partial class App : Application
    {
        class Win32ImageImporter : IImageImporter
        {
            private readonly IServiceProvider _serviceProvider;

            public Win32ImageImporter(IServiceProvider serviceProvider)
            {
                _serviceProvider = serviceProvider;
            }

            public async Task<string> GetImageKeyAsync()
            {
                var dlg = new OpenFileDialog() { Filter = "All (*.*)|*.*" };
                if (dlg.ShowDialog(_serviceProvider.GetService<MainWindow>()) == true)
                {
                    var path = dlg.FileName;
                    var bytes = System.IO.File.ReadAllBytes(path);
                    var key = _serviceProvider.GetService<ProjectEditor>().Project.AddImageFromFile(path, bytes);
                    return await Task.Run(() => key);
                }
                return null;
            }
        }

        class AppModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterType<Win32ImageImporter>().As<IImageImporter>().InstancePerLifetimeScope();
            }
        }

        class ViewModule : Module
        {
            protected override void Load(ContainerBuilder builder)
            {
                builder.RegisterType<MainWindow>().As<MainWindow>().InstancePerLifetimeScope();
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var builder = new ContainerBuilder();

            builder.RegisterAssemblyModules(typeof(MainWindow).Assembly);

            builder.RegisterType<SvgWriter>().As<IFileWriter>().InstancePerLifetimeScope();

            using (IContainer container = builder.Build())
            {
                container.Resolve<MainWindow>().ShowDialog();
            }
        }
    }
}
