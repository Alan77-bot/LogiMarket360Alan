using LogiMarket.Domain.Enums;

namespace LogiMarket.Domain.Estados;
// Patron State: cada estado declara sus destinos permitidos; el formulario no escribe el estado directamente.
public abstract class EstadoBase(params EstadoPedido[] siguientes)
{
    public void Exigir(EstadoPedido destino)
    {
        if (!siguientes.Contains(destino))
            throw new InvalidOperationException($"Transicion no permitida hacia {destino}.");
    }
}
