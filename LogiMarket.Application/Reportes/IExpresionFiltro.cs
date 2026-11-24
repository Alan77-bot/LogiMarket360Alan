using LogiMarket.Domain.Pedidos;
using System.Globalization;
using System.Text.RegularExpressions;

namespace LogiMarket.Application.Reportes;
// Interpreter: cada nodo del filtro sabe evaluar un pedido sin conocer la base de datos.
public interface IExpresionFiltro
{
    bool Evaluar(Pedido pedido);
}
