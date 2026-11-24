using LogiMarket.Domain.Entidades;
using LogiMarket.Domain.Pedidos;

namespace LogiMarket.Application.Validaciones;
// Responsabilidad de ContextoValidacion: colaborar desde LogiMarket.Application.Validaciones sin mezclar reglas, persistencia y pantallas.
public sealed record ContextoValidacion(Pedido Pedido, Cliente? Cliente, IReadOnlyList<Producto> Productos);
