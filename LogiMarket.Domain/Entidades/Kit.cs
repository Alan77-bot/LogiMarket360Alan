namespace LogiMarket.Domain.Entidades;
// Esta entidad guarda la composicion persistida Composite se construye a partir de ella en CatalogoCompuesto
public sealed class Kit
{
    public Guid Id { get; private set; } = Guid.NewGuid();
    public string Sku { get; private set; } = "";
    public string Nombre { get; private set; } = "";

    private List<KitComponente> componentes = [];
    public IReadOnlyList<KitComponente> Componentes => componentes.AsReadOnly();

    private Kit()
    {
    }

    public Kit(string sku, string nombre)
    {
        Renombrar(sku, nombre);
    }

    public void Renombrar(string sku, string nombre)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        ArgumentException.ThrowIfNullOrWhiteSpace(nombre);
        Sku = sku.Trim().ToUpperInvariant();
        Nombre = nombre.Trim();
    }

    public void Reemplazar(IEnumerable<KitComponente> componentes)
    {
        var nuevos = componentes.ToList();
        if (nuevos.Count == 0)
            throw new ArgumentException("El kit necesita componentes.");
        this.componentes = nuevos;
    }
}
