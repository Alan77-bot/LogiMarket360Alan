using System.Collections;

namespace LogiMarket.Domain.Catalogo;
// Patron Visitor permite agregar operaciones de reporte sin llenarlas de condiciones dentro del catalogo
public interface IVisitanteCatalogo
{
    void Visitar(ProductoSimple producto, int cantidad);
    void Visitar(KitCompuesto kit, int cantidad);
}
