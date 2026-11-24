using LogiMarket.Application.Contratos;
using LogiMarket.Application.DTOs;
using LogiMarket.Application.Seguridad;
using System.Collections.Concurrent;

namespace LogiMarket.Infrastructure.Pagos;
// Patron Decorator: agrega inicio, fin o error alrededor del cobro y conserva el resultado del colaborador.
// Decorator: agrega trazas sin cambiar el contrato del proveedor.
public sealed class AuditoriaPagoDecorator(IPasarelaPago interno, IBitacora bitacora, SesionActual sesion) : IPasarelaPago
{
    public async Task<ResultadoCobro> CobrarAsync(decimal importe, string clave)
    {
        await bitacora.Registrar(sesion.Nombre, "COBRO", "INICIO", clave);
        try
        {
            var resultado = await interno.CobrarAsync(importe, clave);
            await bitacora.Registrar(sesion.Nombre, "COBRO", resultado.Aceptado ? "FIN" : "ERROR: " + resultado.Mensaje, clave);
            return resultado;
        }
        catch (Exception ex)
        {
            await bitacora.Registrar(sesion.Nombre, "COBRO", "ERROR: " + ex.Message, clave);
            throw;
        }
    }
}
