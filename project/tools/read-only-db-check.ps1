#requires -Version 7.0
[CmdletBinding()]
param([string]$ConfigurationPath = (Join-Path $PSScriptRoot '../src/CevatBiyik.Web/appsettings.Development.json'))
$ErrorActionPreference = 'Stop'
$taskRoot = Split-Path $PSScriptRoot -Parent
# No arbitrary SQL, credentials in output, schema writes or migration execution.
$config = Get-Content -LiteralPath $ConfigurationPath -Raw | ConvertFrom-Json -AsHashtable
$libraryRoot = Join-Path $taskRoot 'tests/CevatBiyik.IntegrationTests/bin/Release/net8.0'
[Reflection.Assembly]::LoadFrom((Join-Path $libraryRoot 'Microsoft.Extensions.Logging.Abstractions.dll')) | Out-Null
[Reflection.Assembly]::LoadFrom((Join-Path $libraryRoot 'MySqlConnector.dll')) | Out-Null
$builder = [MySqlConnector.MySqlConnectionStringBuilder]::new($config.ConnectionStrings.DefaultConnection)
if ($builder.Database -cne 'cevodbtest') { throw 'Only the explicitly approved cevodbtest database is allowed.' }
$builder.ConnectionTimeout = 10
$builder.DefaultCommandTimeout = 10
$connection = [MySqlConnector.MySqlConnection]::new($builder.ConnectionString)
try {
    $connection.Open()
    $command = $connection.CreateCommand()
    $command.CommandText = 'SELECT VERSION(), @@character_set_database, @@collation_database, @@lower_case_table_names'
    $reader = $command.ExecuteReader()
    $null = $reader.Read()
    $result = [ordered]@{ checkedUtc = [DateTime]::UtcNow.ToString('O'); database = 'cevodbtest'; readOnly = $true; serverVersion = $reader.GetString(0); charset = $reader.GetString(1); collation = $reader.GetString(2); lowerCaseTableNames = $reader.GetInt32(3) }
    $reader.Dispose()
    $command.CommandText = "SHOW SESSION STATUS LIKE 'Ssl_cipher'"
    $reader = $command.ExecuteReader()
    $null = $reader.Read()
    $result.encryptedTransport = -not [string]::IsNullOrEmpty($reader.GetString(1))
    $reader.Dispose()
    $command.CommandText = 'SELECT COUNT(*) FROM __EFMigrationsHistory'
    $result.migrationCount = [int]$command.ExecuteScalar()
    $command.CommandText = 'SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = @database'
    $null = $command.Parameters.AddWithValue('@database', 'cevodbtest')
    $reader = $command.ExecuteReader()
    $tables = [Collections.Generic.List[string]]::new()
    while ($reader.Read()) { $tables.Add($reader.GetString(0)) }
    $reader.Dispose()
    $result.tableCount = $tables.Count
    $result.missingRequiredTables = @(@('Articles','PracticeAreas','ContactMessages','SiteSettings','LawyerProfiles','PageContents','FaqItems','Locations','RedirectRules','MediaAssets','AspNetUsers','__EFMigrationsHistory') | Where-Object { $_ -notin $tables })
    $outputFolder = Join-Path $taskRoot '../artifacts/test-runs'
    New-Item -ItemType Directory -Path $outputFolder -Force | Out-Null
    $result | ConvertTo-Json -Depth 4 | Set-Content -LiteralPath (Join-Path $outputFolder 'read-only-db-check.json') -Encoding utf8
    $result | ConvertTo-Json -Depth 4 | Write-Output
    if ($result.missingRequiredTables.Count) { throw 'Expected tables missing; see the redacted diagnostic.' }
} catch {
    # Never print connection exceptions: they may contain account or host details.
    Write-Error 'Read-only database check did not complete. No migration or data write was requested.'
} finally { $connection.Dispose(); $builder.Clear(); $config.Clear() }
