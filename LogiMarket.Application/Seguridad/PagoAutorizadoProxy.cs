using LogiMarket.Application.Contratos;
using LogiMarket.Application.DTOs;
using LogiMarket.Domain.Enums;

namespace LogiMarket.Application.Seguridad;
// Patron Proxy: rechaza al rol antes del Decorator y del proveedor. Ocultar un boton no basta para autorizar.
// Proxy: autoriza antes de alcanzar al decorador o al proveedor.
public sealed class PagoAutorizadoProxy(Autorizador autorizador, IPasarelaPago interno) : IPasarelaPago
{
    public async Task<ResultadoCobro> CobrarAsync(decimal importe, string clave)
    {
        await autorizador.Exigir(Rol.VENTAS);
        return await interno.CobrarAsync(importe, clave);
    }
}
