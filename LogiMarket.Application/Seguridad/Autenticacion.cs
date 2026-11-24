using LogiMarket.Application.Contratos;
using LogiMarket.Domain.Entidades;

namespace LogiMarket.Application.Seguridad;
// Responsabilidad de Autenticacion: colaborar desde LogiMarket.Application.Seguridad sin mezclar reglas, persistencia y pantallas.
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
