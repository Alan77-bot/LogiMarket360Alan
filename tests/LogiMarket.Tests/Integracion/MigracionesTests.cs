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
// Escenarios de MigracionesTests con datos controlados. Comprobamos el efecto y los cambios que no deben ocurrir.
public sealed class MigracionesTests
{
    private static async Task SinEfectos(Escenario e, Guid id) => await e.Usar(async s =>
    {
        var repo = s.GetRequiredService<IRepositorio>();
        Assert.Equal(8, (await repo.Productos()).Single(p => p.Sku == "P01").Stock);
        Assert.Null(await repo.PagoPorPedido(id));
        Assert.Empty(await s.GetRequiredService<IConsultasOperativas>().Eventos());
        Assert.Empty(await s.GetRequiredService<IConsultasOperativas>().Movimientos());
    });
    [Fact]
    public async Task P01_MigracionesYSeedIdempotente()
    {
        await using var e = new Escenario();
        await e.Iniciar();
        await e.Usar(s => s.GetRequiredService<Inicializador>().Ejecutar());
        await e.Usar(async s =>
        {
            var db = s.GetRequiredService<LogiMarketDbContext>();
            Assert.Equal(5, await db.Productos.CountAsync());
            Assert.Equal(2, await db.Kits.CountAsync());
            Assert.Equal(4, await db.Usuarios.CountAsync());
            Assert.NotEmpty(await db.Database.GetAppliedMigrationsAsync());
            Assert.Empty(await db.Database.GetPendingMigrationsAsync());
        });
    }
}
