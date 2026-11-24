using LogiMarket.Domain.Enums;

namespace LogiMarket.Domain.Entregas;
// Abstract Factory EXPRES: USD 12, un dia y etiqueta de la misma familia.
public sealed class EntregaExpresFactory : IFabricaEntrega
{
    public TipoEntrega Tipo => TipoEntrega.EXPRES;

    public ITarifaEntrega CrearTarifa() => new Tarifa(12m);
    public IPlazoEntrega CrearPlazo() => new Plazo(1);
    public IEtiquetaEntrega CrearEtiqueta() => new Etiqueta("EXP");
}
