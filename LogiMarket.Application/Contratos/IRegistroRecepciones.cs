namespace LogiMarket.Application.Contratos;
// Este puerto define lo que necesita Application. Infrastructure proporciona la implementacion concreta.
public interface IRegistroRecepciones
{
    Task<bool> Recibido(Guid evento, string suscriptor);
    void Marcar(Guid evento, string suscriptor);
}
