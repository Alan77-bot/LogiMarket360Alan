using LogiMarket.Domain.Pedidos;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LogiMarket.Application.Reportes;
// Responsabilidad de Conjuncion: colaborar desde LogiMarket.Application.Reportes sin mezclar reglas, persistencia y pantallas.
internal sealed record Conjuncion(IExpresionFiltro Izquierda, IExpresionFiltro Derecha) : IExpresionFiltro
{
    public bool Evaluar(Pedido p) => Izquierda.Evaluar(p) && Derecha.Evaluar(p);
}
