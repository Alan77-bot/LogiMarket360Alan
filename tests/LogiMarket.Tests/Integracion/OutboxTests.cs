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
// Escenarios de OutboxTests con datos controlados. Comprobamos el efecto y los cambios que no deben ocurrir.
public sealed class OutboxTests
{
    [Theory]
    [InlineData("EMAIL")]
    [InlineData("SMS")]
    public async Task P11_P30_ObserverYFactoryPublicanUnaVez(string canal)
    {
        await using var e = new Escenario();
        await e.Iniciar();
        e.Proveedor.GetRequiredService<OpcionesDemostracion>().Canal = canal;
        await e.Pagar();
        await e.Usar(async s => Assert.Equal(1, await s.GetRequiredService<IProcesadorOutbox>().Procesar()));
        await e.Usar(async s => Assert.Equal(0, await s.GetRequiredService<IProcesadorOutbox>().Procesar()));
        Assert.Equal(1, e.Proveedor.GetRequiredService<MetricasSesion>().Pagos);
        await e.Usar(async s =>
        {
            var consultas = s.GetRequiredService<IConsultasOperativas>();
            Assert.Equal(canal, Assert.Single(await consultas.Avisos()).Canal);
            Assert.Single((await consultas.Auditorias()).Where(a => a.Operacion == "PedidoPagado"));
        });
    }

    [Fact]
    public async Task P37_RecuperacionOutboxConNuevoProcesador()
    {
        await using var e = new Escenario();
        await e.Iniciar();
        await e.Pagar();
        var opciones = e.Proveedor.GetRequiredService<OpcionesDemostracion>();
        opciones.FallarNotificacion = true;
        await e.Usar(async s => Assert.Equal(0, await s.GetRequiredService<IProcesadorOutbox>().Procesar()));
        await e.Usar(async s =>
        {
            var evento = Assert.Single(await s.GetRequiredService<IConsultasOperativas>().Eventos());
            Assert.Null(evento.ProcesadoEn);
            Assert.Equal(1, evento.Intentos);
        });
        opciones.FallarNotificacion = false;
        await e.Usar(async s => Assert.Equal(1, await s.GetRequiredService<IProcesadorOutbox>().Procesar()));
        await e.Usar(async s =>
        {
            Assert.Single(await s.GetRequiredService<IConsultasOperativas>().Avisos());
            Assert.Equal(1, await s.GetRequiredService<LogiMarketDbContext>().Set<Pago>().CountAsync());
        });
    }
}
