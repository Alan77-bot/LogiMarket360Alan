using LogiMarket.Application.Contratos;
using LogiMarket.Domain.Operaciones;

namespace LogiMarket.Infrastructure.Notificaciones;
// Responsabilidad de NotificadorEmailSimulado: colaborar desde LogiMarket.Infrastructure.Notificaciones sin mezclar reglas, persistencia y pantallas.
public sealed class NotificadorEmailSimulado(IRepositorio repo) : INotificador
{
    public Task Enviar(EventoOutbox evento)
    {
        repo.Agregar(new Aviso(evento.Id, "EMAIL", $"Correo simulado: {evento.Tipo}, pedido {evento.PedidoId}"));
        return Task.CompletedTask;
    }
}
