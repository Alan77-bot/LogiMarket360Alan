namespace LogiMarket.Domain.Operaciones;
// Solo representa un pago aprobado. Pedido, clave y referencia tendran restricciones unicas en SQLite.
public sealed class Pago
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid PedidoId { get; private set; }
    public string Clave { get; private set; } = "";
    public string Referencia { get; private set; } = "";
    public decimal Importe { get; private set; }
    public DateTimeOffset Fecha { get; private set; } = DateTimeOffset.UtcNow;

    private Pago()
    {
    }

    public Pago(Guid pedido, string clave, string referencia, decimal importe)
    {
        PedidoId = pedido;
        Clave = clave;
        Referencia = referencia;
        Importe = importe;
    }
}
