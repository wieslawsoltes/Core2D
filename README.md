# Core2D

[![Gitter](https://badges.gitter.im/wieslawsoltes/Core2D.svg)](https://gitter.im/wieslawsoltes/Core2D?utm_source=badge&utm_medium=badge&utm_campaign=pr-badge)

[![Build status](https://ci.appveyor.com/api/projects/status/7k1e0voeit7od9bw/branch/master?svg=true)](https://ci.appveyor.com/project/wieslawsoltes/core2d/branch/master)
[![Build Status](https://travis-ci.org/wieslawsoltes/Core2D.svg?branch=master)](https://travis-ci.org/wieslawsoltes/Core2D)
[![CircleCI](https://circleci.com/gh/wieslawsoltes/Core2D/tree/master.svg?style=svg)](https://circleci.com/gh/wieslawsoltes/Core2D/tree/master)

[![NuGet](https://img.shields.io/nuget/v/Core2D.svg)](https://www.nuget.org/packages/Core2D)
[![MyGet](https://img.shields.io/myget/dock-nightly/vpre/Core2D.svg?label=myget)](https://www.myget.org/gallery/core2d-nightly) 

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
* The clipboard data is stored as `Json` string.

## Supported Platforms

* `Windows 7 SP1 or higher` using `Core2D.Wpf` or `Core2D.Avalonia` project.
* `Linux` using `Core2D.Avalonia` project.
* `macOS` using `Core2D.Avalonia` project.
* `.NET Core` supported platforms using `Core2D.Avalonia` project.

The core libraries are portable and should work on all platforms where C# is supported.

## Keyboard Shortcuts

### MainControl

#### File

| Shortcut          | Description                                                                   |
|-------------------|-------------------------------------------------------------------------------|
| Ctrl+N            | Create new project, document or page.                                         |
| Ctrl+O            | Open project.                                                                 |
| Ctrl+F4           | Close project.                                                                |
| Ctrl+S            | Save project.                                                                 |
| Ctrl+Shift+S      | Save project as.                                                              |
| Ctrl+Shift+X      | Import xaml.                                                                  |
| Ctrl+Shift+J      | Import json.                                                                  |
| Ctrl+E            | Export project, document or page.                                             |
| Alt+F4            | Close application view.                                                       |

#### Edit

| Shortcut          | Description                                                                   |
|-------------------|-------------------------------------------------------------------------------|
| Ctrl+Z            | Undo last action.                                                             |
| Ctrl+Y            | Redo last action.                                                             |
| Ctrl+Shift+C      | Copy page or selected shapes to clipboard as Emf.                             |
| Ctrl+X            | Cut selected document, page or shapes.                                        |
| Ctrl+C            | Copy document, page or shapes to clipboard.                                   |
| Ctrl+V            | Paste text from clipboard as document, page or shapes.                        |
| Ctrl+A            | Select all shapes.                                                            |
| Ctrl+G            | Group selected shapes.                                                        |
| Ctrl+U            | Ungroup selected shapes.                                                      |

#### View

| Shortcut          | Description                                                                   |
|-------------------|-------------------------------------------------------------------------------|
| Ctrl+J            | Show object browser.                                                          |
| Ctrl+D            | Show document viewer.                                                         |
| Ctrl+R            | Show script editor.                                                           |

### ContainerControl

#### Edit

| Shortcut          | Description                                                                   |
|-------------------|-------------------------------------------------------------------------------|
| Delete            | Delete selected document, page, layer or shapes.                              |
| Escape            | De-select all shapes.                                                         |
| Shift+-           | Bring selected shapes to the bottom of the stack.                             |
| Shift++           | Bring selected shapes to the top of the stack.                                |
| -                 | Bring selected shapes one step down within the stack.                         |
| +                 | Bring selected shapes one step closer to the front of the stack.              |
| Up                | Move selected shapes up.                                                      |
| Down              | Move selected shapes down.                                                    |
| Left              | Move selected shapes left.                                                    |
| Right             | Move selected shapes right.                                                   |

#### View

| Shortcut          | Description                                                                   |
|-------------------|-------------------------------------------------------------------------------|
| Z                 | Reset view size to defaults.                                                  |
| X                 | Auto-fit view to the available extents.                                       |

#### Tool

| Shortcut          | Description                                                                   |
|-------------------|-------------------------------------------------------------------------------|
| N                 | Set current tool to `None`.                                                   |
| S                 | Set current tool to `Selection`.                                              |
| P                 | Set current tool to `Point`.                                                  |
| L                 | Set current tool or path tool to `Line`.                                      |
| A                 | Set current tool or path tool to `Arc`.                                       |
| B                 | Set current tool or path tool to `CubicBezier`.                               |
| Q                 | Set current tool or path tool to `QuadraticBezier`.                           |
| H                 | Set current tool to `Path`.                                                   |
| M                 | Set current path tool to `Move`.                                              |
| R                 | Set current tool to `Rectangle`.                                              |
| E                 | Set current tool to `Ellipse`.                                                |
| T                 | Set current tool to `Text`.                                                   |
| I                 | Set current tool to `Image`.                                                  |

#### Options

| Shortcut          | Description                                                                   |
|-------------------|-------------------------------------------------------------------------------|
| K                 | Toggle `DefaultIsStroked` option.                                             |
| F                 | Toggle `DefaultIsFilled` option.                                              |
| D                 | Toggle `DefaultIsClosed` option.                                              |
| J                 | Toggle `DefaultIsSmoothJoin` option.                                          |
| G                 | Toggle `SnapToGrid` option.                                                   |
| C                 | Toggle `TryToConnect` option.                                                 |
| Y                 | Toggle `CloneStyle` option.                                                   |

### BrowserControl

#### Tree

| Shortcut          | Description                                                                   |
|-------------------|-------------------------------------------------------------------------------|
| Ctrl+E            | Export object.                                                                |

#### Xaml Text

| Shortcut          | Description                                                                   |
|-------------------|-------------------------------------------------------------------------------|
| Ctrl+E            | Export xaml.                                                                  |

#### Json Text

| Shortcut          | Description                                                                   |
|-------------------|-------------------------------------------------------------------------------|
| Ctrl+E            | Export json.                                                                  |

### GroupsControl

#### List

| Shortcut          | Description                                                                   |
|-------------------|-------------------------------------------------------------------------------|
| Ctrl+N            | Add group.                                                                    |
| Delete            | Remove group.                                                                 |
| Ctrl+E            | Export group.                                                                 |

### LayersControl

#### List

| Shortcut          | Description                                                                   |
|-------------------|-------------------------------------------------------------------------------|
| Ctrl+N            | Add layer to container.                                                       |
| Delete            | Remove layer.                                                                 |

### ProjectControl

#### Tree

| Shortcut          | Description                                                                   |
|-------------------|-------------------------------------------------------------------------------|
| Ctrl+N            | Create new project, document or page.                                         |
| Ctrl+X            | Cut selected document, page.                                                  |
| Ctrl+C            | Copy document, page to clipboard.                                             |
| Ctrl+V            | Paste text from clipboard as document, page.                                  |
| Delete            | Delete selected document, page.                                               |
| Ctrl+E            | Export project, document or page.                                             |

### ShapesControl

#### List

| Shortcut          | Description                                                                   |
|-------------------|-------------------------------------------------------------------------------|
| Delete            | Remove shape.                                                                 |

### StylesControl

#### List

| Shortcut          | Description                                                                   |
|-------------------|-------------------------------------------------------------------------------|
| Ctrl+N            | Add style.                                                                    |
| Delete            | Remove style.                                                                 |
| Ctrl+E            | Export style.                                                                 |

### TemplatesControl

#### List

| Shortcut          | Description                                                                   |
|-------------------|-------------------------------------------------------------------------------|
| Ctrl+N            | Set page template.                                                            |
| Ctrl+T            | Edit template.                                                                |
| Delete            | Remove template.                                                              |
| Ctrl+E            | Export template.                                                              |

### RecordsControl

#### List

| Shortcut          | Description                                                                   |
|-------------------|-------------------------------------------------------------------------------|
| Ctrl+N            | Add record to database.                                                       |
| Delete            | Remove record from database.                                                  |

### ScriptWindow

#### List

| Shortcut          | Description                                                                   |
|-------------------|-------------------------------------------------------------------------------|
| Ctrl+Enter        | Execute code script in repl.                                                  |

## Downloads

| Version               | Framework      | Download                                                                                                                                                                                              |
|-----------------------|----------------|-------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------|
| Windows 7 8.1 10 x64  | netcoreapp2.1  | [Core2D.Avalonia-netcoreapp2.1-win7-x64.zip](https://ci.appveyor.com/api/projects/wieslawsoltes/Core2D/artifacts/artifacts/Core2D.Avalonia-netcoreapp2.1-win7-x64.zip?branch=master)                  |

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
PS> .\build.ps1 -BuildSources -TestSources -PackSources -BuildApps -PublishApps -CopyRedist -ZipApps
```

### Build on Linux/OSX using script

Open up a terminal prompt and execute the bootstrapper script:
```PowerShell
$ pwsh -File build.ps1 -BuildSources -TestSources -BuildApps -DisabledFrameworks net461
```

### Build using .NET Core

* [.NET Core](https://www.microsoft.com/net/download/core) for `Windows`, `Linux` and `macOS` builds.

Open up a Powershell prompt and execute:
```PowerShell
cd src/Core2D.Avalonia
dotnet restore
dotnet build
```

Open up a terminal prompt and execute:
```Bash
cd src/Core2D.Avalonia
dotnet restore
dotnet build
```

### Publishing self-contained .NET Core application

You can publish self-contained `Core2D` application and all of its dependencies for one of the [.NET Core supported runtimes](https://docs.microsoft.com/en-us/dotnet/articles/core/rid-catalog). Below are few command-line examples.

Open up a Powershell prompt and execute:
```PowerShell
cd src/Core2D.Avalonia
dotnet restore
dotnet publish -r win7-x64 -o win7-x64
```

Open up a Powershell prompt and execute:
```PowerShell
cd src/Core2D.Avalonia
dotnet restore
dotnet publish -r win7-x86 -o win7-x86
```

Open up a terminal prompt and execute:
```Bash
cd src/Core2D.Avalonia
dotnet restore
dotnet publish -r ubuntu.14.04-x64 -o ubuntu.14.04-x64
```

## Package Sources

* https://api.nuget.org/v3/index.json
* https://www.myget.org/F/avalonia-ci/api/v2
* https://www.myget.org/F/xamlbehaviors-nightly/api/v2
* https://www.myget.org/F/panandzoom-nightly/api/v2
* https://www.myget.org/F/dock-nightly/api/v2
* https://ci.appveyor.com/nuget/portable-xaml

## SkiaSharp

The `libSkiaSharp.dll` from SkiaSharp package requires [Microsoft Visual C++ 2015 Redistributable](https://www.microsoft.com/en-us/download/details.aspx?id=52982) installed or included as part of distribution. License terms for redistributable
[MICROSOFT SOFTWARE LICENSE TERMS, MICROSOFT VISUAL STUDIO COMMUNITY 2015](https://www.visualstudio.com/en-us/support/legal/mt171547) and information about [Distributable Code for Microsoft Visual Studio 2015](https://www.visualstudio.com/en-us/downloads/2015-redistribution-vs.aspx).

### Required Visual C++ Runtime Files

Projects referencing `SkiaSharp` require `Visual C++ Runtime Files` from `Visual Studio Community 2017`.

#### x86 Platform

```
c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Redist\MSVC\14.14.26405\x86\Microsoft.VC150.CRT\msvcp140.dll
c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Redist\MSVC\14.14.26405\x86\Microsoft.VC150.CRT\vcruntime140.dll
```

#### x64 Platform

```
c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Redist\MSVC\14.14.26405\x64\Microsoft.VC150.CRT\msvcp140.dll
c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Redist\MSVC\14.14.26405\x64\Microsoft.VC150.CRT\vcruntime140.dll
```

## Resources

* [GitHub source code repository.](https://github.com/wieslawsoltes/Core2D)

## License

Core2D is licensed under the [MIT license](LICENSE.TXT).
