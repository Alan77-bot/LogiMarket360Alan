using LogiMarket.Domain.Enums;

namespace LogiMarket.Domain.Operaciones;
// Registro trazable del negocio; las restricciones de persistencia completan su consistencia.
public sealed class Devolucion
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid PedidoId { get; private set; }
    public MotivoDevolucion Motivo { get; private set; }
    public decimal Reembolso { get; private set; }
    public string Referencia { get; private set; } = "";
    public DateTimeOffset Fecha { get; private set; }

    private Devolucion()
    {
    }

    public Devolucion(Guid pedido, MotivoDevolucion motivo, decimal reembolso, string referencia, DateTimeOffset fecha)
    {
        PedidoId = pedido;
        Motivo = motivo;
        Reembolso = reembolso;
        Referencia = referencia;
        Fecha = fecha;
    }
}
