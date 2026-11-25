namespace LogiMarket.Domain.Operaciones;
// Se guarda junto con la venta para recuperar las reacciones pendientes sin cobrar otra vez
public sealed class EventoOutbox
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid PedidoId { get; private set; }
    public string Tipo { get; private set; } = "";
    public DateTimeOffset Fecha { get; private set; } = DateTimeOffset.UtcNow;
    public DateTimeOffset? ProcesadoEn { get; private set; }
    public int Intentos { get; private set; }
    public string? Error { get; private set; }

    private EventoOutbox()
    {
    }

    public EventoOutbox(Guid pedido, string tipo)
    {
        PedidoId = pedido;
        Tipo = tipo;
    }

    public void Procesado()
    {
        ProcesadoEn = DateTimeOffset.UtcNow;
        Error = null;
    }

    public void Fallido(string error)
    {
        Intentos++;
        Error = error;
    }
}
