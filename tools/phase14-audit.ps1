#requires -Version 7.0
[CmdletBinding()]
param([ValidateSet('baseline', 'after')][string]$Stage = 'after', [switch]$NoBuild, [switch]$BrowserChecksOnly)
$ErrorActionPreference = 'Stop'
$taskRoot = Split-Path $PSScriptRoot -Parent
$fixture = $null
Push-Location $taskRoot
try {
    if (-not $NoBuild) {
        dotnet build CevatBiyik.sln -c Release --no-restore -m:1 -p:UseSharedCompilation=false
        if ($LASTEXITCODE -ne 0) { throw 'Audit build failed.' }
    }
    if (Get-NetTCPConnection -LocalPort 57413 -State Listen -ErrorAction SilentlyContinue) { throw 'Fixture port occupied; refusing another server.' }
    $logs = Join-Path $taskRoot "artifacts/phase14/$Stage"
    New-Item -ItemType Directory -Path $logs -Force | Out-Null
    $dll = Join-Path $taskRoot 'tests/CevatBiyik.E2EHost/bin/Release/net8.0/CevatBiyik.E2EHost.dll'
    $fixture = Start-Process -FilePath (Get-Command dotnet).Source -ArgumentList @('"' + $dll + '"', '--fixture-only') -WorkingDirectory $taskRoot -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $logs 'fixture.out.log') -RedirectStandardError (Join-Path $logs 'fixture.err.log')
    $ready = $false
    for ($attempt = 0; $attempt -lt 40; $attempt++) {
        if ($fixture.HasExited) { throw 'Fixture exited; inspect audit logs.' }
        node -e "const r=require('node:https').get('https://localhost:57413/',{rejectUnauthorized:false},s=>{s.resume();process.exit(s.statusCode===200&&s.headers['x-test-fixture']==='isolated-phase13'?0:1)});r.on('error',()=>process.exit(1));r.setTimeout(2000,()=>r.destroy());"
        if ($LASTEXITCODE -eq 0) { $ready = $true; break }
        Start-Sleep -Milliseconds 500
    }
    if (-not $ready) { throw 'Fixture not ready.' }
    if ($BrowserChecksOnly) {
        foreach ($suite in @('e2e', 'phase14', 'admin-ux')) {
            node --test --test-concurrency=1 "tests/CevatBiyik.BrowserTests/$suite.test.cjs"
            if ($LASTEXITCODE -ne 0) { throw "$suite browser checks failed." }
        }
    }
    else { node tests/CevatBiyik.BrowserTests/phase14-audit.mjs $Stage }
    if ($LASTEXITCODE -ne 0) { throw 'Phase 14 audit failed; inspect summary.json.' }
} finally {
    if ($null -ne $fixture -and -not $fixture.HasExited) { $fixture.Kill($true); $fixture.WaitForExit(5000) | Out-Null }
    Pop-Location
}
