# Core2D

[![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Core2D/Core2D?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

A multi-platform data driven 2D diagram editor.

| Build Server                | Platform     | Status                                                                                                                                                                     |
|-----------------------------|--------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| AppVeyor                    | Windows      | [![Build status](https://ci.appveyor.com/api/projects/status/7k1e0voeit7od9bw/branch/master?svg=true)](https://ci.appveyor.com/project/wieslawsoltes/core2d/branch/master) |
| Travis                      | Linux / OS X | [![Build Status](https://travis-ci.org/Core2D/Core2D.svg?branch=master)](https://travis-ci.org/Core2D/Core2D)                                                              |
| Bitrise                     | Android      | [![Build Status](https://www.bitrise.io/app/0eddf30a82243ed8.svg?token=IEGVKM7S6KBI3HdecpD8Cg&branch=master)](https://www.bitrise.io/app/0eddf30a82243ed8)                 |

## Install

| Package                     | Latest release                                                                                                                              | Pre-release                                                                                                                                  |
|-----------------------------|---------------------------------------------------------------------------------------------------------------------------------------------|---------------------------------------------------------------------------------------------------------------------------------------------|
| Core2D.Avalonia.Cairo       | [![Chocolatey](https://img.shields.io/chocolatey/vpre/Core2D.Avalonia.Cairo.svg)](https://chocolatey.org/packages/Core2D.Avalonia.Cairo)    | [![Chocolatey](https://img.shields.io/chocolatey/vpre/Core2D.Avalonia.Cairo.svg)](https://chocolatey.org/packages/Core2D.Avalonia.Cairo)    | 
| Core2D.Avalonia.Direct2D    | [![Chocolatey](https://img.shields.io/chocolatey/v/Core2D.Avalonia.Direct2D.svg)](https://chocolatey.org/packages/Core2D.Avalonia.Direct2D) | [![Chocolatey](https://img.shields.io/chocolatey/vpre/Core2D.Avalonia.Direct2D.svg)](https://chocolatey.org/packages/Core2D.Avalonia.Direct2D) |
| Core2D.Avalonia.Skia        | [![Chocolatey](https://img.shields.io/chocolatey/v/Core2D.Avalonia.Skia.svg)](https://chocolatey.org/packages/Core2D.Avalonia.Skia)         | [![Chocolatey](https://img.shields.io/chocolatey/vpre/Core2D.Avalonia.Skia.svg)](https://chocolatey.org/packages/Core2D.Avalonia.Skia)         |
| Core2D.Wpf                  | [![Chocolatey](https://img.shields.io/chocolatey/v/Core2D.Wpf.svg)](https://chocolatey.org/packages/Core2D.Wpf)                             | [![Chocolatey](https://img.shields.io/chocolatey/vpre/Core2D.Wpf.svg)](https://chocolatey.org/packages/Core2D.Wpf)                             |

## Table of Contents

1. [About](https://github.com/Core2D/Core2D#about)
2. [Documentation](https://github.com/Core2D/Core2D#documentation)
3. [Data Formats](https://github.com/Core2D/Core2D#data-formats)
4. [Supported Platforms](https://github.com/Core2D/Core2D#supported-platforms)
5. [Building Core2D](https://github.com/Core2D/Core2D#building-core2d)
    - [Build using IDE](https://github.com/Core2D/Core2D#build-using-ide)
    - [Build on Windows using script](https://github.com/Core2D/Core2D#build-on-windows-using-script)
    - [Build on Linux/OSX using script](https://github.com/Core2D/Core2D#build-on-linuxosx-using-script)
6. [NuGet](https://github.com/Core2D/Core2D#nuget)
    - [NuGet Packages](https://github.com/Core2D/Core2D#nuget-packages)
    - [MyGet Packages](https://github.com/Core2D/Core2D#myget-packages)
    - [Package Dependencies](https://github.com/Core2D/Core2D#package-dependencies)
    - [Package Sources](https://github.com/Core2D/Core2D#package-sources)
    - [Other Dependencies](https://github.com/Core2D/Core2D#other-dependencies)
7. [Resources](https://github.com/Core2D/Core2D#resources)
8. [License](https://github.com/Core2D/Core2D#license)

## About

Core2D is a multi-platform application for making data driven 2D diagrams.

<a href='https://www.youtube.com/watch?v=P7G0kmX7EcU' target='_blank'>![](https://i.ytimg.com/vi/P7G0kmX7EcU/hqdefault.jpg)<a/>

## Documentation

You can read the latest documentation at [http://core2d.github.io/](http://core2d.github.io/).

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

First, clone the repository or download the latest zip.
```
git clone https://github.com/Core2D/Core2D.git
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

## NuGet

Core2D core library is delivered as a NuGet package.

You can find the packages here [NuGet](https://www.nuget.org/packages/Core2D/) or by using nightly build feed:
* Add `https://www.myget.org/F/core2d-nightly/api/v2` to your package sources
* Update your package using `Core2D` feed

You can install the package like this:

`Install-Package Core2D -Pre`

### NuGet Packages

| Package                             | Latest release                                                                                                                                            | Pre-release                                                                                                                                                  |
|-------------------------------------|-----------------------------------------------------------------------------------------------------------------------------------------------------------|--------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Core2D                              | [![NuGet](https://img.shields.io/nuget/v/Core2D.svg)](https://www.nuget.org/packages/Core2D)                                                              | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.svg)](https://www.nuget.org/packages/Core2D)                                                              |
| Core2D.Avalonia                     | [![NuGet](https://img.shields.io/nuget/v/Core2D.Avalonia.svg)](https://www.nuget.org/packages/Core2D.Avalonia)                                            | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.Avalonia.svg)](https://www.nuget.org/packages/Core2D.Avalonia)                                            |
| Core2D.FileSystem.DotNetFx          | [![NuGet](https://img.shields.io/nuget/v/Core2D.FileSystem.DotNetFx.svg)](https://www.nuget.org/packages/Core2D.FileSystem.DotNetFx)                      | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.FileSystem.DotNetFx.svg)](https://www.nuget.org/packages/Core2D.FileSystem.DotNetFx)                      |
| Core2D.FileWriter.Dxf               | [![NuGet](https://img.shields.io/nuget/v/Core2D.FileWriter.Dxf.svg)](https://www.nuget.org/packages/Core2D.FileWriter.Dxf)                                | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.FileWriter.Dxf.svg)](https://www.nuget.org/packages/Core2D.FileWriter.Dxf)                                |
| Core2D.FileWriter.Emf               | [![NuGet](https://img.shields.io/nuget/v/Core2D.FileWriter.Emf.svg)](https://www.nuget.org/packages/Core2D.FileWriter.Emf)                                | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.FileWriter.Emf.svg)](https://www.nuget.org/packages/Core2D.FileWriter.Emf)                                |
| Core2D.FileWriter.PdfCore           | [![NuGet](https://img.shields.io/nuget/v/Core2D.FileWriter.PdfCore.svg)](https://www.nuget.org/packages/Core2D.FileWriter.PdfCore)                        | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.FileWriter.PdfCore.svg)](https://www.nuget.org/packages/Core2D.FileWriter.PdfCore)                        |
| Core2D.FileWriter.PdfSkiaSharp      | [![NuGet](https://img.shields.io/nuget/v/Core2D.FileWriter.PdfSkiaSharp.svg)](https://www.nuget.org/packages/Core2D.FileWriter.PdfSkiaSharp)              | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.FileWriter.PdfSkiaSharp.svg)](https://www.nuget.org/packages/Core2D.FileWriter.PdfSkiaSharp)              |
| Core2D.FileWriter.PdfWpf            | [![NuGet](https://img.shields.io/nuget/v/Core2D.FileWriter.PdfWpf.svg)](https://www.nuget.org/packages/Core2D.FileWriter.PdfWpf)                          | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.FileWriter.PdfWpf.svg)](https://www.nuget.org/packages/Core2D.FileWriter.PdfWpf)                          |
| Core2D.FileWriter.Vdx               | [![NuGet](https://img.shields.io/nuget/v/Core2D.FileWriter.Vdx.svg)](https://www.nuget.org/packages/Core2D.FileWriter.Vdx)                                | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.FileWriter.Vdx.svg)](https://www.nuget.org/packages/Core2D.FileWriter.Vdx)                                |
| Core2D.Log.Trace                    | [![NuGet](https://img.shields.io/nuget/v/Core2D.Log.Trace.svg)](https://www.nuget.org/packages/Core2D.Log.Trace)                                          | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.Log.Trace.svg)](https://www.nuget.org/packages/Core2D.Log.Trace)                                          |
| Core2D.NetDxf                       | [![NuGet](https://img.shields.io/nuget/v/Core2D.NetDxf.svg)](https://www.nuget.org/packages/Core2D.NetDxf)                                                | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.NetDxf.svg)](https://www.nuget.org/packages/Core2D.NetDxf)                                                |
| Core2D.PdfSharpCore                 | [![NuGet](https://img.shields.io/nuget/v/Core2D.PdfSharpCore.svg)](https://www.nuget.org/packages/Core2D.PdfSharpCore)                                    | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.PdfSharpCore.svg)](https://www.nuget.org/packages/Core2D.PdfSharpCore)                                    |
| Core2D.PdfSharpWpf                  | [![NuGet](https://img.shields.io/nuget/v/Core2D.PdfSharpWpf.svg)](https://www.nuget.org/packages/Core2D.PdfSharpWpf)                                      | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.PdfSharpWpf.svg)](https://www.nuget.org/packages/Core2D.PdfSharpWpf)                                      |
| Core2D.Renderer.Avalonia            | [![NuGet](https://img.shields.io/nuget/v/Core2D.Renderer.Avalonia.svg)](https://www.nuget.org/packages/Core2D.Renderer.Avalonia)                          | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.Renderer.Avalonia.svg)](https://www.nuget.org/packages/Core2D.Renderer.Avalonia)                          |
| Core2D.Renderer.Dxf                 | [![NuGet](https://img.shields.io/nuget/v/Core2D.Renderer.Dxf.svg)](https://www.nuget.org/packages/Core2D.Renderer.Dxf)                                    | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.Renderer.Dxf.svg)](https://www.nuget.org/packages/Core2D.Renderer.Dxf)                                    |
| Core2D.Renderer.PdfSharpCore        | [![NuGet](https://img.shields.io/nuget/v/Core2D.Renderer.PdfSharpCore.svg)](https://www.nuget.org/packages/Core2D.Renderer.PdfSharpCore)                  | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.Renderer.PdfSharpCore.svg)](https://www.nuget.org/packages/Core2D.Renderer.PdfSharpCore)                  |
| Core2D.Renderer.PdfSharpWpf         | [![NuGet](https://img.shields.io/nuget/v/Core2D.Renderer.PdfSharpWpf.svg)](https://www.nuget.org/packages/Core2D.Renderer.PdfSharpWpf)                    | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.Renderer.PdfSharpWpf.svg)](https://www.nuget.org/packages/Core2D.Renderer.PdfSharpWpf)                    |
| Core2D.Renderer.SkiaSharp           | [![NuGet](https://img.shields.io/nuget/v/Core2D.Renderer.SkiaSharp.svg)](https://www.nuget.org/packages/Core2D.Renderer.SkiaSharp)                        | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.Renderer.SkiaSharp.svg)](https://www.nuget.org/packages/Core2D.Renderer.SkiaSharp)                        |
| Core2D.Renderer.Vdx                 | [![NuGet](https://img.shields.io/nuget/v/Core2D.Renderer.Vdx.svg)](https://www.nuget.org/packages/Core2D.Renderer.Vdx)                                    | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.Renderer.Vdx.svg)](https://www.nuget.org/packages/Core2D.Renderer.Vdx)                                    |
| Core2D.Renderer.WinForms            | [![NuGet](https://img.shields.io/nuget/v/Core2D.Renderer.WinForms.svg)](https://www.nuget.org/packages/Core2D.Renderer.WinForms)                          | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.Renderer.WinForms.svg)](https://www.nuget.org/packages/Core2D.Renderer.WinForms)                          |
| Core2D.Renderer.Wpf                 | [![NuGet](https://img.shields.io/nuget/v/Core2D.Renderer.Wpf.svg)](https://www.nuget.org/packages/Core2D.Renderer.Wpf)                                    | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.Renderer.Wpf.svg)](https://www.nuget.org/packages/Core2D.Renderer.Wpf)                                    |
| Core2D.Serializer.Newtonsoft        | [![NuGet](https://img.shields.io/nuget/v/Core2D.Serializer.Newtonsoft.svg)](https://www.nuget.org/packages/Core2D.Serializer.Newtonsoft)                  | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.Serializer.Newtonsoft.svg)](https://www.nuget.org/packages/Core2D.Serializer.Newtonsoft)                  |
| Core2D.Serializer.Xaml              | [![NuGet](https://img.shields.io/nuget/v/Core2D.Serializer.Xaml.svg)](https://www.nuget.org/packages/Core2D.Serializer.Xaml)                              | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.Serializer.Xaml.svg)](https://www.nuget.org/packages/Core2D.Serializer.Xaml)                              |
| Core2D.TextFieldReader.CsvHelper    | [![NuGet](https://img.shields.io/nuget/v/Core2D.TextFieldReader.CsvHelper.svg)](https://www.nuget.org/packages/Core2D.TextFieldReader.CsvHelper)          | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.TextFieldReader.CsvHelper.svg)](https://www.nuget.org/packages/Core2D.TextFieldReader.CsvHelper)          |
| Core2D.TextFieldWriter.CsvHelper    | [![NuGet](https://img.shields.io/nuget/v/Core2D.TextFieldWriter.CsvHelper.svg)](https://www.nuget.org/packages/Core2D.TextFieldWriter.CsvHelper)          | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.TextFieldWriter.CsvHelper.svg)](https://www.nuget.org/packages/Core2D.TextFieldWriter.CsvHelper)          |
| Core2D.VisioAutomation.VDX          | [![NuGet](https://img.shields.io/nuget/v/Core2D.VisioAutomation.VDX.svg)](https://www.nuget.org/packages/Core2D.VisioAutomation.VDX)                      | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.VisioAutomation.VDX.svg)](https://www.nuget.org/packages/Core2D.VisioAutomation.VDX)                      |

### MyGet Packages

| Package                            | Latest release                                                                                                                                                    | Pre-release                                                                                                                                                          |
|------------------------------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Core2D                             | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                                    | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                                    |
| Core2D.Avalonia                    | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.Avalonia.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                           | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.Avalonia.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                           |
| Core2D.FileSystem.DotNetFx         | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.FileSystem.DotNetFx.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.FileSystem.DotNetFx.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                |
| Core2D.FileWriter.Dxf              | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.FileWriter.Dxf.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                     | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.FileWriter.Dxf.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                     |
| Core2D.FileWriter.Emf              | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.FileWriter.Emf.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                     | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.FileWriter.Emf.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                     |
| Core2D.FileWriter.PdfCore          | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.FileWriter.PdfCore.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                 | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.FileWriter.PdfCore.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                 |
| Core2D.FileWriter.PdfSkiaSharp     | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.FileWriter.PdfSkiaSharp.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)            | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.FileWriter.PdfSkiaSharp.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)            |
| Core2D.FileWriter.PdfWpf           | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.FileWriter.PdfWpf.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                  | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.FileWriter.PdfWpf.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                  |
| Core2D.FileWriter.Vdx              | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.FileWriter.Vdx.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                     | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.FileWriter.Vdx.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                     |
| Core2D.Log.Trace                   | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.Log.Trace.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                          | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.Log.Trace.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                          |
| Core2D.NetDxf                      | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.NetDxf.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                             | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.NetDxf.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                             |
| Core2D.PdfSharpCore                | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.PdfSharpCore.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                       | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.PdfSharpCore.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                       |
| Core2D.PdfSharpWpf                 | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.PdfSharpWpf.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                        | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.PdfSharpWpf.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                        |
| Core2D.Renderer.Avalonia           | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.Renderer.Avalonia.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                  | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.Renderer.Avalonia.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                  |
| Core2D.Renderer.Dxf                | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.Renderer.Dxf.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                       | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.Renderer.Dxf.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                       |
| Core2D.Renderer.PdfSharpCore       | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.Renderer.PdfSharpCore.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)              | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.Renderer.PdfSharpCore.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)              |
| Core2D.Renderer.PdfSharpWpf        | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.Renderer.PdfSharpWpf.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)               | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.Renderer.PdfSharpWpf.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)               |
| Core2D.Renderer.SkiaSharp          | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.Renderer.SkiaSharp.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                 | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.Renderer.SkiaSharp.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                 |
| Core2D.Renderer.Vdx                | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.Renderer.Vdx.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                       | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.Renderer.Vdx.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                       |
| Core2D.Renderer.WinForms           | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.Renderer.WinForms.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                  | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.Renderer.WinForms.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                  |
| Core2D.Renderer.Wpf                | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.Renderer.Wpf.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                       | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.Renderer.Wpf.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                       |
| Core2D.Serializer.Newtonsoft       | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.Serializer.Newtonsoft.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)              | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.Serializer.Newtonsoft.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)              |
| Core2D.Serializer.Xaml             | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.Serializer.Xaml.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                    | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.Serializer.Xaml.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                    |
| Core2D.TextFieldReader.CsvHelper   | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.TextFieldReader.CsvHelper.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)          | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.TextFieldReader.CsvHelper.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)          |
| Core2D.TextFieldWriter.CsvHelper   | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.TextFieldWriter.CsvHelper.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)          | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.TextFieldWriter.CsvHelper.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)          |
| Core2D.VisioAutomation.VDX         | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.VisioAutomation.VDX.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.VisioAutomation.VDX.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                |

### Package Dependencies

* Common
  * System.Collections.Immutable
  * System.Reactive.Core
  * System.Reactive.Interfaces
  * Portable.Xaml
  * Newtonsoft.Json
  * CsvHelper
  * SkiaSharp
* WPF
  * Xceed.Wpf.AvalonDock
  * Xceed.Products.Wpf.Toolkit.AvalonDock
  * System.Windows.Interactivity.WPF
  * Wpf.Controls.PanAndZoom
* Avalonia
  * Avalonia
  * Avalonia.Desktop
  * Avalonia.Skia.Desktop
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

### Package Sources

* https://api.nuget.org/v3/index.json
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
