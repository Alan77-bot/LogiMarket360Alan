namespace LogiMarket.Application.Contratos;
// Responsabilidad de IReloj: colaborar desde LogiMarket.Application.Contratos sin mezclar reglas, persistencia y pantallas.
public interface IReloj
{
    DateTimeOffset Ahora { get; }
}
