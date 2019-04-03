// Copyright (c) Wiesław Šoltés. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Reactive.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Markup.Xaml.Styling;
using Avalonia.Styling;
using ReactiveUI;

namespace Core2D.UI.Avalonia
{
    public class Theme : ReactiveObject
    {
        private string _name;
        private IStyle _style;
        private ThemeManager _manager;

        public string Name
        {
            get => _name;
            set => this.RaiseAndSetIfChanged(ref _name, value);
        }

        public IStyle Style
        {
            get => _style;
            set => this.RaiseAndSetIfChanged(ref _style, value);
        }

        public ThemeManager Manager
        {
            get => _manager;
            set => this.RaiseAndSetIfChanged(ref _manager, value);
        }
    }

    public class ThemeManager : ReactiveObject
    {
        public static ThemeManager Instance = new ThemeManager();

        private ObservableCollection<Window> _windows;
        private Theme _selectedTheme;
        private ObservableCollection<Theme> _themes;

        public Theme SelectedTheme
        {
            get => _selectedTheme;
            set => this.RaiseAndSetIfChanged(ref _selectedTheme, value);
        }

        public ObservableCollection<Theme> Themes
        {
            get => _themes;
            set => this.RaiseAndSetIfChanged(ref _themes, value);
        }

        public ObservableCollection<Window> Windows
        {
            get => _windows;
            set => this.RaiseAndSetIfChanged(ref _windows, value);
        }

        public ThemeManager()
        {
            _themes = new ObservableCollection<Theme>();

            foreach (string file in System.IO.Directory.EnumerateFiles("Themes", "*.xaml"))
            {
                try
                {
                    var name = System.IO.Path.GetFileNameWithoutExtension(file);
                    var style = AvaloniaXamlLoader.Parse<IStyle>(System.IO.File.ReadAllText(file));
                    var theme = new Theme()
                    {
                        Name = name,
                        Style = style,
                        Manager = this
                    };
                    _themes.Add(theme);
                }
                catch (Exception) { }
            }

            if (_themes.Count == 0)
            {
                _themes.Add(new Theme() { Name = "Light", Style = AvaloniaXamlLoader.Parse<StyleInclude>(@"<StyleInclude xmlns='https://github.com/avaloniaui' Source='avares://Avalonia.Themes.Default/Accents/BaseLight.xaml'/>"), Manager = this });
                _themes.Add(new Theme() { Name = "Dark", Style = AvaloniaXamlLoader.Parse<StyleInclude>(@"<StyleInclude xmlns='https://github.com/avaloniaui' Source='avares://Avalonia.Themes.Default/Accents/BaseDark.xaml'/>"), Manager = this });
            }

            _selectedTheme = _themes.FirstOrDefault();

            _windows = new ObservableCollection<Window>();
        }

        public void EnableThemes(Window window)
        {
            IDisposable disposable = null;

            window.Styles.Add(Instance.SelectedTheme.Style);

            window.Opened += (sender, e) =>
            {
                Debug.WriteLine("Window opened.");
                _windows.Add(window);
                disposable = this.WhenAnyValue(x => x.SelectedTheme).Where(x => x != null).Subscribe(x =>
                {
                    Debug.WriteLine($"Theme changed: {x.Name}");
                    window.Styles[0] = x.Style;
                });
            };

            window.Closing += (sender, e) =>
            {
                Debug.WriteLine("Window closed.");
                disposable?.Dispose();
                _windows.Remove(window);
            };
        }

        public void ApplyTheme(Theme theme)
        {
            Debug.WriteLine($"Apply theme: {theme?.Name}");
            SelectedTheme = theme;

            if (theme != null)
            {
                foreach (var window in _windows)
                {
                    window.Styles[0] = theme.Style;
                }
            }
        }
    }
}
