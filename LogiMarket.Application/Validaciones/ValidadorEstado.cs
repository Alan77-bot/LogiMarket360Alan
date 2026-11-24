using LogiMarket.Domain.Enums;
using LogiMarket.Domain.Pedidos;

namespace LogiMarket.Application.Validaciones;
// Chain of Responsibility: esta regla devuelve el primer error y deja continuar solo si se cumple.
public sealed class ValidadorEstado : ValidadorPedido
{
    protected override ResultadoValidacion Revisar(ContextoValidacion c) => c.Pedido.Estado == EstadoPedido.VALIDADO ? ResultadoValidacion.Correcto : new(false, "ESTADO", "Valide el pedido antes de cobrar.");
}
