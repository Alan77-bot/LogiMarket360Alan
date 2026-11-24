namespace LogiMarket.Domain.Pedidos;
// Responsabilidad de InstantaneaHijo: colaborar desde LogiMarket.Domain.Pedidos sin mezclar reglas, persistencia y pantallas.
public sealed record InstantaneaHijo(int Cantidad, InstantaneaElemento Elemento);
