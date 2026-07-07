[CmdletBinding(SupportsShouldProcess = $true)]
param(
    [string]$DestinationPath = 'C:\OneDrive\src\Production\Agent01',
    [string]$Configuration = 'Release',
    [switch]$CleanDestination
)

Set-StrictMode -Version Latest
$ErrorActionPreference = 'Stop'

$projectPath = Join-Path $PSScriptRoot 'agent01.csproj'

if (-not (Test-Path -LiteralPath $projectPath)) {
    throw "Project file not found: $projectPath"
}

Get-Command dotnet -ErrorAction Stop | Out-Null

if (-not (Test-Path -LiteralPath $DestinationPath)) {
    if ($PSCmdlet.ShouldProcess($DestinationPath, 'Create publish directory')) {
        New-Item -ItemType Directory -Path $DestinationPath -Force | Out-Null
    }
}
elseif ($CleanDestination.IsPresent) {
    if ($PSCmdlet.ShouldProcess($DestinationPath, 'Remove existing publish directory contents')) {
        Get-ChildItem -LiteralPath $DestinationPath -Force | Remove-Item -Recurse -Force
    }
}

$publishArguments = @(
    'publish'
    $projectPath
    '-c'
    $Configuration
    '-o'
    $DestinationPath
    '--nologo'
)

if ($PSCmdlet.ShouldProcess($DestinationPath, "Publish $Configuration build")) {
    & dotnet @publishArguments

    if ($LASTEXITCODE -ne 0) {
        throw "dotnet publish failed with exit code $LASTEXITCODE"
    }
}

$localSettingsPath = Join-Path $DestinationPath 'appsettings.local.json'

if (Test-Path -LiteralPath $localSettingsPath) {
    if ($PSCmdlet.ShouldProcess($localSettingsPath, 'Remove local settings from published output')) {
        Remove-Item -LiteralPath $localSettingsPath -Force
    }
}

Write-Host "Publish script completed. Output folder: $DestinationPath"