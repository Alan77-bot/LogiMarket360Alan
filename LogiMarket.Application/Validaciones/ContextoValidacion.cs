using LogiMarket.Domain.Entidades;
using LogiMarket.Domain.Pedidos;

namespace LogiMarket.Application.Validaciones;

public sealed record ContextoValidacion(Pedido Pedido, Cliente? Cliente, IReadOnlyList<Producto> Productos);
