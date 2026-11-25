# Estructura implementada

La solucion tiene exactamente cinco proyectos. Los archivos que antes eran marcadores contienen implementaciones. Rol.cs conserva los valores que escribio el usuario.

- LogiMarket.Domain/Enums: perfiles, estados, entrega, categorias y motivos.
- LogiMarket.Domain/Entidades: usuario, cliente, producto, kit y componente.
- LogiMarket.Domain/Catalogo: Composite, Iterator y Visitor.
- LogiMarket.Domain/Promociones: Strategy y calculadora monetaria.
- LogiMarket.Domain/Entregas: Abstract Factory de tarifa/plazo/etiqueta.
- LogiMarket.Domain/Pedidos y Estados: agregado, instantaneas, Builder, Prototype, Memento y State.
- LogiMarket.Domain/Operaciones: pago, movimientos, despacho, devolucion, eventos y auditoria.
- LogiMarket.Application/Contratos y DTOs: puertos y datos intercambiados.
- LogiMarket.Application/Seguridad: acceso, autorizacion y Proxy.
- LogiMarket.Application/Catalogo y Clientes: casos de uso y Flyweight.
- LogiMarket.Application/Pedidos, Validaciones y Logistica: Facade, Chain y Template Method.
- LogiMarket.Application/Comandos, Eventos, Reportes y Comprobantes: Command, Observer, Singleton, Mediator, Interpreter y Bridge.
- LogiMarket.Infrastructure/Persistencia: EF Core, repositorios, transacciones, configuraciones y migracion nueva.
- LogiMarket.Infrastructure/Pagos, Notificaciones, Outbox y Documentos: Adapter, Decorator, Factory Method, reintentos y salida TXT.
- LogiMarket.Infrastructure/InyeccionDependencias: registro de colaboradores y tiempos de vida.
- LogiMarket.WinForms/Formularios: acceso, ventana principal y dialogos.
- LogiMarket.WinForms/Vistas: modulos operativos; Cobro, Despacho y Devoluciones reutilizan la base VistaPedidos.
- LogiMarket.WinForms/Compartido: estilo, permisos visuales, operaciones por ambito, Mediator y vista previa.
- tests/LogiMarket.Tests: Soporte, Dominio, Aplicacion, Integracion, Patrones, Arquitectura y Aceptacion.
- scripts: verificaciones reproducibles con ejecutores temporales fuera de la solucion.

NotificacionObserver vive en Infrastructure porque utiliza creadores concretos; OpcionesDemostracion vive en Application como opciones compartidas. Se retiraron los marcadores redundantes que ubicaban esos tipos en capas incorrectas.

## Dependencias

Domain no tiene referencias propias. Application referencia Domain. Infrastructure referencia Application y Domain. WinForms referencia Application e Infrastructure para su composicion. Tests referencia los tres proyectos internos. Solo WinForms se ejecuta como aplicacion.