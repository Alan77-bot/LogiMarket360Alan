# Mapa de los 23 patrones GoF

Implementacion de LogiMarket360Alan. Los comentarios junto a cada clase explican su papel. Las rutas parten de la raiz del proyecto. Para encontrar una clase, usar Ctrl+T; Shift+F12 muestra sus usos.

| Patron | Archivo principal | Recorrido que debes explicar | Caso |
|---|---|---|---|
| Factory Method | [CreadorNotificador](../../LogiMarket.Infrastructure/Notificaciones/CreadorNotificador.cs) | Crear -> Enviar | P11 |
| Abstract Factory | [IFabricaEntrega](../../LogiMarket.Domain/Entregas/IFabricaEntrega.cs) | ServicioPedidos/ServicioDespachos -> fabrica -> tarifa, plazo y etiqueta | P12 |
| Builder | [PedidoBuilder](../../LogiMarket.Domain/Pedidos/PedidoBuilder.cs) | ServicioPedidos.Preparar -> Para -> ConEntrega -> Agregar -> Construir | P13 |
| Prototype | [Pedido](../../LogiMarket.Domain/Pedidos/Pedido.cs) | ServicioPedidos.Clonar -> ClonarBorrador -> Copiar | P14 |
| Singleton | [MetricasSesion](../../LogiMarket.Application/Eventos/MetricasSesion.cs) | RegistroServicios.AddSingleton -> vistas y MetricasObserver | P15 |
| Adapter | [PasarelaLegadaAdapter](../../LogiMarket.Infrastructure/Pagos/PasarelaLegadaAdapter.cs) | CobrarAsync -> CobrarCentavos | P16 |
| Bridge | [Comprobante](../../LogiMarket.Application/Comprobantes/Comprobante.cs) | Venta/Devolucion -> ISalidaDocumento -> vista previa/TXT | P17 |
| Composite | [KitCompuesto](../../LogiMarket.Domain/Catalogo/KitCompuesto.cs) | CatalogoCompuesto -> KitCompuesto -> hojas de ProductoSimple | P18 |
| Decorator | [AuditoriaPagoDecorator](../../LogiMarket.Infrastructure/Pagos/AuditoriaPagoDecorator.cs) | inicio -> proveedor -> fin/error | P19 |
| Facade | [ConfirmacionPedidoFacade](../../LogiMarket.Application/Pedidos/ConfirmacionPedidoFacade.cs) | Confirmar -> permisos -> validacion -> pago -> stock/pedido/Outbox | P20 |
| Flyweight | [CatalogoVisualFlyweightFactory](../../LogiMarket.Application/Catalogo/CatalogoVisualFlyweightFactory.cs) | Obtener reutiliza MetadatosSku para 1000 filas | P21 |
| Proxy | [PagoAutorizadoProxy](../../LogiMarket.Application/Seguridad/PagoAutorizadoProxy.cs) | Autorizador -> Decorator -> Adapter, solamente si el rol puede cobrar | P22 |
| Chain of Responsibility | [ValidadorPedido](../../LogiMarket.Application/Validaciones/ValidadorPedido.cs) | cliente -> lineas -> stock -> estado; primera causa de rechazo | P23 |
| Command | [InvocadorComandos](../../LogiMarket.Application/Comandos/InvocadorComandos.cs) | Vista -> comando -> invocador -> receptor -> bitacora | P24 |
| Interpreter | [InterpreterFiltro](../../LogiMarket.Application/Reportes/InterpreterFiltro.cs) | Analizar -> arbol AND/OR -> Evaluar pedido | P25-P26 |
| Iterator | [IteradorFisico](../../LogiMarket.Domain/Catalogo/IteradorFisico.cs) | MoveNext recorre hojas y multiplica cantidades | P27 |
| Mediator | [MediadorVistas](../../LogiMarket.Application/Eventos/MediadorVistas.cs) | Registrar -> Actualizar -> callbacks de vistas permitidas | P28 |
| Memento | [HistorialBorrador](../../LogiMarket.Domain/Pedidos/HistorialBorrador.cs) | Capturar -> DeshacerCompleto/RehacerCompleto; copias independientes | P29 |
| Observer | [PublicadorEventos](../../LogiMarket.Application/Eventos/PublicadorEventos.cs) | Publicar -> suscriptores -> recepcion unica por evento/suscriptor | P30 |
| State | [EstadoBase](../../LogiMarket.Domain/Estados/EstadoBase.cs) | Pedido.Transicionar -> CatalogoEstados.Obtener -> Exigir | P31 |
| Strategy | [CalculadoraPrecios](../../LogiMarket.Domain/Promociones/CalculadoraPrecios.cs) | politicas -> prioridad -> primera aplicable -> calculo decimal | P32 |
| Template Method | [ProcesarDevolucion](../../LogiMarket.Application/Logistica/ProcesarDevolucion.cs) | Ejecutar -> Inspeccionar segun motivo -> reembolso/reintegro | P33 |
| Visitor | [IVisitanteCatalogo](../../LogiMarket.Domain/Catalogo/IVisitanteCatalogo.cs) | Aceptar -> PesoVisitor o DesgloseVisitor | P34 |

## Problema, beneficio y costo

### Factory Method

Problema y participantes: Crear EMAIL/SMS sin acoplar los eventos al concreto; CreadorNotificador, CreadorEmail, CreadorSms.

Beneficio y costo: Se cambia canal sin tocar vistas; mas clases de creacion.

### Abstract Factory

Problema y participantes: Mantener tarifa, plazo y guia de una misma familia.

Beneficio y costo: Familias coherentes; tres contratos de productos.

### Builder

Problema y participantes: Evitar pedidos sin cliente o lineas.

Beneficio y costo: Construccion legible y valida; pasos adicionales.

### Prototype

Problema y participantes: Reutilizar borradores independientes.

Beneficio y costo: Copia profunda con identidad nueva; hay que mantener la copia al agregar campos.

### Singleton

Problema y participantes: Contadores compartidos de sesion.

Beneficio y costo: Identidad unica y concurrencia segura; no sustituye datos historicos.

### Adapter

Problema y participantes: Legado recibe centavos, aplicacion usa decimal.

Beneficio y costo: Conserva legado y contrato estable; conversion de unidades.

### Bridge

Problema y participantes: Independencia entre comprobante y salida.

Beneficio y costo: Cuatro combinaciones sin cuatro clases combinadas; hay dos jerarquias.

### Composite

Problema y participantes: Productos y kits anidados uniformes.

Beneficio y costo: Recursion y calculo uniforme; exige control de ciclos.

### Decorator

Problema y participantes: Agregar trazas al pago sin alterar proveedor.

Beneficio y costo: Auditoria transversal; orden de envolturas relevante.

### Facade

Problema y participantes: Una entrada para confirmar sin logica en el boton.

Beneficio y costo: Coordina sin SQL ni tasas propias; debe evitar crecer como clase Dios.

### Flyweight

Problema y participantes: Compartir metadatos en 1000 filas.

Beneficio y costo: Menos objetos repetidos; cache y separacion de datos externos.

### Proxy

Problema y participantes: Autorizar antes de logs/proveedor.

Beneficio y costo: Proteccion fuera de UI; configuracion de cadena.

### Chain of Responsibility

Problema y participantes: Primera causa de rechazo clara.

Beneficio y costo: Validadores pequenos y ordenados; orden debe documentarse.

### Command

Problema y participantes: Separar boton de receptor y registrar invocacion.

Beneficio y costo: Misma accion desde UI/tests; objetos adicionales.

### Interpreter

Problema y participantes: Consultas con gramatica acotada segura.

Beneficio y costo: Sin SQL concatenado y precedencia explicita; parser que mantener.

### Iterator

Problema y participantes: Recorrer hojas con cantidades y orden estable.

Beneficio y costo: Oculta colecciones; pila y protocolo MoveNext.

### Mediator

Problema y participantes: Refrescar vistas sin referencias entre ellas.

Beneficio y costo: Reduce acoplamiento entre formularios; punto de coordinacion compartido.

### Memento

Problema y participantes: Deshacer/rehacer cinco estados completos.

Beneficio y costo: Recuperacion de ediciones; memoria de copias y limite de historial.

### Observer

Problema y participantes: Varias reacciones a eventos durables.

Beneficio y costo: Suscriptores independientes; recepciones y reintentos requieren cuidado.

### State

Problema y participantes: Transiciones segun estado actual.

Beneficio y costo: Sin switch central de transiciones; un tipo por estado.

### Strategy

Problema y participantes: Descuentos excluyentes y extensibles.

Beneficio y costo: Nueva promocion sin cambiar calculadora; prioridades deben ser unicas y explicitas.

### Template Method

Problema y participantes: Secuencia fija y politica por motivo.

Beneficio y costo: Flujo consistente; herencia limita variaciones.

### Visitor

Problema y participantes: Peso y desglose sin agregarlos a cada entidad.

Beneficio y costo: Reportes separados; agregar un tipo de elemento obliga a extender visitantes.

## Evidencia

Los casos se enlazan con los resultados nuevos en ../pruebas/MATRIZ.md. Bridge tiene capturas de ventanas reales y TXT; Flyweight tiene medicion en ../evidencias/flujo-ui.txt. Las capturas de codigo del IDE se toman al preparar la defensa; no se presentan imagenes generadas como si fueran capturas de Visual Studio.
