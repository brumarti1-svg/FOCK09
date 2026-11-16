# Converte un Orbite.mdb (Access/Jet) in Orbite.db (SQLite) con lo schema
# di un Orbite.db di riferimento (es. quello di FOC6). Non serve Access.
# Va eseguito con PowerShell a 32 bit, perché i driver Jet/ACE sono a 32 bit:
#   %WINDIR%\SysWOW64\WindowsPowerShell\v1.0\powershell.exe -ExecutionPolicy Bypass -File mdb2sqlite.ps1 `
#     -Mdb <Orbite.mdb> -SchemaDb <Orbite.db di riferimento> -OutDb <nuovo Orbite.db> -BinDir <cartella con System.Data.SQLite.dll>
# Formati: date gg/mm/aaaa hh:mm[:ss], ore hh:mm, numeri REAL, Fasi.Ultima 0/1,
# altri booleani True/False, Satelliti.Verso = FALSO (colonna assente negli mdb).
param([string]$Mdb, [string]$SchemaDb, [string]$OutDb, [string]$BinDir)
$ErrorActionPreference = 'Stop'
Add-Type -Path (Join-Path $BinDir 'System.Data.SQLite.dll')
$inv = [Globalization.CultureInfo]::InvariantCulture
if (Test-Path $OutDb) { Remove-Item $OutDb }

# Schema di riferimento
$ref = New-Object System.Data.SQLite.SQLiteConnection("Data Source=$SchemaDb;Read Only=True"); $ref.Open()
$c = $ref.CreateCommand(); $c.CommandText = "SELECT name, sql FROM sqlite_master WHERE type='table' AND name NOT LIKE 'sqlite_%'"
$r = $c.ExecuteReader(); $schema = [ordered]@{}
while ($r.Read()) { $schema[$r.GetString(0)] = $r.GetString(1) }
$r.Close()
$colTypes = @{}
foreach ($t in $schema.Keys) {
    $c.CommandText = "PRAGMA table_info([$t])"; $r = $c.ExecuteReader(); $h = [ordered]@{}
    while ($r.Read()) { $h[$r.GetString(1)] = "$($r.GetValue(2))".ToUpper() }
    $r.Close(); $colTypes[$t] = $h
}
$ref.Close()

$out = New-Object System.Data.SQLite.SQLiteConnection("Data Source=$OutDb"); $out.Open()
foreach ($t in $schema.Keys) { $c = $out.CreateCommand(); $c.CommandText = $schema[$t]; [void]$c.ExecuteNonQuery() }

function Convert-Val($v, [string]$col, [string]$type) {
    if ($v -is [DBNull] -or $null -eq $v) { return [DBNull]::Value }
    if ($v -is [DateTime]) {
        $base = [DateTime]::new(1899, 12, 30)
        if ($v.Date -eq $base) { return $(if ($v.Second) { $v.ToString('HH:mm:ss') } else { $v.ToString('HH:mm') }) }
        if ($v.TimeOfDay.Ticks -eq 0) { return $v.ToString('dd/MM/yyyy') }
        return $(if ($v.Second) { $v.ToString('dd/MM/yyyy HH:mm:ss') } else { $v.ToString('dd/MM/yyyy HH:mm') })
    }
    if ($v -is [bool]) {
        if ($col -eq 'Ultima') { return [int]$v }
        return $(if ($v) { 'True' } else { 'False' })
    }
    if ($v -is [single]) { $v = [double]::Parse($v.ToString('R', $inv), $inv) }
    if ($v -is [double] -or $v -is [decimal]) {
        if ($type -eq 'TEXT') { return ([double]$v).ToString('R', $inv) }
        return [double]$v
    }
    if ($v -is [string]) {
        if ($type -in 'INTEGER', 'REAL') {
            if ($v.Trim() -eq '') { return [DBNull]::Value }
            $d = 0.0
            if ([double]::TryParse($v.Trim(), [Globalization.NumberStyles]::Float, $inv, [ref]$d)) { return $d }
        }
        return $v
    }
    return $v
}

$o = New-Object System.Data.OleDb.OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=$Mdb;Mode=Read"); $o.Open()
$mdbTabs = $o.GetSchema('Tables') | Where-Object { $_.TABLE_TYPE -eq 'TABLE' } | ForEach-Object { $_.TABLE_NAME }
foreach ($t in $schema.Keys) {
    if ($mdbTabs -notcontains $t) { "  $t : non presente nell'mdb, lasciata vuota"; continue }
    $cmd = $o.CreateCommand(); $cmd.CommandText = "SELECT * FROM [$t]"; $rd = $cmd.ExecuteReader()
    $src = @{}; for ($i = 0; $i -lt $rd.FieldCount; $i++) { $src[$rd.GetName($i)] = $i }
    $cols = @($colTypes[$t].Keys | Where-Object { $src.ContainsKey($_) })
    $extra = @($colTypes[$t].Keys | Where-Object { -not $src.ContainsKey($_) })
    $lost = @($src.Keys | Where-Object { -not $colTypes[$t].Contains($_) })
    $all = $cols + $extra
    $ins = $out.CreateCommand()
    $ins.CommandText = "INSERT INTO [$t] (" + (($all | ForEach-Object { "[$_]" }) -join ',') + ") VALUES (" + (($all | ForEach-Object { '@p' + [array]::IndexOf($all, $_) }) -join ',') + ")"
    for ($k = 0; $k -lt $all.Count; $k++) { [void]$ins.Parameters.Add((New-Object System.Data.SQLite.SQLiteParameter("@p$k"))) }
    $tx = $out.BeginTransaction(); $n = 0
    while ($rd.Read()) {
        for ($k = 0; $k -lt $cols.Count; $k++) { $ins.Parameters[$k].Value = Convert-Val $rd.GetValue($src[$cols[$k]]) $cols[$k] $colTypes[$t][$cols[$k]] }
        for ($k = $cols.Count; $k -lt $all.Count; $k++) { $ins.Parameters[$k].Value = $(if ($all[$k] -eq 'Verso') { 'FALSO' } else { [DBNull]::Value }) }
        [void]$ins.ExecuteNonQuery(); $n++
    }
    $tx.Commit(); $rd.Close()
    $msg = "  {0}: {1} righe" -f $t, $n
    if ($extra) { $msg += "; colonne nuove: " + ($extra -join ',') }
    if ($lost) { $msg += "; colonne mdb scartate: " + ($lost -join ',') }
    $msg
}
$o.Close(); $out.Close()
