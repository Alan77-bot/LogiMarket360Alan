using LogiMarket.Domain.Enums;
using LogiMarket.Domain.Pedidos;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LogiMarket.Application.Reportes;
// State: este participante declara las transiciones que se permiten desde su estado.
internal sealed record EstadoExpresion(EstadoPedido Estado) : IExpresionFiltro
{
    public bool Evaluar(Pedido p) => p.Estado == Estado;
}
