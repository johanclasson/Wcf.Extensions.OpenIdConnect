#tool "nuget:?package=xunit.runner.console"
#addin "Cake.ExtendedNuGet"

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////

// Define directories.
var clientBuildDir = Directory("./src/Client/bin") + Directory(configuration);
var hostBuildDir = Directory("./src/Host/bin") + Directory(configuration);
var slnFile = File("./Wcf.Extensions.OpenIdConnect.sln");
var outputDir = Directory("./build");
string version = "";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(clientBuildDir);
    CleanDirectory(hostBuildDir);
});

Task("Restore-NuGet-Packages")
    .Does(() =>
{
    NuGetRestore(slnFile);
});

Task("UpdateAssemblyInfo")
    .Does(() =>
{
    var result = GitVersion(new GitVersionSettings {
        UpdateAssemblyInfo = true
    });
    version = result.LegacySemVerPadded;
    Information("LegacySemVerPadded: {0}", version);
});

Task("Build")
    .IsDependentOn("Clean")
    .IsDependentOn("Restore-NuGet-Packages")
    .IsDependentOn("UpdateAssemblyInfo")
    .Does(() =>
{
      MSBuild(slnFile, settings =>
        settings
        .SetConfiguration(configuration)
        .SetVerbosity(Verbosity.Minimal));
});

Task("Run-Unit-Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    XUnit2("./src/**/bin/" + configuration + "/*.Specs.dll",
        new XUnit2Settings {
            Parallelism = ParallelismOption.All,
            HtmlReport = true,
            NoAppDomain = true,
            OutputDirectory = outputDir.ToString()
        });
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Run-Unit-Tests");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
