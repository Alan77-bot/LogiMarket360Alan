# El paquete sale del ultimo commit, no de binarios ni archivos locales sueltos.
$ErrorActionPreference = 'Stop'
$raizProyecto = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$carpetaSalida = Join-Path $raizProyecto 'output'
New-Item -ItemType Directory -Path $carpetaSalida -Force | Out-Null
$archivoZip = Join-Path $carpetaSalida 'LogiMarket360Alan-codigo.zip'
git -C $raizProyecto archive --format=zip --output=$archivoZip HEAD
if ($LASTEXITCODE -ne 0) { throw 'No se pudo exportar el codigo.' }
Add-Type -AssemblyName System.IO.Compression.FileSystem
$paquete = [IO.Compression.ZipFile]::OpenRead($archivoZip)
try {
    $prohibidos = @($paquete.Entries | Where-Object { $_.FullName -match '(^|/)(bin|obj|\.vs|\.git|tmp)/|\.(db|sqlite|suo|user)$' })
    if ($prohibidos.Count -gt 0) { throw 'El ZIP contiene archivos locales no permitidos.' }
    if (@($paquete.Entries | Where-Object FullName -Like '*.csproj').Count -ne 5) { throw 'Se esperaban cinco proyectos.' }
    Write-Output "Paquete: $archivoZip"
    Write-Output "Entradas: $($paquete.Entries.Count)"
} finally { $paquete.Dispose() }