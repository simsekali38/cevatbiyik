#requires -Version 7.0
[CmdletBinding()]
param([switch]$SkipTests)
$ErrorActionPreference = 'Stop'
$taskRoot = Split-Path $PSScriptRoot -Parent
Push-Location $taskRoot
try {
    # Fresh immutable release directory; never delete or synchronize an existing deployment.
    $release = Join-Path $taskRoot ('../artifacts/releases/' + [DateTime]::UtcNow.ToString('yyyyMMdd-HHmmss') + '-' + [Guid]::NewGuid().ToString('N').Substring(0, 8))
    if (-not $SkipTests) {
        & (Join-Path $PSScriptRoot 'test.ps1')
        if ($LASTEXITCODE -ne 0) { throw 'Pre-publish verification failed.' }
    }
    dotnet publish src/CevatBiyik.Web/CevatBiyik.Web.csproj -c Release --no-restore --self-contained false -p:UseAppHost=false -p:UseSharedCompilation=false -o $release
    if ($LASTEXITCODE -ne 0) { throw 'Publish failed.' }
    python tools/verify_publish.py $release
    if ($LASTEXITCODE -ne 0) { throw 'Release verification failed; do not deploy this directory.' }
    $manifest = Get-ChildItem -LiteralPath $release -File -Recurse | ForEach-Object {
        [ordered]@{ path = [IO.Path]::GetRelativePath($release, $_.FullName).Replace('\', '/'); sha256 = (Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256).Hash; bytes = $_.Length }
    }
    $commit = git rev-parse HEAD
    [ordered]@{ commit = $commit; dirtyWorkingTree = [bool](git status --porcelain); createdUtc = [DateTime]::UtcNow.ToString('O'); target = 'net8.0 / framework-dependent Windows x64 / IIS out-of-process'; files = @($manifest) } |
        ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $release 'RELEASE_MANIFEST.json') -Encoding utf8
    Write-Host "Verified release directory: $release"
    Write-Host 'Server-owned JSON, migrations, database, uploads and private files are not applied by this command.'
} finally { Pop-Location }
