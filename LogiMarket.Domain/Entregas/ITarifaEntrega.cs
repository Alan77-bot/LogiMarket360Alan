namespace LogiMarket.Domain.Entregas;
// Responsabilidad de ITarifaEntrega: colaborar desde LogiMarket.Domain.Entregas sin mezclar reglas, persistencia y pantallas.
public interface ITarifaEntrega
{
    decimal Importe { get; }
}
