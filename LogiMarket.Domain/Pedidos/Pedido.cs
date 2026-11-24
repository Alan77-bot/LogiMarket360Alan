using LogiMarket.Domain.Entidades;
using LogiMarket.Domain.Entregas;
using LogiMarket.Domain.Enums;
using LogiMarket.Domain.Estados;
using LogiMarket.Domain.Promociones;

namespace LogiMarket.Domain.Pedidos;
// Agregado de la venta. Aqui viven las invariantes, Prototype en ClonarBorrador y State al cambiar de estado.
public sealed class Pedido
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ClienteId { get; private set; }
    public string ClienteNombre { get; private set; } = "";
    public bool ClienteVip { get; private set; }
    public TipoEntrega Entrega { get; private set; }
    public EstadoPedido Estado { get; private set; } = EstadoPedido.BORRADOR;
    public DateTimeOffset Fecha { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? EntregadoEn { get; private set; }
    public decimal Subtotal { get; private set; }
    public decimal Descuento { get; private set; }
    public decimal Tarifa { get; private set; }
    public decimal Total { get; private set; }
    public Guid Version { get; private set; } = Guid.NewGuid();

    private List<PedidoDetalle> detalles = [];
    private List<HistorialEstado> historial = [];
    private bool necesitaValoracion;
    public IReadOnlyList<PedidoDetalle> Detalles => detalles.AsReadOnly();
    public IReadOnlyList<HistorialEstado> Historial => historial.AsReadOnly();

    private Pedido()
    {
    }

    internal Pedido(Cliente cliente, TipoEntrega entrega, IEnumerable<PedidoDetalle> detalles)
    {
        ClienteId = cliente.Id;
        ClienteNombre = cliente.Nombre;
        ClienteVip = cliente.Categoria == CategoriaCliente.VIP;
        Entrega = entrega;
        this.detalles = detalles.Select(d => d.Copiar()).ToList();
        if (Detalles.Count == 0)
            throw new ArgumentException("Agregue al menos una linea.");
        necesitaValoracion = true;
        historial.Add(new(Id, Estado, "Creacion"));
    }

    public void Valorar(CalculadoraPrecios calculadora, decimal tarifa)
    {
        ExigirEditable();
        var componentes = Detalles.SelectMany(d => d.Componentes).ToArray();
        var totales = calculadora.Calcular(componentes.Sum(c => c.Precio * c.Cantidad), tarifa, new(ClienteVip, componentes.Sum(c => c.Cantidad)));
        Subtotal = totales.Subtotal;
        Descuento = totales.Descuento;
        Tarifa = totales.Tarifa;
        Total = totales.Total;
        necesitaValoracion = false;
    }

    public void ExigirEditable()
    {
        if (Estado != EstadoPedido.BORRADOR)
            throw new InvalidOperationException("Solo se puede editar un borrador.");
    }

    public void Transicionar(EstadoPedido destino, string usuario, DateTimeOffset ahora)
    {
        // State: cada objeto de estado conoce sus siguientes estados validos.
        CatalogoEstados.Obtener(Estado).Exigir(destino);
        // Una cantidad editada no puede validar importes calculados anteriormente.
        if (destino == EstadoPedido.VALIDADO && (necesitaValoracion || Total <= 0))
            throw new InvalidOperationException("Recalcule los importes antes de validar.");
        Estado = destino;
        Version = Guid.NewGuid();
        if (destino == EstadoPedido.ENTREGADO)
            EntregadoEn = ahora;
        historial.Add(new(Id, destino, usuario));
    }

    // Prototype: se copian las colecciones y se genera una identidad nueva.
    public Pedido ClonarBorrador()
    {
        ExigirEditable();
        var copia = new Pedido
        {
            ClienteId = ClienteId,
            ClienteNombre = ClienteNombre,
            ClienteVip = ClienteVip,
            Entrega = Entrega,
            Subtotal = Subtotal,
            Descuento = Descuento,
            Tarifa = Tarifa,
            Total = Total,
            necesitaValoracion = necesitaValoracion,
            detalles = Detalles.Select(d => d.Copiar()).ToList()
        };
        copia.historial.Add(new(copia.Id, EstadoPedido.BORRADOR, "Clonacion"));
        return copia;
    }

    public void AplicarCambios(Pedido nuevo)
    {
        ExigirEditable();
        nuevo.ExigirEditable();
        ClienteId = nuevo.ClienteId;
        ClienteNombre = nuevo.ClienteNombre;
        ClienteVip = nuevo.ClienteVip;
        Entrega = nuevo.Entrega;
        detalles = nuevo.Detalles.Select(d => d.Copiar()).ToList();
        Subtotal = nuevo.Subtotal;
        Descuento = nuevo.Descuento;
        Tarifa = nuevo.Tarifa;
        Total = nuevo.Total;
        Version = Guid.NewGuid();
        necesitaValoracion = nuevo.necesitaValoracion;
    }

    public void CambiarCantidad(int indice, int cantidad)
    {
        ExigirEditable();
        detalles[indice] = detalles[indice].ConCantidad(cantidad);
        necesitaValoracion = true;
        Version = Guid.NewGuid();
    }
}
