#requires -Version 7.0
[CmdletBinding()]
param(
    [ValidateSet('Debug', 'Release')][string]$Configuration = 'Release',
    [switch]$Browser,
    [switch]$MySql,
    [switch]$MigrationMySql,
    [switch]$HostedMySql,
    [switch]$SecurityScan
)
$ErrorActionPreference = 'Stop'
Set-StrictMode -Version Latest
$taskRoot = Split-Path $PSScriptRoot -Parent
$savedLocation = Get-Location
$mysqlName = 'CEVATBIYIK_TEST_MYSQL'
$migrationName = 'CEVATBIYIK_TEST_MYSQL_MIGRATIONS'
$savedMysql = [Environment]::GetEnvironmentVariable($mysqlName, 'Process')
$savedMigration = [Environment]::GetEnvironmentVariable($migrationName, 'Process')
$savedCompatibility = [Environment]::GetEnvironmentVariable('CEVATBIYIK_TEST_HOSTED_COMPATIBILITY', 'Process')
$fixture = $null
function Invoke-Check([string]$Command, [string[]]$Arguments) {
    & $Command @Arguments
    if ($LASTEXITCODE -ne 0) { throw "$Command verification failed (exit $LASTEXITCODE)." }
}
try {
    Set-Location -LiteralPath $taskRoot
    if ($HostedMySql) {
        Write-Host 'Hosted compatibility diagnosis: known MariaDB 10.3.39 target, NOT MySQL 8 verification.'
        [Environment]::SetEnvironmentVariable('CEVATBIYIK_TEST_HOSTED_COMPATIBILITY', '1', 'Process')
        if ($MySql) { throw 'HostedMySql selects its own connection; do not combine with MySql.' }
        $secretFile = Join-Path $env:APPDATA 'Microsoft/UserSecrets/cevat-biyik-integration-tests/secrets.json'
        if (-not (Test-Path -LiteralPath $secretFile)) { throw 'Configure the test project user-secrets connection first.' }
        $secretValues = Get-Content -LiteralPath $secretFile -Raw | ConvertFrom-Json -AsHashtable
        $hostedConnection = $secretValues['Testing:MySql:ConnectionString']
        if ([string]::IsNullOrWhiteSpace($hostedConnection)) { throw 'Missing hosted test connection in user-secrets.' }
        if ($MigrationMySql) {
            [Environment]::SetEnvironmentVariable($migrationName, $hostedConnection, 'Process')
        } else {
            $MySql = $true
            [Environment]::SetEnvironmentVariable($mysqlName, $hostedConnection, 'Process')
        }
    }
    if ($MySql -and [string]::IsNullOrWhiteSpace([Environment]::GetEnvironmentVariable($mysqlName, 'Process'))) { throw 'Dedicated MySQL test connection is required.' }
    if ($MigrationMySql -and [string]::IsNullOrWhiteSpace([Environment]::GetEnvironmentVariable($migrationName, 'Process'))) { throw 'Empty migration-test database is required.' }
    if (-not $MySql) { [Environment]::SetEnvironmentVariable($mysqlName, $null, 'Process') }
    if (-not $MigrationMySql) { [Environment]::SetEnvironmentVariable($migrationName, $null, 'Process') }
    Invoke-Check dotnet @('build', 'CevatBiyik.sln', '-c', $Configuration, '--no-restore', '-m:1', '-p:UseSharedCompilation=false')
    Invoke-Check dotnet @('test', 'CevatBiyik.sln', '-c', $Configuration, '--no-build', '--no-restore', '-m:1', '--logger', 'trx;LogFilePrefix=regression')
    Invoke-Check node @('--test', 'tests/CevatBiyik.BrowserTests/analytics.test.cjs')
    Invoke-Check node @('tools/build-assets.cjs', '--check')
    Invoke-Check python @('tools/security_scan.py', '--self-test')
    Invoke-Check dotnet @('format', 'style', 'CevatBiyik.sln', '--verify-no-changes', '--no-restore', '--diagnostics', 'IDE0011')
    if ($Browser) {
        Invoke-Check node @('-e', "require(require.resolve('playwright', {paths:['./tests/CevatBiyik.BrowserTests']}))")
        if (Get-NetTCPConnection -LocalPort 57413 -State Listen -ErrorAction SilentlyContinue) { throw 'Fixture port 57413 is already in use; refusing to reuse a running server.' }
        $logs = Join-Path $taskRoot 'artifacts/test-runs'
        New-Item -ItemType Directory -Path $logs -Force | Out-Null
        $dll = Join-Path $taskRoot "tests/CevatBiyik.E2EHost/bin/$Configuration/net8.0/CevatBiyik.E2EHost.dll"
        $fixture = Start-Process -FilePath (Get-Command dotnet).Source -ArgumentList @('"' + $dll + '"', '--fixture-only') -WorkingDirectory $taskRoot -WindowStyle Hidden -PassThru -RedirectStandardOutput (Join-Path $logs 'fixture.out.log') -RedirectStandardError (Join-Path $logs 'fixture.err.log')
        $ready = $false
        for ($attempt = 0; $attempt -lt 40; $attempt++) {
            if ($fixture.HasExited) { throw 'Isolated fixture exited; inspect artifacts/test-runs/fixture.err.log.' }
            # Fixed loopback probe only; no proxy, external URL or OS certificate-trust changes.
            & node -e "const r=require('node:https').get('https://localhost:57413/',{rejectUnauthorized:false},s=>{s.resume();process.exit(s.statusCode===200 && s.headers['x-test-fixture']==='isolated-phase13'?0:1)});r.on('error',()=>process.exit(1));r.setTimeout(2000,()=>r.destroy());"
            if ($LASTEXITCODE -eq 0) { $ready = $true; break }
            Start-Sleep -Milliseconds 500
        }
        if (-not $ready) { throw 'Isolated HTTPS fixture did not become ready.' }
        Invoke-Check node @('--test', '--test-concurrency=1', 'tests/CevatBiyik.BrowserTests/e2e.test.cjs')
        Invoke-Check node @('--test', '--test-concurrency=1', 'tests/CevatBiyik.BrowserTests/phase14.test.cjs')
        Invoke-Check node @('--test', '--test-concurrency=1', 'tests/CevatBiyik.BrowserTests/admin-ux.test.cjs')
    }
    if ($SecurityScan) { Invoke-Check python @('tools/security_scan.py', '--history') }
    Write-Host 'Requested checks passed. Explicit MySQL skips are not live provider verification.'
} finally {
    if ($null -ne $fixture -and -not $fixture.HasExited) { $fixture.Kill($true); $fixture.WaitForExit(5000) | Out-Null }
    [Environment]::SetEnvironmentVariable($mysqlName, $savedMysql, 'Process')
    [Environment]::SetEnvironmentVariable($migrationName, $savedMigration, 'Process')
    [Environment]::SetEnvironmentVariable('CEVATBIYIK_TEST_HOSTED_COMPATIBILITY', $savedCompatibility, 'Process')
    Set-Location -LiteralPath $savedLocation.Path
}
