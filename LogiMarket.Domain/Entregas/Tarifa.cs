namespace LogiMarket.Domain.Entregas;
// Responsabilidad de Tarifa: colaborar desde LogiMarket.Domain.Entregas sin mezclar reglas, persistencia y pantallas.
internal sealed record Tarifa(decimal Importe) : ITarifaEntrega;
