// dotnet tool install -g Cake.Tool --version 0.30.0
// dotnet cake build.cake -Target="Build"
// dotnet cake build.cake --target="Build"

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
    DotNetCoreClean(parameters.Solution, new DotNetCoreCleanSettings
    {
        Configuration = parameters.Configuration
    });
});

Task("Restore")
    .Does<Parameters>(parameters => 
{
    DotNetCoreRestore(parameters.Solution, new DotNetCoreRestoreSettings {
        DisableParallel = false
    });
});

Task("Build")
    .Does<Parameters>(parameters => 
{
    DotNetCoreBuild(parameters.Solution, new DotNetCoreBuildSettings {
        Configuration = parameters.Configuration,
        VersionSuffix = parameters.VersionSuffix
    });
});

Task("Test")
    .Does<Parameters>(parameters => 
{
    foreach(var project in parameters.TestProjects)
    {
        (string path, string name) = project;
        Information($"Test: {name}");
        DotNetCoreTest($"{path}/{name}/{name}.csproj", new DotNetCoreTestSettings {
            Configuration = parameters.Configuration
        });
    }
});

Task("Publish")
    .Does<Parameters>(parameters => 
{
    CleanDirectory($"{parameters.Artifacts}/zip");
    foreach(var project in parameters.PublishProjects)
    {
        (string path, string name, string framework, string runtime) = project;
        Information($"Publish: {name}, {framework}, {runtime}");
        DotNetCorePublish($"{path}/{name}/{name}.csproj", new DotNetCorePublishSettings {
            Configuration = parameters.Configuration,
            VersionSuffix = parameters.VersionSuffix,
            Framework = framework,
            Runtime = runtime,
            OutputDirectory = $"./{parameters.Artifacts}/publish/{name}-{framework}-{runtime}"
        });
        Zip($"{parameters.Artifacts}/publish/{name}-{framework}-{runtime}", $"{parameters.Artifacts}/zip/{name}-{framework}-{runtime}.zip");
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
    .Does<Parameters>(parameters => 
{
    DotNetCoreNuGetPush($"{parameters.Artifacts}/nuget/*.nupkg", new DotNetCoreNuGetPushSettings {
        Source = "",
        ApiKey = ""
    });
});

Task("Default")
  .IsDependentOn("Build");

Task("CI")
  .IsDependentOn("Clean")
  .IsDependentOn("Restore")
  .IsDependentOn("Build")
  .IsDependentOn("Test")
  .IsDependentOn("Publish")
  .IsDependentOn("Pack")
  .IsDependentOn("Push");

RunTarget(Context.Argument("target", "Default"));
