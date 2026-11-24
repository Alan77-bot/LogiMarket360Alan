using LogiMarket.Domain.Pedidos;

namespace LogiMarket.Application.Validaciones;
// Chain of Responsibility: esta regla devuelve el primer error y deja continuar solo si se cumple.
public sealed class ValidadorLineas : ValidadorPedido
{
    protected override ResultadoValidacion Revisar(ContextoValidacion c) => c.Pedido.Detalles.Count > 0 && c.Pedido.Detalles.All(d => d.Cantidad > 0 && d.Componentes.Count > 0) ? ResultadoValidacion.Correcto : new(false, "LINEAS", "Lineas incompletas.");
}
