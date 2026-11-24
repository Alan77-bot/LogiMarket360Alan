namespace LogiMarket.Domain.Entidades;
// El producto protege precio, peso y stock. Cambiar existencias siempre pasa por Mover.
public sealed class Producto
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Sku { get; private set; } = "";
    public string Nombre { get; private set; } = "";
    public decimal Precio { get; private set; }
    public decimal Peso { get; private set; }
    public int Stock { get; private set; }
    public Guid Version { get; private set; } = Guid.NewGuid();

    private Producto()
    {
    }

    public Producto(string sku, string nombre, decimal precio, decimal peso, int stock)
    {
        Editar(sku, nombre, precio, peso);
        if (stock < 0)
            throw new ArgumentException("Stock invalido.");
        Stock = stock;
    }

    public void Editar(string sku, string nombre, decimal precio, decimal peso)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);
        if (precio <= 0 || decimal.Round(precio, 2) != precio || peso < 0)
            throw new ArgumentException("Precio positivo con dos decimales y peso no negativo requeridos.");
        Sku = sku.Trim().ToUpperInvariant();
        Nombre = nombre.Trim();
        Precio = precio;
        Peso = peso;
        Version = Guid.NewGuid();
    }

    public void Mover(int cantidad)
    {
        if (cantidad == 0 || (long)Stock + cantidad < 0 || (long)Stock + cantidad > int.MaxValue)
            throw new InvalidOperationException($"Stock insuficiente o movimiento invalido: {Sku}.");
        Stock = checked(Stock + cantidad);
        Version = Guid.NewGuid();
    }
}
