namespace LogiMarket.Domain.Operaciones;
// Registro trazable del negocio; las restricciones de persistencia completan su consistencia.
public sealed class Despacho
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid PedidoId { get; private set; }
    public string Guia { get; private set; } = "";
    public DateTimeOffset Fecha { get; private set; }
    public DateTimeOffset Estimada { get; private set; }

    private Despacho()
    {
    }

    public Despacho(Guid pedido, string guia, DateTimeOffset fecha, DateTimeOffset estimada)
    {
        PedidoId = pedido;
        Guia = guia;
        Fecha = fecha;
        Estimada = estimada;
    }
}
