namespace LogiMarket.Domain.Operaciones;
// Una cantidad negativa es salida y una positiva es entrada. La referencia permite rastrear su causa.
public sealed class MovimientoInventario
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid ProductoId { get; private set; }
    public int Cantidad { get; private set; }
    public string Motivo { get; private set; } = "";
    public string Referencia { get; private set; } = "";
    public DateTimeOffset Fecha { get; private set; } = DateTimeOffset.UtcNow;

    private MovimientoInventario()
    {
    }

    public MovimientoInventario(Guid producto, int cantidad, string motivo, string referencia)
    {
        ProductoId = producto;
        Cantidad = cantidad;
        Motivo = motivo;
        Referencia = referencia;
    }
}
