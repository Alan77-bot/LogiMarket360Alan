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

namespace LogiMarket.Tests.Aceptacion;
// Escenarios de CasosOficialesTests con datos controlados Comprobamos el efecto y los cambios que no deben ocurrir
public sealed class CasosOficialesTests
{
    [Theory]
    [InlineData("Ana", TipoEntrega.ESTANDAR, "KIT_TELETRABAJO", 1, "", 0, 59, "P01", 7)]
    [InlineData("Luis", TipoEntrega.EXPRES, "P04", 2, "P05", 2, 102, "P04", 2)]
    [InlineData("Carmen", TipoEntrega.ESTANDAR, "P03", 10, "", 0, 90, "P03", 10)]
    public async Task P02_P04_CasosOficiales(string cliente, TipoEntrega entrega, string sku, int cantidad, string otro, int otraCantidad, decimal total, string stockSku, int stock)
    {
        await using var e = new Escenario();
        await e.Iniciar();
        var lineas = new List<LineaEntrada>
        {
            new(sku, cantidad)
        };
        if (otro != "")
            lineas.Add(new(otro, otraCantidad));
        var id = await e.Borrador(cliente, entrega, lineas.ToArray());
        await e.Usar(s => s.GetRequiredService<ServicioPedidos>().Validar(id));
        await e.Usar(async s => await s.GetRequiredService<ConfirmacionPedidoFacade>().Confirmar(id, "oficial"));
        Assert.Equal(total, (await e.Pedido(id)).Total);
        Assert.Equal(EstadoPedido.PAGADO, (await e.Pedido(id)).Estado);
        await e.Usar(async s =>
        {
            var repo = s.GetRequiredService<IRepositorio>();
            Assert.Equal(stock, (await repo.Productos()).Single(p => p.Sku == stockSku).Stock);
            Assert.NotNull(await repo.PagoPorPedido(id));
            if (cliente == "Ana")
            {
                Assert.Equal(4, (await repo.Productos()).Single(p => p.Sku == "P02").Stock);
                Assert.Equal(19, (await repo.Productos()).Single(p => p.Sku == "P03").Stock);
            }

            if (cliente == "Luis")
                Assert.Equal(28, (await repo.Productos()).Single(p => p.Sku == "P05").Stock);
            if (cliente == "Carmen")
                Assert.Equal(15, (await repo.Pedido(id)).Descuento);
        });
    }
}
