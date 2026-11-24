namespace LogiMarket.Domain.Promociones;
// Responsabilidad de ContextoPromocion: colaborar desde LogiMarket.Domain.Promociones sin mezclar reglas, persistencia y pantallas.
public sealed record ContextoPromocion(bool EsVip, int UnidadesFisicas);
