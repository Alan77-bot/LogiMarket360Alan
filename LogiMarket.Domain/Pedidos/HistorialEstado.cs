using LogiMarket.Domain.Entidades;
using LogiMarket.Domain.Enums;

namespace LogiMarket.Domain.Pedidos;
// Cada transicion deja fecha y usuario. El estado actual sirve para operar; este registro explica el recorrido.
public sealed class HistorialEstado
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid PedidoId { get; private set; }
    public EstadoPedido Estado { get; private set; }
    public string Usuario { get; private set; } = "";
    public DateTimeOffset Fecha { get; private set; } = DateTimeOffset.UtcNow;

    private HistorialEstado()
    {
    }

    public HistorialEstado(Guid pedido, EstadoPedido estado, string usuario)
    {
        PedidoId = pedido;
        Estado = estado;
        Usuario = usuario;
    }
}
