using LogiMarket.Application.Contratos;

namespace LogiMarket.Infrastructure.Notificaciones;
// Factory Method: construye el colaborador EMAIL simulado que registra el aviso local.
public sealed class CreadorEmail(IRepositorio repo) : CreadorNotificador
{
    public override INotificador Crear() => new NotificadorEmailSimulado(repo);
}
