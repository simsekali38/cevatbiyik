param()
$ErrorActionPreference = 'Stop'
$repoRoot = Split-Path $PSScriptRoot -Parent
$binaryRoot = Join-Path $repoRoot 'tests/CevatBiyik.IntegrationTests/bin/Release/net8.0'
Add-Type -Path (Join-Path $binaryRoot 'MySqlConnector.dll')
$secretPath = Join-Path $env:APPDATA 'Microsoft/UserSecrets/cevat-biyik-integration-tests/secrets.json'
$secretValues = Get-Content -LiteralPath $secretPath -Raw | ConvertFrom-Json -AsHashtable
$connectionValue = $secretValues['Testing:MySql:ConnectionString']
$builder = [MySqlConnector.MySqlConnectionStringBuilder]::new($connectionValue)
if ($builder.Database -cne 'cevodbtest' -or $builder.Server -cne '217.195.207.24' -or $builder.UserID -cne 'cevodbusertest') { throw 'Only approved cevodbtest is permitted.' }
$spec = Get-Content -LiteralPath (Join-Path $repoRoot 'docs/database/PROD_SAMPLE_DATA_COUNTS.json') -Raw | ConvertFrom-Json -AsHashtable
$db = [MySqlConnector.MySqlConnection]::new($connectionValue)
$helperCreated = $false
$helperName = 'seed_prod_sample_verify_' + [Guid]::NewGuid().ToString('N')
function Run-Sql([string] $sql) {
    $script:lastSqlStage = ($sql.Trim() -split '\s+')[0]
    $command = $db.CreateCommand(); $command.CommandText = $sql; $command.CommandTimeout = 60
    try { return $command.ExecuteScalar() } finally { $command.Dispose() }
}
function Get-Snapshot {
    $counts = [ordered]@{}
    foreach ($table in @($spec.tables.Keys | Sort-Object) + @('__EFMigrationsHistory')) {
        if ($table -notmatch '^[A-Za-z_]+$') { throw 'Invalid table name.' }
        $counts[$table] = [long](Run-Sql ('SELECT COUNT(*) FROM `cevodbtest`.`' + $table + '`'))
    }
    return $counts | ConvertTo-Json -Compress
}
try {
    Add-Type -Path (Join-Path $binaryRoot 'FluentValidation.dll')
    Add-Type -Path (Join-Path $binaryRoot 'CevatBiyik.Infrastructure.dll')
    $assets = Get-Content -LiteralPath (Join-Path $repoRoot 'docs/database/prod-sample-assets/manifest.json') -Raw | ConvertFrom-Json
    foreach ($asset in $assets) {
        $file = Join-Path $repoRoot ('docs/database/prod-sample-assets/' + $asset.path)
        if ((Get-Item -LiteralPath $file).Length -ne $asset.bytes -or (Get-FileHash -LiteralPath $file -Algorithm SHA256).Hash -cne $asset.sha256) { throw 'Fixture integrity mismatch.' }
        if ($asset.kind -eq 'document') { [CevatBiyik.Infrastructure.Media.PdfPolicy]::Validate([System.IO.File]::ReadAllBytes($file)) }
    }
    Write-Output 'PASS: 8 image and 3 PDF fixture hashes/sizes; PDF fixtures pass application PdfPolicy.'
    $db.Open(); $version = [string](Run-Sql 'SELECT VERSION()')
    if (-not ($version.StartsWith('10.3.39-MariaDB') -or $version.StartsWith('8.0.'))) { throw 'Unapproved server version.' }
    [void](Run-Sql 'SET NAMES utf8mb4 COLLATE utf8mb4_unicode_ci')
    $oldMode = [string](Run-Sql 'SELECT @@SESSION.sql_mode')
    $snapshot = Get-Snapshot; $before = $snapshot | ConvertFrom-Json -AsHashtable
    $tableCount = [int](Run-Sql "SELECT COUNT(*) FROM information_schema.TABLES WHERE TABLE_SCHEMA = 'cevodbtest' AND TABLE_TYPE = 'BASE TABLE'")
    if ($tableCount -ne $spec.tables.Count + 1) { throw 'Schema inventory changed; review all-table fixtures.' }
    $source = Get-Content -LiteralPath (Join-Path $repoRoot 'docs/database/PROD_SAMPLE_DATA.sql') -Raw
    $match = [regex]::Match($source, '(?s)CREATE PROCEDURE.*?END\$\$')
    if (-not $match.Success) { throw 'Routine boundary missing.' }
    $routine = $match.Value.Substring(0, $match.Value.Length - 2).Replace('seed_prod_sample_v2', $helperName)
    $checks = @()
    foreach ($table in $spec.tables.Keys) { $checks += ('(SELECT COUNT(*) FROM `cevodbtest`.`' + $table + '`) <> ' + ($before[$table] + $spec.tables[$table])) }
    $assertions = '    IF ' + ($checks -join "`n       OR ") + " THEN SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'All-table counts differ'; END IF;`n"
    $assertions += @'
    IF (SELECT COUNT(*) FROM `cevodbtest`.`Articles` WHERE Slug LIKE 'demo-%' AND IsActive = 1 AND PublishedAt <= v_now) <> 18
       OR (SELECT COUNT(*) FROM `cevodbtest`.`Articles` WHERE Slug LIKE 'demo-%' AND IsActive = 0 AND PublishedAt IS NULL) <> 3
       OR (SELECT COUNT(*) FROM `cevodbtest`.`Articles` WHERE Slug LIKE 'demo-%' AND IsActive = 1 AND PublishedAt > v_now) <> 3
       OR (SELECT COUNT(*) FROM `cevodbtest`.`AspNetUsers` WHERE Id LIKE 'prod-sample-v2-user-%' AND IsActive = 0 AND LockoutEnd > v_now) <> 2
       OR EXISTS (SELECT 1 FROM `cevodbtest`.`ContactMessages` WHERE Email LIKE 'demo.visitor%@example.invalid' AND NotificationPending = 1)
       OR EXISTS (SELECT 1 FROM `cevodbtest`.`Locations` WHERE Slug LIKE 'demo-%' AND FactsVerified = 1)
       OR NOT EXISTS (SELECT 1 FROM `cevodbtest`.`Locations` c JOIN `cevodbtest`.`Locations` p ON c.ParentId=p.Id WHERE c.Slug='demo-caglayan' AND c.Kind='Region' AND p.Slug='demo-sisli')
       OR (SELECT JSON_LENGTH(JSON_EXTRACT(Value, '$.Articles')) FROM `cevodbtest`.`SiteSettings` WHERE `Key`='TestData.ProdSample.v2.Ids') <> 24
    THEN SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Demo state validation failed'; END IF;
'@
    $successRoutine = $routine.Replace('    COMMIT;', $assertions + "`n    ROLLBACK;")
    [void](Run-Sql $successRoutine); $helperCreated = $true
    [void](Run-Sql ('CALL `cevodbtest`.`' + $helperName + '`()'))
    if ((Get-Snapshot) -cne $snapshot) { throw 'Counts changed after rollback.' }
    Write-Output ('PASS: all ' + $spec.tables.Count + ' data tables filled; ' + $spec.totalRows + ' inserts; 18 published / 3 draft / 3 scheduled; hierarchy, FKs and safety states.')
    [void](Run-Sql ('DROP PROCEDURE `cevodbtest`.`' + $helperName + '`')); $helperCreated = $false
    foreach ($failure in @("    SIGNAL SQLSTATE '45000' SET MESSAGE_TEXT = 'Forced verification failure';", "    INSERT INTO ``cevodbtest``.``SiteSettings`` (``Key``, Value, CreatedAt, IsActive, IsDeleted) VALUES ('TestData.ProdSample.v2.Manifest', 'collision test', v_now, 0, 0);`n    COMMIT;")) {
        [void](Run-Sql ($routine.Replace('    COMMIT;', $failure))); $helperCreated = $true
        $rejected = $false
        try { [void](Run-Sql ('CALL `cevodbtest`.`' + $helperName + '`()')) } catch { $rejected = $true }
        if (-not $rejected -or (Get-Snapshot) -cne $snapshot) { throw 'Failure rollback validation failed.' }
        if ([int](Run-Sql 'SELECT @@SESSION.foreign_key_checks') -ne 1 -or [string](Run-Sql 'SELECT @@SESSION.sql_mode') -cne $oldMode) { throw 'Session state was not restored.' }
        [void](Run-Sql ('DROP PROCEDURE `cevodbtest`.`' + $helperName + '`')); $helperCreated = $false
    }
    Write-Output 'PASS: forced late failure + duplicate-key rollback; all table/migration counts, FK checks and SQL mode preserved.'
    Write-Output ('Server verified: ' + $version + '; no permanent sample content inserted.')
} catch {
    $cause = $_.Exception
    while ($cause.InnerException) { $cause = $cause.InnerException }
    Write-Error ('SQL sample verification failed (' + $_.Exception.GetType().Name + '; step=' + $lastSqlStage + '; code=' + $cause.Number + ').')
} finally {
    if ($db.State -eq [System.Data.ConnectionState]::Open -and $helperCreated) {
        try { [void](Run-Sql ('DROP PROCEDURE `cevodbtest`.`' + $helperName + '`')) } catch { Write-Warning 'Temporary verification routine requires cleanup.' }
    }
    $db.Dispose()
}
