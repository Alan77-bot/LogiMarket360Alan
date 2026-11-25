# Guia para explicar y modificar el proyecto

Abre la solucion y usa Ctrl+T para buscar una clase. Shift+F12 muestra sus usos. Antes de modificar, define que regla cambia y que resultados deben seguir iguales. El mapa de patrones contiene rutas directas.

## Recorrido de una confirmacion

VistaPedidos solicita ConfirmarPedidoCommand al InvocadorComandos. ConfirmacionPedidoFacade comprueba permisos, idempotencia y la cadena de validacion. PagoAutorizadoProxy protege al Decorator y al proveedor adaptado. La transaccion guarda pago, movimientos, estado y Outbox. Despues PublicadorEventos coordina suscriptores y Mediator refresca pantallas.

## Cambios tipicos

| Solicitud | Donde empezar | Que comprobar |
|---|---|---|
| Nueva promocion | Domain/Promociones/IPoliticaDescuento y RegistroServicios | Condicion, tasa y prioridad; no acumular descuentos |
| Otra entrega | Domain/Entregas/IFabricaEntrega y sus familias | Tarifa, plazo, etiqueta, enum, DI y seleccion visual |
| Nueva validacion | Application/Validaciones/ValidadorPedido y CadenaConfirmacion | Orden de rechazo y ausencia de cobro/stock al fallar |
| Otro proveedor | Infrastructure/Pagos e IPasarelaPago | Contrato, idempotencia, SelectorPasarela y Proxy antes de Decorator |
| Nuevo motivo de devolucion | ProcesarDevolucion y sus derivados | Plazo, sin uso, enum, UI, reembolso unico y sin flete |
| Otra reaccion al pago | ISuscriptorEvento y RegistroServicios | Recepcion unica y recuperacion Outbox |
| Otro formato de salida | ISalidaDocumento | Documento y salida independientes, salida real y prueba |
| Cambiar permisos | PermisosVista y Autorizador/servicios | Menu, botones y rechazo directo del servicio |
| Agregar un campo persistido | Entidad, caso de uso, mapeo EF y formulario | Migracion nueva, lectura despues de reiniciar |
| Cambiar aspecto | WinForms/Compartido/Ui y la vista | Redimensionamiento, foco, lectura y ausencia de reglas de negocio en UI |

Anteponer LogiMarket. al nombre de la capa de la tabla. Para registrar un producto existente solo utiliza el CRUD; no cambies la semilla cada vez que quieras agregar datos.

## Como responder sobre un patron

Explica problema, participantes, recorrido, prueba y costo. Por ejemplo, Strategy permite otra politica sin editar la calculadora; cuesta mantener prioridades explicitas. Proxy decide acceso, Decorator agrega trazas y Adapter traduce contratos. No son intercambiables aunque participen en el mismo cobro.

## Como probar un cambio

Cerrar la app, crear rama codex/nombre, revisar el diff, compilar, ejecutar pruebas y hacer la demostracion pertinente. Guardar solo los archivos de esa finalidad en un commit. No agregar pruebas falsas ni dividir commits retrospectivamente para simular etapas que no ocurrieron.

No cambies los ejemplos oficiales para conseguir que una prueba pase. Si agregas una politica nueva, comprueba tambien que los escenarios 59.00, 102.00 y 90.00 conservan su resultado cuando la nueva politica no aplica.