using LogiMarket.Application.Logistica;

namespace LogiMarket.Application.Comandos;
// Command: conserva la entrada y delega la operacion en su receptor, sin duplicar reglas de negocio.
public sealed record ProcesarDevolucionCommand(ProcesarDevolucion Receptor, Guid Id, bool SinUso) : IComando
{
    public string Entrada => $"{Id} sinUso={SinUso}";

    public async Task Ejecutar() => await Receptor.Ejecutar(Id, SinUso);
}
