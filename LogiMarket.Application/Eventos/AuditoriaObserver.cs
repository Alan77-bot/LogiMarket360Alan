using LogiMarket.Application.Contratos;
using LogiMarket.Domain.Operaciones;
using System.Collections.Concurrent;

namespace LogiMarket.Application.Eventos;
// Observer registra la publicacion de negocio sin acoplar la confirmacion a esta reaccion
public sealed class AuditoriaObserver(IRepositorio repo) : ISuscriptorEvento
{
    public string Nombre => "AUDITORIA";

    public Task Recibir(EventoOutbox evento)
    {
        repo.Agregar(new Auditoria("OUTBOX", evento.Tipo, "PUBLICADO", evento.Id.ToString()));
        return Task.CompletedTask;
    }
}
