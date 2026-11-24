using System.Collections.Concurrent;

namespace LogiMarket.Infrastructure.Pagos;
// Responsabilidad de PasarelaLegadaSimulada: colaborar desde LogiMarket.Infrastructure.Pagos sin mezclar reglas, persistencia y pantallas.
// El legado solo acepta centavos; el adaptador conserva su firma incompatible.
public sealed class PasarelaLegadaSimulada
{
    private readonly ConcurrentDictionary<string, (long Importe, string Referencia)> cobros = new();
    public string CobrarCentavos(long centavos, string clave)
    {
        if (centavos <= 0)
            throw new ArgumentException("Importe invalido.");
        var cobro = cobros.GetOrAdd(clave, _ => (centavos, $"LEG-{Guid.NewGuid():N}"));
        if (cobro.Importe != centavos)
            throw new InvalidOperationException("Clave reutilizada con otro importe.");
        return cobro.Referencia;
    }
}
