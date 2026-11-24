using LogiMarket.Domain.Operaciones;

namespace LogiMarket.Infrastructure.Notificaciones;
// Responsabilidad de INotificador: colaborar desde LogiMarket.Infrastructure.Notificaciones sin mezclar reglas, persistencia y pantallas.
public interface INotificador
{
    Task Enviar(EventoOutbox evento);
}
