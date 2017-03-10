# Core2D

[![Gitter](https://badges.gitter.im/wieslawsoltes/Core2D.svg)](https://gitter.im/wieslawsoltes/Core2D?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

[![Build status](https://ci.appveyor.com/api/projects/status/7k1e0voeit7od9bw/branch/master?svg=true)](https://ci.appveyor.com/project/wieslawsoltes/core2d/branch/master)

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

* `Windows` 7/8/8.1/10 using `Core2D.Wpf` builds.

The core library and editor are portable and should work on all platforms where C# is supported.

## Building Core2D

First, clone the repository or download the latest zip.
```
git clone https://github.com/wieslawsoltes/Core2D.git
git submodule update --init --recursive
```

### Build using IDE

* [Visual Studio Community 2017](https://www.visualstudio.com/pl/vs/community/) for `Windows` builds.

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
