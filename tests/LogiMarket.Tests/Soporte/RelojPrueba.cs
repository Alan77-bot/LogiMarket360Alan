using LogiMarket.Application.Contratos;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LogiMarket.Tests.Soporte;
// Coordina esta responsabilidad a traves de contratos; las reglas del negocio permanecen en su capa.
public sealed class RelojPrueba : IReloj
{
    public DateTimeOffset Ahora { get; set; } = DateTimeOffset.UtcNow;
}
