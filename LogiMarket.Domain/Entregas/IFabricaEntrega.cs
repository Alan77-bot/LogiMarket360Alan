using LogiMarket.Domain.Enums;

namespace LogiMarket.Domain.Entregas;
// Patron Abstract Factory: una familia entrega tarifa, plazo y etiqueta coherentes entre si.
// Abstract Factory: una familia produce tarifa, plazo y etiqueta compatibles.
public interface IFabricaEntrega
{
    TipoEntrega Tipo { get; }

    ITarifaEntrega CrearTarifa();
    IPlazoEntrega CrearPlazo();
    IEtiquetaEntrega CrearEtiqueta();
}
