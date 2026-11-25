# Diseno de LogiMarket360Alan

La base se crea mediante EF Core 8 y una migracion nueva. No se editaron tablas manualmente. Los tipos de Domain no conocen EF, SQLite ni controles de pantalla.

## Contexto

```mermaid
flowchart LR
    ADMIN --> LM[LogiMarket360Alan WinForms]
    VENTAS --> LM
    BODEGA --> LM
    CONSULTA --> LM
    LM --> DB[(SQLite local)]
    LM --> PS[Proveedores de pago simulados]
    LM --> AV[Avisos EMAIL/SMS locales]
    LM --> TXT[Comprobantes internos]
```

## Componentes y referencias de compilacion

```mermaid
flowchart LR
    WinForms --> Application
    WinForms --> Infrastructure
    Application --> Domain
    Infrastructure --> Application
    Infrastructure --> Domain
    Tests --> Domain
    Tests --> Application
    Tests --> Infrastructure
```

La referencia WinForms -> Infrastructure se utiliza en Program para configurar DI e inicializar datos. Las vistas solo utilizan servicios de Application y tipos de Domain. Las flechas son referencias de compilacion, no el orden de una llamada.

## Modelo relacional

```mermaid
erDiagram
    Cliente ||--o{ Pedido : compra
    Pedido ||--|{ PedidoDetalle : contiene
    PedidoDetalle ||--|{ PedidoDetalleComponente : conserva
    Pedido ||--|{ HistorialEstado : registra
    Pedido ||--o| Pago : cobra
    Pedido ||--o| Despacho : envia
    Pedido ||--o| Devolucion : devuelve
    Pedido ||--o{ EventoOutbox : produce
    EventoOutbox ||--o{ RecepcionEvento : confirma
    EventoOutbox ||--o| Aviso : notifica
    Producto ||--o{ MovimientoInventario : mueve
    Kit ||--|{ KitComponente : contiene
    Producto o|--o{ KitComponente : fisico
    Kit o|--o{ KitComponente : anidado
```

Usuario conserva rol y hash; Auditoria registra usuario, fecha, operacion, resultado y correlacion. Las instantaneas conservan SKU, cantidades, precios y pesos; ComposicionJson preserva ademas el arbol del kit. La cantidad del componente ya incluye la cantidad de la linea y no se multiplica dos veces.

SKU y nombre de usuario tienen indices unicos. Pago tiene unicidad por pedido, clave y referencia. Devolucion es unica por pedido y referencia. Movimiento tiene unicidad por producto/referencia/motivo. RecepcionEvento tiene clave compuesta evento/suscriptor. Las FK reales y los checks complementan las invariantes del dominio. La unicidad entre SKU de productos y kits se comprueba en Application, ademas de los indices de cada tabla.

## Clases criticas

```mermaid
classDiagram
    PedidoBuilder --> Pedido : construye
    Pedido "1" *-- "1..*" PedidoDetalle
    PedidoDetalle "1" *-- "1..*" PedidoDetalleComponente
    Pedido --> IEstadoPedido : consulta transicion
    EstadoBase ..|> IEstadoPedido
    Pedido --> CalculadoraPrecios : valora
    CalculadoraPrecios --> IPoliticaDescuento : Strategy
    ConfirmacionPedidoFacade --> IRepositorio
    ConfirmacionPedidoFacade --> ITransacciones
    ConfirmacionPedidoFacade --> IPasarelaPago
    PagoAutorizadoProxy ..|> IPasarelaPago
    AuditoriaPagoDecorator ..|> IPasarelaPago
    PasarelaLegadaAdapter ..|> IPasarelaPago
    RepositorioEf ..|> IRepositorio
    RepositorioEf ..|> ITransacciones
```

## Estados

```mermaid
stateDiagram-v2
    [*] --> BORRADOR
    BORRADOR --> VALIDADO
    BORRADOR --> CANCELADO
    VALIDADO --> CANCELADO
    VALIDADO --> PAGADO
    PAGADO --> DESPACHADO
    DESPACHADO --> ENTREGADO
```

La devolucion se registra aparte; no se inventa un estado DEVUELTO fuera del ciclo oficial. No se permite deshacer un pago con Memento ni volver atras escribiendo el estado desde la UI.

## Confirmacion aprobada

```mermaid
sequenceDiagram
    actor Usuario
    Usuario->>Vista: Confirmar cobro
    Vista->>Invocador: ConfirmarPedidoCommand
    Invocador->>Facade: Confirmar(id, clave)
    Facade->>Autorizador: Exigir VENTAS/ADMIN
    Facade->>Repositorio: BEGIN IMMEDIATE
    Facade->>Repositorio: Leer clave, pedido y stock
    Facade->>Chain: Validar cliente, lineas, stock, estado
    Facade->>Proxy: CobrarAsync
    Proxy->>Decorator: Cobro autorizado
    Decorator->>Adapter: Registrar e invocar
    Adapter->>Legado: CobrarCentavos
    Facade->>Repositorio: Pago, stock, estado y Outbox
    Repositorio-->>Facade: COMMIT
    Vista->>Outbox: Procesar
    Outbox->>Observer: Publicar a suscriptores
    Vista->>Mediator: Actualizar vistas
```

## Operacion rechazada

```mermaid
sequenceDiagram
    actor Consulta
    Consulta->>Proxy: Intentar cobro directo
    Proxy->>Autorizador: Exigir permiso
    Autorizador->>Bitacora: SEGURIDAD DENEGADO
    Autorizador-->>Proxy: UnauthorizedAccessException
    Note over Proxy: Decorator y proveedor no son invocados
```

## Consistencia y recuperacion

BEGIN IMMEDIATE toma el bloqueo de escritura antes de leer existencias; las versiones aportan control optimista adicional. Los cambios locales de venta se guardan juntos; el proveedor simulado tambien reconoce su clave de reintento. No se afirma atomicidad distribuida con una pasarela bancaria real.

Outbox se crea en la transaccion de negocio y se procesa despues. Las recepciones evitan repetir reacciones ya completadas. Las metricas son de sesion y no sustituyen un reporte historico. Tras cerrar una sesion se reconstruyen menu y vistas del nuevo rol.

## Reglas numericas

- KIT_OFICINA: 50.00; KIT_TELETRABAJO: 60.00, peso 1.00 kg.
- Mayorista: al menos diez unidades fisicas, tasa 15%, prioridad sobre VIP 10%; normal 0%.
- ESTANDAR: 5.00 y tres dias; EXPRES: 12.00 y un dia.
- decimal y redondeo AwayFromZero a dos decimales. Sin IVA no solicitado.
- Ana: 59.00 y stock 7/4/19; Luis: 102.00; Carmen: 90.00 con diez P03.
- DEFECTO hasta 30 dias; ARREPENTIMIENTO hasta siete dias y sin uso.
- Reembolso total de productos despues del descuento, sin tarifa; reintegro unico.

El valor numerico de los roles se conserva desde Rol.cs del usuario (1 a 4). Las bases de ejecucion de Alan no se comparten con la referencia anterior.
