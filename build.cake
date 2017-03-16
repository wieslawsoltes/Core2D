#addin "nuget:?package=Polly&version=5.0.6"
#tool "nuget:?package=xunit.runner.console&version=2.2.0"
#tool "nuget:https://dotnet.myget.org/F/nuget-build/?package=NuGet.CommandLine&version=4.3.0-beta1-2361&prerelease"

using System;
using Polly;

var target = Argument("target", "Default");
var platform = Argument("platform", "AnyCPU");
var configuration = Argument("configuration", "Release");
var isPlatformAnyCPU = StringComparer.OrdinalIgnoreCase.Equals(platform, "AnyCPU");
var isPlatformX86 = StringComparer.OrdinalIgnoreCase.Equals(platform, "x86");
var isPlatformX64 = StringComparer.OrdinalIgnoreCase.Equals(platform, "x64");
var MSBuildSolution = "./Core2D.sln";
var unitTestsFramework = "net461";
var version = XmlPeek("./build.targets", "//*[local-name()='Version']/text()");

Information("Version: {0}", version);
if (BuildSystem.AppVeyor.IsRunningOnAppVeyor)
{
    if (BuildSystem.AppVeyor.Environment.Repository.Tag.IsTag && !string.IsNullOrWhiteSpace(BuildSystem.AppVeyor.Environment.Repository.Tag.Name))
        version = BuildSystem.AppVeyor.Environment.Repository.Tag.Name;
    else
        version += "-build" + EnvironmentVariable("APPVEYOR_BUILD_NUMBER");
}

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

Task("Clean")
    .Does(() =>
{
    CleanDirectories(buildDirs);
    CleanDirectory(testResultsDir);
    CleanDirectory(zipRootDir);
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

Task("Build-NetCore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    var projectNetCore = "./apps/Core2D.Avalonia.NetCore";
    var projectNetCoreName = System.IO.Path.GetFileName(projectNetCore);
    var frameworkNetCore = "netcoreapp1.1";
    var runtimesNetCore = new List<string>() { "win7-x86", "win7-x64", "ubuntu.16.10-x64" };

    DotNetCoreRestore(projectNetCore);
    DotNetCoreBuild(projectNetCore, new DotNetCoreBuildSettings {
        Configuration = configuration
    });

    //if (IsRunningOnWindows())
    //{
        foreach(var runtime in runtimesNetCore)
        {
            var outputDir = zipRootDir.Combine(projectNetCoreName + "-" + runtime);
            var zipFile = zipRootDir.CombineWithFilePath(projectNetCoreName + "-" + runtime + "-" + configuration + "-" + version + ".zip");

            DotNetCorePublish(projectNetCore, new DotNetCorePublishSettings {
                Framework = frameworkNetCore,
                Configuration = configuration,
                Runtime = runtime,
                OutputDirectory = outputDir.FullPath
            });

            if (runtime == "win7-x86")
            {
                CopyFileToDirectory(msvcp140_x86, outputDir);
                CopyFileToDirectory(vcruntime140_x86, outputDir);
            }

            if (runtime == "win7-x64")
            {
                CopyFileToDirectory(msvcp140_x64, outputDir);
                CopyFileToDirectory(vcruntime140_x64, outputDir);
            }

            Zip(outputDir.FullPath, zipFile);
        }
    //}
});

Task("Run-Unit-Tests-NetCore")
    .IsDependentOn("Clean")
    .Does(() =>
{
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

Task("Default")
  .IsDependentOn("Run-Unit-Tests");

Task("AppVeyor")
  .IsDependentOn("Build-NetCore")
  .IsDependentOn("Run-Unit-Tests-NetCore")
  .IsDependentOn("Run-Unit-Tests")
  .IsDependentOn("Copy-Redist-Files")
  .IsDependentOn("Zip-Files");

Task("Travis")
  .IsDependentOn("Build-NetCore")
  .IsDependentOn("Run-Unit-Tests-NetCore");

RunTarget(target);
