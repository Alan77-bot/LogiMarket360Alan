using LogiMarket.Domain.Pedidos;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LogiMarket.Application.Reportes;
// Responsabilidad de TotalExpresion: colaborar desde LogiMarket.Application.Reportes sin mezclar reglas, persistencia y pantallas.
internal sealed record TotalExpresion(decimal Minimo) : IExpresionFiltro
{
    public bool Evaluar(Pedido p) => p.Total >= Minimo;
}
