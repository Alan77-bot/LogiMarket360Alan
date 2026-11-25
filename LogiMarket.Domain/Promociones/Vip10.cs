namespace LogiMarket.Domain.Promociones;
// Strategy para clientes VIP cuando no aplica una promocion con mayor prioridad
public sealed class Vip10 : IPoliticaDescuento
{
    public int Prioridad => 10;
    public decimal Tasa => 0.10m;

    public bool Aplica(ContextoPromocion contexto) => contexto.EsVip;
}
