# LogiMarket360Alan

Aplicacion de escritorio academica con C# 12, .NET 8, WinForms, EF Core 8 y SQLite. Interfaz sin imagenes, navegacion por rol, ventas, kits anidados, stock, cobros simulados, despacho, devoluciones, auditoria y reportes.

## Ejecutar

Abrir LogiMarket360Alan.slnx en Visual Studio y establecer LogiMarket.WinForms como proyecto de inicio. El formato slnx requiere una version compatible de Visual Studio/SDK; el SDK instalado es 10.0.401 y los proyectos apuntan a .NET 8. Tambien se puede compilar el csproj directamente con SDK .NET 8.

```powershell
dotnet restore LogiMarket360Alan.slnx
dotnet build LogiMarket360Alan.slnx
dotnet run --project LogiMarket.WinForms
```

Cerrar la aplicacion antes de volver a compilar para evitar archivos bloqueados.

## Cuentas ficticias

Usuarios admin, ventas, bodega y consulta. Clave de demostracion: Demo123! para los cuatro. Se almacena hash PBKDF2 con sal y 210000 iteraciones; no claves reales. ADMIN opera todo; VENTAS clientes, pedidos, cobros, devoluciones y reportes; BODEGA inventario y despacho; CONSULTA solo lectura/exportacion. Catalogo es de lectura salvo para ADMIN.

## Base de datos

Al arrancar se aplican las migraciones y la semilla idempotente. La base se crea en %LOCALAPPDATA%\LogiMarket360Alan\logimarket.db y los TXT en comprobantes. No utiliza la base del proyecto anterior. Para reiniciar datos, cerrar y renombrar la carpeta como respaldo; no borrar datos que se quieran conservar.

```powershell
dotnet tool restore
dotnet ef migrations list --project LogiMarket.Infrastructure
```

## Probar

```powershell
dotnet test tests/LogiMarket.Tests
powershell -File scripts/verificar_roles.ps1
powershell -File scripts/verificar_roles.ps1 -Escenario RevisionFlujo
```

Las pruebas crean bases temporales independientes. El ejecutor visual abre formularios reales, conserva capturas/logs en tmp y no altera datos del usuario. La ultima ejecucion completa registrada tiene 62 pruebas aprobadas.

Flujo de demostracion: VENTAS prepara Ana + KIT_TELETRABAJO x1 ESTANDAR, guarda, valida y cobra 59.00. BODEGA despacha y entrega. VENTAS devuelve por DEFECTO: 54.00 sin envio, reintegro de componentes una sola vez. Las cifras requieren datos iniciales controlados.

## Estudiar y defender

- docs/ESTRUCTURA.md: organizacion de proyectos.
- docs/patrones/MAPA.md: 23 patrones, archivos, recorridos y pruebas.
- docs/DEFENSA.md: como localizar cambios y explicarlos.
- docs/diseno/ARQUITECTURA.md: diagramas y decisiones.
- docs/pruebas/MATRIZ.md: 38 casos oficiales y resultados nuevos.
- docs/calidad/REVISION.md: riesgos, comprobaciones y limites reales.

La implementacion adapta la referencia del mismo LogiMarket360 a los archivos separados del proyecto Alan, conservando el Rol escrito por el usuario. No se presenta el codigo como una implementacion manual completa del estudiante ni se reutilizan resultados/capturas anteriores como evidencia de Alan. Git conserva cambios funcionales reales en commits separados; el repositorio creado para este proyecto es https://github.com/Alan77-bot/LogiMarket360Alan y la subida esta pendiente de aprobar la publicacion de metadatos personales.

El PDF y los datos de portada siguen pendientes por indicacion del usuario. Las integraciones son simuladas y los comprobantes no tienen validez tributaria.
## Publicacion y entrega

Repositorio creado https://github.com/Alan77-bot/LogiMarket360Alan

Version prevista v1.0.0 con ZIP del codigo y evidencias La subida y la Release todavia estan pendientes

Consultar docs/ENTREGA.md para distinguir lo implementado de los pendientes academicos El PDF y los datos de portada siguen pendientes No se afirma que existan diez pares de refactorizaciones con pruebas antes y despues ni una revision manual exhaustiva de accesibilidad
