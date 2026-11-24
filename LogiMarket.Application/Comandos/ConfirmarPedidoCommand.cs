using LogiMarket.Application.Pedidos;

namespace LogiMarket.Application.Comandos;
// Command: conserva la entrada y delega la operacion en su receptor, sin duplicar reglas de negocio.
public sealed record ConfirmarPedidoCommand(ConfirmacionPedidoFacade Receptor, Guid Id, string Clave) : IComando
{
    public string Entrada => $"{Id} {Clave}";

    public async Task Ejecutar() => await Receptor.Confirmar(Id, Clave);
}
