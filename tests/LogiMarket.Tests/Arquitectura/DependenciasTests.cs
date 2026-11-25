using LogiMarket.Application.Catalogo;
using LogiMarket.Domain.Entidades;
using LogiMarket.Infrastructure.Persistencia;
using Xunit;
using LogiMarket.Infrastructure.InyeccionDependencias;

namespace LogiMarket.Tests.Arquitectura;
// Escenarios de DependenciasTests con datos controlados. Comprobamos el efecto y los cambios que no deben ocurrir.
public sealed class DependenciasTests
{
    [Fact]
    public void P38_DependenciasInternasSinFrameworksDePersistenciaNiUi()
    {
        var dominio = typeof(Producto).Assembly.GetReferencedAssemblies().Select(a => a.Name!).ToArray();
        Assert.DoesNotContain(dominio, n => n.StartsWith("LogiMarket.") || n.Contains("EntityFramework") || n.Contains("Windows.Forms"));
        var aplicacion = typeof(ServicioCatalogo).Assembly.GetReferencedAssemblies().Select(a => a.Name!).ToArray();
        Assert.DoesNotContain(aplicacion, n => n.Contains("Infrastructure") || n.Contains("EntityFramework") || n.Contains("Windows.Forms"));
        Assert.Contains("LogiMarket.Domain", aplicacion);
        Assert.Contains("LogiMarket.Application", typeof(LogiMarketDbContext).Assembly.GetReferencedAssemblies().Select(a => a.Name));
    }
}
