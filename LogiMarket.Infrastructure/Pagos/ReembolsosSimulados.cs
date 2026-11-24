using LogiMarket.Application.Contratos;
using System.Collections.Concurrent;

namespace LogiMarket.Infrastructure.Pagos;
// Responsabilidad de ReembolsosSimulados: colaborar desde LogiMarket.Infrastructure.Pagos sin mezclar reglas, persistencia y pantallas.
public sealed class ReembolsosSimulados : IReembolsosSimulados
{
    private readonly ConcurrentDictionary<Guid, (decimal Importe, string Referencia)> registros = new();
    public Task<string> ReembolsarAsync(Guid pedido, decimal importe)
    {
        if (pedido == Guid.Empty || importe <= 0 || decimal.Round(importe, 2) != importe)
            throw new ArgumentException("Reembolso invalido.");
        var registro = registros.GetOrAdd(pedido, id => (importe, $"REEMBOLSO-{id:N}"));
        if (registro.Importe != importe)
            throw new InvalidOperationException("Reembolso previo con importe distinto.");
        return Task.FromResult(registro.Referencia);
    }
}
