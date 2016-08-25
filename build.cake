///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////

#addin "nuget:?package=Polly&version=4.2.0"

///////////////////////////////////////////////////////////////////////////////
// TOOLS
///////////////////////////////////////////////////////////////////////////////

#tool "nuget:?package=xunit.runner.console&version=2.1.0"

///////////////////////////////////////////////////////////////////////////////
// USINGS
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using Polly;

///////////////////////////////////////////////////////////////////////////////
// ARGUMENTS
///////////////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var platform = Argument("platform", "AnyCPU");
var configuration = Argument("configuration", "Release");

///////////////////////////////////////////////////////////////////////////////
// CONFIGURATION
///////////////////////////////////////////////////////////////////////////////

var MainRepo = "Core2D/Core2D";
var MasterBranch = "master";
var AssemblyInfoPath = File("./src/Core2D.Shared/SharedAssemblyInfo.cs");
var ReleasePlatform = "AnyCPU";
var ReleaseConfiguration = "Release";
var MSBuildSolution = "./Core2D.sln";
var XBuildSolution = "./Core2D.mono.sln";

///////////////////////////////////////////////////////////////////////////////
// PARAMETERS
///////////////////////////////////////////////////////////////////////////////

var isPlatformAnyCPU = StringComparer.OrdinalIgnoreCase.Equals(platform, "AnyCPU");
var isPlatformX86 = StringComparer.OrdinalIgnoreCase.Equals(platform, "x86");
var isPlatformX64 = StringComparer.OrdinalIgnoreCase.Equals(platform, "x64");
var isLocalBuild = BuildSystem.IsLocalBuild;
var isRunningOnUnix = IsRunningOnUnix();
var isRunningOnWindows = IsRunningOnWindows();
var isRunningOnAppVeyor = BuildSystem.AppVeyor.IsRunningOnAppVeyor;
var isPullRequest = BuildSystem.AppVeyor.Environment.PullRequest.IsPullRequest;
var isMainRepo = StringComparer.OrdinalIgnoreCase.Equals(MainRepo, BuildSystem.AppVeyor.Environment.Repository.Name);
var isMasterBranch = StringComparer.OrdinalIgnoreCase.Equals(MasterBranch, BuildSystem.AppVeyor.Environment.Repository.Branch);
var isTagged = BuildSystem.AppVeyor.Environment.Repository.Tag.IsTag 
               && !string.IsNullOrWhiteSpace(BuildSystem.AppVeyor.Environment.Repository.Tag.Name);
var isReleasable = StringComparer.OrdinalIgnoreCase.Equals(ReleasePlatform, platform) 
                   && StringComparer.OrdinalIgnoreCase.Equals(ReleaseConfiguration, configuration);
var isMyGetRelease = !isTagged && isReleasable && isPlatformAnyCPU;
var isNuGetRelease = isTagged && isReleasable && isPlatformAnyCPU;

///////////////////////////////////////////////////////////////////////////////
// VERSION
///////////////////////////////////////////////////////////////////////////////

var version = ParseAssemblyInfo(AssemblyInfoPath).AssemblyVersion;

if (isRunningOnAppVeyor)
{
    if (isTagged)
    {
        // Use Tag Name as version
        version = BuildSystem.AppVeyor.Environment.Repository.Tag.Name;
    }
    else
    {
        // Use AssemblyVersion with Build as version
        version += "-build" + EnvironmentVariable("APPVEYOR_BUILD_NUMBER") + "-alpha";
    }
}

///////////////////////////////////////////////////////////////////////////////
// DIRECTORIES
///////////////////////////////////////////////////////////////////////////////

var artifactsDir = (DirectoryPath)Directory("./artifacts");
var testResultsDir = artifactsDir.Combine("test-results");
var nugetRoot = artifactsDir.Combine("nuget");
var chocolateyRoot = artifactsDir.Combine("chocolatey");
var zipRoot = artifactsDir.Combine("zip");
var binRoot = artifactsDir.Combine("bin");

var dirSuffix = platform + "/" + configuration;
var dirSuffixSkia = (isPlatformAnyCPU ? "x86" : platform) + "/" + configuration;

Func<IFileSystemInfo, bool> ExcludeSkia = i => {
    return !(i.Path.FullPath.IndexOf("Skia", StringComparison.OrdinalIgnoreCase) >= 0);
};

Func<string, DirectoryPathCollection> GetSkiaDirectories = pattern => {
    return GetDirectories(pattern) - GetDirectories(pattern, ExcludeSkia);
};

var buildDirs = 
    GetDirectories("./src/**/bin/" + dirSuffix, ExcludeSkia) + 
    GetSkiaDirectories("./src/**/bin/" + dirSuffixSkia) +
    GetDirectories("./src/**/obj/" + dirSuffix, ExcludeSkia) + 
    GetSkiaDirectories("./src/**/obj/" + dirSuffixSkia) + 
    GetDirectories("./dependencies/**/bin/" + dirSuffix, ExcludeSkia) + 
    GetSkiaDirectories("./dependencies/**/bin/" + dirSuffixSkia) + 
    GetDirectories("./dependencies/**/obj/" + dirSuffix, ExcludeSkia) + 
    GetSkiaDirectories("./dependencies/**/obj/" + dirSuffixSkia) + 
    GetDirectories("./tests/**/bin/" + dirSuffix, ExcludeSkia) + 
    GetSkiaDirectories("./tests/**/bin/" + dirSuffixSkia) + 
    GetDirectories("./tests/**/obj/" + dirSuffix, ExcludeSkia) + 
    GetSkiaDirectories("./tests/**/obj/" + dirSuffixSkia);

var fileZipSuffix = platform + "-" + configuration + "-" + version + ".zip";
var fileZipSuffixSkia = (isPlatformAnyCPU ? "x86" : platform) + "-" + configuration + "-" + version + ".zip";

var zipCoreArtifacts = zipRoot.CombineWithFilePath("Core2D-" + fileZipSuffix);

var zipSourceCairoDirs = (DirectoryPath)Directory("./src/Core2D.Avalonia.Cairo/bin/" + dirSuffix);
var zipSourceDirect2DDirs = (DirectoryPath)Directory("./src/Core2D.Avalonia.Direct2D/bin/" + dirSuffix);
var zipSourceSkiaDirs = (DirectoryPath)Directory("./src/Core2D.Avalonia.Skia/bin/" + dirSuffixSkia);
var zipSourceWpfDirs = (DirectoryPath)Directory("./src/Core2D.Wpf/bin/" + dirSuffix);

var zipTargetCairoDirs = zipRoot.CombineWithFilePath("Core2D.Avalonia.Cairo-" + fileZipSuffix);
var zipTargetDirect2DDirs = zipRoot.CombineWithFilePath("Core2D.Avalonia.Direct2D-" + fileZipSuffix);
var zipTargetSkiaDirs = zipRoot.CombineWithFilePath("Core2D.Avalonia.Skia-" + fileZipSuffixSkia);
var zipTargetWpfDirs = zipRoot.CombineWithFilePath("Core2D.Wpf-" + fileZipSuffix);

///////////////////////////////////////////////////////////////////////////////
// NUGET NUSPECS
///////////////////////////////////////////////////////////////////////////////

var SystemCollectionsImmutableVersion = "1.2.0";
var SystemReactiveVersion = "3.0.0";
var NewtonsoftJsonVersion = "9.0.1";
var PortableXamlVersion = "0.14.0";
var CsvHelperVersion = "2.16.0.0";
var AvaloniaVersion = "0.4.1-build1936-alpha";
var AvaloniaXamlBehaviorsVersion = "0.4.1-build301-alpha";
var AvaloniaControlsPanAndZoomVersion = "0.4.1-build35-alpha";
var SkiaSharpVersion = "1.53.0";

var SetNuGetNuspecCommonProperties = new Action<NuGetPackSettings> ((nuspec) => {
    nuspec.Version = version;
    nuspec.Authors = new [] { "wieslaw.soltes" };
    nuspec.Owners = new [] { "wieslaw.soltes" };
    nuspec.LicenseUrl = new Uri("http://opensource.org/licenses/MIT");
    nuspec.ProjectUrl = new Uri("https://github.com/Core2D/Core2D/");
    nuspec.RequireLicenseAcceptance = false;
    nuspec.Symbols = false;
    nuspec.NoPackageAnalysis = true;
    nuspec.Description = "A multi-platform data driven 2D diagram editor.";
    nuspec.Copyright = "Copyright 2016";
    nuspec.Tags = new [] { "Core2D", "Diagram", "Editor", "2D", "Graphics", "Drawing", "Data", "Managed", "C#" };
});

var nuspecNuGetSettingsCore = new []
{
    ///////////////////////////////////////////////////////////////////////////////
    // src: Core2D
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "System.Collections.Immutable", Version = SystemCollectionsImmutableVersion },
            new NuSpecDependency() { Id = "System.Reactive.Interfaces", Version = SystemReactiveVersion },
            new NuSpecDependency() { Id = "System.Reactive.Core", Version = SystemReactiveVersion }
        },
        Files = new []
        {
            new NuSpecContent { Source = "Core2D.dll", Target = "lib/portable-windows8+net45" }
        },
        BasePath = Directory("./src/Core2D/bin/" + dirSuffix),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // src: Core2D.Avalonia
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.Avalonia",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "System.Collections.Immutable", Version = SystemCollectionsImmutableVersion },
            new NuSpecDependency() { Id = "Core2D", Version = version },
            new NuSpecDependency() { Id = "Utilities.Avalonia", Version = version },
            new NuSpecDependency() { Id = "Avalonia", Version = AvaloniaVersion },
            new NuSpecDependency() { Id = "Avalonia.Xaml.Behaviors", Version = AvaloniaXamlBehaviorsVersion },
            new NuSpecDependency() { Id = "Avalonia.Controls.PanAndZoom", Version = AvaloniaControlsPanAndZoomVersion }
        },
        Files = new []
        {
            new NuSpecContent { Source = "Core2D.Avalonia.dll", Target = "lib/portable-windows8+net45" }
        },
        BasePath = Directory("./src/Core2D.Avalonia/bin/" + dirSuffix),
        OutputDirectory = nugetRoot
    }
};

var nuspecNuGetSettingsDependencies = new []
{
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: FileSystem.DotNetFx
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.FileSystem.DotNetFx",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "Core2D", Version = version }
        },
        Files = new []
        {
            new NuSpecContent { Source = "FileSystem.DotNetFx.dll", Target = "lib/net45" }
        },
        BasePath = Directory("./dependencies/FileSystem.DotNetFx/bin/" + dirSuffix),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: FileWriter.Dxf
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.FileWriter.Dxf",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "System.Collections.Immutable", Version = SystemCollectionsImmutableVersion },
            new NuSpecDependency() { Id = "Core2D", Version = version },
            new NuSpecDependency() { Id = "Core2D.Renderer.Dxf", Version = version }
        },
        Files = new []
        {
            new NuSpecContent { Source = "FileWriter.Dxf.dll", Target = "lib/net45" }
        },
        BasePath = Directory("./dependencies/FileWriter.Dxf/bin/" + dirSuffix),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: FileWriter.Emf
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.FileWriter.Emf",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "System.Collections.Immutable", Version = SystemCollectionsImmutableVersion },
            new NuSpecDependency() { Id = "Core2D", Version = version },
            new NuSpecDependency() { Id = "Core2D.Renderer.WinForms", Version = version }
        },
        Files = new []
        {
            new NuSpecContent { Source = "FileWriter.Emf.dll", Target = "lib/net45" }
        },
        BasePath = Directory("./dependencies/FileWriter.Emf/bin/" + dirSuffix),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: FileWriter.Pdf_core
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.FileWriter.PdfCore",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "System.Collections.Immutable", Version = SystemCollectionsImmutableVersion },
            new NuSpecDependency() { Id = "Core2D", Version = version },
            new NuSpecDependency() { Id = "Core2D.Renderer.PdfSharpCore", Version = version }
        },
        Files = new []
        {
            new NuSpecContent { Source = "FileWriter.Pdf-core.dll", Target = "lib/net45" }
        },
        BasePath = Directory("./dependencies/FileWriter.Pdf-core/bin/" + dirSuffix),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: FileWriter.Pdf_wpf
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.FileWriter.PdfWpf",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "System.Collections.Immutable", Version = SystemCollectionsImmutableVersion },
            new NuSpecDependency() { Id = "Core2D", Version = version },
            new NuSpecDependency() { Id = "Core2D.Renderer.PdfSharpWpf", Version = version }
        },
        Files = new []
        {
            new NuSpecContent { Source = "FileWriter.Pdf-wpf.dll", Target = "lib/net45" }
        },
        BasePath = Directory("./dependencies/FileWriter.Pdf-wpf./bin/" + dirSuffix),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: FileWriter.Vdx
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.FileWriter.Vdx",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "System.Collections.Immutable", Version = SystemCollectionsImmutableVersion },
            new NuSpecDependency() { Id = "Core2D", Version = version },
            new NuSpecDependency() { Id = "Core2D.Renderer.Vdx", Version = version }
        },
        Files = new []
        {
            new NuSpecContent { Source = "FileWriter.Vdx.dll", Target = "lib/net45" }
        },
        BasePath = Directory("./dependencies/FileWriter.Vdx/bin/" + dirSuffix),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: Log.Trace
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.Log.Trace",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "System.Collections.Immutable", Version = SystemCollectionsImmutableVersion },
            new NuSpecDependency() { Id = "Core2D", Version = version }
        },
        Files = new []
        {
            new NuSpecContent { Source = "Log.Trace.dll", Target = "lib/net45" }
        },
        BasePath = Directory("./dependencies/Log.Trace/bin/" + dirSuffix),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: Renderer.Avalonia
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.Renderer.Avalonia",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "System.Collections.Immutable", Version = SystemCollectionsImmutableVersion },
            new NuSpecDependency() { Id = "Core2D", Version = version },
            new NuSpecDependency() { Id = "Avalonia", Version = AvaloniaVersion },
            new NuSpecDependency() { Id = "Avalonia.Controls.PanAndZoom", Version = AvaloniaControlsPanAndZoomVersion }
        },
        Files = new []
        {
            new NuSpecContent { Source = "Renderer.Avalonia.dll", Target = "portable-windows8+net45" }
        },
        BasePath = Directory("./dependencies/Renderer.Avalonia/bin/" + dirSuffix),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: Renderer.Dxf
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.Renderer.Dxf",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "System.Collections.Immutable", Version = SystemCollectionsImmutableVersion },
            new NuSpecDependency() { Id = "Core2D", Version = version },
            new NuSpecDependency() { Id = "Core2D.NetDxf", Version = version }
        },
        Files = new []
        {
            new NuSpecContent { Source = "Renderer.Dxf.dll", Target = "lib/net45" }
        },
        BasePath = Directory("./dependencies/Renderer.Dxf/bin/" + dirSuffix),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: Renderer.PdfSharp_core
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.Renderer.PdfSharpCore",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "System.Collections.Immutable", Version = SystemCollectionsImmutableVersion },
            new NuSpecDependency() { Id = "Core2D", Version = version },
            new NuSpecDependency() { Id = "Core2D.PdfSharpCore", Version = version }
        },
        Files = new []
        {
            new NuSpecContent { Source = "Renderer.PdfSharp-core.dll", Target = "lib/net45" }
        },
        BasePath = Directory("./dependencies/Renderer.PdfSharp-core/bin/" + dirSuffix),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: Renderer.PdfSharp-wpf
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.Renderer.PdfSharpWpf",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "System.Collections.Immutable", Version = SystemCollectionsImmutableVersion },
            new NuSpecDependency() { Id = "Core2D", Version = version },
            new NuSpecDependency() { Id = "Core2D.PdfSharpWpf", Version = version }
        },
        Files = new []
        {
            new NuSpecContent { Source = "Renderer.PdfSharp-wpf.dll", Target = "lib/net45" }
        },
        BasePath = Directory("./dependencies/Renderer.PdfSharp-wpf/bin/" + dirSuffix),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: Renderer.Vdx
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.Renderer.Vdx",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "System.Collections.Immutable", Version = SystemCollectionsImmutableVersion },
            new NuSpecDependency() { Id = "Core2D", Version = version },
            new NuSpecDependency() { Id = "Core2D.VisioAutomation.VDX", Version = version }
        },
        Files = new []
        {
            new NuSpecContent { Source = "Renderer.Vdx.dll", Target = "lib/net45" }
        },
        BasePath = Directory("./dependencies/Renderer.Vdx/bin/" + dirSuffix),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: Renderer.WinForms
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.Renderer.WinForms",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "System.Collections.Immutable", Version = SystemCollectionsImmutableVersion },
            new NuSpecDependency() { Id = "Core2D", Version = version }
        },
        Files = new []
        {
            new NuSpecContent { Source = "Renderer.WinForms.dll", Target = "lib/net45" }
        },
        BasePath = Directory("./dependencies/Renderer.WinForms/bin/" + dirSuffix),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: Renderer.Wpf
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.Renderer.Wpf",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "System.Collections.Immutable", Version = SystemCollectionsImmutableVersion },
            new NuSpecDependency() { Id = "Core2D", Version = version }
        },
        Files = new []
        {
            new NuSpecContent { Source = "Renderer.Wpf.dll", Target = "lib/net45" }
        },
        BasePath = Directory("./dependencies/Renderer.Wpf/bin/" + dirSuffix),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: Serializer.Newtonsoft
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.Serializer.Newtonsoft",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "System.Collections.Immutable", Version = SystemCollectionsImmutableVersion },
            new NuSpecDependency() { Id = "Core2D", Version = version },
            new NuSpecDependency() { Id = "Newtonsoft.Json", Version = NewtonsoftJsonVersion }
        },
        Files = new []
        {
            new NuSpecContent { Source = "Serializer.Newtonsoft.dll", Target = "lib/portable-windows8+net45" }
        },
        BasePath = Directory("./dependencies/Serializer.Newtonsoft/bin/" + dirSuffix),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: Serializer.Xaml
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.Serializer.Xaml",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "System.Collections.Immutable", Version = SystemCollectionsImmutableVersion },
            new NuSpecDependency() { Id = "Core2D", Version = version },
            new NuSpecDependency() { Id = "Portable.Xaml", Version = PortableXamlVersion }
        },
        Files = new []
        {
            new NuSpecContent { Source = "Serializer.Xaml.dll", Target = "lib/portable-windows8+net45" }
        },
        BasePath = Directory("./dependencies/Serializer.Xaml/bin/" + dirSuffix),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: TextFieldReader.CsvHelper
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.TextFieldReader.CsvHelper",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "System.Collections.Immutable", Version = SystemCollectionsImmutableVersion },
            new NuSpecDependency() { Id = "Core2D", Version = version },
            new NuSpecDependency() { Id = "CsvHelper", Version = CsvHelperVersion }
        },
        Files = new []
        {
            new NuSpecContent { Source = "TextFieldReader.CsvHelper.dll", Target = "lib/portable-windows8+net45" }
        },
        BasePath = Directory("./dependencies/TextFieldReader.CsvHelper/bin/" + dirSuffix),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: TextFieldWriter.CsvHelper
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.TextFieldWriter.CsvHelper",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "System.Collections.Immutable", Version = SystemCollectionsImmutableVersion },
            new NuSpecDependency() { Id = "Core2D", Version = version },
            new NuSpecDependency() { Id = "CsvHelper", Version = CsvHelperVersion }
        },
        Files = new []
        {
            new NuSpecContent { Source = "TextFieldWriter.CsvHelper.dll", Target = "lib/portable-windows8+net45" }
        },
        BasePath = Directory("./dependencies/TextFieldWriter.CsvHelper/bin/" + dirSuffix),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: Utilities.Avalonia
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.Utilities.Avalonia",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "Core2D", Version = version },
            new NuSpecDependency() { Id = "System.Reactive.Interfaces", Version = SystemReactiveVersion },
            new NuSpecDependency() { Id = "System.Reactive.Core", Version = SystemReactiveVersion },
            new NuSpecDependency() { Id = "System.Reactive.Linq", Version = SystemReactiveVersion },
            new NuSpecDependency() { Id = "Avalonia", Version = AvaloniaVersion },
        },
        Files = new []
        {
            new NuSpecContent { Source = "Utilities.Avalonia.dll", Target = "lib/portable-windows8+net45" }
        },
        BasePath = Directory("./dependencies/Utilities.Avalonia/bin/" + dirSuffix),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: Utilities.Wpf
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.Utilities.Wpf",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "Core2D", Version = version },
            new NuSpecDependency() { Id = "System.Reactive.Interfaces", Version = SystemReactiveVersion },
            new NuSpecDependency() { Id = "System.Reactive.Core", Version = SystemReactiveVersion },
            new NuSpecDependency() { Id = "System.Reactive.Linq", Version = SystemReactiveVersion }
        },
        Files = new []
        {
            new NuSpecContent { Source = "Utilities.Wpf.dll", Target = "lib/net45" }
        },
        BasePath = Directory("./dependencies/Utilities.Wpf/bin/" + dirSuffix),
        OutputDirectory = nugetRoot
    }
};

var nuspecNuGetSettingsDependenciesModules = new []
{
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: PdfSharpCore
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.PdfSharpCore",
        Files = new []
        {
            new NuSpecContent { Source = "PdfSharp.dll", Target = "lib/net45" }
        },
        BasePath = Directory("./dependencies/PDFsharp/src/PdfSharp/bin/" + configuration),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: PdfSharp-wpf
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.PdfSharpWpf",
        Files = new []
        {
            new NuSpecContent { Source = "PdfSharp-wpf.dll", Target = "lib/net45" }
        },
        BasePath = Directory("./dependencies/PDFsharp/src/PdfSharp-wpf/bin/" + configuration),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: VisioAutomation.VDX
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.VisioAutomation.VDX",
        Files = new []
        {
            new NuSpecContent { Source = "VisioAutomation.VDX.dll", Target = "lib/net45" }
        },
        BasePath = Directory("./dependencies/VisioAutomation.VDX/VisioAutomation.VDX/bin/" + configuration),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: netDxf
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.NetDxf",
        Files = new []
        {
            new NuSpecContent { Source = "netDxf.dll", Target = "lib/net45" }
        },
        BasePath = Directory("./dependencies/netDxf/netDxf/bin/" + configuration),
        OutputDirectory = nugetRoot
    }
};

var nuspecNuGetSettingsDependenciesSkia = new []
{
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: FileWriter.PdfSkiaSharp
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.FileWriter.PdfSkiaSharp",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "System.Collections.Immutable", Version = SystemCollectionsImmutableVersion },
            new NuSpecDependency() { Id = "Core2D", Version = version },
            new NuSpecDependency() { Id = "Core2D.Renderer.SkiaSharp", Version = version }
        },
        Files = new []
        {
            new NuSpecContent { Source = "FileWriter.PdfSkiaSharp.dll", Target = "lib/net45" }
        },
        BasePath = Directory("./dependencies/FileWriter.PdfSkiaSharp/bin/" + dirSuffixSkia),
        OutputDirectory = nugetRoot
    },
    ///////////////////////////////////////////////////////////////////////////////
    // dependencies: Renderer.SkiaSharp
    ///////////////////////////////////////////////////////////////////////////////
    new NuGetPackSettings()
    {
        Id = "Core2D.Renderer.SkiaSharp",
        Dependencies = new []
        {
            new NuSpecDependency() { Id = "System.Collections.Immutable", Version = SystemCollectionsImmutableVersion },
            new NuSpecDependency() { Id = "Core2D", Version = version },
            new NuSpecDependency() { Id = "SkiaSharp", Version = SkiaSharpVersion }
        },
        Files = new []
        {
            new NuSpecContent { Source = "Renderer.SkiaSharp.dll", Target = "lib/net45" }
        },
        BasePath = Directory("./dependencies/Renderer.SkiaSharp/bin/" + dirSuffixSkia),
        OutputDirectory = nugetRoot
    }
};

var nuspecNuGetSettings = new List<NuGetPackSettings>();

nuspecNuGetSettings.AddRange(nuspecNuGetSettingsCore);
nuspecNuGetSettings.AddRange(nuspecNuGetSettingsDependencies);
nuspecNuGetSettings.AddRange(nuspecNuGetSettingsDependenciesModules);
nuspecNuGetSettings.AddRange(nuspecNuGetSettingsDependenciesSkia);

nuspecNuGetSettings.ForEach((nuspec) => SetNuGetNuspecCommonProperties(nuspec));

var nugetPackages = nuspecNuGetSettings.Select(nuspec => {
    return nuspec.OutputDirectory.CombineWithFilePath(string.Concat(nuspec.Id, ".", nuspec.Version, ".nupkg"));
}).ToArray();

var binFiles = nuspecNuGetSettings.SelectMany(nuspec => {
    return nuspec.Files.Select(file => {
        return ((DirectoryPath)nuspec.BasePath).CombineWithFilePath(file.Source);
    });
}).GroupBy(f => f.FullPath).Select(g => g.First());

///////////////////////////////////////////////////////////////////////////////
// CHOCOLATEY NUSPECS
///////////////////////////////////////////////////////////////////////////////

var SetChocolateyNuspecCommonProperties = new Action<ChocolateyPackSettings> ((nuspec) => {
    nuspec.Version = version;
    nuspec.Authors = new [] { "wieslaw.soltes" };
    nuspec.Owners = new [] { "wieslaw.soltes" };
    nuspec.LicenseUrl = new Uri("http://opensource.org/licenses/MIT");
    nuspec.ProjectUrl = new Uri("https://github.com/Core2D/Core2D/");
    nuspec.PackageSourceUrl = new Uri("https://github.com/Core2D/Core2D/");
    nuspec.ProjectSourceUrl = new Uri("https://github.com/Core2D/Core2D/");
    nuspec.BugTrackerUrl = new Uri("https://github.com/Core2D/Core2D/issues/");
    nuspec.DocsUrl = new Uri("http://core2d.github.io/");
    nuspec.RequireLicenseAcceptance = false;
    nuspec.Description = "A multi-platform data driven 2D diagram editor.";
    nuspec.Copyright = "Copyright 2016";
    nuspec.Tags = new [] { "Core2D", "Diagram", "Editor", "2D", "Graphics", "Drawing", "Data" };
});

Func<DirectoryPath, ChocolateyNuSpecContent[]> GetChocolateyNuSpecContent = path => {
    var files = GetFiles(path.FullPath + "/*.dll") + GetFiles(path.FullPath + "/*.exe");
    return files.Select(file => new ChocolateyNuSpecContent { Source = file.FullPath, Target = "bin" }).ToArray();
};

var nuspecChocolateySettings = new Dictionary<ChocolateyPackSettings, DirectoryPath>();

///////////////////////////////////////////////////////////////////////////////
// src: Core2D.Avalonia.Cairo
///////////////////////////////////////////////////////////////////////////////
nuspecChocolateySettings.Add(
    new ChocolateyPackSettings
    {
        Id = "Core2D.Avalonia.Cairo",
        Title = "Core2D (Avalonia/Cairo)",
        OutputDirectory = chocolateyRoot
    },
    zipSourceCairoDirs);

///////////////////////////////////////////////////////////////////////////////
// src: Core2D.Avalonia.Direct2D
///////////////////////////////////////////////////////////////////////////////
nuspecChocolateySettings.Add(
    new ChocolateyPackSettings
    {
        Id = "Core2D.Avalonia.Direct2D",
        Title = "Core2D (Avalonia/Direct2D)",
        OutputDirectory = chocolateyRoot
    },
    zipSourceDirect2DDirs);

///////////////////////////////////////////////////////////////////////////////
// src: Core2D.Avalonia.Skia
///////////////////////////////////////////////////////////////////////////////
nuspecChocolateySettings.Add(
    new ChocolateyPackSettings
    {
        Id = "Core2D.Avalonia.Skia",
        Title = "Core2D (Avalonia/Skia)",
        OutputDirectory = chocolateyRoot
    },
    zipSourceSkiaDirs);

///////////////////////////////////////////////////////////////////////////////
// src: Core2D.Wpf
///////////////////////////////////////////////////////////////////////////////
nuspecChocolateySettings.Add(
    new ChocolateyPackSettings
    {
        Id = "Core2D.Wpf",
        Title = "Core2D (WPF)",
        OutputDirectory = chocolateyRoot
    },
    zipSourceWpfDirs);

nuspecChocolateySettings.ToList().ForEach((nuspec) => SetChocolateyNuspecCommonProperties(nuspec.Key));

var chocolateyPackages = nuspecChocolateySettings.Select(nuspec => {
    return nuspec.Key.OutputDirectory.CombineWithFilePath(string.Concat(nuspec.Key.Id, ".", nuspec.Key.Version, ".nupkg"));
}).ToArray();

///////////////////////////////////////////////////////////////////////////////
// INFORMATION
///////////////////////////////////////////////////////////////////////////////

Information("Building version {0} of Core2D ({1}, {2}, {3}) using version {4} of Cake.", 
    version,
    platform,
    configuration,
    target,
    typeof(ICakeContext).Assembly.GetName().Version.ToString());

if (isRunningOnAppVeyor)
{
    Information("Repository Name: " + BuildSystem.AppVeyor.Environment.Repository.Name);
    Information("Repository Branch: " + BuildSystem.AppVeyor.Environment.Repository.Branch);
}

Information("Target: " + target);
Information("Platform: " + platform);
Information("Configuration: " + configuration);
Information("IsLocalBuild: " + isLocalBuild);
Information("IsRunningOnUnix: " + isRunningOnUnix);
Information("IsRunningOnWindows: " + isRunningOnWindows);
Information("IsRunningOnAppVeyor: " + isRunningOnAppVeyor);
Information("IsPullRequest: " + isPullRequest);
Information("IsMainRepo: " + isMainRepo);
Information("IsMasterBranch: " + isMasterBranch);
Information("IsTagged: " + isTagged);
Information("IsReleasable: " + isReleasable);
Information("IsMyGetRelease: " + isMyGetRelease);
Information("IsNuGetRelease: " + isNuGetRelease);

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectories(buildDirs);
    CleanDirectory(artifactsDir);
    CleanDirectory(testResultsDir);
    CleanDirectory(zipRoot);
    CleanDirectory(binRoot);
    CleanDirectory(nugetRoot);
    CleanDirectory(chocolateyRoot);
});

Task("Restore-NuGet-Packages")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var maxRetryCount = 5;
    var toolTimeout = 1d;
    Policy
        .Handle<Exception>()
        .Retry(maxRetryCount, (exception, retryCount, context) => {
            if (retryCount == maxRetryCount)
            {
                throw exception;
            }
            else
            {
                Verbose("{0}", exception);
                toolTimeout+=0.5;
            }})
        .Execute(()=> {
            if(isRunningOnWindows)
            {
                NuGetRestore(MSBuildSolution, new NuGetRestoreSettings {
                    ToolTimeout = TimeSpan.FromMinutes(toolTimeout)
                });
            }
            else
            {
                NuGetRestore(XBuildSolution, new NuGetRestoreSettings {
                    ToolTimeout = TimeSpan.FromMinutes(toolTimeout)
                });
            }
        });
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if(isRunningOnWindows)
    {
        MSBuild(MSBuildSolution, settings => {
            settings.SetConfiguration(configuration);
            settings.WithProperty("Platform", platform);
            settings.SetVerbosity(Verbosity.Minimal);
        });
    }
    else
    {
        XBuild(XBuildSolution, settings => {
            settings.SetConfiguration(configuration);
            settings.WithProperty("Platform", platform);
            settings.SetVerbosity(Verbosity.Minimal);
        });
    }
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    string pattern = "./tests/**/bin/" + platform + "/" + configuration + "/*.UnitTests.dll";
    if (isPlatformAnyCPU || isPlatformX86)
    {
        XUnit2(pattern, new XUnit2Settings { 
            ToolPath = "./tools/xunit.runner.console/tools/xunit.console.x86.exe",
            OutputDirectory = testResultsDir,
            XmlReportV1 = true,
            NoAppDomain = true
        });
    }
    else
    {
        XUnit2(pattern, new XUnit2Settings { 
            ToolPath = "./tools/xunit.runner.console/tools/xunit.console.exe",
            OutputDirectory = testResultsDir,
            XmlReportV1 = true,
            NoAppDomain = true
        });
    }
});

Task("Copy-Files")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    CopyFiles(binFiles, binRoot);
});

Task("Zip-Files")
    .IsDependentOn("Copy-Files")
    .Does(() =>
{
    Zip(binRoot, zipCoreArtifacts);

    Zip(zipSourceCairoDirs, 
        zipTargetCairoDirs, 
        GetFiles(zipSourceCairoDirs.FullPath + "/*.dll") + 
        GetFiles(zipSourceCairoDirs.FullPath + "/*.exe"));

    if (isRunningOnWindows)
    {
        Zip(zipSourceDirect2DDirs, 
            zipTargetDirect2DDirs, 
            GetFiles(zipSourceDirect2DDirs.FullPath + "/*.dll") + 
            GetFiles(zipSourceDirect2DDirs.FullPath + "/*.exe"));

        Zip(zipSourceSkiaDirs, 
            zipTargetSkiaDirs, 
            GetFiles(zipSourceSkiaDirs.FullPath + "/*.dll") + 
            GetFiles(zipSourceSkiaDirs.FullPath + "/*.exe"));

        Zip(zipSourceWpfDirs, 
            zipTargetWpfDirs, 
            GetFiles(zipSourceWpfDirs.FullPath + "/*.dll") + 
            GetFiles(zipSourceWpfDirs.FullPath + "/*.exe"));
    }
});

Task("Create-NuGet-Packages")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    foreach(var nuspec in nuspecNuGetSettings)
    {
        NuGetPack(nuspec);
    }
});

Task("Create-Chocolatey-Packages")
    .IsDependentOn("Run-Unit-Tests")
    .WithCriteria(() => isRunningOnWindows)
    .Does(() =>
{
    foreach(var nuspec in nuspecChocolateySettings)
    {
        nuspec.Key.Files = GetChocolateyNuSpecContent(nuspec.Value);
        ChocolateyPack(nuspec.Key);
    }
});

Task("Publish-MyGet")
    .IsDependentOn("Create-NuGet-Packages")
    .WithCriteria(() => !isLocalBuild)
    .WithCriteria(() => !isPullRequest)
    .WithCriteria(() => isMainRepo)
    .WithCriteria(() => isMasterBranch)
    .WithCriteria(() => isMyGetRelease)
    .Does(() =>
{
    var apiKey = EnvironmentVariable("MYGET_API_KEY");
    if(string.IsNullOrEmpty(apiKey)) 
    {
        throw new InvalidOperationException("Could not resolve MyGet API key.");
    }

    var apiUrl = EnvironmentVariable("MYGET_API_URL");
    if(string.IsNullOrEmpty(apiUrl)) 
    {
        throw new InvalidOperationException("Could not resolve MyGet API url.");
    }

    foreach(var nupkg in nugetPackages)
    {
        NuGetPush(nupkg, new NuGetPushSettings {
            Source = apiUrl,
            ApiKey = apiKey
        });
    }
})
.OnError(exception =>
{
    Information("Publish-MyGet Task failed, but continuing with next Task...");
});

Task("Publish-NuGet")
    .IsDependentOn("Create-NuGet-Packages")
    .WithCriteria(() => !isLocalBuild)
    .WithCriteria(() => !isPullRequest)
    .WithCriteria(() => isMainRepo)
    .WithCriteria(() => isMasterBranch)
    .WithCriteria(() => isNuGetRelease)
    .Does(() =>
{
    var apiKey = EnvironmentVariable("NUGET_API_KEY");
    if(string.IsNullOrEmpty(apiKey)) 
    {
        throw new InvalidOperationException("Could not resolve NuGet API key.");
    }

    var apiUrl = EnvironmentVariable("NUGET_API_URL");
    if(string.IsNullOrEmpty(apiUrl)) 
    {
        throw new InvalidOperationException("Could not resolve NuGet API url.");
    }

    foreach(var nupkg in nugetPackages)
    {
        NuGetPush(nupkg, new NuGetPushSettings {
            ApiKey = apiKey,
            Source = apiUrl
        });
    }
})
.OnError(exception =>
{
    Information("Publish-NuGet Task failed, but continuing with next Task...");
});

Task("Publish-Chocolatey")
    .IsDependentOn("Create-Chocolatey-Packages")
    .WithCriteria(() => !isLocalBuild)
    .WithCriteria(() => !isPullRequest)
    .WithCriteria(() => isMainRepo)
    .WithCriteria(() => isMasterBranch)
    .WithCriteria(() => isNuGetRelease)
    .Does(() =>
{
    var apiKey = EnvironmentVariable("CHOCOLATEY_API_KEY");
    if(string.IsNullOrEmpty(apiKey)) 
    {
        throw new InvalidOperationException("Could not resolve Chocolatey API key.");
    }

    var apiUrl = EnvironmentVariable("CHOCOLATEY_API_URL");
    if(string.IsNullOrEmpty(apiUrl)) 
    {
        throw new InvalidOperationException("Could not resolve Chocolatey API url.");
    }

    foreach(var nupkg in chocolateyPackages)
    {
        ChocolateyPush(nupkg, new ChocolateyPushSettings {
            ApiKey = apiKey,
            Source = apiUrl
        });
    }
})
.OnError(exception =>
{
    Information("Publish-Chocolatey Task failed, but continuing with next Task...");
});

///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Package")
  .IsDependentOn("Zip-Files")
  .IsDependentOn("Create-NuGet-Packages")
  .IsDependentOn("Create-Chocolatey-Packages");

Task("Default")
  .IsDependentOn("Package");

Task("AppVeyor")
  .IsDependentOn("Zip-Files")
  .IsDependentOn("Publish-MyGet")
  .IsDependentOn("Publish-NuGet")
  .IsDependentOn("Publish-Chocolatey");

Task("Travis")
  .IsDependentOn("Run-Unit-Tests");

///////////////////////////////////////////////////////////////////////////////
// EXECUTE
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);
