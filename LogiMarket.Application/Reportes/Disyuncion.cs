using LogiMarket.Domain.Pedidos;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LogiMarket.Application.Reportes;
// Interpreter este nodo representa una parte del filtro y se combina con las demas expresiones
internal sealed record Disyuncion(IExpresionFiltro Izquierda, IExpresionFiltro Derecha) : IExpresionFiltro
{
    public bool Evaluar(Pedido p) => Izquierda.Evaluar(p) || Derecha.Evaluar(p);
}
