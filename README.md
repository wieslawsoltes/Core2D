# Test2d

Data driven 2D diagram editor.

## About

Test2d is an application for making data driven 2D diagrams.

## Building Test2d

* [Visual Studio Community 2015](https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx) for Windows builds.
* [SharpDevelop](http://www.icsharpcode.net/OpenSource/SD/Download/#SharpDevelop5x) for Windows builds.
* [MonoDevelop](http://www.monodevelop.com/) for Linux builds.

### Supported Platforms

The recommended UI for Test2d is Test2d.UI.Wpf.

* Windows 7/8/8.1/10 for WPF, EtoForms.Direct2D, EtoForms.Gtk2, EtoForms.WinForms, EtoForms.Wpf and WinForms builds.
* Windows 10 for Universal Windows Platform (UWP) build.
* Ubuntu 15.04 for EtoForms.Gtk2, EtoForms.WinForms and WinForms builds.

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

### NuGet Package Sources

* https://www.nuget.org/api/v2/
* https://www.myget.org/F/eto/
* https://www.myget.org/F/perspex-nightly/api/v2

### Other Dependencies

* [GTK# for .NET](http://www.mono-project.com/download/#download-win) Needed for Eto.Platform.Gtk on Windows.
* [.net dxf Reader-Writer](http://netdxf.codeplex.com/) Run "git clone https://git01.codeplex.com/netdxf" in project parent directory.

## Contact

https://github.com/wieslawsoltes/Test2d

## License

Test2d is licensed under the [MIT license](LICENSE.TXT).
