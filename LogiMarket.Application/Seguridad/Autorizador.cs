using LogiMarket.Application.Contratos;
using LogiMarket.Domain.Enums;

namespace LogiMarket.Application.Seguridad;
// La seguridad se comprueba en el servicio para que tambien funcione cuando no se utiliza la interfaz.
public sealed class Autorizador(SesionActual sesion, IBitacora bitacora)
{
    public async Task Exigir(params Rol[] roles)
    {
        if (sesion.Rol == Rol.ADMIN || sesion.Rol is not null && roles.Contains(sesion.Rol.Value))
            return;
        await bitacora.Registrar(sesion.Nombre, "SEGURIDAD", "DENEGADO", Guid.NewGuid().ToString());
        throw new UnauthorizedAccessException("Su rol no permite esta operacion.");
    }

    public Task Lectura() => Exigir(Rol.VENTAS, Rol.BODEGA, Rol.CONSULTA);
}
