using LogiMarket.Application.Contratos;

namespace LogiMarket.Infrastructure.Notificaciones;
// Factory Method: construye el colaborador SMS simulado que registra el aviso local.
public sealed class CreadorSms(IRepositorio repo) : CreadorNotificador
{
    public override INotificador Crear() => new NotificadorSmsSimulado(repo);
}
