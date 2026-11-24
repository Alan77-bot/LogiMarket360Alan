using LogiMarket.Domain.Catalogo;
using LogiMarket.Domain.Entidades;
using LogiMarket.Domain.Enums;

namespace LogiMarket.Domain.Pedidos;
// Patron Builder: reunimos cliente, entrega y lineas antes de construir un pedido valido.
// Builder: no entrega agregados sin cliente y lineas completas.
public sealed class PedidoBuilder
{
    private Cliente? cliente;
    private TipoEntrega entrega;
    private readonly List<PedidoDetalle> detalles = [];
    public PedidoBuilder Para(Cliente valor)
    {
        cliente = valor;
        return this;
    }

    public PedidoBuilder ConEntrega(TipoEntrega valor)
    {
        entrega = valor;
        return this;
    }

    public PedidoBuilder Agregar(IElementoCatalogo elemento, int cantidad)
    {
        detalles.Add(new(elemento, cantidad));
        return this;
    }

    public Pedido Construir() => new(cliente ?? throw new InvalidOperationException("Seleccione cliente."), entrega, detalles);
}
