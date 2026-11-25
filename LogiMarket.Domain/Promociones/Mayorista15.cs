namespace LogiMarket.Domain.Promociones;
// Strategy para diez o mas unidades fisicas Su prioridad supera VIP y los descuentos no se acumulan
public sealed class Mayorista15 : IPoliticaDescuento
{
    public int Prioridad => 20;
    public decimal Tasa => 0.15m;

    public bool Aplica(ContextoPromocion contexto) => contexto.UnidadesFisicas >= 10;
}
