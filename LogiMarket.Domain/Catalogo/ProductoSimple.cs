namespace LogiMarket.Domain.Catalogo;
// Composite, participante hoja. Aporta un producto fisico al recorrido sin conocer sus kits padres.
public sealed class ProductoSimple : IElementoCatalogo
{
    public void Aceptar(IVisitanteCatalogo visitante, int cantidad = 1) => visitante.Visitar(this, cantidad);
    public string Sku { get; }
    public string Nombre { get; }
    public decimal Precio { get; }
    public decimal Peso { get; }

    public ProductoSimple(string sku, string nombre, decimal precio, decimal peso)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);
        if (precio <= 0 || peso < 0)
            throw new ArgumentException("Precio o peso invalido.");
        Sku = sku;
        Nombre = nombre;
        Precio = precio;
        Peso = peso;
    }

    public IEnumerable<ComponenteFisico> Expandir(int cantidad = 1)
    {
        if (cantidad <= 0)
            throw new ArgumentOutOfRangeException(nameof(cantidad));
        yield return new(Sku, cantidad, Precio, Peso);
    }
}
