# LogiMarket

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
```

Las pruebas usan bases SQLite temporales independientes.

Flujo de demostracion: VENTAS prepara Ana + KIT_TELETRABAJO x1 ESTANDAR, guarda, valida y cobra 59.00. BODEGA despacha y entrega. VENTAS devuelve por DEFECTO: 54.00 sin envio, reintegro de componentes una sola vez. Las cifras requieren datos iniciales controlados.

## Version

Version actual v1.0.2 Consultar CHANGELOG.md

Las integraciones de pago y notificacion son simuladas Los comprobantes no tienen validez tributaria
