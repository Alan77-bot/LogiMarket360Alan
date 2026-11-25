using LogiMarket.Application.Contratos;
using LogiMarket.Domain.Entidades;

namespace LogiMarket.Application.Seguridad;
// La seguridad se comprueba en el servicio para que tambien funcione cuando no se utiliza la interfaz.
public sealed class Autenticacion(IRepositorio repositorio, IHashContrasena hash, SesionActual sesion, IBitacora bitacora)
{
    public async Task Entrar(string nombre, string clave)
    {
        var usuario = await repositorio.Usuario(nombre.Trim().ToLowerInvariant());
        var valido = usuario is not null && hash.Verificar(clave, usuario.Hash);
        await bitacora.Registrar(nombre, "ACCESO", valido ? "ACEPTADO" : "RECHAZADO", Guid.NewGuid().ToString());
        if (!valido)
            throw new UnauthorizedAccessException("Credenciales incorrectas.");
        sesion.Iniciar(usuario!);
    }
}
