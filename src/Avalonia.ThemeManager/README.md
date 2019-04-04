# Avalonia Theme Manager

## About

Theme manager for [Avalonia](https://github.com/AvaloniaUI/Avalonia) applications.

## Usage

Theme manager searches user provided themes directory for `*.xaml` theme files otherwise built-in `Light` and `Dark` theme are used.

The theme manager static `Instance` property neeeds to be initalized before using `ThemeSelector` class.

`Program.cs`
```
using System;
using Avalonia;
using Avalonia.ThemeManager;

namespace AvaloniaApp
{
    static class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            BuildAvaloniaApp().Start(AppMain, args);
        }

        static void AppMain(Application app, string[] args)
        {
            ThemeSelector.Instance = new ThemeSelector("Themes");
            ThemeSelector.Instance.LoadSelectedTheme("AvaloniaApp.theme");

            app.Run(new MainWindow());

            ThemeSelector.Instance.SaveSelectedTheme("AvaloniaApp.theme");
        }

        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                         .UsePlatformDetect();
    }
}
```

`MainWindow.xaml`
```XAML
<Window x:Class="AvaloniaApp.MainWindow"
        xmlns="https://github.com/avaloniaui"
        xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
        xmlns:Themes="clr-namespace:Avalonia.ThemeManager;assembly=Avalonia.ThemeManager"
        Title="AvaloniaApp" Width="800" Height="600">
    <Window.Resources>
        <Themes:ObjectEqualityMultiConverter x:Key="ObjectEqualityMultiConverter"/>
    </Window.Resources>
    <Menu>
        <MenuItem Header="_View">
            <MenuItem Header="_Theme" DataContext="{x:Static Themes:ThemeSelector.Instance}" Items="{Binding Themes}">
                <MenuItem.Styles>
                    <Style Selector="MenuItem">
                        <Setter Property="Header" Value="{Binding Name}"/>
                        <Setter Property="Command" Value="{Binding Selector.ApplyTheme}"/>
                        <Setter Property="CommandParameter" Value="{Binding}"/>
                        <Setter Property="Icon">
                            <Template>
                                <CheckBox>
                                    <CheckBox.IsChecked>
                                        <MultiBinding Mode="OneWay" Converter="{StaticResource ObjectEqualityMultiConverter}">
                                            <Binding Path="DataContext" RelativeSource="{RelativeSource Self}"/>
                                            <Binding Path="Selector.SelectedTheme"/>
                                        </MultiBinding>
                                    </CheckBox.IsChecked>
                                </CheckBox>
                            </Template>
                        </Setter>
                    </Style>
                </MenuItem.Styles>
            </MenuItem>
        </MenuItem>
    </Menu>
</Window>
```

MainWindow.xaml.xs
```C#
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ThemeManager;

namespace AvaloniaApp.Views
{
    public class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            this.AttachDevTools();
            ThemeSelector.Instance.EnableThemes(this);
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
```

## License

Avalonia.ThemeManager is licensed under the [MIT license](LICENSE.TXT).
