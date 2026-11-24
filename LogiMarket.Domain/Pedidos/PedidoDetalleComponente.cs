using LogiMarket.Domain.Catalogo;

namespace LogiMarket.Domain.Pedidos;
// Cantidad fisica total de esta linea: ya incluye cuantas unidades del kit se compraron.
public sealed class PedidoDetalleComponente
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid PedidoDetalleId { get; private set; }
    public string Sku { get; private set; } = "";
    public int Cantidad { get; private set; }
    public decimal Precio { get; private set; }
    public decimal Peso { get; private set; }

    private PedidoDetalleComponente()
    {
    }

    public PedidoDetalleComponente(ComponenteFisico c)
    {
        Sku = c.Sku;
        Cantidad = c.Cantidad;
        Precio = c.Precio;
        Peso = c.Peso;
    }

    internal PedidoDetalleComponente Copiar() => new(new(Sku, Cantidad, Precio, Peso));
}
