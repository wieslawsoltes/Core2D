# Core2D

[![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Core2D/Core2D?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

[![Build status](https://ci.appveyor.com/api/projects/status/7k1e0voeit7od9bw/branch/master?svg=true)](https://ci.appveyor.com/project/wieslawsoltes/core2d/branch/master)

Data driven 2D diagram editor.

## About

Core2D is an application for making data driven 2D diagrams.

## Building Core2D

* [Visual Studio Community 2015](https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx) for Windows builds.
* [SharpDevelop](http://www.icsharpcode.net/OpenSource/SD/Download/#SharpDevelop5x) for Windows builds.
* [MonoDevelop](http://www.monodevelop.com/) for Linux builds.

### Supported Platforms

The recommended UI for Core2D is Core2D.UI.Wpf.

* Windows 7/8/8.1/10 for WPF, EtoForms.Direct2D, EtoForms.Gtk2, Perspex.Desktop and WinForms builds.
* Windows 10 for Universal Windows Platform (UWP) build.
* XUbuntu 15.04 for EtoForms.Gtk2, Perspex.Desktop and WinForms builds.

The core library and editor are portable and should work on all platforms where C# is supported.

### NuGet Packages

* System.Collections.Immutable
* Newtonsoft.Json
* CsvHelper
* PDFsharp (Core PDFsharp for WinForms Linux builds.)
* PDFsharp-wpf (Core PDFsharp does not implement XGraphicsPath.AddArc method.)
* System.Windows.Interactivity.WPF
* Xceed.Wpf.AvalonDock
* Xceed.Products.Wpf.Toolkit.AvalonDock
* Eto.Forms
* Eto.Platform.Direct2D
* Eto.Platform.Gtk
* Eto.Platform.Windows
* Eto.Platform.Wpf
* Perspex
* Perspex.Desktop

### NuGet Package Sources

* https://www.nuget.org/api/v2/
* https://www.myget.org/F/eto/
* https://www.myget.org/F/perspex-nightly/api/v2

### Other Dependencies

* [GTK# for .NET](http://www.mono-project.com/download/#download-win) Needed for Eto.Platform.Gtk on Windows.
* [.net dxf Reader-Writer](http://netdxf.codeplex.com/) Run "git submodule update --init" in project directory.

## Contact

https://github.com/Core2D/Core2D

## License

Core2D is licensed under the [MIT license](LICENSE.TXT).
