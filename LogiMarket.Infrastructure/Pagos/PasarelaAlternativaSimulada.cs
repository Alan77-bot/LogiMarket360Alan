using LogiMarket.Application.Contratos;
using LogiMarket.Application.DTOs;
using System.Collections.Concurrent;

namespace LogiMarket.Infrastructure.Pagos;
// Colaborador de pago simulado. Sus referencias permiten rastrear la operacion sin integrar cobros reales.
public sealed class PasarelaAlternativaSimulada(OpcionesDemostracion opciones) : IPasarelaPago
{
    private readonly ConcurrentDictionary<string, (decimal Importe, string Referencia)> cobros = new();
    public Task<ResultadoCobro> CobrarAsync(decimal importe, string clave)
    {
        PasarelaLegadaAdapter.Validar(importe, clave);
        if (opciones.RechazarCobro)
            return Task.FromResult(new ResultadoCobro(false, "", "Cobro rechazado por simulacion."));
        var cobro = cobros.GetOrAdd(clave, _ => (importe, $"ALT-{Guid.NewGuid():N}"));
        if (cobro.Importe != importe)
            throw new InvalidOperationException("Clave reutilizada con otro importe.");
        return Task.FromResult(new ResultadoCobro(true, cobro.Referencia, "Aceptado"));
    }
}
