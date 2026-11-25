using LogiMarket.Domain.Operaciones;

namespace LogiMarket.Infrastructure.Notificaciones;
// Patron Factory Method la operacion comun utiliza Crear sin conocer el notificador concreto
// Factory Method los creadores especializados producen el notificador concreto
public abstract class CreadorNotificador
{
    public abstract INotificador Crear();
    public Task Notificar(EventoOutbox evento) => Crear().Enviar(evento);
}
