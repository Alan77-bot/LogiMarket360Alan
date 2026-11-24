using LogiMarket.Application.Contratos;
using LogiMarket.Application.DTOs;
using System.Collections.Concurrent;

namespace LogiMarket.Infrastructure.Pagos;
// Responsabilidad de SelectorPasarela: colaborar desde LogiMarket.Infrastructure.Pagos sin mezclar reglas, persistencia y pantallas.
public sealed class SelectorPasarela(PasarelaLegadaAdapter legado, PasarelaAlternativaSimulada alternativo, OpcionesDemostracion opciones) : IPasarelaPago
{
    public Task<ResultadoCobro> CobrarAsync(decimal importe, string clave) => (opciones.ProveedorAlternativo ? (IPasarelaPago)alternativo : legado).CobrarAsync(importe, clave);
}
