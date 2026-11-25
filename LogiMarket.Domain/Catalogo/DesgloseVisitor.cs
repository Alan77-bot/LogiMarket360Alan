using System.Collections;

namespace LogiMarket.Domain.Catalogo;
// Patron Visitor agrupa las cantidades finales por SKU para consultar y verificar la composicion
public sealed class DesgloseVisitor : IVisitanteCatalogo
{
    private readonly SortedDictionary<string, int> cantidades = new(StringComparer.Ordinal);
    public IReadOnlyDictionary<string, int> Cantidades => cantidades;

    public void Visitar(ProductoSimple producto, int cantidad) => cantidades[producto.Sku] = cantidades.GetValueOrDefault(producto.Sku) + cantidad;
    public void Visitar(KitCompuesto kit, int cantidad)
    {
        foreach (var hijo in kit.Componentes)
            hijo.Elemento.Aceptar(this, checked(cantidad * hijo.Cantidad));
    }
}
