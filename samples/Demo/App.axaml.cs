using System;
using System.Linq;
using Autofac;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Core2D.ViewModels;
using Core2D.ViewModels.Editor;
using Demo.ViewModels;
using Demo.Views;

namespace Demo
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var builder = new ContainerBuilder();
            builder.RegisterModule<AppModule>();

            var container = builder.Build();

            var serviceProvider = container.Resolve<IServiceProvider>();

            var editor = serviceProvider.GetService<ProjectEditorViewModel>();
            if (editor is { })
            {
                editor.CurrentTool = editor.Tools.FirstOrDefault(t => t.Title == "Selection");
                editor.CurrentPathTool = editor.PathTools.FirstOrDefault(t => t.Title == "Line");
                editor.IsToolIdle = true;

                editor.OnNewProject();
            }

            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel()
                    {
                        Editor = editor
                    }
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}
