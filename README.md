# Core2D

[![Gitter](https://badges.gitter.im/Join%20Chat.svg)](https://gitter.im/Core2D/Core2D?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

A multi-platform data driven 2D diagram editor.

| Build Server                | Platform     | Status                                                                                                                                                                     |
|-----------------------------|--------------|----------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| AppVeyor                    | Windows      | [![Build status](https://ci.appveyor.com/api/projects/status/7k1e0voeit7od9bw/branch/master?svg=true)](https://ci.appveyor.com/project/wieslawsoltes/core2d/branch/master) |
| Travis                      | Linux / OS X | [![Build Status](https://travis-ci.org/Core2D/Core2D.svg?branch=master)](https://travis-ci.org/Core2D/Core2D)                                                              |

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
* `Android` support is planned using `Avalonia.Android`.
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

[Using Core2D as NuGet packages and other external dependencies.](https://github.com/Core2D/Core2D/blob/master/NuGet.md)

## Resources

* [Project website and API Reference.](http://core2d.github.io/)
* [GitHub source code repository.](https://github.com/Core2D/Core2D)

## License

Core2D is licensed under the [MIT license](LICENSE.TXT).
