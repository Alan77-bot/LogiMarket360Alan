using LogiMarket.Domain.Enums;

namespace LogiMarket.Domain.Entregas;
// Abstract Factory ESTANDAR USD 5 tres dias y etiqueta de la misma familia
public sealed class EntregaEstandarFactory : IFabricaEntrega
{
    public TipoEntrega Tipo => TipoEntrega.ESTANDAR;

    public ITarifaEntrega CrearTarifa() => new Tarifa(5m);
    public IPlazoEntrega CrearPlazo() => new Plazo(3);
    public IEtiquetaEntrega CrearEtiqueta() => new Etiqueta("STD");
}
