namespace LogiMarket.Domain.Catalogo;
// Patron Composite: calcula y expande una estructura anidada, rechazando ciclos.
public sealed class KitCompuesto : IElementoCatalogo
{
    public IReadOnlyList<(IElementoCatalogo Elemento, int Cantidad)> Componentes => componentes.AsReadOnly();

    public void Aceptar(IVisitanteCatalogo visitante, int cantidad = 1) => visitante.Visitar(this, cantidad);
    private readonly List<(IElementoCatalogo Elemento, int Cantidad)> componentes = [];
    public string Sku { get; }
    public decimal Precio => componentes.Sum(c => c.Elemento.Precio * c.Cantidad);

    public KitCompuesto(string sku)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(sku);
        Sku = sku;
    }

    public void Agregar(IElementoCatalogo elemento, int cantidad = 1)
    {
        ArgumentNullException.ThrowIfNull(elemento);
        if (cantidad <= 0)
            throw new ArgumentOutOfRangeException(nameof(cantidad));
        // Se comprueba el arbol antes de modificar la composicion.
        if (Contiene(elemento, Sku))
            throw new InvalidOperationException("El kit produciria un ciclo.");
        componentes.Add((elemento, cantidad));
    }

    private static bool Contiene(IElementoCatalogo elemento, string sku) => elemento.Sku == sku || elemento is KitCompuesto kit && kit.componentes.Any(c => Contiene(c.Elemento, sku));
    public IEnumerable<ComponenteFisico> Expandir(int cantidad = 1)
    {
        if (cantidad <= 0)
            throw new ArgumentOutOfRangeException(nameof(cantidad));
        using var iterador = new IteradorFisico(this, cantidad);
        while (iterador.MoveNext())
            yield return iterador.Current;
    }
}
