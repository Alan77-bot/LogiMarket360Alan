namespace LogiMarket.Domain.Catalogo;
// Patron Composite: contrato comun para hojas de producto y kits que contienen otros elementos.
// Composite: productos y kits comparten el mismo contrato.
public interface IElementoCatalogo
{
    string Sku { get; }

    decimal Precio { get; }

    IEnumerable<ComponenteFisico> Expandir(int cantidad = 1);
    void Aceptar(IVisitanteCatalogo visitante, int cantidad = 1);
}
