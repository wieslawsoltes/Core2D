#addin "nuget:?package=Polly&version=4.2.0"
#tool "nuget:?package=xunit.runner.console&version=2.1.0"

using System;
using Polly;

var target = Argument("target", "Default");
var platform = Argument("platform", "AnyCPU");
var configuration = Argument("configuration", "Release");

var msBuildSolution = "./Core2D.sln";
var xBuildSolution = "./Core2D.mono.sln";

var binSourceDirs = GetDirectories("./src/**/bin/" + platform + "/" + configuration);
var objSourceDirs = GetDirectories("./src/**/obj/" + platform + "/" + configuration);
var binTestsDirs = GetDirectories("./tests/**/bin/" + platform + "/" + configuration);
var objTestsDirs = GetDirectories("./testssrc/**/obj/" + platform + "/" + configuration);
var binDependenciesDirs = GetDirectories("./dependencies/**/bin/" + platform + "/" + configuration);
var objDependenciesDirs = GetDirectories("./dependencies/**/obj/" + platform + "/" + configuration);

Task("Clean")
    .Does(() =>
{
    CleanDirectories(binSourceDirs);
    CleanDirectories(objSourceDirs);
    CleanDirectories(binTestsDirs);
    CleanDirectories(objTestsDirs);
    CleanDirectories(binDependenciesDirs);
    CleanDirectories(objDependenciesDirs);
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
    if(IsRunningOnWindows())
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
            ToolPath = "./tools/xunit.runner.console/tools/xunit.console.x86.exe" 
        });
    }
    else
    {
        XUnit2(pattern, new XUnit2Settings { 
            ToolPath = "./tools/xunit.runner.console/tools/xunit.console.exe" 
        });
    }
});

Task("Default")
  .IsDependentOn("Run-Unit-Tests");

Task("AppVeyor")
  .IsDependentOn("Run-Unit-Tests");

Task("Travis")
  .IsDependentOn("Run-Unit-Tests");

RunTarget(target);