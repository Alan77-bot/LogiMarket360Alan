using LogiMarket.Domain.Pedidos;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LogiMarket.Application.Reportes;
// Responsabilidad de ClienteExpresion: colaborar desde LogiMarket.Application.Reportes sin mezclar reglas, persistencia y pantallas.
internal sealed record ClienteExpresion(string Nombre) : IExpresionFiltro
{
    public bool Evaluar(Pedido p) => p.ClienteNombre.Equals(Nombre, StringComparison.OrdinalIgnoreCase);
}
