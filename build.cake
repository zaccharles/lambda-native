var target = Argument("target", "Default");

var configuration = 
    HasArgument("Configuration") ? Argument<string>("Configuration") :
    EnvironmentVariable("Configuration") != null ? EnvironmentVariable("Configuration") : "Release";
var preReleaseSuffix =
    HasArgument("PreReleaseSuffix") ? Argument<string>("PreReleaseSuffix") :
    (AppVeyor.IsRunningOnAppVeyor && AppVeyor.Environment.Repository.Tag.IsTag) ? null :
    EnvironmentVariable("PreReleaseSuffix") != null ? EnvironmentVariable("PreReleaseSuffix") :
    "beta";
var buildNumber =
    HasArgument("BuildNumber") ? Argument<int>("BuildNumber") :
    AppVeyor.IsRunningOnAppVeyor ? AppVeyor.Environment.Build.Number :
    TravisCI.IsRunningOnTravisCI ? TravisCI.Environment.Build.BuildNumber :
    EnvironmentVariable("BuildNumber") != null ? int.Parse(EnvironmentVariable("BuildNumber")) :
    0;

var versionSuffix = string.IsNullOrEmpty(preReleaseSuffix) ? null : preReleaseSuffix + "-" + buildNumber.ToString("D4");
var artifactsDir = Directory("./artifacts");

Task("Clean")
    .Does(() =>
{
    CleanDirectory(artifactsDir);
    DeleteDirectories(GetDirectories("**/bin"), new DeleteDirectorySettings() { Force = true, Recursive = true });
    DeleteDirectories(GetDirectories("**/obj"), new DeleteDirectorySettings() { Force = true, Recursive = true });
});

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
{
    DotNetCoreRestore();
});

Task("Build")
    .IsDependentOn("Restore")
    .Does(() =>
{
    DotNetCoreBuild(
        ".",
        new DotNetCoreBuildSettings()
        {
            Configuration = configuration,
            NoRestore = true,
            VersionSuffix = versionSuffix
        });
});

Task("Test")
    .IsDependentOn("Build")
    .Does(() =>
{
    foreach(var project in GetFiles("./test/*.Tests.csproj"))
    {
        DotNetCoreTest(
            project.ToString(),
            new DotNetCoreTestSettings()
            {
                Configuration = configuration,
                Logger = $"trx;LogFileName={project.GetFilenameWithoutExtension()}.trx",
                NoBuild = true,
                NoRestore = true,
                ResultsDirectory = artifactsDir
            });
    }
});

Task("Pack")
    .IsDependentOn("Test")
    .Does(() =>
    {
        DotNetCorePack(
            ".",
            new DotNetCorePackSettings()
            {
                Configuration = configuration,
                NoBuild = true,
                NoRestore = true,
                OutputDirectory = artifactsDir,
                VersionSuffix = versionSuffix
            });
    });

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);