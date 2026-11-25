using LogiMarket.Domain.Entregas;

namespace LogiMarket.Domain.Promociones;
// Resultado del calculo de productos descuento envio y total
public sealed record Totales(decimal Subtotal, decimal Descuento, decimal Tarifa, decimal Total);
