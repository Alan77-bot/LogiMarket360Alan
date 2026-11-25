using LogiMarket.Infrastructure.InyeccionDependencias;
using LogiMarket.Application.Catalogo;
using LogiMarket.Application.Comandos;
using LogiMarket.Application.Contratos;
using LogiMarket.Application.DTOs;
using LogiMarket.Application.Eventos;
using LogiMarket.Application.Logistica;
using LogiMarket.Application.Pedidos;
using LogiMarket.Domain.Enums;
using LogiMarket.Domain.Operaciones;
using LogiMarket.Domain.Pedidos;
using LogiMarket.Infrastructure.Persistencia;
using LogiMarket.Tests.Soporte;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LogiMarket.Tests.Integracion;
// Escenarios de PersistenciaTests con datos controlados Comprobamos el efecto y los cambios que no deben ocurrir
public sealed class PersistenciaTests
{
    [Fact]
    public async Task P10_PersistenciaConOtroProveedorDi()
    {
        await using var e = new Escenario();
        await e.Iniciar();
        var id = await e.Pagar();
        using var otro = new ServiceCollection().AgregarLogiMarket(e.Ruta).BuildServiceProvider();
        using var scope = otro.CreateScope();
        var repo = scope.ServiceProvider.GetRequiredService<IRepositorio>();
        Assert.Equal(EstadoPedido.PAGADO, (await repo.Pedido(id)).Estado);
        Assert.NotNull(await repo.PagoPorPedido(id));
        Assert.Equal(7, (await repo.Productos()).Single(p => p.Sku == "P01").Stock);
        Assert.NotEmpty(await scope.ServiceProvider.GetRequiredService<IConsultasOperativas>().Auditorias());
    }
}
