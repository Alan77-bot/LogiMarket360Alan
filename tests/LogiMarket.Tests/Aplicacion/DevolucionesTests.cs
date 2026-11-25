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
using LogiMarket.Application.Clientes;

namespace LogiMarket.Tests.Aplicacion;
// Escenarios de DevolucionesTests con datos controlados. Comprobamos el efecto y los cambios que no deben ocurrir.
public sealed class DevolucionesTests
{
    private sealed class ReembolsoConFallo : IReembolsosSimulados
    {
        public Task<string> ReembolsarAsync(Guid pedido, decimal importe) => throw new IOException("Reembolso indisponible.");
    }

    [Fact]
    public async Task P35_DevolucionAnaUnicaPor54()
    {
        await using var e = new Escenario();
        await e.Iniciar();
        var id = await e.Pagar();
        await e.Usar(s => s.GetRequiredService<ServicioDespachos>().Despachar(id));
        await e.Usar(s => s.GetRequiredService<ServicioDespachos>().Entregar(id));
        e.Reloj.Ahora = e.Reloj.Ahora.AddDays(20);
        var devolucion = await e.Usar(s => s.GetRequiredService<DevolucionDefecto>().Ejecutar(id, false));
        Assert.Equal(54, devolucion.Reembolso);
        await Assert.ThrowsAsync<InvalidOperationException>(() => e.Usar(s => s.GetRequiredService<DevolucionDefecto>().Ejecutar(id, true)));
        await e.Usar(async s =>
        {
            Assert.Single(await s.GetRequiredService<IConsultasOperativas>().Devoluciones());
            Assert.Equal(8, (await s.GetRequiredService<IRepositorio>().Productos()).Single(p => p.Sku == "P01").Stock);
        });
    }

    [Fact]
    public async Task P33_ArrepentimientoDiaDiezNoAlteraStock()
    {
        await using var e = new Escenario();
        await e.Iniciar();
        var id = await e.Pagar();
        await e.Usar(s => s.GetRequiredService<ServicioDespachos>().Despachar(id));
        await e.Usar(s => s.GetRequiredService<ServicioDespachos>().Entregar(id));
        e.Reloj.Ahora = e.Reloj.Ahora.AddDays(10);
        await Assert.ThrowsAsync<InvalidOperationException>(() => e.Usar(s => s.GetRequiredService<DevolucionArrepentimiento>().Ejecutar(id, true)));
        await e.Usar(async s =>
        {
            Assert.Empty(await s.GetRequiredService<IConsultasOperativas>().Devoluciones());
            Assert.Equal(7, (await s.GetRequiredService<IRepositorio>().Productos()).Single(p => p.Sku == "P01").Stock);
        });
    }

    [Fact]
    public async Task FalloDeReembolsoNoReintegraStock()
    {
        await using var e = new Escenario(s => s.AddSingleton<IReembolsosSimulados, ReembolsoConFallo>());
        await e.Iniciar();
        var id = await e.Pagar();
        await e.Usar(s => s.GetRequiredService<LogiMarket.Application.Logistica.ServicioDespachos>().Despachar(id));
        await e.Usar(s => s.GetRequiredService<LogiMarket.Application.Logistica.ServicioDespachos>().Entregar(id));
        await Assert.ThrowsAsync<IOException>(() => e.Usar(s => s.GetRequiredService<LogiMarket.Application.Logistica.DevolucionDefecto>().Ejecutar(id, true)));
        await e.Usar(async s =>
        {
            Assert.Null(await s.GetRequiredService<IRepositorio>().DevolucionPorPedido(id));
            Assert.Equal(7, (await s.GetRequiredService<IRepositorio>().Productos()).Single(p => p.Sku == "P01").Stock);
            Assert.DoesNotContain(await s.GetRequiredService<IConsultasOperativas>().Eventos(), x => x.Tipo == "DevolucionAceptada");
        });
    }
}
