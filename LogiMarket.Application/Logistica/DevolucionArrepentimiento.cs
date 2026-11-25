using LogiMarket.Application.Contratos;
using LogiMarket.Application.Seguridad;
using LogiMarket.Domain.Enums;

namespace LogiMarket.Application.Logistica;
// Template Method paso variable exige sin uso y como maximo siete dias
public sealed class DevolucionArrepentimiento(IRepositorio r, ITransacciones t, Autorizador a, IReloj reloj, IReembolsosSimulados reembolsos) : ProcesarDevolucion(r, t, a, reloj, reembolsos)
{
    protected override MotivoDevolucion Motivo => MotivoDevolucion.ARREPENTIMIENTO;

    protected override void Inspeccionar(TimeSpan antiguedad, bool sinUso)
    {
        if (!sinUso || antiguedad > TimeSpan.FromDays(7))
            throw new InvalidOperationException("Arrepentimiento requiere producto sin uso y hasta 7 dias.");
    }
}
