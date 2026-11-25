using LogiMarket.Domain.Catalogo;

namespace LogiMarket.Domain.Pedidos;
// Coordina esta responsabilidad a traves de contratos; las reglas del negocio permanecen en su capa.
// La instantanea conserva tambien el arbol anidado, ademas del desglose relacional.
public sealed record InstantaneaElemento(string Sku, decimal Precio, decimal Peso, IReadOnlyList<InstantaneaHijo> Hijos)
{
    public static InstantaneaElemento Capturar(IElementoCatalogo elemento) => elemento is ProductoSimple producto ? new(producto.Sku, producto.Precio, producto.Peso, Array.Empty<InstantaneaHijo>()) : new(elemento.Sku, elemento.Precio, 0, ((KitCompuesto)elemento).Componentes.Select(c => new InstantaneaHijo(c.Cantidad, Capturar(c.Elemento))).ToArray());
}
