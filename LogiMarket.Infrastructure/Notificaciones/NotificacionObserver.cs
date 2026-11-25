using LogiMarket.Application.Contratos;
using LogiMarket.Domain.Operaciones;

namespace LogiMarket.Infrastructure.Notificaciones;
// Observer llama al Factory Method elegido el evento no necesita conocer como se envia EMAIL o SMS
public sealed class NotificacionObserver(CreadorEmail email, CreadorSms sms, OpcionesDemostracion opciones) : ISuscriptorEvento
{
    public string Nombre => "NOTIFICACION";

    public Task Recibir(EventoOutbox evento)
    {
        if (opciones.FallarNotificacion)
            throw new InvalidOperationException("Fallo de notificacion simulado.");
        return (opciones.Canal == "SMS" ? (CreadorNotificador)sms : email).Notificar(evento);
    }
}
