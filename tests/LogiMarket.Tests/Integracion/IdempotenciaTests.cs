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
// Escenarios de IdempotenciaTests con datos controlados. Comprobamos el efecto y los cambios que no deben ocurrir.
public sealed class IdempotenciaTests
{
    [Fact]
    public async Task P07_ConfirmacionIdempotente()
    {
        await using var e = new Escenario();
        await e.Iniciar();
        var id = await e.Pagar();
        var pago = await e.Usar(s => s.GetRequiredService<ConfirmacionPedidoFacade>().Confirmar(id, id.ToString()));
        var repetido = await e.Usar(s => s.GetRequiredService<ConfirmacionPedidoFacade>().Confirmar(id, id.ToString()));
        Assert.Equal(pago.Id, repetido.Id);
        await e.Usar(async s =>
        {
            var db = s.GetRequiredService<LogiMarketDbContext>();
            Assert.Equal(1, await db.Set<Pago>().CountAsync());
            Assert.Equal(3, await db.Set<MovimientoInventario>().CountAsync());
        });
    }
}
