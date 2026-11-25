# Matriz P01-P38

Fecha de ejecucion: 2026-11-24T19:12:21.5931330-05:00.
Resultados: 62/62 aprobados.
La comprobacion automatica y los ejecutores visuales se distinguen de una prueba manual. Los escenarios originales se conservan en CASOS_OFICIALES.json.

| ID | Resultado |
|---|---|
| P01 | APROBADO |
| P02 | APROBADO |
| P03 | APROBADO |
| P04 | APROBADO |
| P05 | APROBADO |
| P06 | APROBADO |
| P07 | APROBADO |
| P08 | APROBADO |
| P09 | APROBADO |
| P10 | APROBADO |
| P11 | APROBADO |
| P12 | APROBADO |
| P13 | APROBADO |
| P14 | APROBADO |
| P15 | APROBADO |
| P16 | APROBADO |
| P17 | APROBADO |
| P18 | APROBADO |
| P19 | APROBADO |
| P20 | APROBADO |
| P21 | APROBADO |
| P22 | APROBADO |
| P23 | APROBADO |
| P24 | APROBADO |
| P25 | APROBADO |
| P26 | APROBADO |
| P27 | APROBADO |
| P28 | APROBADO |
| P29 | APROBADO |
| P30 | APROBADO |
| P31 | APROBADO |
| P32 | APROBADO |
| P33 | APROBADO |
| P34 | APROBADO |
| P35 | APROBADO |
| P36 | APROBADO |
| P37 | APROBADO |
| P38 | APROBADO |

## P01 - APROBADO

Entrada: Creación de DB desde archivo inexistente y seed.

Esperado: Migraciones aplicadas; cinco productos, dos kits y roles; sin duplicados al relanzar seed.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Base nueva creada por migraciones, cinco productos, dos kits, cuatro usuarios y ninguna migracion pendiente; seed repetido sin duplicados.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Integracion/MigracionesTests.cs

Metodos: LogiMarket.Tests.Integracion.MigracionesTests.P01_MigracionesYSeedIdempotente


## P02 - APROBADO

Entrada: Ana VIP compra KIT_TELETRABAJO ×1 ESTÁNDAR, base recién preparada.

Esperado: Total 59.00; stock P01=7, P02=4, P03=19; estado PAGADO y un pago.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Total 59.00, PAGADO y un pago; stock P01=7, P02=4 y P03=19 comprobado.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Aceptacion/CasosOficialesTests.cs

Metodos: LogiMarket.Tests.Aceptacion.CasosOficialesTests.P02_P04_CasosOficiales


## P03 - APROBADO

Entrada: Luis NORMAL compra 2 P04 + 2 P05 EXPRÉS, base recién preparada.

Esperado: Total 102.00; P04=2, P05=28; un pago.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Total 102.00 y un pago; stock P04=2 y P05=28 comprobado.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Aceptacion/CasosOficialesTests.cs

Metodos: LogiMarket.Tests.Aceptacion.CasosOficialesTests.P02_P04_CasosOficiales


## P04 - APROBADO

Entrada: Carmen MAYORISTA compra P03 ×10 ESTÁNDAR, base recién preparada.

Esperado: Total 90.00; P03=10; descuento 15.00.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Total 90.00, descuento 15.00 y stock P03=10 comprobado.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Aceptacion/CasosOficialesTests.cs

Metodos: LogiMarket.Tests.Aceptacion.CasosOficialesTests.P02_P04_CasosOficiales


## P05 - APROBADO

Entrada: KIT_TELETRABAJO ×6 con stock inicial, intentar confirmar.

Esperado: Rechazo por P02 insuficiente; sin pago/stock/evento de éxito.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Rechazo por P02; sin pago, movimientos ni evento de exito y sin descontar P01.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Aplicacion/ConfirmacionTests.cs

Metodos: LogiMarket.Tests.Aplicacion.ConfirmacionTests.P05_KitPorSeisSinEfectos


## P06 - APROBADO

Entrada: Simular error de cobro en pedido válido.

Esperado: Se conserva inventario y estado previo; sin pago aprobado ni Outbox de éxito.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Cobro rechazado, estado VALIDADO e inventario conservado; sin pago aprobado ni Outbox de exito.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Aplicacion/ConfirmacionTests.cs

Metodos: LogiMarket.Tests.Aplicacion.ConfirmacionTests.P06_RechazoNoDescuentaStock


## P07 - APROBADO

Entrada: Repetir confirmación con misma clave de idempotencia.

Esperado: Un pago y un movimiento por SKU; devuelve el resultado original sin cobro doble.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Mismo identificador de pago en reintentos; un pago y tres movimientos, uno por SKU.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Integracion/IdempotenciaTests.cs

Metodos: LogiMarket.Tests.Integracion.IdempotenciaTests.P07_ConfirmacionIdempotente


## P08 - APROBADO

Entrada: Pedido PAGADO; intentar cancelar y modificar precio.

Esperado: Ambas operaciones denegadas; datos e historial intactos.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Cancelar y editar el pedido pagado provocan rechazo; total e historial conservados.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Dominio/EstadosTests.cs

Metodos: LogiMarket.Tests.Dominio.EstadosTests.P08_PagadoNoAdmiteCancelarNiEditar


## P09 - APROBADO

Entrada: Despachar y entregar con BODEGA; intentar despachar sin pago.

Esperado: Éxito solo en secuencia permitida; rechazo sin cambios en estado inválido.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Despacho sin pago rechazado; BODEGA completa despacho y entrega del pedido pagado.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Aplicacion/PermisosTests.cs

Metodos: LogiMarket.Tests.Aplicacion.PermisosTests.P09_DespachoEntregaYRechazoSinPago


## P10 - APROBADO

Entrada: Cerrar y reabrir; consultar pedido, stock, pago y auditoría.

Esperado: Información persiste en SQLite y coincide con operaciones anteriores.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Otro proveedor DI abre la misma SQLite y recupera pedido PAGADO, pago, stock P01=7 y auditoria.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Integracion/PersistenciaTests.cs

Metodos: LogiMarket.Tests.Integracion.PersistenciaTests.P10_PersistenciaConOtroProveedorDi


## P11 - APROBADO

Entrada: Factory Method: alternar EMAIL/SMS tras evento.

Esperado: Cada creador retorna notificador concreto y registra el aviso correcto.

Obtenido: 2 prueba(s) automatizada(s) aprobada(s). EMAIL y SMS registran el canal elegido; una sola notificacion por evento.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Integracion/OutboxTests.cs

Metodos: LogiMarket.Tests.Integracion.OutboxTests.P11_P30_ObserverYFactoryPublicanUnaVez


## P12 - APROBADO

Entrada: Abstract Factory: ESTÁNDAR/EXPRÉS.

Esperado: Tarifas 5/12 y plazos 3/1 días; guías coherentes con la familia.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). ESTANDAR: tarifa 5, plazo 3 y prefijo STD. EXPRES: tarifa 12, plazo 1 y prefijo EXP.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Patrones/CreacionalesTests.cs

Metodos: LogiMarket.Tests.Patrones.CreacionalesTests.P12_FamiliasCompatibles


## P13 - APROBADO

Entrada: Builder: pedido vacío y pedido completo.

Esperado: Incompleto rechazado; completo crea agregado válido.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Constructor sin cliente o sin lineas rechazado; agregado completo construido.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Patrones/CreacionalesTests.cs

Metodos: LogiMarket.Tests.Patrones.CreacionalesTests.P13_BuilderRechazaIncompleto


## P14 - APROBADO

Entrada: Prototype: clonar KIT_TELETRABAJO; editar cantidad.

Esperado: Copia profunda; plantilla/copia previa permanecen intactas.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Copia profunda comprobada con cambio de cantidad y arbol historico serializado.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Patrones/CreacionalesTests.cs

Metodos: LogiMarket.Tests.Patrones.CreacionalesTests.P14_PrototypeProfundo


## P15 - APROBADO

Entrada: Singleton: consultar métricas desde dos vistas.

Esperado: Misma instancia compartida, contadores observables y thread-safe.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Dos ambitos reciben la misma instancia; 1000 registros concurrentes del mismo evento cuentan un pago.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Patrones/CreacionalesTests.cs

Metodos: LogiMarket.Tests.Patrones.CreacionalesTests.P15_SingletonIdentidadYConcurrencia


## P16 - APROBADO

Entrada: Adapter: proveedor legado y alternativo.

Esperado: Cobros desde ambos con mismo contrato; legado intacto.

Obtenido: 2 prueba(s) automatizada(s) aprobada(s). Ambos proveedores aceptan 59.00, conservan referencia en el reintento y rechazan entradas invalidas.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Patrones/EstructuralesTests.cs

Metodos: LogiMarket.Tests.Patrones.EstructuralesTests.P16_P38_LspProveedores


## P17 - APROBADO

Entrada: Bridge: dos comprobantes × dos medios.

Esperado: Cuatro combinaciones; VistaPrevia visible y TXT real.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Cuatro combinaciones ejecutadas; vistas previas reales y TXT en docs/evidencias/comprobantes.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Patrones/EstructuralesTests.cs

Metodos: LogiMarket.Tests.Patrones.EstructuralesTests.P17_BridgeCuatroCombinacionesContenidoYArchivo


## P18 - APROBADO

Entrada: Composite: kits y composición anidada; intentar ciclo.

Esperado: Precios 50/60; expansión correcta; ciclo rechazado.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Precios 50/60, expansion P01/P02/P03 y cantidades propagadas; ciclo indirecto rechazado.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Dominio/KitsTests.cs

Metodos: LogiMarket.Tests.Dominio.KitsTests.KitAnidadoExpandeCantidadesYRechazaCicloIndirecto


## P19 - APROBADO

Entrada: Decorator: cobro autorizado, exitoso/fallido.

Esperado: LOG de inicio y fin/error rodea la ejecución, no altera resultado.

Obtenido: 2 prueba(s) automatizada(s) aprobada(s). Secuencia observada INICIO, PASARELA y FIN/ERROR, conservando el resultado del proveedor.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Patrones/EstructuralesTests.cs

Metodos: LogiMarket.Tests.Patrones.EstructuralesTests.P19_DecoradorRodeaEjecucion


## P20 - APROBADO

Entrada: Facade: confirmar con fallo en un colaborador.

Esperado: Operación consistente; ninguna persistencia parcial indebida.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Fallo del colaborador deja estado VALIDADO, stock P01=8, sin pago ni evento de exito.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Aplicacion/ConfirmacionTests.cs

Metodos: LogiMarket.Tests.Aplicacion.ConfirmacionTests.P20_FalloColaboradorNoDejaPersistenciaParcial


## P21 - APROBADO

Entrada: Flyweight: 1.000 filas con metadatos repetidos.

Esperado: ReferenceEquals true; identidad y memoria documentadas.

Obtenido: 1000 filas, una instancia compartida y ReferenceEquals True; medicion nueva: 104176 frente a 56360 bytes asignados. Condiciones en flujo-ui.txt.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Patrones/EstructuralesTests.cs

Metodos: LogiMarket.Tests.Patrones.EstructuralesTests.P21_MilFilasCompartenIdentidadConComparacionEquivalente


## P22 - APROBADO

Entrada: Proxy: rol CONSULTA/BODEGA intenta pagar.

Esperado: Rechazo antes de Decorator/Adapter; sin cobro ni LOG de cobro.

Obtenido: 2 prueba(s) automatizada(s) aprobada(s). CONSULTA y BODEGA reciben UnauthorizedAccessException antes del proveedor; ningun LOG de COBRO.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Aplicacion/PermisosTests.cs

Metodos: LogiMarket.Tests.Aplicacion.PermisosTests.P22_ProxyDetieneAntesDeCobro


## P23 - APROBADO

Entrada: Chain: pedido sin cliente y sin stock; corregir un error.

Esperado: Se informa primera validación y luego la siguiente; sin ejecución de cobro.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Primero se informa CLIENTE y, al corregirlo, STOCK; no se invoca cobro.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Patrones/ComportamentalesTests.cs

Metodos: LogiMarket.Tests.Patrones.ComportamentalesTests.P23_CadenaPrimeroClienteLuegoStock


## P24 - APROBADO

Entrada: Command: confirmar desde UI e invocador de prueba.

Esperado: Mismo comando y resultado funcional; historial de invocación.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). El boton de cobro y el test invocan ConfirmarPedidoCommand mediante InvocadorComandos.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Aplicacion/ComandosTests.cs

Metodos: LogiMarket.Tests.Aplicacion.ComandosTests.P24_ComandoInvocadoRegistraResultado


## P25 - APROBADO

Entrada: Interpreter: ESTADO = PAGADO AND TOTAL >= 50.

Esperado: Devuelve únicamente pedidos que cumplen el filtro.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). ESTADO = PAGADO AND TOTAL >= 50 acepta el pedido pagado de 65.00 y descarta el filtro de BORRADOR.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Patrones/ComportamentalesTests.cs

Metodos: LogiMarket.Tests.Patrones.ComportamentalesTests.P25_InterpreterSeleccionaPagado


## P26 - APROBADO

Entrada: Interpreter: sintaxis errónea y paréntesis.

Esperado: Error legible sin SQL concatenado; precedencia correcta.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). AND tiene prioridad sobre OR; parentesis cambian el resultado y sintaxis invalida genera FormatException.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Patrones/ComportamentalesTests.cs

Metodos: LogiMarket.Tests.Patrones.ComportamentalesTests.P26_InterpreterPrecedenciaYErrores


## P27 - APROBADO

Entrada: Iterator: recorrer KIT_TELETRABAJO.

Esperado: P01, P02, P03 una vez por unidad requerida, sin bucle.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Iterador propio produce P01, P02 y P03, termina y permanece terminado.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Patrones/ComportamentalesTests.cs

Metodos: LogiMarket.Tests.Patrones.ComportamentalesTests.P27_P34_IteratorYVisitorKits


## P28 - APROBADO

Entrada: Mediator: confirmar y revisar tres vistas.

Esperado: Catálogo, pedidos y métricas se refrescan sin acoplamiento directo.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Verificacion automatica del coordinador y capturas 07, 08 y 09: pedido pagado, stock actualizado y metrica de pago=1.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Patrones/MediatorTests.cs

Metodos: LogiMarket.Tests.Patrones.MediatorTests.P28_MediatorRefrescaStockPedidosYMetricas


## P29 - APROBADO

Entrada: Memento: tres ediciones, dos deshacer y un rehacer.

Esperado: Se restauran borradores completos; no se deshace un pago.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Tres estados: deshacer devuelve cantidades 2 y 1; rehacer devuelve 2. Historial bloqueado rechaza deshacer.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Patrones/ComportamentalesTests.cs

Metodos: LogiMarket.Tests.Patrones.ComportamentalesTests.P29_MementoTresCambiosDosDeshacerUnRehacer


## P30 - APROBADO

Entrada: Observer: PedidoPagado con tres suscriptores.

Esperado: Notificación, auditoría y métrica actualizadas una vez.

Obtenido: 2 prueba(s) automatizada(s) aprobada(s). Un aviso, una auditoria de PedidoPagado y una metrica; segundo procesamiento no publica de nuevo.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Integracion/OutboxTests.cs

Metodos: LogiMarket.Tests.Integracion.OutboxTests.P11_P30_ObserverYFactoryPublicanUnaVez


## P31 - APROBADO

Entrada: State: ciclo válido y PAGADO→CANCELADO inválido.

Esperado: Transiciones con reglas por estado, sin escritura directa.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Ciclo permitido completo; despacho prematuro, cancelacion y repeticion rechazados.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Dominio/EstadosTests.cs

Metodos: LogiMarket.Tests.Dominio.EstadosTests.P31_StateRechazaRetrocesoYRepeticion


## P32 - APROBADO

Entrada: Strategy: VIP, MAYORISTA, NORMAL y nueva promoción.

Esperado: Descuentos 6/15/0; extensión sin editar la calculadora central.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Politica nueva aplica 20% sin editar la calculadora; pruebas numericas adicionales verifican VIP, MAYORISTA y NORMAL.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Dominio/PromocionesTests.cs

Metodos: LogiMarket.Tests.Dominio.PromocionesTests.P32_ExtenderStrategySinCambiarCalculadora


## P33 - APROBADO

Entrada: Template Method: defecto día 20; arrepentimiento día 10.

Esperado: Defecto aceptado; arrepentimiento rechazado sin reembolso/stock.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Arrepentimiento dia 10 rechazado; defecto dia 20 tambien verificado en P35.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Aplicacion/DevolucionesTests.cs

Metodos: LogiMarket.Tests.Aplicacion.DevolucionesTests.P33_ArrepentimientoDiaDiezNoAlteraStock


## P34 - APROBADO

Entrada: Visitor: KIT_TELETRABAJO.

Esperado: Detalle P01/P02/P03; peso total 1.00 kg.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Visitor obtiene tres SKU y peso total 1.00 kg del kit teletrabajo.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Patrones/ComportamentalesTests.cs

Metodos: LogiMarket.Tests.Patrones.ComportamentalesTests.P27_P34_IteratorYVisitorKits


## P35 - APROBADO

Entrada: Devolver pedido ENTREGADO por defecto y repetir solicitud.

Esperado: Para Ana: USD 54.00 de reembolso sin flete; un solo reintegro y devolución única.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Ana recibe 54.00, sin envio; stock P01 vuelve a 8 y la solicitud repetida se rechaza; una devolucion.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Aplicacion/DevolucionesTests.cs

Metodos: LogiMarket.Tests.Aplicacion.DevolucionesTests.P35_DevolucionAnaUnicaPor54


## P36 - APROBADO

Entrada: Dos cobros concurrentes sobre un SKU cuyo stock se configuró previamente en una unidad.

Esperado: Máximo un cobro aprobado; stock no negativo; sin sobreventa.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Con P01=1 y dos confirmaciones concurrentes: una aceptada, un pago y stock final cero.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Integracion/ConcurrenciaTests.cs

Metodos: LogiMarket.Tests.Integracion.ConcurrenciaTests.P36_DosConfirmacionesConcurrentesSinSobreventa


## P37 - APROBADO

Entrada: Outbox: falla notificador y reiniciar procesador.

Esperado: Evento pendiente se recupera, se marca procesado una vez y sin duplicar cobro.

Obtenido: 1 prueba(s) automatizada(s) aprobada(s). Fallo deja evento pendiente con un intento; procesador nuevo lo recupera; un aviso y un pago.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Integracion/OutboxTests.cs

Metodos: LogiMarket.Tests.Integracion.OutboxTests.P37_RecuperacionOutboxConNuevoProcesador


## P38 - APROBADO

Entrada: Pruebas de LSP/ISP/DIP y build desde código limpio.

Esperado: Reemplazo de proveedor, contratos segregados, referencias correctas y 25 pruebas automatizadas aprobadas.

Obtenido: Contratos y referencias comprobados por las pruebas; compilacion limpia registrada en docs/evidencias/compilacion-limpia.txt.

Fecha: 2026-11-24T19:12:21.5931330-05:00

Evidencia: docs/evidencias/resultados/aceptacion-alan.trx

Archivos: tests/LogiMarket.Tests/Arquitectura/DependenciasTests.cs; tests/LogiMarket.Tests/Patrones/EstructuralesTests.cs

Metodos: LogiMarket.Tests.Patrones.EstructuralesTests.P16_P38_LspProveedores; LogiMarket.Tests.Arquitectura.DependenciasTests.P38_DependenciasInternasSinFrameworksDePersistenciaNiUi

