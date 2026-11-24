using LogiMarket.Domain.Operaciones;

namespace LogiMarket.Application.Contratos;
// Responsabilidad de ISuscriptorEvento: colaborar desde LogiMarket.Application.Contratos sin mezclar reglas, persistencia y pantallas.
public interface ISuscriptorEvento
{
    string Nombre { get; }

    Task Recibir(EventoOutbox evento);
}
