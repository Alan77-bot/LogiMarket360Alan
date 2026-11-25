using LogiMarket.Application.Logistica;

namespace LogiMarket.Application.Comandos;
// Command conserva la entrada y delega la operacion en su receptor sin duplicar reglas de negocio
public sealed record DespacharPedidoCommand(ServicioDespachos Receptor, Guid Id) : IComando
{
    public string Entrada => Id.ToString();

    public Task Ejecutar() => Receptor.Despachar(Id);
}
