using LogiMarket.Domain.Enums;

namespace LogiMarket.Domain.Estados;
// State: conserva las instancias de cada estado y permite seleccionar su comportamiento.
public static class CatalogoEstados
{
    private static readonly IReadOnlyDictionary<EstadoPedido, EstadoBase> mapa = new Dictionary<EstadoPedido, EstadoBase>
    {
        [EstadoPedido.BORRADOR] = new EstadoBorrador(),
        [EstadoPedido.VALIDADO] = new EstadoValidado(),
        [EstadoPedido.PAGADO] = new EstadoPagado(),
        [EstadoPedido.DESPACHADO] = new EstadoDespachado(),
        [EstadoPedido.ENTREGADO] = new EstadoEntregado(),
        [EstadoPedido.CANCELADO] = new EstadoCancelado()
    };
    public static EstadoBase Obtener(EstadoPedido estado) => mapa[estado];
}
