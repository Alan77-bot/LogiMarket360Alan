namespace LogiMarket.Domain.Enums;
// State este participante declara las transiciones que se permiten desde su estado
public enum EstadoPedido
{
    BORRADOR,
    VALIDADO,
    PAGADO,
    DESPACHADO,
    ENTREGADO,
    CANCELADO
}
