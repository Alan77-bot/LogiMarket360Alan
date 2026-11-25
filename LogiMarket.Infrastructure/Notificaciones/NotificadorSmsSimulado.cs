using LogiMarket.Application.Contratos;
using LogiMarket.Domain.Operaciones;

namespace LogiMarket.Infrastructure.Notificaciones;
// Factory Method utiliza este colaborador simulado; el resultado se registra localmente, sin envios externos.
public sealed class NotificadorSmsSimulado(IRepositorio repo) : INotificador
{
    public Task Enviar(EventoOutbox evento)
    {
        repo.Agregar(new Aviso(evento.Id, "SMS", $"SMS simulado: {evento.Tipo}, pedido {evento.PedidoId}"));
        return Task.CompletedTask;
    }
}
