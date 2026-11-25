using LogiMarket.Application.Contratos;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LogiMarket.Tests.Soporte;
// Responsabilidad de RelojPrueba: colaborar desde LogiMarket.Tests.Soporte sin mezclar reglas, persistencia y pantallas.
public sealed class RelojPrueba : IReloj
{
    public DateTimeOffset Ahora { get; set; } = DateTimeOffset.UtcNow;
}
