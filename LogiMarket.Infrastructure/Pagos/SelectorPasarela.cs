using LogiMarket.Application.Contratos;
using LogiMarket.Application.DTOs;
using System.Collections.Concurrent;

namespace LogiMarket.Infrastructure.Pagos;
// Colaborador de pago simulado Sus referencias permiten rastrear la operacion sin integrar cobros reales
public sealed class SelectorPasarela(PasarelaLegadaAdapter legado, PasarelaAlternativaSimulada alternativo, OpcionesDemostracion opciones) : IPasarelaPago
{
    public Task<ResultadoCobro> CobrarAsync(decimal importe, string clave) => (opciones.ProveedorAlternativo ? (IPasarelaPago)alternativo : legado).CobrarAsync(importe, clave);
}
