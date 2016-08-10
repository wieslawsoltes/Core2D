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

var isLocalBuild = BuildSystem.IsLocalBuild;
var isRunningOnUnix = IsRunningOnUnix();
var isRunningOnWindows = IsRunningOnWindows();
var isRunningOnAppVeyor = BuildSystem.AppVeyor.IsRunningOnAppVeyor;
var isPullRequest = BuildSystem.AppVeyor.Environment.PullRequest.IsPullRequest;
var isMainRepo = StringComparer.OrdinalIgnoreCase.Equals("Core2D/Core2D", BuildSystem.AppVeyor.Environment.Repository.Name);
var isMasterBranch = StringComparer.OrdinalIgnoreCase.Equals("master", BuildSystem.AppVeyor.Environment.Repository.Branch);
var isTagged = BuildSystem.AppVeyor.Environment.Repository.Tag.IsTag 
               && !string.IsNullOrWhiteSpace(BuildSystem.AppVeyor.Environment.Repository.Tag.Name);
var isRelease = StringComparer.OrdinalIgnoreCase.Equals("AnyCPU", platform) 
                && StringComparer.OrdinalIgnoreCase.Equals("Release", configuration);

///////////////////////////////////////////////////////////////////////////////
// VERSION
///////////////////////////////////////////////////////////////////////////////

var version = ParseAssemblyInfo("./src/Core2D.Shared/SharedAssemblyInfo.cs").AssemblyVersion;

if (isRunningOnAppVeyor)
{
    if (isTagged)
    {
        // Use Tag Name as version
        version = BuildSystem.AppVeyor.Environment.Repository.Tag.Name;
    }
    else
    {
        // Use AssemblyVersion with AppVeyor Build as version
        version += "-build" + EnvironmentVariable("APPVEYOR_BUILD_NUMBER") + "-alpha";
    }
}

///////////////////////////////////////////////////////////////////////////////
// SOLUTIONS
///////////////////////////////////////////////////////////////////////////////

var msBuildSolution = "./Core2D.sln";
var xBuildSolution = "./Core2D.mono.sln";

///////////////////////////////////////////////////////////////////////////////
// DIRECTORIES
///////////////////////////////////////////////////////////////////////////////

var artifactsDir = (DirectoryPath)Directory("./artifacts");
var testResultsDir = artifactsDir.Combine("test-results");
var nugetRoot = artifactsDir.Combine("nuget");
var zipRoot = artifactsDir.Combine("zip");

var dirSuffix = platform + "/" + configuration;

var binSourceDirs = GetDirectories("./src/**/bin/" + dirSuffix);
var objSourceDirs = GetDirectories("./src/**/obj/" + dirSuffix);
var binTestsDirs = GetDirectories("./tests/**/bin/" + dirSuffix);
var objTestsDirs = GetDirectories("./testssrc/**/obj/" + dirSuffix);
var binDependenciesDirs = GetDirectories("./dependencies/**/bin/" + dirSuffix);
var objDependenciesDirs = GetDirectories("./dependencies/**/obj/" + dirSuffix);

///////////////////////////////////////////////////////////////////////////////
// ZIP
///////////////////////////////////////////////////////////////////////////////

var zipSuffix = platform + "-" + configuration + "-" + version + ".zip";

var zipSource_Cairo = (DirectoryPath)Directory("./src/Core2D.Avalonia.Cairo/bin/" + dirSuffix);
var zipTarget_Cairo = zipRoot.CombineWithFilePath("Core2D.Avalonia.Cairo." + zipSuffix);

var zipSource_Direct2D = (DirectoryPath)Directory("./src/Core2D.Avalonia.Direct2D/bin/" + dirSuffix);
var zipTarget_Direct2D = zipRoot.CombineWithFilePath("Core2D.Avalonia.Direct2D-" + zipSuffix);

var zipSource_Skia = (DirectoryPath)Directory("./src/Core2D.Avalonia.Skia/bin/" + dirSuffix);
var zipTarget_Skia = zipRoot.CombineWithFilePath("Core2D.Avalonia.Skia-" + zipSuffix);

var zipSource_Wpf = (DirectoryPath)Directory("./src/Core2D.Wpf/bin/" + dirSuffix);
var zipTarget_Wpf = zipRoot.CombineWithFilePath("Core2D.Wpf-" + zipSuffix);

///////////////////////////////////////////////////////////////////////////////
// NUSPECS
///////////////////////////////////////////////////////////////////////////////

var nuspecSettings_Core2D = new NuGetPackSettings()
{
    Id = "Core2D",
    Version = version,
    Authors = new [] { "wieslaw.soltes" },
    Owners = new [] { "wieslaw.soltes" },
    LicenseUrl = new Uri("http://opensource.org/licenses/MIT"),
    ProjectUrl = new Uri("https://github.com/Core2D/Core2D/"),
    RequireLicenseAcceptance = false,
    Description = "A multi-platform data driven 2D diagram editor.",
    Copyright = "Copyright 2016",
    Tags = new [] { "Diagram", "Editor", "2D", "Graphics", "Drawing", "Data", "Managed", "C#" },
    Dependencies = new []
    {
        new NuSpecDependency() { Id = "System.Collections.Immutable", Version = "1.2.0" }
    },
    Files = new []
    {
        new NuSpecContent { Source = "Core2D.dll", Target = "lib/portable-windows8+net45" },
    },
    BasePath = Directory("./src/Core2D/bin/" + dirSuffix),
    OutputDirectory = nugetRoot.Combine("Core2D")
};

var nuspecSettings = new []
{
    nuspecSettings_Core2D
};

var nugetPackages = nuspecSettings.Select(nuspec => {
        return nuspec.OutputDirectory.CombineWithFilePath(string.Concat(nuspec.Id, ".", version, ".nupkg"));
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
Information("IsRelease: " + isRelease);

///////////////////////////////////////////////////////////////////////////////
// TASKS
///////////////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    // Builds
    CleanDirectories(binSourceDirs);
    CleanDirectories(objSourceDirs);
    CleanDirectories(binTestsDirs);
    CleanDirectories(objTestsDirs);
    CleanDirectories(binDependenciesDirs);
    CleanDirectories(objDependenciesDirs);

    // Artifacts
    CleanDirectory(artifactsDir);

    // Tests
    CleanDirectory(testResultsDir);

    // Zips
    CleanDirectory(zipRoot);

    // NuGets
    CleanDirectory(nugetRoot);

    foreach (var package in nuspecSettings) 
    {
        CleanDirectory(package.OutputDirectory);
    }
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
                NuGetRestore(msBuildSolution, new NuGetRestoreSettings {
                    ToolTimeout = TimeSpan.FromMinutes(toolTimeout)
                });
            }
            else
            {
                NuGetRestore(xBuildSolution, new NuGetRestoreSettings {
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
	    MSBuild(msBuildSolution, settings => {
	        settings.SetConfiguration(configuration);
	        settings.WithProperty("Platform", platform);
	        settings.SetVerbosity(Verbosity.Minimal);
        });
    }
    else
    {
	    XBuild(xBuildSolution, settings => {
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
    if (platform == "x86")
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

Task("Zip-Files")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    Zip(zipSource_Cairo, 
        zipTarget_Cairo, 
        GetFiles(zipSource_Cairo.FullPath + "/*.dll") + 
        GetFiles(zipSource_Cairo.FullPath + "/*.exe"));

    if (isRunningOnWindows)
    {
        Zip(zipSource_Direct2D, 
            zipTarget_Direct2D, 
            GetFiles(zipSource_Direct2D.FullPath + "/*.dll") + 
            GetFiles(zipSource_Direct2D.FullPath + "/*.exe"));

        if (!StringComparer.OrdinalIgnoreCase.Equals(platform, "AnyCPU"))
        {
            Zip(zipSource_Skia, 
                zipTarget_Skia, 
                GetFiles(zipSource_Skia.FullPath + "/*.dll") + 
                GetFiles(zipSource_Skia.FullPath + "/*.exe"));
        }

        Zip(zipSource_Wpf, 
            zipTarget_Wpf, 
            GetFiles(zipSource_Wpf.FullPath + "/*.dll") + 
            GetFiles(zipSource_Wpf.FullPath + "/*.exe"));
    }
});

Task("Create-NuGet-Packages")
    .IsDependentOn("Run-Unit-Tests")
    .Does(() =>
{
    foreach(var package in nuspecSettings)
    {
        NuGetPack(package);
    }
});

Task("Upload-AppVeyor-Artifacts")
    .IsDependentOn("Zip-Files")
    .IsDependentOn("Create-NuGet-Packages")
    .WithCriteria(() => isRunningOnAppVeyor)
    .Does(() =>
{
    foreach(var package in GetFiles(zipRoot + "/*"))
    {
        AppVeyor.UploadArtifact(package);
    }

    foreach(var package in GetFiles(nugetRoot + "/*"))
    {
        AppVeyor.UploadArtifact(package);
    }
});

Task("Publish-MyGet")
    .IsDependentOn("Create-NuGet-Packages")
    .WithCriteria(() => !isLocalBuild)
    .WithCriteria(() => !isPullRequest)
    .WithCriteria(() => isMainRepo)
    .WithCriteria(() => isMasterBranch)
    .WithCriteria(() => !isTagged)
    .WithCriteria(() => isRelease)
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

    foreach(var package in nugetPackages)
    {
        NuGetPush(package, new NuGetPushSettings {
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
    .WithCriteria(() => isTagged)
    .WithCriteria(() => isRelease)
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

    foreach(var package in nugetPackages)
    {
        NuGetPush(package, new NuGetPushSettings {
            ApiKey = apiKey,
            Source = apiUrl
        });
    }
})
.OnError(exception =>
{
    Information("Publish-NuGet Task failed, but continuing with next Task...");
});

///////////////////////////////////////////////////////////////////////////////
// TARGETS
///////////////////////////////////////////////////////////////////////////////

Task("Package")
  .IsDependentOn("Zip-Files")
  .IsDependentOn("Create-NuGet-Packages");

Task("Default")
  .IsDependentOn("Package");

Task("AppVeyor")
  .IsDependentOn("Upload-AppVeyor-Artifacts")
  .IsDependentOn("Publish-MyGet")
  .IsDependentOn("Publish-NuGet");

Task("Travis")
  .IsDependentOn("Run-Unit-Tests");

///////////////////////////////////////////////////////////////////////////////
// EXECUTE
///////////////////////////////////////////////////////////////////////////////

RunTarget(target);
