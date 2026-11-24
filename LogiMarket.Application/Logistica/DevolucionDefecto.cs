using LogiMarket.Application.Contratos;
using LogiMarket.Application.Seguridad;
using LogiMarket.Domain.Enums;

namespace LogiMarket.Application.Logistica;
// Template Method, paso variable: acepta defecto hasta treinta dias despues de entregar.
public sealed class DevolucionDefecto(IRepositorio r, ITransacciones t, Autorizador a, IReloj reloj, IReembolsosSimulados reembolsos) : ProcesarDevolucion(r, t, a, reloj, reembolsos)
{
    protected override MotivoDevolucion Motivo => MotivoDevolucion.DEFECTO;

    protected override void Inspeccionar(TimeSpan antiguedad, bool sinUso)
    {
        if (antiguedad > TimeSpan.FromDays(30))
            throw new InvalidOperationException("Defecto fuera de 30 dias.");
    }
}
