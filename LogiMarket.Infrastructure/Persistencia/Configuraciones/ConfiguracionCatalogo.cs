using LogiMarket.Domain.Entidades;
using LogiMarket.Domain.Operaciones;
using LogiMarket.Domain.Pedidos;
using Microsoft.EntityFrameworkCore;

namespace LogiMarket.Infrastructure.Persistencia.Configuraciones;
// Las restricciones de este grupo se aplican tambien si alguien intenta guardar datos fuera de la UI.
public static class ConfiguracionCatalogo
{
    public static void Aplicar(ModelBuilder m)
    {
        m.Entity<Producto>().HasIndex(x => x.Sku).IsUnique();
        m.Entity<Producto>().Property(x => x.Version).IsConcurrencyToken();
        m.Entity<Producto>().ToTable(t =>
        {
            t.HasCheckConstraint("CK_Producto_Stock", "Stock >= 0");
            t.HasCheckConstraint("CK_Producto_Precio", "CAST(Precio AS REAL) > 0");
        });
        m.Entity<Kit>().HasIndex(x => x.Sku).IsUnique();
        m.Entity<Kit>().HasMany(x => x.Componentes).WithOne().HasForeignKey(x => x.KitId).OnDelete(DeleteBehavior.Cascade);
        m.Entity<KitComponente>().HasOne<Producto>().WithMany().HasForeignKey(x => x.ProductoId).OnDelete(DeleteBehavior.Restrict);
        m.Entity<KitComponente>().HasOne<Kit>().WithMany().HasForeignKey(x => x.KitHijoId).OnDelete(DeleteBehavior.Restrict);
        m.Entity<KitComponente>().ToTable(t => t.HasCheckConstraint("CK_KitComponente_Destino", "Cantidad > 0 AND ((ProductoId IS NOT NULL AND KitHijoId IS NULL) OR (ProductoId IS NULL AND KitHijoId IS NOT NULL))"));
    }
}
