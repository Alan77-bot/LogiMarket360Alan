using LogiMarket.Application.Contratos;
using LogiMarket.Domain.Operaciones;
using System.Collections.Concurrent;

namespace LogiMarket.Application.Eventos;
// Observer actualiza la instancia compartida de MetricasSesion cuando llega un evento de negocio
public sealed class MetricasObserver(MetricasSesion metricas) : ISuscriptorEvento
{
    public string Nombre => "METRICAS";

    public Task Recibir(EventoOutbox evento)
    {
        metricas.Registrar(evento);
        return Task.CompletedTask;
    }
}
