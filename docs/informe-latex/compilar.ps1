$ErrorActionPreference = "Stop"
Push-Location $PSScriptRoot
try {
    tectonic -b https://data1.fullyjustified.net/tlextras-2021.3r1.tar informe.tex
    if ($LASTEXITCODE -ne 0) { throw "No se pudo compilar el informe" }
} finally { Pop-Location }
