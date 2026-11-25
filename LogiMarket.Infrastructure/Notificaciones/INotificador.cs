using LogiMarket.Domain.Operaciones;

namespace LogiMarket.Infrastructure.Notificaciones;
// Factory Method utiliza este colaborador simulado el resultado se registra localmente sin envios externos
public interface INotificador
{
    Task Enviar(EventoOutbox evento);
}
