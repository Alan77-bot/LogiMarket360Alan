namespace LogiMarket.Domain.Promociones;
// Patron Strategy seleccionamos una politica no sumamos promociones El dinero usa decimal y AwayFromZero
public sealed class CalculadoraPrecios(IEnumerable<IPoliticaDescuento> politicas)
{
    public Totales Calcular(decimal subtotal, decimal tarifa, ContextoPromocion contexto)
    {
        if (subtotal < 0 || tarifa < 0 || contexto.UnidadesFisicas <= 0)
            throw new ArgumentException("Importes o cantidades invalidos.");
        var politica = politicas.OrderByDescending(p => p.Prioridad).First(p => p.Aplica(contexto));
        var descuento = decimal.Round(subtotal * politica.Tasa, 2, MidpointRounding.AwayFromZero);
        return new(subtotal, descuento, tarifa, subtotal - descuento + tarifa);
    }
}
