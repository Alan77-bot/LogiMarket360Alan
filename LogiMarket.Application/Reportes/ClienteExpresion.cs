using LogiMarket.Domain.Pedidos;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LogiMarket.Application.Reportes;
// Interpreter: este nodo representa una parte del filtro y se combina con las demas expresiones.
internal sealed record ClienteExpresion(string Nombre) : IExpresionFiltro
{
    public bool Evaluar(Pedido p) => p.ClienteNombre.Equals(Nombre, StringComparison.OrdinalIgnoreCase);
}
