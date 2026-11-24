using LogiMarket.Domain.Entregas;

namespace LogiMarket.Domain.Promociones;
// Responsabilidad de Totales: colaborar desde LogiMarket.Domain.Promociones sin mezclar reglas, persistencia y pantallas.
public sealed record Totales(decimal Subtotal, decimal Descuento, decimal Tarifa, decimal Total);
