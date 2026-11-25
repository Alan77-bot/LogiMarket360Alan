namespace LogiMarket.Domain.Promociones;
// Coordina esta responsabilidad a traves de contratos; las reglas del negocio permanecen en su capa.
public sealed record ContextoPromocion(bool EsVip, int UnidadesFisicas);
