using LogiMarket.Domain.Entregas;

namespace LogiMarket.Domain.Promociones;
// Coordina esta responsabilidad a traves de contratos; las reglas del negocio permanecen en su capa.
public sealed record Totales(decimal Subtotal, decimal Descuento, decimal Tarifa, decimal Total);
