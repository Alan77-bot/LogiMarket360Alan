using LogiMarket.Domain.Entidades;
using LogiMarket.Domain.Enums;

namespace LogiMarket.Application.Seguridad;
// La seguridad se comprueba en el servicio para que tambien funcione cuando no se utiliza la interfaz.
public sealed class SesionActual
{
    public Guid? UsuarioId { get; private set; }
    public string Nombre { get; private set; } = "Anonimo";
    public Rol? Rol { get; private set; }

    public void Iniciar(Usuario usuario)
    {
        UsuarioId = usuario.Id;
        Nombre = usuario.Nombre;
        Rol = usuario.Rol;
    }

    public void Cerrar()
    {
        UsuarioId = null;
        Nombre = "Anonimo";
        Rol = null;
    }
}
