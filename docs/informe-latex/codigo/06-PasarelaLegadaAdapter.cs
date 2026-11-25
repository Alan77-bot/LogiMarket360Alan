using LogiMarket.Application.Contratos;
using LogiMarket.Application.DTOs;
using System.Collections.Concurrent;

namespace LogiMarket.Infrastructure.Pagos;
// Patron Adapter convierte el importe decimal a centavos para conservar el contrato de la pasarela legada
public sealed class PasarelaLegadaAdapter(PasarelaLegadaSimulada legado, OpcionesDemostracion opciones) : IPasarelaPago
{
    public Task<ResultadoCobro> CobrarAsync(decimal importe, string clave)
    {
        Validar(importe, clave);
        return Task.FromResult(opciones.RechazarCobro ? new ResultadoCobro(false, "", "Cobro rechazado por simulacion.") : new(true, legado.CobrarCentavos(checked((long)(importe * 100)), clave), "Aceptado"));
    }

    internal static void Validar(decimal importe, string clave)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(clave);
        if (importe <= 0 || decimal.Round(importe, 2) != importe)
            throw new ArgumentException("Importe invalido.");
    }
}
