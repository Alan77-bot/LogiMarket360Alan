using LogiMarket.Application.Contratos;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LogiMarket.Tests.Soporte;

public sealed class RelojPrueba : IReloj
{
    public DateTimeOffset Ahora { get; set; } = DateTimeOffset.UtcNow;
}
