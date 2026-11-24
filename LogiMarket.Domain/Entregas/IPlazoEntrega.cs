namespace LogiMarket.Domain.Entregas;
// Responsabilidad de IPlazoEntrega: colaborar desde LogiMarket.Domain.Entregas sin mezclar reglas, persistencia y pantallas.
public interface IPlazoEntrega
{
    int Dias { get; }
}
