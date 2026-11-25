using LogiMarket.Application.Contratos;
using LogiMarket.Application.Eventos;
using LogiMarket.Domain.Enums;
using LogiMarket.Domain.Pedidos;
using LogiMarket.Tests.Soporte;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using LogiMarket.Infrastructure.InyeccionDependencias;

namespace LogiMarket.Tests.Patrones;
// Escenarios de MediatorTests con datos controlados. Comprobamos el efecto y los cambios que no deben ocurrir.
public sealed class MediatorTests
{
    [Fact]
    public async Task P28_MediatorRefrescaStockPedidosYMetricas()
    {
        await using var e = new Escenario();
        await e.Iniciar();
        var mediador = new MediadorVistas();
        int stock = 0, pagos = 0;
        EstadoPedido? estado = null;
        Guid id = default;
        mediador.Registrar(async () => stock = await e.Usar(async s => (await s.GetRequiredService<IRepositorio>().Productos()).Single(p => p.Sku == "P01").Stock));
        mediador.Registrar(async () => estado = (await e.Pedido(id)).Estado);
        mediador.Registrar(() =>
        {
            pagos = e.Proveedor.GetRequiredService<MetricasSesion>().Pagos;
            return Task.CompletedTask;
        });
        id = await e.Pagar();
        await e.Usar(async s => await s.GetRequiredService<IProcesadorOutbox>().Procesar());
        await mediador.Actualizar();
        Assert.Equal(7, stock);
        Assert.Equal(EstadoPedido.PAGADO, estado);
        Assert.Equal(1, pagos);
    }
}
