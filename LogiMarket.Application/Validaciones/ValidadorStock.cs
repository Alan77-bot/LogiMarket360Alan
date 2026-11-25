using LogiMarket.Domain.Pedidos;

namespace LogiMarket.Application.Validaciones;
// Chain of Responsibility esta regla devuelve el primer error y deja continuar solo si se cumple
public sealed class ValidadorStock : ValidadorPedido
{
    protected override ResultadoValidacion Revisar(ContextoValidacion c)
    {
        foreach (var grupo in c.Pedido.Detalles.SelectMany(d => d.Componentes).GroupBy(p => p.Sku))
            if (c.Productos.SingleOrDefault(p => p.Sku == grupo.Key)?.Stock is not int stock || stock < grupo.Sum(p => p.Cantidad))
                return new(false, "STOCK", $"Stock insuficiente: {grupo.Key}.");
        return ResultadoValidacion.Correcto;
    }
}
