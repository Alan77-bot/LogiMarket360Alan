using LogiMarket.Domain.Enums;

namespace LogiMarket.Domain.Entidades;
// Guardamos el hash recibido nunca la clave escrita por el usuario El rol se comprueba en los servicios
public sealed class Usuario
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Nombre { get; private set; } = "";
    public string Hash { get; private set; } = "";
    public Rol Rol { get; private set; }

    private Usuario()
    {
    }

    public Usuario(string nombre, string hash, Rol rol)
    {
        Nombre = nombre;
        Hash = hash;
        Rol = rol;
    }
}
