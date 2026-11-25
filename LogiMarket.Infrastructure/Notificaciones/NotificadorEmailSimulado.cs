using LogiMarket.Application.Contratos;
using LogiMarket.Domain.Operaciones;

namespace LogiMarket.Infrastructure.Notificaciones;
// Factory Method utiliza este colaborador simulado; el resultado se registra localmente, sin envios externos.
public sealed class NotificadorEmailSimulado(IRepositorio repo) : INotificador
{
    public Task Enviar(EventoOutbox evento)
    {
        repo.Agregar(new Aviso(evento.Id, "EMAIL", $"Correo simulado: {evento.Tipo}, pedido {evento.PedidoId}"));
        return Task.CompletedTask;
    }
}
