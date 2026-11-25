using LogiMarket.Domain.Catalogo;
using LogiMarket.Domain.Entidades;

namespace LogiMarket.Application.Catalogo;
// Colabora con la composicion del catalogo manteniendo los datos de las filas separados de las reglas de venta
public sealed class CatalogoCompuesto
{
    public static IReadOnlyDictionary<string, IElementoCatalogo> Construir(List<Producto> productos, List<Kit> kits)
    {
        var resultado = productos.ToDictionary(p => p.Sku, p => (IElementoCatalogo)new ProductoSimple(p.Sku, p.Nombre, p.Precio, p.Peso));
        var visitando = new HashSet<Guid>();
        IElementoCatalogo ConstruirKit(Kit kit)
        {
            if (resultado.TryGetValue(kit.Sku, out var listo))
                return listo;
            if (!visitando.Add(kit.Id))
                throw new InvalidOperationException("Referencia circular en kit.");
            var compuesto = new KitCompuesto(kit.Sku);
            foreach (var c in kit.Componentes.OrderBy(c => c.ProductoId is Guid productoId ? productos.Single(p => p.Id == productoId).Sku : kits.Single(k => k.Id == c.KitHijoId).Sku, StringComparer.Ordinal))
                compuesto.Agregar(c.ProductoId is Guid id ? resultado[productos.Single(p => p.Id == id).Sku] : ConstruirKit(kits.Single(k => k.Id == c.KitHijoId)), c.Cantidad);
            if (compuesto.Componentes.Count == 0)
                throw new InvalidOperationException("Kit sin componentes.");
            visitando.Remove(kit.Id);
            resultado.Add(kit.Sku, compuesto);
            return compuesto;
        }

        foreach (var kit in kits)
            ConstruirKit(kit);
        return resultado;
    }
}
