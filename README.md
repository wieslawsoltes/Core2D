# Core2D

[![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Core2D/Core2D?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)
[![Build status](https://ci.appveyor.com/api/projects/status/7k1e0voeit7od9bw/branch/master?svg=true)](https://ci.appveyor.com/project/wieslawsoltes/core2d/branch/master)
[![Build Status](https://travis-ci.org/Core2D/Core2D.svg?branch=master)](https://travis-ci.org/Core2D/Core2D)
[![Build Status](https://www.bitrise.io/app/0eddf30a82243ed8.svg?token=IEGVKM7S6KBI3HdecpD8Cg&branch=master)](https://www.bitrise.io/app/0eddf30a82243ed8)

A multi-platform data driven 2D diagram editor.

<a href='https://www.youtube.com/watch?v=P7G0kmX7EcU' target='_blank'>![](https://i.ytimg.com/vi/P7G0kmX7EcU/hqdefault.jpg)<a/>

## About

Core2D is a multi-platform application for making data driven 2D diagrams.

## Data Formats

* The project model is stored as `Json` in `zip` archives.
* The project images are stored  as files in `zip` archives.
* Resources can be defined in `Json` or `Xaml` format.
* The `Json` format is supported for imported and exported resources. 
* The `Xaml` format is supported for imported and exported resources. 
* Database records can be imported, exported and updated from `csv` file format.
* The clipboard data is stored as `Json` text.

## Supported Platforms

* `Windows` 7/8/8.1/10 using `Core2D.Wpf`, `Core2D.Avalonia.Direct2D` and `Core2D.Avalonia.Skia` builds.
* `XUbuntu` 16.04 using `Core2D.Avalonia.Skia` and `Core2D.Avalonia.Cairo` builds.
* `Android` using `Core2D.Avalonia.Droid` build.
* `iOS` support is planned using `Avalonia.iOS`.

The core library and editor are portable and should work on all platforms where C# is supported.

## Building Core2D

* [Visual Studio Community 2015](https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx) for `Windows` builds.
* [MonoDevelop](http://www.monodevelop.com/) for `Linux` builds.

## NuGet

Core2D core library is delivered as a NuGet package.

You can find the packages here [NuGet](https://www.nuget.org/packages/Core2D/) or by using nightly build feed:
* Add `https://www.myget.org/F/core2d-nightly/api/v2` to your package sources
* Update your package using `Core2D` feed

You can install the package like this:

`Install-Package Core2D -Pre`

### NuGet Packages

* Common
  * System.Collections.Immutable
  * Portable.Xaml
  * Newtonsoft.Json
  * CsvHelper
* WPF
  * Xceed.Wpf.AvalonDock
  * Xceed.Products.Wpf.Toolkit.AvalonDock
  * System.Windows.Interactivity.WPF
  * Wpf.Controls.PanAndZoom
* Avalonia
  * Avalonia
  * Avalonia.Desktop
  * Avalonia.Skia.Desktop
  * SkiaSharp
  * System.Reactive
  * System.Reactive.Core
  * System.Reactive.Interfaces
  * System.Reactive.Linq
  * System.Reactive.PlatformServices
  * Serilog
  * SharpDX
  * SharpDX.Direct2D1
  * SharpDX.DXGI
  * Splat
  * Sprache
  * Avalonia.Xaml.Behaviors
  * Avalonia.Controls.PanAndZoom

### NuGet Package Sources

* https://www.nuget.org/api/v2/
* https://ci.appveyor.com/nuget/portable-xaml
* https://www.myget.org/F/avalonia-ci/api/v2
* https://www.myget.org/F/xamlbehaviors-nightly/api/v2
* https://www.myget.org/F/panandzoom-nightly/api/v2

### Other Dependencies

* [Port of Windows UWP Xaml Behaviors for Avalonia Xaml.](https://github.com/XamlBehaviors/XamlBehaviors) Needed for Xaml Behaviors support.
* [Pan and zoom control for WPF and Avalonia.](https://github.com/wieslawsoltes/MatrixPanAndZoomDemo) Needed for Pan and Zoom support.
* [Portable .NET library for reading/writing xaml files.](https://github.com/cwensley/Portable.Xaml) Needed for Xaml support.
* [xUnit.net unit testing tool for the .NET Framework.](https://github.com/xunit/xunit) Needed to run tests.
* [GTK# for .NET](http://www.mono-project.com/download/#download-win) Needed for Gtk on Windows.
* [.net dxf Reader-Writer](http://netdxf.codeplex.com/) Needed for `DXF` support. Run `git submodule update --init --recursive` in project directory.
* [PDFsharp A .NET library for processing PDF](https://github.com/empira/PDFsharp) Needed for `PDF` support. Run `git submodule update --init --recursive` in project directory.
* For building `Core2D` mirror repository is used for [.net dxf Reader-Writer](https://github.com/Core2D/netdxf).
* For building `Core2D` mirror repository is used for [PDFsharp](https://github.com/Core2D/PDFsharp). 
* `PDFsharp` core is used for `Avalonia` and non-windows builds and `PDFsharp-wpf` is used for WPF version (`PDFsharp` core does not implement `XGraphicsPath.AddArc` method.).

## Resources

* [Project website and API Reference.](http://core2d.github.io/)
* [GitHub source code repository.](https://github.com/Core2D/Core2D)

## License

Core2D is licensed under the [MIT license](LICENSE.TXT).
