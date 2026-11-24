using LogiMarket.Domain.Pedidos;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LogiMarket.Application.Reportes;
// Responsabilidad de Disyuncion: colaborar desde LogiMarket.Application.Reportes sin mezclar reglas, persistencia y pantallas.
internal sealed record Disyuncion(IExpresionFiltro Izquierda, IExpresionFiltro Derecha) : IExpresionFiltro
{
    public bool Evaluar(Pedido p) => Izquierda.Evaluar(p) || Derecha.Evaluar(p);
}
