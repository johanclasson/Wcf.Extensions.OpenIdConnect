void PacketRestore()
{
    string command = @"if (-not (Test-Path .\.paket\paket.exe)) { .\.paket\paket.bootstrapper.exe }; .\.paket\paket.exe restore";
    RunPowerShellCommand(command);
}

void RunPowerShellCommand(string command)
{
    Information("Running command: {0}", command);
    var settings = new ProcessSettings
    {
        Arguments = string.Format("-NoProfile -Command \"{0}\"", command)
    };
    var exitCodeWithArgument = StartProcess("powershell", settings);
    if (exitCodeWithArgument != 0) {
        throw new Exception("Something bad happened. Exit code: " + exitCodeWithArgument);
    }
}

void PacketOutdated()
{
    string command = "$ok = $true; " +
                     "./.paket/paket.exe outdated | %{ " +
                     @"$ok = $ok -and !($_ -match 'Outdated packages found:') -or ($_ -match '^\d+ '); "+
                     "if ($ok) { Write-Host $_ } else { Write-Warning $_ } }";
    RunPowerShellCommand(command);
}

class PacketPackSettings
{
    public string OutputDirectory = "./out";
    public string Configuration = "release";
    public string Version = "1.0.0";
    public string BuildPlatform = "AnyCPU";
}

void PacketPack(PacketPackSettings settings)
{
    if (settings == null)
        settings = new PacketPackSettings();
    var command = string.Format(
        "./.paket/paket.exe pack {0} --include-referenced-projects --minimum-from-lock-file --build-platform {1} --version {2} --build-config {3}", 
        settings.OutputDirectory, settings.BuildPlatform,
        settings.Version, settings.Configuration);
    RunPowerShellCommand(command);
}
