# Revision y limites reales

Codigo revisado el 24 de noviembre de 2026. Implementacion adaptada de la referencia del mismo LogiMarket360 y distribuida en clases separadas. Los comentarios ubican patrones y decisiones sin tildes. La aplicacion no utiliza imagenes; las capturas son evidencias, no recursos graficos de la UI.

## Riesgos atendidos

1. Formularios con calculos: los importes se calculan en Domain/Application.
2. UI como unica seguridad: los servicios autorizan aunque no exista boton visible.
3. Cobro repetido: clave, referencia y pedido tienen restricciones unicas; reintento devuelve pago original.
4. Sobreventa concurrente: SQLite BEGIN IMMEDIATE y tokens Version.
5. Guardado parcial: transacciones de confirmacion y devolucion.
6. Fallo de aviso despues de cobrar: Outbox durable y recepcion por suscriptor.
7. Cambios de precios retroactivos: instantaneas relacionales y arbol JSON.
8. Kits circulares: deteccion antes de guardar y al construir el catalogo compuesto.
9. Deshacer pagos: Memento limitado a borradores.
10. Devolucion duplicada o con flete: politica y unicidad por pedido; importe sin envio.
11. Cambio de SKU historico: se rechaza si una venta lo referencia.
12. Hijo EF interpretado como existente: identidad Guid de dominio con ValueGenerated.Never.
13. Totales viejos tras editar cantidades: se invalida la valoracion y se recalcula antes de validar.
14. Datos de prueba mezclados con el usuario: bases temporales y carpeta LocalAppData propia de Alan.
15. Vistas ocultas que siguen consultando: solo se construyen las del rol actual.
16. Cabeceras y perfil poco legibles: contraste y distribucion corregidos despues de revisar capturas reales.

Esta lista no representa dieciseis defectos nuevos observados en rojo. La mayor parte son decisiones adaptadas de una referencia ya corregida. No hay diez pares nuevos de pruebas antes/despues que acrediten ese requisito de la guia. El historial actual no debe interpretarse como evidencia retrospectiva de esos pares.

## SOLID y Clean Code

SRP: calculadora, repositorio y vistas tienen responsabilidades diferentes. OCP: IPoliticaDescuento admite otra promocion. LSP: ambos proveedores pasan el contrato de cobro. ISP: pago, reembolso, salida y consultas son puertos separados. DIP: los casos de uso dependen de interfaces; EF queda fuera de las capas internas. KISS/DRY: se reutilizan estilos, transacciones, familias de entrega y la vista base de pedidos. YAGNI: no se agregaron pagos reales, REST, nube ni facturacion fiscal.

## Verificacion realizada

62 pruebas aprobadas, cero fallidas/omitidas; TRX nuevo en evidencias/resultados. Flujo de servicios presentado en formularios reales: venta 59.00, stock 7/4/19, entrega, rechazo de cancelacion, devolucion 54.00, stock 8/5/20, recuperacion Outbox, cuatro combinaciones Bridge y Flyweight 1000 filas. Los ejecutores generan capturas mediante DrawToBitmap; no se afirma un recorrido manual de todos los botones.

La recuperacion de persistencia se comprobo con otro contenedor DI y DbContext contra el mismo SQLite; esto no se presenta como una prueba manual de reiniciar Windows. Falta revision manual exhaustiva de TAB/foco y DPI de monitor. El PDF y los datos personales siguen pendientes. Las capturas antiguas del proyecto de referencia no se incluyen como evidencia nueva.
La distribucion inicial de pruebas se limpio para retirar 42 ayudas privadas no utilizadas. Se repitio el conjunto completo y se conservaron las 62 aprobadas. Esto reduce duplicacion sin agregar escenarios ficticios.
