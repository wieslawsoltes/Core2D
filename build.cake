#load "./parameters.cake"

Setup<Parameters>(context =>
{
    Information("Running tasks...");
    return new Parameters(context);
});

Teardown<Parameters>((context, parameters) =>
{
    Information("Finished running tasks.");
});

Task("Clean")
    .Does<Parameters>(parameters => 
{
    foreach(var project in parameters.BuildProjects)
    {
        (string path, string name) = project;
        Information($"Clean: {name}");
        DotNetCoreClean($"{path}/{name}/{name}.csproj", new DotNetCoreCleanSettings {
            Configuration = parameters.Configuration,
            Verbosity = DotNetCoreVerbosity.Minimal
        });
    }
});

Task("Build")
    .Does<Parameters>(parameters => 
{
    foreach(var project in parameters.BuildProjects)
    {
        (string path, string name) = project;
        Information($"Build: {name}");
        DotNetCoreBuild($"{path}/{name}/{name}.csproj", new DotNetCoreBuildSettings {
            Configuration = parameters.Configuration,
            VersionSuffix = parameters.VersionSuffix
        });
    }
});

Task("Test")
    .Does<Parameters>(parameters => 
{
    CleanDirectory($"{parameters.Artifacts}/tests");
    foreach(var project in parameters.TestProjects)
    {
        (string path, string name) = project;
        Information($"Test: {name}");
        DotNetCoreTest($"{path}/{name}/{name}.csproj", new DotNetCoreTestSettings {
            Configuration = parameters.Configuration,
            ResultsDirectory = $"{parameters.Artifacts}/tests",
            Logger = "trx"
        });
    }
});
Task("Pack")
    .Does<Parameters>(parameters => 
{
    CleanDirectory($"{parameters.Artifacts}/nuget");
    foreach(var project in parameters.PackProjects)
    {
        (string path, string name) = project;
        Information($"Pack: {name}");
        DotNetCorePack($"{path}/{name}/{name}.csproj", new DotNetCorePackSettings {
            Configuration = parameters.Configuration,
            VersionSuffix = parameters.VersionSuffix,
            OutputDirectory = $"{parameters.Artifacts}/nuget"
        });
    }
});

Task("Push")
    .WithCriteria<Parameters>((context, parameters) => parameters.PushNuGet)
    .Does<Parameters>(parameters => 
{
    var apiKey = EnvironmentVariable(parameters.IsNugetRelease ? "NUGET_API_KEY" : "MYGET_API_KEY");
    var apiUrl = EnvironmentVariable(parameters.IsNugetRelease ? "NUGET_API_URL" : "MYGET_API_URL");
    var packages = GetFiles($"{parameters.Artifacts}/nuget/*.nupkg");
    foreach (var package in packages)
    {
        DotNetCoreNuGetPush(package.FullPath, new DotNetCoreNuGetPushSettings {
            Source = apiUrl,
            ApiKey = apiKey
        });
    }
});

Task("Default")
  .IsDependentOn("Build");

Task("AppVeyor")
  .IsDependentOn("Clean")
  .IsDependentOn("Build")
  .IsDependentOn("Test")
  .IsDependentOn("Pack")
  .IsDependentOn("Push");

Task("Azure-Windows")
  .IsDependentOn("Clean")
  .IsDependentOn("Build")
  .IsDependentOn("Test")
  .IsDependentOn("Pack");

Task("Azure-macOS")
  .IsDependentOn("Test");

Task("Azure-Linux")
  .IsDependentOn("Test");

RunTarget(Context.Argument("target", "Default"));
