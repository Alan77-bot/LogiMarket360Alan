using LogiMarket.Domain.Operaciones;

namespace LogiMarket.Application.Contratos;
// Este puerto define lo que necesita Application. Infrastructure proporciona la implementacion concreta.
public interface ISuscriptorEvento
{
    string Nombre { get; }

    Task Recibir(EventoOutbox evento);
}
