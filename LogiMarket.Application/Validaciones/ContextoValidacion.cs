using LogiMarket.Domain.Entidades;
using LogiMarket.Domain.Pedidos;

namespace LogiMarket.Application.Validaciones;
// Coordina esta responsabilidad a traves de contratos; las reglas del negocio permanecen en su capa.
public sealed record ContextoValidacion(Pedido Pedido, Cliente? Cliente, IReadOnlyList<Producto> Productos);
