using LogiMarket.Application.Pedidos;

namespace LogiMarket.Application.Comandos;
// Command: conserva la entrada y delega la operacion en su receptor, sin duplicar reglas de negocio.
public sealed record CancelarPedidoCommand(ServicioPedidos Receptor, Guid Id) : IComando
{
    public string Entrada => Id.ToString();

    public Task Ejecutar() => Receptor.Cancelar(Id);
}
