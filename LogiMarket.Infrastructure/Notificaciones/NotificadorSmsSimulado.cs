using LogiMarket.Application.Contratos;
using LogiMarket.Domain.Operaciones;

namespace LogiMarket.Infrastructure.Notificaciones;
// Responsabilidad de NotificadorSmsSimulado: colaborar desde LogiMarket.Infrastructure.Notificaciones sin mezclar reglas, persistencia y pantallas.
public sealed class NotificadorSmsSimulado(IRepositorio repo) : INotificador
{
    public Task Enviar(EventoOutbox evento)
    {
        repo.Agregar(new Aviso(evento.Id, "SMS", $"SMS simulado: {evento.Tipo}, pedido {evento.PedidoId}"));
        return Task.CompletedTask;
    }
}
