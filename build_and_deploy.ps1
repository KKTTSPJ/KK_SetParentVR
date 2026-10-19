# Build (and optionally deploy) KK_SetParentVR (BepInEx 5) for Koikatsu official VR and Koikatsu Party VR.
#
#   pwsh -File build_and_deploy.ps1                 build -> checks -> copy to the deploy folder
#   pwsh -File build_and_deploy.ps1 -SkipDeploy     build and checks only
#
# Reference paths default to the development layout (MyPlugin\KoikatuVR_Data\Managed, MyPlugin\BepInEx\core).
# Override with -GameManagedPath <game>\KoikatuVR_Data\Managed -BepInExCorePath <game>\BepInEx\core
#
# Note (development layout): MyPlugin\Deploy is a junction to the BepInEx\plugins\Deploy of the KK and KKP games, so
# deploying makes the build live immediately. The BepInEx 4 version (BepInEx\KK_SetParentVR.dll, loaded through the
# BepInEx 4 patcher) must not be installed at the same time.
param(
    [switch]$SkipDeploy,
    [string]$DeployDir = "",
    [string]$GameManagedPath = "",
    [string]$BepInExCorePath = ""
)

$ErrorActionPreference = "Stop"
$Root = Split-Path -Parent $MyInvocation.MyCommand.Definition
if (-not $DeployDir) { $DeployDir = Join-Path $Root "..\Deploy\KK_SetParentVR" }
if (-not $GameManagedPath) { $GameManagedPath = Join-Path $Root "..\KoikatuVR_Data\Managed" }
if (-not $BepInExCorePath) { $BepInExCorePath = Join-Path $Root "..\BepInEx\core" }
$Project = Join-Path $Root "SetParentKK\SetParentKK.csproj"
$OutputDll = Join-Path $Root "SetParentKK\bin\Release\net35\KK_SetParentVR.dll"

# RegexOptions.Compiled breaks type initialization on Unity 5.6 / Mono (net35); never allow it.
$compiled = Select-String -Path (Join-Path $Root "SetParentKK\*.cs") -Pattern "RegexOptions\.Compiled"
if ($compiled) {
    Write-Error "RegexOptions.Compiled is not allowed: $($compiled -join ', ')"
    exit 1
}

Write-Host "=== Building KK_SetParentVR ===" -ForegroundColor Green
dotnet build $Project -c Release --nologo -v q "-p:GameManagedPath=$GameManagedPath" "-p:BepInExCorePath=$BepInExCorePath"
if ($LASTEXITCODE -ne 0) {
    Write-Error "Build failed."
    exit 1
}
$info = Get-Item $OutputDll
Write-Host ("Build OK: {0} ({1} bytes, {2})" -f $info.FullName, $info.Length, $info.LastWriteTime) -ForegroundColor Cyan

if ($SkipDeploy) {
    Write-Host "Deploy skipped (-SkipDeploy)." -ForegroundColor Yellow
    exit 0
}

if (-not (Test-Path $DeployDir)) {
    New-Item -ItemType Directory -Force $DeployDir | Out-Null
}
Copy-Item $OutputDll $DeployDir -Force
$deployed = Get-Item (Join-Path $DeployDir "KK_SetParentVR.dll")
Write-Host ("=== Deployed to {0} ({1} bytes, {2}) ===" -f $deployed.FullName, $deployed.Length, $deployed.LastWriteTime) -ForegroundColor Green
