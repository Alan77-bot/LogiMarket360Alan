# Extrae una copia nueva y comprueba la solucion sin reutilizar bin ni obj.
param([Parameter(Mandatory=$true)][string]$RutaZip)
$ErrorActionPreference = 'Stop'
$raizProyecto = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$rutaArchivo = [IO.Path]::GetFullPath($RutaZip)
$directorioPrueba = Join-Path $raizProyecto ("tmp/verificacion-" + [Guid]::NewGuid().ToString('N'))
$rutaLog = Join-Path $raizProyecto 'docs/evidencias/compilacion-limpia.txt'
New-Item -ItemType Directory -Path $directorioPrueba -Force | Out-Null
Expand-Archive -LiteralPath $rutaArchivo -DestinationPath $directorioPrueba
$proyectos = @(Get-ChildItem -LiteralPath $directorioPrueba -Filter '*.csproj' -Recurse)
if ($proyectos.Count -ne 5) { throw 'Se esperaban exactamente cinco proyectos.' }
$generados = @(Get-ChildItem -LiteralPath $directorioPrueba -Directory -Recurse | Where-Object Name -in 'bin','obj','.vs')
if ($generados.Count -ne 0) { throw 'La copia no esta limpia.' }
@("Archivo: $rutaArchivo", "SHA256: $((Get-FileHash -LiteralPath $rutaArchivo -Algorithm SHA256).Hash)", "Proyectos: $($proyectos.Count)", "Copia sin bin/obj: $directorioPrueba") | Set-Content -LiteralPath $rutaLog
Push-Location $directorioPrueba
try {
    dotnet build LogiMarket360Alan.slnx -m:1 -p:NuGetAudit=false -v:minimal 2>&1 | Tee-Object -FilePath $rutaLog -Append
    if ($LASTEXITCODE -ne 0) { throw 'Fallo la compilacion desde la copia limpia.' }
    'RESULTADO: APROBADO' | Tee-Object -FilePath $rutaLog -Append
} finally { Pop-Location }
