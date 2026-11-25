namespace LogiMarket.Domain.Pedidos;
// Coordina esta responsabilidad a traves de contratos; las reglas del negocio permanecen en su capa.
public sealed record InstantaneaHijo(int Cantidad, InstantaneaElemento Elemento);
