using System.Collections.Concurrent;

namespace LogiMarket.Infrastructure.Pagos;
// Colaborador de pago simulado Sus referencias permiten rastrear la operacion sin integrar cobros reales
// El legado solo acepta centavos el adaptador conserva su firma incompatible
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
