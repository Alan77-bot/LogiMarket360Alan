using LogiMarket.Domain.Operaciones;
using System.Collections.Concurrent;

namespace LogiMarket.Application.Eventos;
// Patron Singleton mediante DI todas las vistas comparten esta instancia Las metricas no sustituyen al historial
// Singleton por DI los identificadores previenen contar dos veces el mismo evento
public sealed class MetricasSesion
{
    private readonly ConcurrentDictionary<Guid, string> eventos = new();
    public void Registrar(EventoOutbox evento) => eventos.TryAdd(evento.Id, evento.Tipo);
    public int Pagos => eventos.Values.Count(t => t == "PedidoPagado");
    public int Despachos => eventos.Values.Count(t => t == "DespachoRegistrado");
    public int Devoluciones => eventos.Values.Count(t => t == "DevolucionAceptada");
}
