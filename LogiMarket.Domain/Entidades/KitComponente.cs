namespace LogiMarket.Domain.Entidades;
// Cada componente apunta a un producto o a un kit hijo, nunca a ambos. La cantidad debe ser positiva.
public sealed class KitComponente
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public Guid KitId { get; private set; }
    public Guid? ProductoId { get; private set; }
    public Guid? KitHijoId { get; private set; }
    public int Cantidad { get; private set; }

    private KitComponente()
    {
    }

    public KitComponente(Guid kitId, Guid? productoId, Guid? kitHijoId, int cantidad)
    {
        if ((productoId is null) == (kitHijoId is null) || cantidad <= 0)
            throw new ArgumentException("Componente invalido.");
        KitId = kitId;
        ProductoId = productoId;
        KitHijoId = kitHijoId;
        Cantidad = cantidad;
    }
}
