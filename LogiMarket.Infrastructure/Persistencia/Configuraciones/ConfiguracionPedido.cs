using LogiMarket.Domain.Entidades;
using LogiMarket.Domain.Operaciones;
using LogiMarket.Domain.Pedidos;
using Microsoft.EntityFrameworkCore;

namespace LogiMarket.Infrastructure.Persistencia.Configuraciones;
// Las restricciones de este grupo se aplican tambien si alguien intenta guardar datos fuera de la UI
public static class ConfiguracionPedido
{
    public static void Aplicar(ModelBuilder m)
    {
        m.Entity<Pedido>().HasOne<Cliente>().WithMany().HasForeignKey(x => x.ClienteId).OnDelete(DeleteBehavior.Restrict);
        m.Entity<Pedido>().Property(x => x.Version).IsConcurrencyToken();
        m.Entity<Pedido>().HasMany(x => x.Detalles).WithOne().HasForeignKey(x => x.PedidoId).OnDelete(DeleteBehavior.Cascade);
        m.Entity<Pedido>().HasMany(x => x.Historial).WithOne().HasForeignKey(x => x.PedidoId).OnDelete(DeleteBehavior.Cascade);
        m.Entity<PedidoDetalle>().HasMany(x => x.Componentes).WithOne().HasForeignKey(x => x.PedidoDetalleId).OnDelete(DeleteBehavior.Cascade);
    }
}
