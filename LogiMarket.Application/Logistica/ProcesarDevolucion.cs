using LogiMarket.Application.Contratos;
using LogiMarket.Application.Seguridad;
using LogiMarket.Domain.Enums;
using LogiMarket.Domain.Operaciones;
using LogiMarket.Domain.Pedidos;

namespace LogiMarket.Application.Logistica;
// Patron Template Method: fija la secuencia completa y deja la inspeccion particular a cada motivo.
// Template Method: el esqueleto es comun; cada motivo decide plazo y condicion.
public abstract class ProcesarDevolucion(IRepositorio repo, ITransacciones tx, Autorizador auth, IReloj reloj, IReembolsosSimulados reembolsos)
{
    protected abstract MotivoDevolucion Motivo { get; }

    protected abstract void Inspeccionar(TimeSpan antiguedad, bool sinUso);
    public async Task<Devolucion> Ejecutar(Guid id, bool sinUso)
    {
        await auth.Exigir(Rol.VENTAS);
        return await tx.Ejecutar(async () =>
        {
            var pedido = await repo.Pedido(id);
            if (pedido.Estado != EstadoPedido.ENTREGADO || pedido.EntregadoEn is null)
                throw new InvalidOperationException("Se requiere pedido entregado.");
            if (await repo.DevolucionPorPedido(id)is not null)
                throw new InvalidOperationException("Pedido ya devuelto.");
            var edad = reloj.Ahora - pedido.EntregadoEn.Value;
            if (edad < TimeSpan.Zero)
                throw new InvalidOperationException("Fecha de entrega futura.");
            Inspeccionar(edad, sinUso);
            var importe = pedido.Subtotal - pedido.Descuento;
            var referencia = await reembolsos.ReembolsarAsync(id, importe);
            var devolucion = new Devolucion(id, Motivo, importe, referencia, reloj.Ahora);
            var productos = await repo.Productos();
            foreach (var grupo in pedido.Detalles.SelectMany(d => d.Componentes).GroupBy(c => c.Sku))
            {
                var producto = productos.Single(p => p.Sku == grupo.Key);
                var cantidad = grupo.Sum(c => c.Cantidad);
                producto.Mover(cantidad);
                repo.Agregar(new MovimientoInventario(producto.Id, cantidad, "DEVOLUCION", id.ToString()));
            }

            repo.Agregar(devolucion);
            repo.Agregar(new EventoOutbox(id, "DevolucionAceptada"));
            return devolucion;
        });
    }
}
