using LogiMarket.Domain.Entidades;
using LogiMarket.Domain.Operaciones;
using LogiMarket.Domain.Pedidos;
using Microsoft.EntityFrameworkCore;

namespace LogiMarket.Infrastructure.Persistencia.Configuraciones;
// Las restricciones de este grupo se aplican tambien si alguien intenta guardar datos fuera de la UI.
public static class ConfiguracionUsuario
{
    public static void Aplicar(ModelBuilder m)
    {
        m.Entity<Cliente>().HasIndex(x => x.Nombre);
        m.Entity<Usuario>().HasIndex(x => x.Nombre).IsUnique();
        m.Entity<Auditoria>().HasIndex(x => x.Correlacion);
    }
}
