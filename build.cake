#addin "nuget:?package=Polly"
#addin "nuget:?package=NuGet.Core"
#tool "nuget:?package=xunit.runner.console"

using System;
using Polly;
using NuGet;

var target = Argument("target", "Default");
var platform = Argument("platform", "AnyCPU");
var configuration = Argument("configuration", "Release");
var isPlatformAnyCPU = StringComparer.OrdinalIgnoreCase.Equals(platform, "AnyCPU");
var isPlatformX86 = StringComparer.OrdinalIgnoreCase.Equals(platform, "x86");
var isPlatformX64 = StringComparer.OrdinalIgnoreCase.Equals(platform, "x64");
var MSBuildSolution = "./Core2D.sln";
var version = XmlPeek("./build.targets", "//*[local-name()='Version']/text()");
Information("Version: {0}", version);
if (BuildSystem.AppVeyor.IsRunningOnAppVeyor)
{
    if (BuildSystem.AppVeyor.Environment.Repository.Tag.IsTag && !string.IsNullOrWhiteSpace(BuildSystem.AppVeyor.Environment.Repository.Tag.Name))
        version = BuildSystem.AppVeyor.Environment.Repository.Tag.Name;
    else
        version += "-build" + EnvironmentVariable("APPVEYOR_BUILD_NUMBER");
}
var dirSuffix = platform + "/" + configuration;
var buildDirs = 
    GetDirectories("./src/**/bin/" + dirSuffix) + 
    GetDirectories("./src/**/obj/" + dirSuffix) + 
    GetDirectories("./dependencies/**/bin/" + dirSuffix) + 
    GetDirectories("./dependencies/**/obj/" + dirSuffix) + 
    GetDirectories("./tests/**/bin/" + dirSuffix) + 
    GetDirectories("./tests/**/obj/" + dirSuffix);
var artifactsDir = (DirectoryPath)Directory("./artifacts");
var testResultsDir = artifactsDir.Combine("test-results");	
var zipRootDir = artifactsDir.Combine("zip");
var fileZipSuffix = platform + "-" + configuration + "-" + version + ".zip";
var zipSourceWpfDirs = (DirectoryPath)Directory("./src/Core2D.Wpf/bin/" + dirSuffix);
var zipTargetWpfDirs = zipRootDir.CombineWithFilePath("Core2D.Wpf-" + fileZipSuffix);

Task("Clean")
    .Does(() =>
{
    CleanDirectories(buildDirs);
    CleanDirectory(testResultsDir);
    CleanDirectory(zipRootDir);
});

Task("Validate-NuGet-Packages")
    .Does(() =>
{
    var packageVersions = new Dictionary<string, IList<Tuple<string,string>>>();
    var sourcesDir = (DirectoryPath)Directory("./src");

    System.IO.Directory.EnumerateFiles(sourcesDir.FullPath, "packages.config", SearchOption.AllDirectories).ToList().ForEach(fileName =>
    {
        var file = new PackageReferenceFile(fileName);
        foreach (PackageReference packageReference in file.GetPackageReferences())
        {
            IList<Tuple<string, string>> versions;
            packageVersions.TryGetValue(packageReference.Id, out versions);
            if (versions == null)
            {
                versions = new List<Tuple<string, string>>();
                packageVersions[packageReference.Id] = versions;
            }
            versions.Add(Tuple.Create(packageReference.Version.ToString(), fileName));
        }
    });

    Information("Checking installed NuGet package dependencies versions:");

    packageVersions.ToList().ForEach(package =>
    {
        var packageVersion = package.Value.First().Item1;
        bool isValidVersion = package.Value.All(x => x.Item1 == packageVersion);
        if (!isValidVersion)
        {
            Information("Info: package {0} has multiple versions installed:", package.Key);
            foreach (var v in package.Value)
            {
                Information("  {0}, file: {1}", v.Item1, v.Item2);
            }
        }
    });
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
            if(IsRunningOnWindows())
            {
                NuGetRestore(MSBuildSolution, new NuGetRestoreSettings {
                    ToolTimeout = TimeSpan.FromMinutes(toolTimeout)
                });
            }
            if(IsRunningOnUnix())
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
    if(IsRunningOnWindows())
    {
        MSBuild(MSBuildSolution, settings => {
            settings.SetConfiguration(configuration);
            settings.WithProperty("Platform", platform);
            settings.SetVerbosity(Verbosity.Minimal);
        });
    }
    if(IsRunningOnUnix())
    {
        XBuild(MSBuildSolution, settings => {
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
    else if (isPlatformX64)
    {
        XUnit2(pattern, new XUnit2Settings { 
            ToolPath = "./tools/xunit.runner.console/tools/xunit.console.exe",
            OutputDirectory = testResultsDir,
            XmlReportV1 = true,
            NoAppDomain = true
        });
    }
    else
    {
        throw new PlatformNotSupportedException("Not supported XUnit platform.");
    }
});

Task("Zip-Files")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    if (IsRunningOnWindows())
    {
        Zip(zipSourceWpfDirs, 
            zipTargetWpfDirs, 
            GetFiles(zipSourceWpfDirs.FullPath + "/*.dll") + 
            GetFiles(zipSourceWpfDirs.FullPath + "/*.exe"));
    }
});

Task("Default")
  .IsDependentOn("Run-Unit-Tests");

Task("AppVeyor")
  .IsDependentOn("Validate-NuGet-Packages")
  .IsDependentOn("Run-Unit-Tests")
  .IsDependentOn("Zip-Files");

RunTarget(target);
