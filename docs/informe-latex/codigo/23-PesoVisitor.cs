using System.Collections;

namespace LogiMarket.Domain.Catalogo;
// Patron Visitor acumula el peso de los componentes respetando las cantidades de cada kit
public sealed class PesoVisitor : IVisitanteCatalogo
{
    public decimal Peso { get; private set; }

    public void Visitar(ProductoSimple producto, int cantidad) => Peso += producto.Peso * cantidad;
    public void Visitar(KitCompuesto kit, int cantidad)
    {
        foreach (var hijo in kit.Componentes)
            hijo.Elemento.Aceptar(this, checked(cantidad * hijo.Cantidad));
    }
}
