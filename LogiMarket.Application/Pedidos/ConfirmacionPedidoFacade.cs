using LogiMarket.Application.Contratos;
using LogiMarket.Application.Seguridad;
using LogiMarket.Application.Validaciones;
using LogiMarket.Domain.Enums;
using LogiMarket.Domain.Operaciones;
using LogiMarket.Domain.Pedidos;

namespace LogiMarket.Application.Pedidos;
// Patron Facade: coordina permisos, Chain, pago, stock y Outbox en una transaccion. No conoce EF ni SQL.
// Facade: coordina colaboradores; no conoce EF, SQL ni tasas de descuento.
public sealed class ConfirmacionPedidoFacade(IRepositorio repo, ITransacciones tx, Autorizador auth, IPasarelaPago pasarela, SesionActual sesion, IReloj reloj)
{
    public async Task<Pago> Confirmar(Guid id, string clave)
    {
        await auth.Exigir(Rol.VENTAS);
        ArgumentException.ThrowIfNullOrWhiteSpace(clave);
        return await tx.Ejecutar(async () =>
        {
            var previo = await repo.PagoPorClave(clave);
            if (previo is not null)
            {
                if (previo.PedidoId != id)
                    throw new InvalidOperationException("Clave usada para otro pedido.");
                return previo;
            }

            var pedido = await repo.Pedido(id);
            if (await repo.PagoPorPedido(id)is not null)
                throw new InvalidOperationException("El pedido ya tiene un pago.");
            var productos = await repo.Productos();
            var resultado = CadenaConfirmacion.Validar(new(pedido, (await repo.Clientes()).SingleOrDefault(c => c.Id == pedido.ClienteId), productos));
            if (!resultado.Valido)
                throw new InvalidOperationException(resultado.Mensaje);
            var cobro = await pasarela.CobrarAsync(pedido.Total, clave);
            if (!cobro.Aceptado)
                throw new InvalidOperationException(cobro.Mensaje);
            foreach (var grupo in pedido.Detalles.SelectMany(d => d.Componentes).GroupBy(c => c.Sku))
            {
                var producto = productos.Single(p => p.Sku == grupo.Key);
                var cantidad = grupo.Sum(c => c.Cantidad);
                producto.Mover(-cantidad);
                repo.Agregar(new MovimientoInventario(producto.Id, -cantidad, "VENTA", id.ToString()));
            }

            var pago = new Pago(id, clave, cobro.Referencia, pedido.Total);
            repo.Agregar(pago);
            pedido.Transicionar(EstadoPedido.PAGADO, sesion.Nombre, reloj.Ahora);
            repo.Agregar(new EventoOutbox(id, "PedidoPagado"));
            return pago;
        });
    }
}
