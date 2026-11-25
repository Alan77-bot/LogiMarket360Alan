using LogiMarket.Domain.Entidades;
using LogiMarket.Domain.Operaciones;
using LogiMarket.Domain.Pedidos;
using Microsoft.EntityFrameworkCore;

namespace LogiMarket.Infrastructure.Persistencia.Configuraciones;
// Las restricciones de este grupo se aplican tambien si alguien intenta guardar datos fuera de la UI
public static class ConfiguracionOperaciones
{
    public static void Aplicar(ModelBuilder m)
    {
        m.Entity<Pago>().HasIndex(x => x.PedidoId).IsUnique();
        m.Entity<Pago>().HasIndex(x => x.Clave).IsUnique();
        m.Entity<Pago>().HasIndex(x => x.Referencia).IsUnique();
        m.Entity<Pago>().HasOne<Pedido>().WithMany().HasForeignKey(x => x.PedidoId).OnDelete(DeleteBehavior.Restrict);
        m.Entity<MovimientoInventario>().HasOne<Producto>().WithMany().HasForeignKey(x => x.ProductoId).OnDelete(DeleteBehavior.Restrict);
        m.Entity<MovimientoInventario>().HasIndex(x => new { x.ProductoId, x.Referencia, x.Motivo }).IsUnique();
        m.Entity<Despacho>().HasIndex(x => x.PedidoId).IsUnique();
        m.Entity<Despacho>().HasOne<Pedido>().WithMany().HasForeignKey(x => x.PedidoId).OnDelete(DeleteBehavior.Restrict);
        m.Entity<Devolucion>().HasIndex(x => x.PedidoId).IsUnique();
        m.Entity<Devolucion>().HasIndex(x => x.Referencia).IsUnique();
        m.Entity<Devolucion>().HasOne<Pedido>().WithMany().HasForeignKey(x => x.PedidoId).OnDelete(DeleteBehavior.Restrict);
        m.Entity<EventoOutbox>().HasOne<Pedido>().WithMany().HasForeignKey(x => x.PedidoId).OnDelete(DeleteBehavior.Restrict);
        m.Entity<RecepcionEvento>().HasKey(x => new { x.EventoId, x.Suscriptor });
        m.Entity<RecepcionEvento>().HasOne<EventoOutbox>().WithMany().HasForeignKey(x => x.EventoId);
        m.Entity<Aviso>().HasIndex(x => x.EventoId).IsUnique();
        m.Entity<Aviso>().HasOne<EventoOutbox>().WithMany().HasForeignKey(x => x.EventoId);
    }
}
