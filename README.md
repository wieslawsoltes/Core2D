# Core2D

[![Gitter](https://badges.gitter.im/wieslawsoltes/Core2D.svg)](https://gitter.im/wieslawsoltes/Core2D?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

[![Build status](https://ci.appveyor.com/api/projects/status/7k1e0voeit7od9bw/branch/master?svg=true)](https://ci.appveyor.com/project/wieslawsoltes/core2d/branch/master)
[![Build Status](https://travis-ci.org/wieslawsoltes/Core2D.svg?branch=master)](https://travis-ci.org/wieslawsoltes/Core2D)
![Build Status](https://wiso.visualstudio.com/_apis/public/build/definitions/6eb4f619-6cfb-4f2c-8220-dbe2c8cfd282/2/badge)
[![Build Status](https://www.bitrise.io/app/4eef57c778c912c7.svg?token=BCX9s7DZh3QgVbECxVe4kA&branch=master)](https://www.bitrise.io/app/4eef57c778c912c7)

A multi-platform data driven 2D diagram editor.

## About

Core2D is a multi-platform application for making data driven 2D diagrams.

## Data Formats

* The project models is stored as `Json` in `zip` archive.
* The project images are stored  as files in `zip` archive.
* Resources are defined as `Json` or `Xaml`.
* The `Json` format is supported for imported and exported resources. 
* The `Xaml` format is supported for imported and exported resources. 
* Database records are imported, exported and updated as `csv`.
* The clipboard data is stored as `Json`.

## Supported Platforms

* `Windows` 7/8/8.1/10 using `Core2D.Wpf`, `Core2D.Avalonia.Direct2D` and `Core2D.Avalonia.Skia` builds.
* `Ubuntu` 16.10 using `Core2D.Avalonia.Skia` and `Core2D.Avalonia.Cairo` builds.
* `Android` support is planned using `Avalonia.Android`.
* `iOS` support is planned using `Avalonia.iOS`.

The core library and editor are portable and should work on all platforms where C# is supported.

## Building Core2D

First, clone the repository or download the latest zip.
```
git clone https://github.com/wieslawsoltes/Core2D.git
git submodule update --init --recursive
```

### Build using IDE

* [Visual Studio Community 2015](https://www.visualstudio.com/en-us/products/visual-studio-community-vs.aspx) for `Windows` builds.
* [MonoDevelop](http://www.monodevelop.com/) for `Linux` builds.

Open `Core2D.sln` in selected IDE and run `Build` command.

### Build on Windows using script

Open up a Powershell prompt and execute the bootstrapper script:
```PowerShell
PS> .\build.ps1 -Target "Default" -Platform "AnyCPU" -Configuration "Release"
```

### Build on Linux/OSX using script

Open up a terminal prompt and execute the bootstrapper script:
```Bash
$ ./build.sh --target "Default" --platform "AnyCPU" --configuration "Release"
```

## Dependencies

* [Port of Windows UWP Xaml Behaviors for Avalonia Xaml.](https://github.com/wieslawsoltes/XamlBehaviors) Needed for Xaml Behaviors support.
* [Pan and zoom control for WPF and Avalonia.](https://github.com/wieslawsoltes/PanAndZoom) Needed for Pan and Zoom support.
* [Portable .NET library for reading/writing xaml files.](https://github.com/cwensley/Portable.Xaml) Needed for Xaml support.
* [xUnit.net unit testing tool for the .NET Framework.](https://github.com/xunit/xunit) Needed to run tests.
* [GTK# for .NET](http://www.mono-project.com/download/#download-win) Needed for Gtk on Windows.
* [.net dxf Reader-Writer](http://netdxf.codeplex.com/) Needed for `DXF` support. Run `git submodule update --init --recursive` in project directory.
* [PDFsharp A .NET library for processing PDF](https://github.com/empira/PDFsharp) Needed for `PDF` support. Run `git submodule update --init --recursive` in project directory.
* For building `Core2D` mirror repository is used for [.net dxf Reader-Writer](https://github.com/wieslawsoltes/netdxf).
* For building `Core2D` mirror repository is used for [PDFsharp](https://github.com/wieslawsoltes/PDFsharp). 
* `PDFsharp` core is used for `Avalonia` and non-windows builds and `PDFsharp-wpf` is used for WPF version (`PDFsharp` core does not implement `XGraphicsPath.AddArc` method.).

* Common
  * System.Collections.Immutable
  * System.Reactive.Core
  * System.Reactive.Interfaces
  * Portable.Xaml
  * Newtonsoft.Json
  * CsvHelper
  * SkiaSharp
  * Microsoft.CodeAnalysis.CSharp
  * Microsoft.Composition
* WPF
  * Autofac
  * System.Reactive.Core
  * System.Reactive.Interfaces
  * System.Reactive.Linq
  * Xceed.Wpf.AvalonDock
  * Xceed.Products.Wpf.Toolkit.AvalonDock
  * System.Windows.Interactivity.WPF
  * Wpf.Controls.PanAndZoom
* Avalonia
  * Autofac
  * System.Reactive
  * System.Reactive.Core
  * System.Reactive.Interfaces
  * System.Reactive.Linq
  * System.Reactive.PlatformServices
  * Avalonia
  * Avalonia.Desktop
  * Avalonia.Skia.Desktop
  * Serilog
  * SharpDX
  * SharpDX.Direct2D1
  * SharpDX.DXGI
  * Splat
  * Sprache
  * Avalonia.Xaml.Behaviors
  * Avalonia.Controls.PanAndZoom

## Package Sources

* https://api.nuget.org/v3/index.json
* https://www.myget.org/F/avalonia-ci/api/v2
* https://www.myget.org/F/xamlbehaviors-nightly/api/v2
* https://www.myget.org/F/panandzoom-nightly/api/v2

## SkiaSharp

The `libSkiaSharp.dll` from SkiaSharp package requires [Microsoft Visual C++ 2015 Redistributable](https://www.microsoft.com/en-us/download/details.aspx?id=52982) installed or included as part of distribution. License terms for redistributable
[MICROSOFT SOFTWARE LICENSE TERMS, MICROSOFT VISUAL STUDIO COMMUNITY 2015](https://www.visualstudio.com/en-us/support/legal/mt171547) and information about [Distributable Code for Microsoft Visual Studio 2015](https://www.visualstudio.com/en-us/downloads/2015-redistribution-vs.aspx).

### Required Visual C++ Runtime Files

#### x86 Platform

```
C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\redist\x86\Microsoft.VC140.CRT\msvcp140.dll
C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\redist\x86\Microsoft.VC140.CRT\vcruntime140.dll
```

#### x64 Platform

```
C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\redist\x64\Microsoft.VC140.CRT\msvcp140.dll
C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\redist\x64\Microsoft.VC140.CRT\vcruntime140.dll
```

### Post-build event command line

Add the foolowing commands to post-build event in project `Build Events` tab.

```
copy /Y "C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\redist\$(PlatformName)\Microsoft.VC140.CRT\msvcp140.dll" $(TargetDir)
copy /Y "C:\Program Files (x86)\Microsoft Visual Studio 14.0\VC\redist\$(PlatformName)\Microsoft.VC140.CRT\vcruntime140.dll" $(TargetDir)
```

## Resources

* [GitHub source code repository.](https://github.com/wieslawsoltes/Core2D)

## License

Core2D is licensed under the [MIT license](LICENSE.TXT).
