# Estructura preparada para aprender paso a paso

Los archivos C# contienen solamente comentarios pendientes, excepto el punto de entrada minimo de WinForms. No hay logica de negocio, base creada, patrones implementados ni pruebas ejecutables. El proyecto Tests es una biblioteca preparada: instalaremos xUnit y el SDK de pruebas en su etapa.

## Referencias

Domain no referencia otros proyectos. Application -> Domain. Infrastructure -> Application y Domain. WinForms -> Application e Infrastructure (solo composicion). Tests -> Domain, Application e Infrastructure.

## Orden de trabajo

1. Enums y entidades simples: Rol, CategoriaCliente, Producto, Cliente y Usuario.
2. Pruebas de invariantes y primera evidencia real.
3. Catalogo compuesto y pedidos; contratos y casos de uso.
4. EF Core 8, SQLite, mapeos, migraciones y seed.
5. Confirmacion, logistica, seguridad y eventos, con pruebas por avance.
6. WinForms, navegacion por rol y flujos completos.
7. Aceptacion P01-P38, documentacion y entrega.

## Inventario de archivos pendientes

- `LogiMarket.Domain/Enums/Rol.cs`
- `LogiMarket.Domain/Enums/CategoriaCliente.cs`
- `LogiMarket.Domain/Enums/EstadoPedido.cs`
- `LogiMarket.Domain/Enums/TipoEntrega.cs`
- `LogiMarket.Domain/Enums/MotivoDevolucion.cs`
- `LogiMarket.Domain/Entidades/Usuario.cs`
- `LogiMarket.Domain/Entidades/Cliente.cs`
- `LogiMarket.Domain/Entidades/Producto.cs`
- `LogiMarket.Domain/Entidades/Kit.cs`
- `LogiMarket.Domain/Entidades/KitComponente.cs`
- `LogiMarket.Domain/Pedidos/Pedido.cs`
- `LogiMarket.Domain/Pedidos/PedidoDetalle.cs`
- `LogiMarket.Domain/Pedidos/PedidoDetalleComponente.cs`
- `LogiMarket.Domain/Pedidos/HistorialEstado.cs`
- `LogiMarket.Domain/Pedidos/PedidoBuilder.cs`
- `LogiMarket.Domain/Pedidos/HistorialBorrador.cs`
- `LogiMarket.Domain/Estados/IEstadoPedido.cs`
- `LogiMarket.Domain/Estados/EstadoBase.cs`
- `LogiMarket.Domain/Estados/EstadoBorrador.cs`
- `LogiMarket.Domain/Estados/EstadoValidado.cs`
- `LogiMarket.Domain/Estados/EstadoPagado.cs`
- `LogiMarket.Domain/Estados/EstadoDespachado.cs`
- `LogiMarket.Domain/Estados/EstadoEntregado.cs`
- `LogiMarket.Domain/Estados/EstadoCancelado.cs`
- `LogiMarket.Domain/Catalogo/IElementoCatalogo.cs`
- `LogiMarket.Domain/Catalogo/ProductoSimple.cs`
- `LogiMarket.Domain/Catalogo/KitCompuesto.cs`
- `LogiMarket.Domain/Catalogo/IteradorFisico.cs`
- `LogiMarket.Domain/Catalogo/IVisitanteCatalogo.cs`
- `LogiMarket.Domain/Catalogo/PesoVisitor.cs`
- `LogiMarket.Domain/Catalogo/DesgloseVisitor.cs`
- `LogiMarket.Domain/Promociones/IPoliticaDescuento.cs`
- `LogiMarket.Domain/Promociones/CalculadoraPrecios.cs`
- `LogiMarket.Domain/Promociones/Mayorista15.cs`
- `LogiMarket.Domain/Promociones/Vip10.cs`
- `LogiMarket.Domain/Promociones/SinDescuento.cs`
- `LogiMarket.Domain/Entregas/IFabricaEntrega.cs`
- `LogiMarket.Domain/Entregas/ITarifaEntrega.cs`
- `LogiMarket.Domain/Entregas/IPlazoEntrega.cs`
- `LogiMarket.Domain/Entregas/IEtiquetaEntrega.cs`
- `LogiMarket.Domain/Entregas/EntregaEstandarFactory.cs`
- `LogiMarket.Domain/Entregas/EntregaExpresFactory.cs`
- `LogiMarket.Domain/Operaciones/Pago.cs`
- `LogiMarket.Domain/Operaciones/MovimientoInventario.cs`
- `LogiMarket.Domain/Operaciones/Despacho.cs`
- `LogiMarket.Domain/Operaciones/Devolucion.cs`
- `LogiMarket.Domain/Operaciones/EventoOutbox.cs`
- `LogiMarket.Domain/Operaciones/Auditoria.cs`
- `LogiMarket.Domain/Operaciones/RecepcionEvento.cs`
- `LogiMarket.Domain/Operaciones/Aviso.cs`
- `LogiMarket.Application/Contratos/IRepositorio.cs`
- `LogiMarket.Application/Contratos/ITransacciones.cs`
- `LogiMarket.Application/Contratos/IConsultasOperativas.cs`
- `LogiMarket.Application/Contratos/IPasarelaPago.cs`
- `LogiMarket.Application/Contratos/IReembolsosSimulados.cs`
- `LogiMarket.Application/Contratos/IBitacora.cs`
- `LogiMarket.Application/Contratos/IHashContrasena.cs`
- `LogiMarket.Application/Contratos/IReloj.cs`
- `LogiMarket.Application/Contratos/IProcesadorOutbox.cs`
- `LogiMarket.Application/Contratos/ISuscriptorEvento.cs`
- `LogiMarket.Application/Contratos/IRegistroRecepciones.cs`
- `LogiMarket.Application/DTOs/SolicitudBorrador.cs`
- `LogiMarket.Application/DTOs/LineaEntrada.cs`
- `LogiMarket.Application/DTOs/FilaCatalogo.cs`
- `LogiMarket.Application/DTOs/ComponenteEntrada.cs`
- `LogiMarket.Application/DTOs/ResultadoCobro.cs`
- `LogiMarket.Application/Seguridad/SesionActual.cs`
- `LogiMarket.Application/Seguridad/Autenticacion.cs`
- `LogiMarket.Application/Seguridad/Autorizador.cs`
- `LogiMarket.Application/Seguridad/PagoAutorizadoProxy.cs`
- `LogiMarket.Application/Catalogo/ServicioCatalogo.cs`
- `LogiMarket.Application/Catalogo/CatalogoCompuesto.cs`
- `LogiMarket.Application/Catalogo/CatalogoVisualFlyweightFactory.cs`
- `LogiMarket.Application/Clientes/ServicioClientes.cs`
- `LogiMarket.Application/Pedidos/ServicioPedidos.cs`
- `LogiMarket.Application/Pedidos/ConfirmacionPedidoFacade.cs`
- `LogiMarket.Application/Validaciones/ContextoValidacion.cs`
- `LogiMarket.Application/Validaciones/ResultadoValidacion.cs`
- `LogiMarket.Application/Validaciones/ValidadorPedido.cs`
- `LogiMarket.Application/Validaciones/ValidadorCliente.cs`
- `LogiMarket.Application/Validaciones/ValidadorLineas.cs`
- `LogiMarket.Application/Validaciones/ValidadorStock.cs`
- `LogiMarket.Application/Validaciones/ValidadorEstado.cs`
- `LogiMarket.Application/Validaciones/CadenaConfirmacion.cs`
- `LogiMarket.Application/Comandos/IComando.cs`
- `LogiMarket.Application/Comandos/InvocadorComandos.cs`
- `LogiMarket.Application/Comandos/ConfirmarPedidoCommand.cs`
- `LogiMarket.Application/Comandos/CancelarPedidoCommand.cs`
- `LogiMarket.Application/Comandos/DespacharPedidoCommand.cs`
- `LogiMarket.Application/Comandos/ProcesarDevolucionCommand.cs`
- `LogiMarket.Application/Logistica/ServicioDespachos.cs`
- `LogiMarket.Application/Logistica/ServicioInventario.cs`
- `LogiMarket.Application/Logistica/ProcesarDevolucion.cs`
- `LogiMarket.Application/Logistica/DevolucionDefecto.cs`
- `LogiMarket.Application/Logistica/DevolucionArrepentimiento.cs`
- `LogiMarket.Application/Eventos/PublicadorEventos.cs`
- `LogiMarket.Application/Eventos/MetricasSesion.cs`
- `LogiMarket.Application/Eventos/NotificacionObserver.cs`
- `LogiMarket.Application/Eventos/AuditoriaObserver.cs`
- `LogiMarket.Application/Eventos/MetricasObserver.cs`
- `LogiMarket.Application/Eventos/MediadorVistas.cs`
- `LogiMarket.Application/Reportes/InterpreterFiltro.cs`
- `LogiMarket.Application/Reportes/IExpresionFiltro.cs`
- `LogiMarket.Application/Comprobantes/ISalidaDocumento.cs`
- `LogiMarket.Application/Comprobantes/Comprobante.cs`
- `LogiMarket.Application/Comprobantes/ComprobanteVenta.cs`
- `LogiMarket.Application/Comprobantes/ComprobanteDevolucion.cs`
- `LogiMarket.Infrastructure/Persistencia/LogiMarketDbContext.cs`
- `LogiMarket.Infrastructure/Persistencia/FabricaDbContext.cs`
- `LogiMarket.Infrastructure/Persistencia/Inicializador.cs`
- `LogiMarket.Infrastructure/Persistencia/RepositorioEf.cs`
- `LogiMarket.Infrastructure/Persistencia/BitacoraEf.cs`
- `LogiMarket.Infrastructure/Persistencia/Configuraciones/ConfiguracionUsuario.cs`
- `LogiMarket.Infrastructure/Persistencia/Configuraciones/ConfiguracionCatalogo.cs`
- `LogiMarket.Infrastructure/Persistencia/Configuraciones/ConfiguracionPedido.cs`
- `LogiMarket.Infrastructure/Persistencia/Configuraciones/ConfiguracionOperaciones.cs`
- `LogiMarket.Infrastructure/Pagos/PasarelaLegadaSimulada.cs`
- `LogiMarket.Infrastructure/Pagos/PasarelaLegadaAdapter.cs`
- `LogiMarket.Infrastructure/Pagos/PasarelaAlternativaSimulada.cs`
- `LogiMarket.Infrastructure/Pagos/SelectorPasarela.cs`
- `LogiMarket.Infrastructure/Pagos/AuditoriaPagoDecorator.cs`
- `LogiMarket.Infrastructure/Pagos/ReembolsosSimulados.cs`
- `LogiMarket.Infrastructure/Notificaciones/INotificador.cs`
- `LogiMarket.Infrastructure/Notificaciones/CreadorNotificador.cs`
- `LogiMarket.Infrastructure/Notificaciones/CreadorEmail.cs`
- `LogiMarket.Infrastructure/Notificaciones/CreadorSms.cs`
- `LogiMarket.Infrastructure/Notificaciones/NotificadorEmailSimulado.cs`
- `LogiMarket.Infrastructure/Notificaciones/NotificadorSmsSimulado.cs`
- `LogiMarket.Infrastructure/Outbox/ProcesadorOutbox.cs`
- `LogiMarket.Infrastructure/Documentos/SalidaArchivoTxt.cs`
- `LogiMarket.Infrastructure/Seguridad/HashContrasena.cs`
- `LogiMarket.Infrastructure/Servicios/RelojSistema.cs`
- `LogiMarket.Infrastructure/Servicios/OpcionesDemostracion.cs`
- `LogiMarket.Infrastructure/InyeccionDependencias/RegistroServicios.cs`
- `LogiMarket.WinForms/Formularios/FormularioAcceso.cs`
- `LogiMarket.WinForms/Formularios/FormularioPrincipal.cs`
- `LogiMarket.WinForms/Formularios/DialogoCampos.cs`
- `LogiMarket.WinForms/Vistas/VistaPanel.cs`
- `LogiMarket.WinForms/Vistas/VistaCatalogo.cs`
- `LogiMarket.WinForms/Vistas/VistaClientes.cs`
- `LogiMarket.WinForms/Vistas/VistaCarrito.cs`
- `LogiMarket.WinForms/Vistas/VistaPedidos.cs`
- `LogiMarket.WinForms/Vistas/VistaCobro.cs`
- `LogiMarket.WinForms/Vistas/VistaDespacho.cs`
- `LogiMarket.WinForms/Vistas/VistaDevoluciones.cs`
- `LogiMarket.WinForms/Vistas/VistaInventario.cs`
- `LogiMarket.WinForms/Vistas/VistaReportes.cs`
- `LogiMarket.WinForms/Vistas/VistaAuditoria.cs`
- `LogiMarket.WinForms/Compartido/Ui.cs`
- `LogiMarket.WinForms/Compartido/OperacionesUi.cs`
- `LogiMarket.WinForms/Compartido/PermisosVista.cs`
- `LogiMarket.WinForms/Compartido/FormularioPrincipalMediator.cs`
- `LogiMarket.WinForms/Compartido/SalidaVistaPrevia.cs`
- `tests/LogiMarket.Tests/Soporte/Escenario.cs`
- `tests/LogiMarket.Tests/Soporte/RelojPrueba.cs`
- `tests/LogiMarket.Tests/Dominio/ProductoTests.cs`
- `tests/LogiMarket.Tests/Dominio/KitsTests.cs`
- `tests/LogiMarket.Tests/Dominio/PedidoTests.cs`
- `tests/LogiMarket.Tests/Dominio/PromocionesTests.cs`
- `tests/LogiMarket.Tests/Dominio/EstadosTests.cs`
- `tests/LogiMarket.Tests/Aplicacion/PermisosTests.cs`
- `tests/LogiMarket.Tests/Aplicacion/ConfirmacionTests.cs`
- `tests/LogiMarket.Tests/Aplicacion/DevolucionesTests.cs`
- `tests/LogiMarket.Tests/Aplicacion/ComandosTests.cs`
- `tests/LogiMarket.Tests/Integracion/MigracionesTests.cs`
- `tests/LogiMarket.Tests/Integracion/PersistenciaTests.cs`
- `tests/LogiMarket.Tests/Integracion/IdempotenciaTests.cs`
- `tests/LogiMarket.Tests/Integracion/ConcurrenciaTests.cs`
- `tests/LogiMarket.Tests/Integracion/OutboxTests.cs`
- `tests/LogiMarket.Tests/Patrones/CreacionalesTests.cs`
- `tests/LogiMarket.Tests/Patrones/EstructuralesTests.cs`
- `tests/LogiMarket.Tests/Patrones/ComportamentalesTests.cs`
- `tests/LogiMarket.Tests/Aceptacion/CasosOficialesTests.cs`
- `tests/LogiMarket.Tests/Arquitectura/DependenciasTests.cs`
