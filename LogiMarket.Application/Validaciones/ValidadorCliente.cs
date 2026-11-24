using LogiMarket.Domain.Entidades;

namespace LogiMarket.Application.Validaciones;
// Chain of Responsibility: esta regla devuelve el primer error y deja continuar solo si se cumple.
public sealed class ValidadorCliente : ValidadorPedido
{
    protected override ResultadoValidacion Revisar(ContextoValidacion c) => c.Cliente is { Activo: true } ? ResultadoValidacion.Correcto : new(false, "CLIENTE", "Cliente inexistente o inactivo.");
}
