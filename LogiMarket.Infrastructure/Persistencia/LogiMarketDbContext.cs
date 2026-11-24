using LogiMarket.Domain.Entidades;
using LogiMarket.Domain.Operaciones;
using LogiMarket.Domain.Pedidos;
using Microsoft.EntityFrameworkCore;

namespace LogiMarket.Infrastructure.Persistencia;
// Mapea el modelo a SQLite. Las FK, checks e indices unicos complementan las validaciones del dominio.
public sealed class LogiMarketDbContext(DbContextOptions<LogiMarketDbContext> opciones) : DbContext(opciones)
{
    public DbSet<Producto> Productos => Set<Producto>();
    public DbSet<Kit> Kits => Set<Kit>();
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Pedido> Pedidos => Set<Pedido>();

    protected override void OnModelCreating(ModelBuilder m)
    {
        LogiMarket.Infrastructure.Persistencia.Configuraciones.ConfiguracionUsuario.Aplicar(m);
        LogiMarket.Infrastructure.Persistencia.Configuraciones.ConfiguracionCatalogo.Aplicar(m);
        LogiMarket.Infrastructure.Persistencia.Configuraciones.ConfiguracionPedido.Aplicar(m);
        LogiMarket.Infrastructure.Persistencia.Configuraciones.ConfiguracionOperaciones.Aplicar(m);
        // Las identidades nacen en el dominio. EF debe insertar hijos nuevos,
        // aunque su Guid ya tenga valor al agregarlos a una coleccion rastreada.
        foreach (var entidad in m.Model.GetEntityTypes())
            if (entidad.FindProperty("Id")is { ClrType: var tipo } propiedad && tipo == typeof(Guid))
                propiedad.ValueGenerated = Microsoft.EntityFrameworkCore.Metadata.ValueGenerated.Never;
    }
}
