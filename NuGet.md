# Core2D NuGet Packages and other external dependencies

## Table of Contents

1. [NuGet](NuGet.md#nuget)
2. [NuGet Packages](NuGet.md#nuget-packages)
3. [MyGet Packages](NuGet.md#myget-packages)
4. [Package Dependencies](NuGet.md#package-dependencies)
5. [Package Sources](NuGet.md#package-sources)
6. [SkiaSharp Dependencies](NuGet.md#skiasharp-dependencies)
7. [Other Dependencies](NuGet.md#other-dependencies)

## NuGet

Core2D core libraries are delivered as a NuGet package.

You can find the packages here [NuGet](https://www.nuget.org/packages/Core2D/) or by using nightly build feed:
* Add `https://www.myget.org/F/core2d-nightly/api/v2` to your package sources
* Update your package using `Core2D` feed

You can install the package like this:

`Install-Package Core2D -Pre`

## NuGet Packages

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
| Core2D.Utilities.Avalonia           | [![NuGet](https://img.shields.io/nuget/v/Core2D.Utilities.Avalonia.svg)](https://www.nuget.org/packages/Core2D.Utilities.Avalonia)                        | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.Utilities.Avalonia.svg)](https://www.nuget.org/packages/Core2D.Utilities.Avalonia)                        |
| Core2D.Utilities.Wpf                | [![NuGet](https://img.shields.io/nuget/v/Core2D.Utilities.Wpf.svg)](https://www.nuget.org/packages/Core2D.Utilities.Wpf)                                  | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.Utilities.Wpf.svg)](https://www.nuget.org/packages/Core2D.Utilities.Wpf)                                  |
| Core2D.VisioAutomation.VDX          | [![NuGet](https://img.shields.io/nuget/v/Core2D.VisioAutomation.VDX.svg)](https://www.nuget.org/packages/Core2D.VisioAutomation.VDX)                      | [![NuGet](https://img.shields.io/nuget/vpre/Core2D.VisioAutomation.VDX.svg)](https://www.nuget.org/packages/Core2D.VisioAutomation.VDX)                      |

## MyGet Packages

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
| Core2D.Utilities.Avalonia          | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.Utilities.Avalonia.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                 | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.Utilities.Avalonia.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                 |
| Core2D.Utilities.Wpf               | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.Utilities.Wpf.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                      | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.Utilities.Wpf.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                      |
| Core2D.VisioAutomation.VDX         | [![MyGet](https://img.shields.io/myget/core2d-nightly/v/Core2D.VisioAutomation.VDX.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                | [![MyGet](https://img.shields.io/myget/core2d-nightly/vpre/Core2D.VisioAutomation.VDX.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly)                |

## Package Dependencies

* Common
  * System.Collections.Immutable
  * System.Reactive.Core
  * System.Reactive.Interfaces
  * Portable.Xaml
  * Newtonsoft.Json
  * CsvHelper
  * SkiaSharp
* WPF
  * System.Reactive.Core
  * System.Reactive.Interfaces
  * System.Reactive.Linq
  * Xceed.Wpf.AvalonDock
  * Xceed.Products.Wpf.Toolkit.AvalonDock
  * System.Windows.Interactivity.WPF
  * Wpf.Controls.PanAndZoom
* Avalonia
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

## SkiaSharp Dependencies

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

## Other Dependencies

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
