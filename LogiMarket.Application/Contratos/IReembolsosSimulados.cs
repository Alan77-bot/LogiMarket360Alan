namespace LogiMarket.Application.Contratos;
// Responsabilidad de IReembolsosSimulados: colaborar desde LogiMarket.Application.Contratos sin mezclar reglas, persistencia y pantallas.
public interface IReembolsosSimulados
{
    Task<string> ReembolsarAsync(Guid pedido, decimal importe);
}
