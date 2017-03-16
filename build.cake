///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////

#addin "nuget:?package=Polly&version=5.0.6"

///////////////////////////////////////////////////////////////////////////////
// TOOLS
///////////////////////////////////////////////////////////////////////////////

#tool "nuget:?package=xunit.runner.console&version=2.2.0"
#tool "nuget:https://dotnet.myget.org/F/nuget-build/?package=NuGet.CommandLine&version=4.3.0-beta1-2361&prerelease"

///////////////////////////////////////////////////////////////////////////////
// USINGS
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
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
// PARAMETERS
///////////////////////////////////////////////////////////////////////////////

var isPlatformAnyCPU = StringComparer.OrdinalIgnoreCase.Equals(platform, "AnyCPU");
var isPlatformX86 = StringComparer.OrdinalIgnoreCase.Equals(platform, "x86");
var isPlatformX64 = StringComparer.OrdinalIgnoreCase.Equals(platform, "x64");

///////////////////////////////////////////////////////////////////////////////
// VERSION
///////////////////////////////////////////////////////////////////////////////

var version = XmlPeek("./build.targets", "//*[local-name()='Version']/text()");
Information("Version: {0}", version);

if (BuildSystem.AppVeyor.IsRunningOnAppVeyor)
{
    if (BuildSystem.AppVeyor.Environment.Repository.Tag.IsTag && !string.IsNullOrWhiteSpace(BuildSystem.AppVeyor.Environment.Repository.Tag.Name))
        version = BuildSystem.AppVeyor.Environment.Repository.Tag.Name;
    else
        version += "-build" + EnvironmentVariable("APPVEYOR_BUILD_NUMBER");
}

///////////////////////////////////////////////////////////////////////////////
// Visual Studio
///////////////////////////////////////////////////////////////////////////////

var MSBuildSolution = "./Core2D.sln";
var unitTestsFramework = "net461";

///////////////////////////////////////////////////////////////////////////////
// .NET Core Projects
///////////////////////////////////////////////////////////////////////////////

var netCoreAppsRoot= "./apps";
var netCoreApps = new string[] { "Core2D.Avalonia.NetCore", "Core2D.Avalonia.NetStandard" };
var netCoreProjects = netCoreApps.Select(name => 
    new {
        Path = string.Format("{0}/{1}", netCoreAppsRoot, name),
        Name = name,
        Framework = XmlPeek(string.Format("{0}/{1}/{1}.csproj", netCoreAppsRoot, name), "//*[local-name()='TargetFramework']/text()"),
        Runtimes = XmlPeek(string.Format("{0}/{1}/{1}.csproj", netCoreAppsRoot, name), "//*[local-name()='RuntimeIdentifiers']/text()").Split(';')
    }).ToList();

/*
var netCoreProjects = new [] {
    new {
        Path = $"./apps/{netCoreApps[0]}",
        Name = netCoreApps[0],
        Framework = XmlPeek($"./apps/{netCoreApps[0]}/{netCoreApps[0]}.csproj", "//*[local-name()='TargetFramework']/text()"),
        Runtimes = XmlPeek($"./apps/{netCoreApps[0]}/{netCoreApps[0]}.csproj", "//*[local-name()='RuntimeIdentifiers']/text()").Split(';')
    },
    new {
        Path = $"./apps/{netCoreApps[1]}",
        Name = netCoreApps[1],
        Framework = XmlPeek($"./apps/{netCoreApps[1]}/{netCoreApps[1]}.csproj", "//*[local-name()='TargetFramework']/text()"),
        Runtimes = XmlPeek($"./apps/{netCoreApps[1]}/{netCoreApps[1]}.csproj", "//*[local-name()='RuntimeIdentifiers']/text()").Split(';')
    }
};
*/

///////////////////////////////////////////////////////////////////////////////
// .NET Core UnitTests
///////////////////////////////////////////////////////////////////////////////

var unitTestsNetCore = new List<string>() { 
    "./tests/Core2D.Spatial.UnitTests",
    "./tests/Core2D.UnitTests",
    "./tests/FileSystem.DotNet.UnitTests",
    "./tests/Serializer.Xaml.UnitTests"
};
var frameworksNetCore = new List<string>() { "netcoreapp1.1" };
if (IsRunningOnWindows())
{
    frameworksNetCore.Add("net461");
}

///////////////////////////////////////////////////////////////////////////////
// PATHS
///////////////////////////////////////////////////////////////////////////////

var buildDirs = 
    GetDirectories("./src/**/bin/**") + 
    GetDirectories("./src/**/obj/**") + 
    GetDirectories("./apps/**/bin/**") + 
    GetDirectories("./apps/**/obj/**") + 
    GetDirectories("./uwp/**/bin/**") + 
    GetDirectories("./uwp/**/obj/**") + 
    GetDirectories("./tests/**/bin/**") + 
    GetDirectories("./tests/**/obj/**");

var artifactsDir = (DirectoryPath)Directory("./artifacts");
var testResultsDir = artifactsDir.Combine("test-results");	
var zipRootDir = artifactsDir.Combine("zip");

var platformZip = (platform == "AnyCPU") ? "x86" : platform;
var dirSuffixZip = platformZip + "/" + configuration;
var fileZipSuffix = platformZip + "-" + configuration + "-" + version + ".zip";

var zipSourceCairoDir = (DirectoryPath)Directory("./apps/Core2D.Avalonia.Cairo/bin/" + dirSuffixZip);
var zipTargetCairoFile = zipRootDir.CombineWithFilePath("Core2D.Avalonia.Cairo-" + fileZipSuffix);
var zipSourceDirect2DDir = (DirectoryPath)Directory("./apps/Core2D.Avalonia.Direct2D/bin/" + dirSuffixZip);
var zipTargetDirect2DFile = zipRootDir.CombineWithFilePath("Core2D.Avalonia.Direct2D-" + fileZipSuffix);
var zipSourceSkiaDir = (DirectoryPath)Directory("./apps/Core2D.Avalonia.Skia/bin/" + dirSuffixZip);
var zipTargetSkiaFile = zipRootDir.CombineWithFilePath("Core2D.Avalonia.Skia-" + fileZipSuffix);
var zipSourceSkiaDemoDir = (DirectoryPath)Directory("./apps/Core2D.SkiaDemo/bin/" + dirSuffixZip);
var zipTargetSkiaDemoFile = zipRootDir.CombineWithFilePath("Core2D.SkiaDemo-" + fileZipSuffix);
var zipSourceWpfDir = (DirectoryPath)Directory("./apps/Core2D.Wpf/bin/" + dirSuffixZip);
var zipTargetWpfFile = zipRootDir.CombineWithFilePath("Core2D.Wpf-" + fileZipSuffix);

var msvcp140_x86 = @"c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Redist\MSVC\14.10.25008\x86\Microsoft.VC150.CRT\msvcp140.dll";
var msvcp140_x64 = @"c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Redist\MSVC\14.10.25008\x64\Microsoft.VC150.CRT\msvcp140.dll";
var vcruntime140_x86 = @"c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Redist\MSVC\14.10.25008\x86\Microsoft.VC150.CRT\vcruntime140.dll";
var vcruntime140_x64 = @"c:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Redist\MSVC\14.10.25008\x64\Microsoft.VC150.CRT\vcruntime140.dll";

///////////////////////////////////////////////////////////////////////////////
// TASKS: COMMON
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectories(buildDirs);
    CleanDirectory(testResultsDir);
    CleanDirectory(zipRootDir);
});

///////////////////////////////////////////////////////////////////////////////
// TASKS: VISUAL STUDIO
///////////////////////////////////////////////////////////////////////////////

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
            if (IsRunningOnWindows())
            {
                NuGetRestore(MSBuildSolution, new NuGetRestoreSettings {
                    ToolPath = "./tools/NuGet.CommandLine/tools/NuGet.exe",
                    ToolTimeout = TimeSpan.FromMinutes(toolTimeout)
                });
            }
            if (IsRunningOnUnix())
            {
                NuGetRestore(MSBuildSolution, new NuGetRestoreSettings {
                    ToolPath = "./tools/NuGet.CommandLine/tools/NuGet.exe",
                    ToolTimeout = TimeSpan.FromMinutes(toolTimeout)
                });
            }
        });
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    if (IsRunningOnWindows())
    {
        MSBuild(MSBuildSolution, settings => {
            settings.WithProperty("UseRoslynPathHack", "true");
            settings.UseToolVersion(MSBuildToolVersion.VS2017);
            settings.SetConfiguration(configuration);
            settings.WithProperty("Platform", platform);
            settings.SetVerbosity(Verbosity.Minimal);
        });   
    }
    if (IsRunningOnUnix())
    {
        XBuild(MSBuildSolution, settings => {
            settings.UseToolVersion(XBuildToolVersion.Default);
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
    var assemblies = GetFiles("./tests/**/bin/" + platform + "/" + configuration + "/" + unitTestsFramework + "/*.UnitTests.dll");
    var settings = new XUnit2Settings { 
        ToolPath = (isPlatformAnyCPU || isPlatformX86) ? 
            "./tools/xunit.runner.console/tools/xunit.console.x86.exe" :
            "./tools/xunit.runner.console/tools/xunit.console.exe",
        OutputDirectory = testResultsDir,
        XmlReportV1 = true,
        NoAppDomain = true,
        Parallelism = ParallelismOption.None,
        ShadowCopy = false
    };
    foreach (var assembly in assemblies)
    {
        XUnit2(assembly.FullPath, settings);
    }
});

Task("Copy-Redist-Files")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    if (IsRunningOnWindows() && (isPlatformAnyCPU || isPlatformX86 || isPlatformX64))
    {
        var msvcp140 = (isPlatformAnyCPU || isPlatformX86) ? msvcp140_x86 : msvcp140_x64;
        var vcruntime140 = (isPlatformAnyCPU || isPlatformX86) ? vcruntime140_x86 : vcruntime140_x64;
        CopyFileToDirectory(msvcp140, zipSourceCairoDir);
        CopyFileToDirectory(vcruntime140, zipSourceCairoDir);
        CopyFileToDirectory(msvcp140, zipSourceDirect2DDir);
        CopyFileToDirectory(vcruntime140, zipSourceDirect2DDir);
        CopyFileToDirectory(msvcp140, zipSourceSkiaDir);
        CopyFileToDirectory(vcruntime140, zipSourceSkiaDir);
        CopyFileToDirectory(msvcp140, zipSourceSkiaDemoDir);
        CopyFileToDirectory(vcruntime140, zipSourceSkiaDemoDir);
        CopyFileToDirectory(msvcp140, zipSourceWpfDir);
        CopyFileToDirectory(vcruntime140, zipSourceWpfDir);
    }
});

Task("Zip-Files")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    Zip(zipSourceCairoDir, 
        zipTargetCairoFile, 
        GetFiles(zipSourceCairoDir.FullPath + "/*.dll") + 
        GetFiles(zipSourceCairoDir.FullPath + "/*.exe"));

    Zip(zipSourceDirect2DDir, 
        zipTargetDirect2DFile, 
        GetFiles(zipSourceDirect2DDir.FullPath + "/*.dll") + 
        GetFiles(zipSourceDirect2DDir.FullPath + "/*.exe"));

    Zip(zipSourceSkiaDir, 
        zipTargetSkiaFile, 
        GetFiles(zipSourceSkiaDir.FullPath + "/*.dll") + 
        GetFiles(zipSourceSkiaDir.FullPath + "/*.exe"));

    if (IsRunningOnWindows())
    {
        Zip(zipSourceSkiaDemoDir, 
            zipTargetSkiaDemoFile, 
            GetFiles(zipSourceSkiaDemoDir.FullPath + "/*.dll") + 
            GetFiles(zipSourceSkiaDemoDir.FullPath + "/*.exe"));

        Zip(zipSourceWpfDir, 
            zipTargetWpfFile, 
            GetFiles(zipSourceWpfDir.FullPath + "/*.dll") + 
            GetFiles(zipSourceWpfDir.FullPath + "/*.exe"));
    }
});

///////////////////////////////////////////////////////////////////////////////
// TASKS: .NET Core
///////////////////////////////////////////////////////////////////////////////

Task("Restore-NetCore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    foreach (var project in netCoreProjects)
    {
        DotNetCoreRestore(project.Path);
    }
});

Task("Run-Unit-Tests-NetCore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    foreach (var unitTest in unitTestsNetCore)
    {
        var project = System.IO.Path.Combine(unitTest, System.IO.Path.GetFileName(unitTest) + ".csproj");
        DotNetCoreRestore(unitTest);
        foreach(var framework in frameworksNetCore)
        {
            DotNetCoreTest(project, new DotNetCoreTestSettings {
                Configuration = configuration,
                Framework = framework
            });
        }
    }
});

Task("Build-NetCore")
    .IsDependentOn("Restore-NetCore")
    .Does(() =>
{
    foreach (var project in netCoreProjects)
    {
        DotNetCoreBuild(project.Path, new DotNetCoreBuildSettings {
            Configuration = configuration
        });
    }
});

Task("Publish-NetCore")
    .IsDependentOn("Restore-NetCore")
    .Does(() =>
{
    foreach (var project in netCoreProjects)
    {
        foreach(var runtime in project.Runtimes)
        {
            var outputDir = zipRootDir.Combine(project.Name + "-" + runtime);
            DotNetCorePublish(project.Path, new DotNetCorePublishSettings {
                Framework = project.Framework,
                Configuration = configuration,
                Runtime = runtime,
                OutputDirectory = outputDir.FullPath
            });
        }
    }
});

Task("Copy-Redist-Files-NetCore")
    .IsDependentOn("Publish-NetCore")
    .Does(() =>
{
    foreach (var project in netCoreProjects)
    {
        foreach(var runtime in project.Runtimes)
        {
            var outputDir = zipRootDir.Combine(project.Name + "-" + runtime);
            if (IsRunningOnWindows() && runtime == "win7-x86")
            {
                CopyFileToDirectory(msvcp140_x86, outputDir);
                CopyFileToDirectory(vcruntime140_x86, outputDir);
            }
            if (IsRunningOnWindows() && runtime == "win7-x64")
            {
                CopyFileToDirectory(msvcp140_x64, outputDir);
                CopyFileToDirectory(vcruntime140_x64, outputDir);
            }
        }
    }
});

Task("Zip-Files-NetCore")
    .IsDependentOn("Publish-NetCore")
    .Does(() =>
{
    foreach (var project in netCoreProjects)
    {
        foreach(var runtime in project.Runtimes)
        {
            var outputDir = zipRootDir.Combine(project.Name + "-" + runtime);
            var zipFile = zipRootDir.CombineWithFilePath(project.Name + "-" + runtime + "-" + configuration + "-" + version + ".zip");
            Zip(outputDir.FullPath, zipFile);
        }
    }
});

///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Default")
  .IsDependentOn("Run-Unit-Tests");

Task("AppVeyor")
  .IsDependentOn("Run-Unit-Tests-NetCore")
  .IsDependentOn("Build-NetCore")
  .IsDependentOn("Publish-NetCore")
  .IsDependentOn("Copy-Redist-Files-NetCore")
  .IsDependentOn("Zip-Files-NetCore")
  .IsDependentOn("Run-Unit-Tests")
  .IsDependentOn("Copy-Redist-Files")
  .IsDependentOn("Zip-Files");

Task("Travis")
  .IsDependentOn("Run-Unit-Tests-NetCore")
  .IsDependentOn("Build-NetCore")
  .IsDependentOn("Publish-NetCore")
  .IsDependentOn("Zip-Files-NetCore");

///////////////////////////////////////////////////////////////////////////////
// EXECUTE
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);
