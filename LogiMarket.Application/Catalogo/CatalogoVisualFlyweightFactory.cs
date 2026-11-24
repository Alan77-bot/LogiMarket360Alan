namespace LogiMarket.Application.Catalogo;
// Patron Flyweight: comparte metadatos por SKU; cantidad y precio de cada fila permanecen fuera del objeto compartido.
public sealed class CatalogoVisualFlyweightFactory
{
    private readonly Dictionary<string, MetadatosSku> cache = [];
    public MetadatosSku Obtener(string sku, string nombre) => cache.TryGetValue(sku, out var valor) ? valor : cache[sku] = new(sku, nombre, "Accesorios", "Catalogo de demostracion");
    public int Instancias => cache.Count;
}
