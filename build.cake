#tool "nuget:?package=xunit.runner.console"
#load ./paket.cake

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
var outputDir = Directory("./out");
string version = "";

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
    .Does(() =>
{
    CleanDirectory(clientBuildDir);
    CleanDirectory(hostBuildDir);
    CleanDirectory(outputDir);
});

Task("Packet Restore")
    .Does(() =>
{
    PacketRestore();
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
    .IsDependentOn("Packet Restore")
    .IsDependentOn("UpdateAssemblyInfo")
    .Does(() =>
{
      MSBuild("./Wcf.Extensions.OpenIdConnect.sln", settings =>
        settings
        .SetConfiguration(configuration)
        .SetVerbosity(Verbosity.Minimal));
});

Task("Undo AssemblyInfo.cs")
    .IsDependentOn("Build")
    .Does(() =>
{
    var command = "gci . -Recurse -Filter 'AssemblyInfo.cs' | %{ git checkout $_.FullName }";
    RunPowerShellCommand(command);
});

Task("Run Unit Tests")
    .IsDependentOn("Build")
    .Does(() =>
{
    XUnit2("./src/**/bin/" + configuration + "/*.Specs.dll",
        new XUnit2Settings
        {
            Parallelism = ParallelismOption.All,
            HtmlReport = true,
            NoAppDomain = true,
            OutputDirectory = outputDir.ToString()
        });
});

Task("Packet Pack")
    .IsDependentOn("Build")
    .Does(() =>
{
    PacketPack(new PacketPackSettings {
        OutputDirectory = outputDir.ToString(),
        Configuration = configuration,
        Version = version
    });
});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("Default")
    .IsDependentOn("Run Unit Tests")
    .IsDependentOn("Undo AssemblyInfo.cs")
    .IsDependentOn("Packet Pack");

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////

RunTarget(target);
