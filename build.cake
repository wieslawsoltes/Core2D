///////////////////////////////////////////////////////////////////////////////
// ADDINS
///////////////////////////////////////////////////////////////////////////////

#addin "nuget:?package=Polly&version=5.3.1"
#addin "nuget:?package=PackageReferenceEditor&version=0.0.3"

///////////////////////////////////////////////////////////////////////////////
// TOOLS
///////////////////////////////////////////////////////////////////////////////

#tool "nuget:?package=NuGet.CommandLine&version=4.3.0"
#tool "nuget:?package=xunit.runner.console&version=2.3.1"

///////////////////////////////////////////////////////////////////////////////
// USINGS
///////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using PackageReferenceEditor;
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

var suffix = BuildSystem.AppVeyor.IsRunningOnAppVeyor ? "-build" + EnvironmentVariable("APPVEYOR_BUILD_NUMBER") : "";
var version = XmlPeek("./build/Base.props", "//*[local-name()='Version']/text()") + suffix;

Information("Version: {0}", version);

///////////////////////////////////////////////////////////////////////////////
// Visual Studio
///////////////////////////////////////////////////////////////////////////////

var MSBuildSolution = "./Core2D.sln";
var UnitTestsFramework = "net461";

///////////////////////////////////////////////////////////////////////////////
// .NET Core Projects
///////////////////////////////////////////////////////////////////////////////

var netCoreAppsRoot= "./src";
var netCoreApps = new string[] { "Core2D.Avalonia" };
var netCoreProjects = netCoreApps.Select(name => 
    new {
        Path = string.Format("{0}/{1}", netCoreAppsRoot, name),
        Name = name,
        Framework = XmlPeek(string.Format("{0}/{1}/{1}.csproj", netCoreAppsRoot, name), "//*[local-name()='TargetFrameworks']/text()").Split(';').FirstOrDefault(),
        Runtimes = XmlPeek(string.Format("{0}/{1}/{1}.csproj", netCoreAppsRoot, name), "//*[local-name()='RuntimeIdentifiers']/text()").Split(';')
    }).ToList();

var netCoreRTProjects = netCoreApps.Select(name => 
    new {
        Path = string.Format("{0}/{1}", netCoreAppsRoot, name),
        Name = name,
        Framework = XmlPeek(string.Format("{0}/{1}/{1}.csproj", netCoreAppsRoot, name), "//*[local-name()='TargetFrameworks']/text()").Split(';').FirstOrDefault(),
        Runtimes = new string[] { "win-x64" }
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
    GetDirectories("./tests/**/bin/**") + 
    GetDirectories("./tests/**/obj/**");
var artifactsDir = (DirectoryPath)Directory("./artifacts");
var testResultsDir = artifactsDir.Combine("test-results");	
var zipRootDir = artifactsDir.Combine("zip");
var dirSuffixZip = platform + "/" + configuration;
var fileZipSuffix = version + ".zip";
var msvcp140_x86 = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Redist\MSVC\14.13.26020\x86\Microsoft.VC141.CRT\msvcp140.dll";
var msvcp140_x64 = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Redist\MSVC\14.13.26020\x64\Microsoft.VC141.CRT\msvcp140.dll";
var vcruntime140_x86 = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Redist\MSVC\14.13.26020\x86\Microsoft.VC141.CRT\vcruntime140.dll";
var vcruntime140_x64 = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Redist\MSVC\14.13.26020\x64\Microsoft.VC141.CRT\vcruntime140.dll";
var editbin = @"C:\Program Files (x86)\Microsoft Visual Studio\2017\Community\VC\Tools\MSVC\14.13.26128\bin\HostX86\x86\editbin.exe";

///////////////////////////////////////////////////////////////////////////////
// VALIDATE
///////////////////////////////////////////////////////////////////////////////

Updater.FindReferences("./build", "*.props", new string[] { }).ValidateVersions();	
//Updater.FindReferences("./", "*.csproj", new string[] { }).ValidateVersions();	

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
            NuGetRestore(MSBuildSolution, new NuGetRestoreSettings {
                ToolTimeout = TimeSpan.FromMinutes(toolTimeout)
            });
        });
});

Task("Build")
    .IsDependentOn("Restore-NuGet-Packages")
    .Does(() =>
{
    MSBuild(MSBuildSolution, settings => {
        settings.UseToolVersion(MSBuildToolVersion.VS2017);
        settings.SetConfiguration(configuration);
        settings.WithProperty("Platform", platform);
        settings.SetVerbosity(Verbosity.Minimal);
        settings.SetMaxCpuCount(0);
    });
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    var assemblies = GetFiles("./tests/**/bin/" + platform + "/" + configuration + "/" + UnitTestsFramework + "/*.UnitTests.dll");
    var settings = new XUnit2Settings { 
        ToolPath = (isPlatformAnyCPU || isPlatformX86) ? 
            Context.Tools.Resolve("xunit.console.x86.exe") :
            Context.Tools.Resolve("xunit.console.exe"),
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

///////////////////////////////////////////////////////////////////////////////
// TASKS: .NET Core
///////////////////////////////////////////////////////////////////////////////

Task("Restore-NetCore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    foreach (var project in netCoreProjects)
    {
        DotNetCoreRestore(project.Path, new DotNetCoreRestoreSettings {
            MSBuildSettings = new DotNetCoreMSBuildSettings().WithProperty("CoreRT", "False")
        });
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
            MSBuildSettings = new DotNetCoreMSBuildSettings() {
                MaxCpuCount = 0
            }
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
                Runtime = runtime,
                OutputDirectory = outputDir.FullPath,
                MSBuildSettings = new DotNetCoreMSBuildSettings().WithProperty("CoreRT", "False")
            });
        }
    }
});

Task("Patch-NetCore")
    .IsDependentOn("Publish-NetCore")
    .Does(() =>
{
    foreach (var project in netCoreProjects)
    {
        foreach(var runtime in project.Runtimes)
        {
            if (IsRunningOnWindows() && (runtime == "win7-x86" || runtime == "win7-x64"))
            {
                var outputDir = zipRootDir.Combine(project.Name + "-" + runtime);
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
    .IsDependentOn("Copy-Redist-Files-NetCore")
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
// TASKS: .NET CoreRT
///////////////////////////////////////////////////////////////////////////////

Task("Restore-NetCoreRT")
    .IsDependentOn("Clean")
    .Does(() =>
{
    foreach (var project in netCoreRTProjects)
    {
        DotNetCoreRestore(project.Path, new DotNetCoreRestoreSettings {
            MSBuildSettings = new DotNetCoreMSBuildSettings().WithProperty("CoreRT", "True")
        });
    }
});

Task("Publish-NetCoreRT")
    .IsDependentOn("Restore-NetCoreRT")
    .Does(() =>
{
    foreach (var project in netCoreRTProjects)
    {
        foreach(var runtime in project.Runtimes)
        {
            var outputDir = zipRootDir.Combine(project.Name + "-" + runtime);
            Information("Publishing: {0}, runtime: {1}", project.Name, runtime);
            DotNetCorePublish(project.Path, new DotNetCorePublishSettings {
                Framework = project.Framework,
                Configuration = configuration,
                Runtime = runtime,
                OutputDirectory = outputDir.FullPath,
                MSBuildSettings = new DotNetCoreMSBuildSettings().WithProperty("CoreRT", "True")
            });
        }
    }
});

Task("Patch-NetCoreRT")
    .IsDependentOn("Publish-NetCoreRT")
    .Does(() =>
{
    foreach (var project in netCoreRTProjects)
    {
        foreach(var runtime in project.Runtimes)
        {
            var outputDir = zipRootDir.Combine(project.Name + "-" + runtime);
            if (IsRunningOnWindows() && runtime == "win-x64")
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

Task("Copy-Redist-Files-NetCoreRT")
    .IsDependentOn("Publish-NetCoreRT")
    .Does(() =>
{
    // HACK:
    DirectoryPath profilePath = EnvironmentVariable("USERPROFILE") ?? EnvironmentVariable("HOME");
    DirectoryPath nugetPackages = EnvironmentVariable("NUGET_PACKAGES") ?? profilePath.Combine(".nuget/packages");
    var libSkiaSharp = nugetPackages.Combine("skiasharp/1.57.1/runtimes/win7-x64/native").CombineWithFilePath("libSkiaSharp.dll");

    // HACK: https://github.com/dotnet/corert/issues/5496
    var ilcompilerTools = GetDirectories(nugetPackages.FullPath + "/runtime.win-x64.microsoft.dotnet.ilcompiler/**/tools").LastOrDefault();
    var clrcompression = ilcompilerTools.CombineWithFilePath("clrcompression.dll");

    foreach (var project in netCoreRTProjects)
    {
        foreach(var runtime in project.Runtimes)
        {
            var outputDir = zipRootDir.Combine(project.Name + "-" + runtime);
            if (IsRunningOnWindows() && runtime == "win-x64")
            {
                Information("Copying redist files for: {0}, runtime: {1}", project.Name, runtime);
                CopyFileToDirectory(msvcp140_x64, outputDir);
                CopyFileToDirectory(vcruntime140_x64, outputDir);

                Information("Copying native files for: {0}, runtime: {1}", project.Name, runtime);
                // HACK: 
                CopyFileToDirectory(libSkiaSharp, outputDir);
                // HACK: 
                CopyFileToDirectory(clrcompression, outputDir);
            }
        }
    }
});

Task("Zip-Files-NetCoreRT")
    .IsDependentOn("Copy-Redist-Files-NetCoreRT")
    .Does(() =>
{
    foreach (var project in netCoreRTProjects)
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
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Package")
  .IsDependentOn("Copy-Redist-Files-NetCore")
  .IsDependentOn("Zip-Files-NetCore");

Task("Default")
  .IsDependentOn("Run-Unit-Tests");

Task("AppVeyor")
  .IsDependentOn("Run-Unit-Tests-NetCore")
  .IsDependentOn("Build-NetCore")
  .IsDependentOn("Publish-NetCore")
  //.IsDependentOn("Patch-NetCore")
  .IsDependentOn("Zip-Files-NetCore")
  .IsDependentOn("Publish-NetCoreRT")
  //.IsDependentOn("Patch-NetCoreRT")
  .IsDependentOn("Zip-Files-NetCoreRT")
  .IsDependentOn("Run-Unit-Tests");

Task("Travis")
  .IsDependentOn("Run-Unit-Tests-NetCore")
  .IsDependentOn("Build-NetCore");

///////////////////////////////////////////////////////////////////////////////
// EXECUTE
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);
