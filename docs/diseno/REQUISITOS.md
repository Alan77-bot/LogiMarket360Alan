# Requisitos y trazabilidad

| ID | Requisito funcional | Verificacion |
|---|---|---|
| RF01 | Acceso local, roles, cierre y hash | Seguridad y pruebas por rol |
| RF02 | CRUD de productos e integridad de SKU | ServicioCatalogo |
| RF03 | Kits anidados sin ciclos | P18, P27, P34 |
| RF04 | Clientes con categoria y estado | ClienteInactivoBloqueaValidacion |
| RF05 | Borradores, lineas y cantidades | P13, P14, P29 |
| RF06 | Descuentos y entrega | P02-P04, P12, P32 |
| RF07 | Validacion y ciclo de estados | P05, P08, P09, P23, P31 |
| RF08 | Cobro simulado e idempotencia | P06, P07, P16, P19, P22 |
| RF09 | Inventario trazable y concurrencia | P02-P07, P36 |
| RF10 | Despacho, guia y entrega | P09, P12 |
| RF11 | Devolucion total y reembolso | P33, P35 |
| RF12 | Outbox, avisos y auditoria | P11, P30, P37 |
| RF13 | Filtros y comprobantes | P17, P25, P26 |
| RF14 | Persistencia y seed repetible | P01, P10 |
| RF15 | Doce areas visuales y Mediator | Revision visual, P28 |

| ID | Requisito no funcional medible | Criterio |
|---|---|---|
| RNF01 | Integridad | Stock nunca negativo; un pago y devolucion por pedido |
| RNF02 | Seguridad local | Hash con sal; permisos verificados sin UI |
| RNF03 | Mantenibilidad | Cinco proyectos y cero referencias prohibidas |
| RNF04 | Testabilidad | Al menos 25 pruebas automaticas ejecutadas |
| RNF05 | Recuperabilidad | Reintento Outbox sin duplicar pago ni aviso |
| RNF06 | Trazabilidad | Fecha, usuario, resultado y correlacion en auditoria |
| RNF07 | Usabilidad | TAB, etiquetas, foco y confirmaciones: criterio de revision manual pendiente de cierre completo |
| RNF08 | Rendimiento de representacion | 1000 filas Flyweight con identidad y asignaciones comparables |
| RNF09 | Reproducibilidad | Migraciones y seed desde archivo inexistente |
| RNF10 | Exactitud monetaria | decimal, dos decimales y AwayFromZero |

Los tiempos de respuesta se deben medir en el equipo de demostracion antes de afirmar una cifra. No se declara un SLA que no se haya medido.
