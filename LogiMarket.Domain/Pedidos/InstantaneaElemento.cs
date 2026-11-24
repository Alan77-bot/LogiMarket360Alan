using LogiMarket.Domain.Catalogo;

namespace LogiMarket.Domain.Pedidos;
// Responsabilidad de InstantaneaElemento: colaborar desde LogiMarket.Domain.Pedidos sin mezclar reglas, persistencia y pantallas.
// La instantanea conserva tambien el arbol anidado, ademas del desglose relacional.
public sealed record InstantaneaElemento(string Sku, decimal Precio, decimal Peso, IReadOnlyList<InstantaneaHijo> Hijos)
{
    public static InstantaneaElemento Capturar(IElementoCatalogo elemento) => elemento is ProductoSimple producto ? new(producto.Sku, producto.Precio, producto.Peso, Array.Empty<InstantaneaHijo>()) : new(elemento.Sku, elemento.Precio, 0, ((KitCompuesto)elemento).Componentes.Select(c => new InstantaneaHijo(c.Cantidad, Capturar(c.Elemento))).ToArray());
}
