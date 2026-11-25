namespace LogiMarket.Domain.Promociones;
// Datos del cliente y de las unidades fisicas para elegir la promocion
public sealed record ContextoPromocion(bool EsVip, int UnidadesFisicas);
