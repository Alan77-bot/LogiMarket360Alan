namespace LogiMarket.Domain.Entregas;
// Responsabilidad de Plazo: colaborar desde LogiMarket.Domain.Entregas sin mezclar reglas, persistencia y pantallas.
internal sealed record Plazo(int Dias) : IPlazoEntrega;
