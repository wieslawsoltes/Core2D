///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////

#addin "nuget:?package=Polly&version=5.1.0"

///////////////////////////////////////////////////////////////////////////////
// TOOLS
///////////////////////////////////////////////////////////////////////////////

#tool "nuget:?package=xunit.runner.console&version=2.2.0"

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

var version = XmlPeek("./build/NuGet.props", "//*[local-name()='Version']/text()").Replace("$(VersionSuffix)", "");
var suffix = "";
var publishNuGet = false;
var publishMyGet = false;

if (BuildSystem.AppVeyor.IsRunningOnAppVeyor)
{
    if (BuildSystem.AppVeyor.Environment.Repository.Tag.IsTag && !string.IsNullOrWhiteSpace(BuildSystem.AppVeyor.Environment.Repository.Tag.Name))
    {
        version = BuildSystem.AppVeyor.Environment.Repository.Tag.Name;
        suffix = "";
        publishNuGet = !BuildSystem.AppVeyor.Environment.PullRequest.IsPullRequest;
    }
    else
    {
        suffix = "-build" + EnvironmentVariable("APPVEYOR_BUILD_NUMBER");
        version += suffix;
        publishMyGet = !BuildSystem.AppVeyor.Environment.PullRequest.IsPullRequest;
    }
}

Information("Version: {0}", version);

///////////////////////////////////////////////////////////////////////////////
// Visual Studio
///////////////////////////////////////////////////////////////////////////////

var MSBuildSolution = "./Core2D.sln";
var UnitTestsFramework = "net461";

///////////////////////////////////////////////////////////////////////////////
// .NET Core Projects
///////////////////////////////////////////////////////////////////////////////

var netCoreAppsRoot= "./apps";
var netCoreApps = new string[] { "Core2D.Avalonia.NetCore" };
var netCoreProjects = netCoreApps.Select(name => 
    new {
        Path = string.Format("{0}/{1}", netCoreAppsRoot, name),
        Name = name,
        Framework = XmlPeek(string.Format("{0}/{1}/{1}.csproj", netCoreAppsRoot, name), "//*[local-name()='TargetFramework']/text()"),
        Runtimes = XmlPeek(string.Format("{0}/{1}/{1}.csproj", netCoreAppsRoot, name), "//*[local-name()='RuntimeIdentifiers']/text()").Split(';')
    }).ToList();

///////////////////////////////////////////////////////////////////////////////
// .NET Core UnitTests
///////////////////////////////////////////////////////////////////////////////

var netCoreUnitTestsRoot= "./tests";
var netCoreUnitTests = new string[] { 
    "Core2D.UnitTests",
    "Core2D.FileSystem.DotNet.UnitTests"
};
var netCoreUnitTestsProjects = netCoreUnitTests.Select(name => 
    new {
        Name = name,
        Path = string.Format("{0}/{1}", netCoreUnitTestsRoot, name),
        File = string.Format("{0}/{1}/{1}.csproj", netCoreUnitTestsRoot, name)
    }).ToList();
var netCoreUnitTestsFrameworks = new List<string>() { "netcoreapp2.0" };
if (IsRunningOnWindows())
{
    netCoreUnitTestsFrameworks.Add("net461");
}

///////////////////////////////////////////////////////////////////////////////
// PATHS
///////////////////////////////////////////////////////////////////////////////

var buildDirs = 
    GetDirectories("./src/**/bin/**") + 
    GetDirectories("./src/**/obj/**") + 
    GetDirectories("./utilities/**/bin/**") + 
    GetDirectories("./utilities/**/obj/**") + 
    GetDirectories("./tests/**/bin/**") + 
    GetDirectories("./tests/**/obj/**") + 
    GetDirectories("./apps/**/bin/**") + 
    GetDirectories("./apps/**/obj/**") +
    GetDirectories("./samples/**/bin/**") + 
    GetDirectories("./samples/**/obj/**");

var artifactsDir = (DirectoryPath)Directory("./artifacts");
var testResultsDir = artifactsDir.Combine("test-results");	
var zipRootDir = artifactsDir.Combine("zip");
var nugetRootDir = artifactsDir.Combine("nuget");

var dirSuffixZip = platform + "/" + configuration;
var fileZipSuffix = version + ".zip";

var zipTargetNuGetFile = zipRootDir.CombineWithFilePath("Core2D-NuGet-Packages-" + fileZipSuffix);

var zipSourceDirect2DDir = (DirectoryPath)Directory("./apps/Core2D.Avalonia.Direct2D/bin/" + dirSuffixZip);
var zipTargetDirect2DFile = zipRootDir.CombineWithFilePath("Core2D.Avalonia.Direct2D-" + fileZipSuffix);

var zipSourceSkiaDir = (DirectoryPath)Directory("./apps/Core2D.Avalonia.Skia/bin/" + dirSuffixZip);
var zipTargetSkiaFile = zipRootDir.CombineWithFilePath("Core2D.Avalonia.Skia-" + fileZipSuffix);

var zipSourceWpfDir = (DirectoryPath)Directory("./apps/Core2D.Wpf/bin/" + dirSuffixZip);
var zipTargetWpfFile = zipRootDir.CombineWithFilePath("Core2D.Wpf-" + fileZipSuffix);

var zipSourceSkiaDemoDir = (DirectoryPath)Directory("./samples/Core2D.SkiaDemo/bin/" + dirSuffixZip);
var zipTargetSkiaDemoFile = zipRootDir.CombineWithFilePath("Core2D.SkiaDemo-" + fileZipSuffix);

var zipSourceSkiaViewAutofacDir = (DirectoryPath)Directory("./samples/Core2D.SkiaViewAutofac/bin/" + dirSuffixZip);
var zipTargetSkiaViewAutofacFile = zipRootDir.CombineWithFilePath("Core2D.SkiaViewAutofac-" + fileZipSuffix);

var zipSourceSkiaViewNoAutofacDir = (DirectoryPath)Directory("./samples/Core2D.SkiaViewNoAutofac/bin/" + dirSuffixZip);
var zipTargetSkiaViewNoAutofacFile = zipRootDir.CombineWithFilePath("Core2D.SkiaViewNoAutofac-" + fileZipSuffix);

var msvcp140_x86 = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Redist\MSVC\14.11.25325\x86\Microsoft.VC141.CRT\msvcp140.dll";
var msvcp140_x64 = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Redist\MSVC\14.11.25325\x64\Microsoft.VC141.CRT\msvcp140.dll";
var vcruntime140_x86 = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Redist\MSVC\14.11.25325\x86\Microsoft.VC141.CRT\vcruntime140.dll";
var vcruntime140_x64 = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Redist\MSVC\14.11.25325\x64\Microsoft.VC141.CRT\vcruntime140.dll";
var editbin = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Tools\MSVC\14.11.25503\bin\HostX86\x86\editbin.exe";

///////////////////////////////////////////////////////////////////////////////
// TASKS: COMMON
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectories(buildDirs);
    CleanDirectory(testResultsDir);
    CleanDirectory(zipRootDir);
    CleanDirectory(nugetRootDir);
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
                    ToolTimeout = TimeSpan.FromMinutes(toolTimeout)
                });
            }
            if (IsRunningOnUnix())
            {
                NuGetRestore(MSBuildSolution, new NuGetRestoreSettings {
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
            settings.UseToolVersion(MSBuildToolVersion.VS2017);
            settings.SetConfiguration(configuration);
            settings.WithProperty("Platform", platform);
            settings.WithProperty("VersionSuffix", suffix);
            settings.SetVerbosity(Verbosity.Minimal);
        });   
    }
    if (IsRunningOnUnix())
    {
        XBuild(MSBuildSolution, settings => {
            settings.UseToolVersion(XBuildToolVersion.Default);
            settings.SetConfiguration(configuration);
            settings.WithProperty("Platform", platform);
            settings.WithProperty("VersionSuffix", suffix);
            settings.SetVerbosity(Verbosity.Minimal);
        });
    }
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    var assemblies = GetFiles("./tests/**/bin/" + platform + "/" + configuration + "/" + UnitTestsFramework + "/*.UnitTests.dll");
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

        CopyFileToDirectory(msvcp140, zipSourceDirect2DDir);
        CopyFileToDirectory(vcruntime140, zipSourceDirect2DDir);

        CopyFileToDirectory(msvcp140, zipSourceSkiaDir);
        CopyFileToDirectory(vcruntime140, zipSourceSkiaDir);

        CopyFileToDirectory(msvcp140, zipSourceWpfDir);
        CopyFileToDirectory(vcruntime140, zipSourceWpfDir);

        CopyFileToDirectory(msvcp140, zipSourceSkiaDemoDir);
        CopyFileToDirectory(vcruntime140, zipSourceSkiaDemoDir);

        CopyFileToDirectory(msvcp140, zipSourceSkiaViewAutofacDir);
        CopyFileToDirectory(vcruntime140, zipSourceSkiaViewAutofacDir);

        CopyFileToDirectory(msvcp140, zipSourceSkiaViewNoAutofacDir);
        CopyFileToDirectory(vcruntime140, zipSourceSkiaViewNoAutofacDir);
    }
});

Task("Zip-Files")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    Zip(zipSourceDirect2DDir.FullPath, zipTargetDirect2DFile);
    Zip(zipSourceSkiaDir.FullPath,  zipTargetSkiaFile);
    Zip(zipSourceWpfDir.FullPath, zipTargetWpfFile);

    Zip(zipSourceSkiaDemoDir.FullPath, zipTargetSkiaDemoFile);
    Zip(zipSourceSkiaViewAutofacDir.FullPath, zipTargetSkiaViewAutofacFile);
    Zip(zipSourceSkiaViewNoAutofacDir.FullPath, zipTargetSkiaViewNoAutofacFile);
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
    foreach (var project in netCoreUnitTestsProjects)
    {
        DotNetCoreRestore(project.Path);
        foreach(var framework in netCoreUnitTestsFrameworks)
        {
            Information("Running tests for: {0}, framework: {1}", project.Name, framework);
            DotNetCoreTest(project.File, new DotNetCoreTestSettings {
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
        Information("Building: {0}", project.Name);
        DotNetCoreBuild(project.Path, new DotNetCoreBuildSettings {
            Configuration = configuration,
            VersionSuffix = suffix
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
            Information("Publishing: {0}, runtime: {1}", project.Name, runtime);
            DotNetCorePublish(project.Path, new DotNetCorePublishSettings {
                Framework = project.Framework,
                Configuration = configuration,
                VersionSuffix = suffix,
                Runtime = runtime,
                OutputDirectory = outputDir.FullPath
            });

            if (IsRunningOnWindows() && (runtime == "win7-x86" || runtime == "win7-x64"))
            {
                Information("Patching executable subsystem for: {0}, runtime: {1}", project.Name, runtime);
                var targetExe = outputDir.CombineWithFilePath(project.Name + ".exe");
                var exitCodeWithArgument = StartProcess(editbin, new ProcessSettings { 
                    Arguments = "/subsystem:windows " + targetExe.FullPath
                });
                Information("The editbin command exit code: {0}", exitCodeWithArgument);
            }
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
                Information("Copying redist files for: {0}, runtime: {1}", project.Name, runtime);
                CopyFileToDirectory(msvcp140_x86, outputDir);
                CopyFileToDirectory(vcruntime140_x86, outputDir);
            }
            if (IsRunningOnWindows() && runtime == "win7-x64")
            {
                Information("Copying redist files for: {0}, runtime: {1}", project.Name, runtime);
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
            var zipFile = zipRootDir.CombineWithFilePath(project.Name + "-" + runtime + "-" + version + ".zip");
            Information("Zip files for: {0}, runtime: {1}", project.Name, runtime);
            Zip(outputDir.FullPath, zipFile);
        }
    }
});

///////////////////////////////////////////////////////////////////////////////
// TASKS: NuGet
///////////////////////////////////////////////////////////////////////////////

Task("Create-NuGet-Packages")
    .IsDependentOn("Run-Unit-Tests")
    .IsDependentOn("Run-Unit-Tests-NetCore")
    .Does(() =>
{
    var projects = GetFiles("./src/**/*.csproj") + 
                   GetFiles("./utilities/**/*.csproj");

    // Workaround for: https://github.com/NuGet/Home/issues/4337

    foreach(var project in projects)
    {
        Information("Restore & Pack : {0}", project);
        MSBuild(project.FullPath, settings => {
            settings.UseToolVersion(MSBuildToolVersion.VS2017);
            settings.SetConfiguration(configuration);
            settings.WithProperty("Platform", platform);
            settings.WithProperty("VersionSuffix", suffix);
            settings.WithProperty("PackageOutputPath", MakeAbsolute(nugetRootDir).FullPath);
            settings.WithTarget("Restore");
            settings.WithTarget("Pack");
            settings.SetVerbosity(Verbosity.Minimal);
        });   
    }

    //foreach(var project in projects)
    //{
    //    Information("Pack: {0}", project);
    //    DotNetCorePack(project.FullPath, new DotNetCorePackSettings {
    //        Configuration = configuration,
    //        VersionSuffix = suffix,
    //        OutputDirectory = nugetRootDir
    //    });
    //}
});

Task("Publish-MyGet")
    .IsDependentOn("Create-NuGet-Packages")
    .WithCriteria(() => publishMyGet)
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

    foreach(var nupkg in GetFiles(nugetRootDir.FullPath + "/*.nupkg"))
    {
        NuGetPush(nupkg, new NuGetPushSettings {
            Source = apiUrl,
            ApiKey = apiKey
        });
    }
});

Task("Publish-NuGet")
    .IsDependentOn("Create-NuGet-Packages")
    .WithCriteria(() => publishNuGet)
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

    foreach(var nupkg in GetFiles(nugetRootDir.FullPath + "/*.nupkg"))
    {
        NuGetPush(nupkg, new NuGetPushSettings {
            ApiKey = apiKey,
            Source = apiUrl
        });
    }
});

Task("Zip-Files-NuGet")
    .IsDependentOn("Create-NuGet-Packages")
    .Does(() =>
{
    Zip(nugetRootDir.FullPath, zipTargetNuGetFile);
});

///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Package")
  .IsDependentOn("Create-NuGet-Packages");

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
  .IsDependentOn("Zip-Files")
  .IsDependentOn("Zip-Files-NuGet")
  .IsDependentOn("Publish-MyGet")
  .IsDependentOn("Publish-NuGet");

Task("Travis")
  .IsDependentOn("Run-Unit-Tests-NetCore")
  .IsDependentOn("Build-NetCore")
  .IsDependentOn("Publish-NetCore")
  .IsDependentOn("Zip-Files-NetCore");

///////////////////////////////////////////////////////////////////////////////
// EXECUTE
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);
