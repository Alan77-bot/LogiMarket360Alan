namespace LogiMarket.Domain.Entregas;
// Abstract Factory: este producto forma parte de una familia de entrega coherente.
internal sealed record Tarifa(decimal Importe) : ITarifaEntrega;
