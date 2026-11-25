namespace LogiMarket.Domain.Promociones;
// Strategy de respaldo una venta normal sigue el mismo contrato y utiliza tasa cero
public sealed class SinDescuento : IPoliticaDescuento
{
    public int Prioridad => 0;
    public decimal Tasa => 0;

    public bool Aplica(ContextoPromocion contexto) => true;
}
