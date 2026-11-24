using LogiMarket.Domain.Enums;

namespace LogiMarket.Domain.Estados;
// State: este participante declara las transiciones que se permiten desde su estado.
public sealed class EstadoDespachado() : EstadoBase(EstadoPedido.ENTREGADO);
