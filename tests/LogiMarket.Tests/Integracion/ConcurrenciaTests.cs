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
// Escenarios de ConcurrenciaTests con datos controlados. Comprobamos el efecto y los cambios que no deben ocurrir.
public sealed class ConcurrenciaTests
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
    public async Task P36_DosConfirmacionesConcurrentesSinSobreventa()
    {
        await using var e = new Escenario();
        await e.Iniciar();
        await e.Usar(async s =>
        {
            var repo = s.GetRequiredService<IRepositorio>();
            (await repo.Productos()).Single(p => p.Sku == "P01").Mover(-7);
            await repo.Guardar();
        });
        var a = await e.Borrador("Ana", TipoEntrega.ESTANDAR, new LineaEntrada("P01", 1));
        var b = await e.Borrador("Luis", TipoEntrega.ESTANDAR, new LineaEntrada("P01", 1));
        await e.Usar(s => s.GetRequiredService<ServicioPedidos>().Validar(a));
        await e.Usar(s => s.GetRequiredService<ServicioPedidos>().Validar(b));
        async Task<bool> Cobrar(Guid id)
        {
            try
            {
                await e.Usar(s => s.GetRequiredService<ConfirmacionPedidoFacade>().Confirmar(id, id.ToString()));
                return true;
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }

        var resultados = await Task.WhenAll(Task.Run(() => Cobrar(a)), Task.Run(() => Cobrar(b)));
        Assert.Single(resultados.Where(r => r));
        await e.Usar(async s =>
        {
            Assert.Equal(0, (await s.GetRequiredService<IRepositorio>().Productos()).Single(p => p.Sku == "P01").Stock);
            Assert.Equal(1, await s.GetRequiredService<LogiMarketDbContext>().Set<Pago>().CountAsync());
        });
    }
}
