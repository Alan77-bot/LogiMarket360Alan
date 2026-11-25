param([ValidateSet('RevisionRoles','RevisionFlujo')][string]$Escenario = 'RevisionRoles')
# Comprueba menus y acciones con cuentas ficticias en una base temporal nueva.
$ErrorActionPreference = 'Stop'
$raizProyecto = [IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$carpetaPrueba = Join-Path $raizProyecto ('tmp/roles-' + [Guid]::NewGuid().ToString('N'))
New-Item -ItemType Directory -Path $carpetaPrueba -Force | Out-Null
$referencia = [Security.SecurityElement]::Escape((Join-Path $raizProyecto 'LogiMarket.WinForms/LogiMarket.WinForms.csproj'))
$fuente = [Security.SecurityElement]::Escape((Join-Path $PSScriptRoot ($Escenario + '.cs')))
@"
<Project Sdk="Microsoft.NET.Sdk">
 <PropertyGroup><OutputType>WinExe</OutputType><TargetFramework>net8.0-windows</TargetFramework><UseWindowsForms>true</UseWindowsForms><ImplicitUsings>enable</ImplicitUsings><Nullable>enable</Nullable></PropertyGroup>
 <ItemGroup><ProjectReference Include="$referencia"/><Compile Include="$fuente" Link="RevisionRoles.cs"/></ItemGroup>
</Project>
"@ | Set-Content -LiteralPath (Join-Path $carpetaPrueba 'RevisionRoles.csproj')
$salidaBuild = Join-Path $carpetaPrueba 'compilacion'
dotnet build (Join-Path $carpetaPrueba 'RevisionRoles.csproj') --artifacts-path $salidaBuild -m:1 -p:NuGetAudit=false -v:minimal
if ($LASTEXITCODE -ne 0) { throw 'Fallo la compilacion de la comprobacion.' }
dotnet (Join-Path $salidaBuild 'bin/RevisionRoles/debug/RevisionRoles.dll') $carpetaPrueba
if ($LASTEXITCODE -ne 0) { throw "Fallo la comprobacion. Ver $carpetaPrueba/roles-ui.txt" }
Get-Content -LiteralPath (Join-Path $carpetaPrueba $(if ($Escenario -eq 'RevisionRoles') { 'roles-ui.txt' } else { 'flujo-ui.txt' }))
Write-Output "Evidencias: $carpetaPrueba"
