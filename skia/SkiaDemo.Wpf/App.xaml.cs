using System;
using System.Windows;
using Autofac;

namespace SkiaDemo.Wpf
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var builder = new ContainerBuilder();

            builder.RegisterAssemblyModules(typeof(MainWindow).Assembly);

            using (IContainer container = builder.Build())
            {
                container.Resolve<MainWindow>().ShowDialog();
            }
        }
    }
}
