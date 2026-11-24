using LogiMarket.Domain.Catalogo;

namespace LogiMarket.Domain.Pedidos;
// La linea conserva una instantanea del kit y sus precios. Un cambio de catalogo no reescribe la venta.
public sealed class PedidoDetalle
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid PedidoId { get; private set; }
    public string Sku { get; private set; } = "";
    public int Cantidad { get; private set; }
    public string ComposicionJson { get; private set; } = "";

    private List<PedidoDetalleComponente> componentes = [];
    public IReadOnlyList<PedidoDetalleComponente> Componentes => componentes.AsReadOnly();

    private PedidoDetalle()
    {
    }

    public PedidoDetalle(IElementoCatalogo elemento, int cantidad)
    {
        Sku = elemento.Sku;
        Cantidad = cantidad;
        componentes = elemento.Expandir(cantidad).Select(c => new PedidoDetalleComponente(c)).ToList();
        ComposicionJson = System.Text.Json.JsonSerializer.Serialize(InstantaneaElemento.Capturar(elemento));
    }

    internal PedidoDetalle Copiar() => new()
    {
        Sku = Sku,
        Cantidad = Cantidad,
        ComposicionJson = ComposicionJson,
        componentes = Componentes.Select(c => c.Copiar()).ToList()
    };
    internal PedidoDetalle ConCantidad(int cantidad)
    {
        if (cantidad <= 0)
            throw new ArgumentOutOfRangeException(nameof(cantidad));
        return new()
        {
            Sku = Sku,
            Cantidad = cantidad,
            ComposicionJson = ComposicionJson,
            componentes = Componentes.Select(c => new PedidoDetalleComponente(new(c.Sku, checked(c.Cantidad / Cantidad * cantidad), c.Precio, c.Peso))).ToList()
        };
    }
}
