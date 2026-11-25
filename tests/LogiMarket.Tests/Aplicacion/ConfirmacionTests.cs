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
using LogiMarket.Domain.Catalogo;
using LogiMarket.Domain.Entidades;
using LogiMarket.Domain.Promociones;

namespace LogiMarket.Tests.Aplicacion;
// Escenarios de ConfirmacionTests con datos controlados Comprobamos el efecto y los cambios que no deben ocurrir
public sealed class ConfirmacionTests
{
    private static async Task SinEfectos(Escenario e, Guid id) => await e.Usar(async s =>
    {
        var repo = s.GetRequiredService<IRepositorio>();
        Assert.Equal(8, (await repo.Productos()).Single(p => p.Sku == "P01").Stock);
        Assert.Null(await repo.PagoPorPedido(id));
        Assert.Empty(await s.GetRequiredService<IConsultasOperativas>().Eventos());
        Assert.Empty(await s.GetRequiredService<IConsultasOperativas>().Movimientos());
    });
    private sealed class PasarelaConFallo : IPasarelaPago
    {
        public Task<ResultadoCobro> CobrarAsync(decimal importe, string clave) => throw new IOException("Colaborador indisponible.");
    }

    [Fact]
    public async Task P05_KitPorSeisSinEfectos()
    {
        await using var e = new Escenario();
        await e.Iniciar();
        var id = await e.Borrador("Ana", TipoEntrega.ESTANDAR, new LineaEntrada("KIT_TELETRABAJO", 6));
        var error = await Assert.ThrowsAsync<InvalidOperationException>(() => e.Usar(s => s.GetRequiredService<ServicioPedidos>().Validar(id)));
        Assert.Contains("P02", error.Message);
        await SinEfectos(e, id);
    }

    [Fact]
    public async Task P06_RechazoNoDescuentaStock()
    {
        await using var e = new Escenario();
        await e.Iniciar();
        var id = await e.Borrador();
        await e.Usar(s => s.GetRequiredService<ServicioPedidos>().Validar(id));
        e.Proveedor.GetRequiredService<OpcionesDemostracion>().RechazarCobro = true;
        await Assert.ThrowsAsync<InvalidOperationException>(() => e.Usar(async s => await s.GetRequiredService<ConfirmacionPedidoFacade>().Confirmar(id, "rechazo")));
        await SinEfectos(e, id);
        Assert.Equal(EstadoPedido.VALIDADO, (await e.Pedido(id)).Estado);
        await e.Usar(async s =>
        {
            var logs = await s.GetRequiredService<IConsultasOperativas>().Auditorias();
            Assert.Contains(logs, a => a.Operacion == "COBRO" && a.Resultado.StartsWith("ERROR"));
        });
    }

    [Fact]
    public async Task P20_FalloColaboradorNoDejaPersistenciaParcial()
    {
        await using var e = new Escenario(s => s.AddScoped<IPasarelaPago, PasarelaConFallo>());
        await e.Iniciar();
        var id = await e.Borrador();
        await e.Usar(s => s.GetRequiredService<ServicioPedidos>().Validar(id));
        await Assert.ThrowsAsync<IOException>(() => e.Usar(s => s.GetRequiredService<ConfirmacionPedidoFacade>().Confirmar(id, "fallo")));
        await e.Usar(async s =>
        {
            var r = s.GetRequiredService<IRepositorio>();
            Assert.Null(await r.PagoPorPedido(id));
            Assert.Equal(8, (await r.Productos()).Single(p => p.Sku == "P01").Stock);
            Assert.Empty(await s.GetRequiredService<IConsultasOperativas>().Eventos());
            Assert.Equal(EstadoPedido.VALIDADO, (await r.Pedido(id)).Estado);
        });
    }

    [Fact]
    public async Task ClienteInactivoBloqueaValidacion()
    {
        await using var e = new Escenario();
        await e.Iniciar();
        var id = await e.Borrador();
        var p = await e.Pedido(id);
        await e.Usar(s => s.GetRequiredService<ServicioClientes>().Guardar(p.ClienteId, "Ana", CategoriaCliente.VIP, false));
        await Assert.ThrowsAsync<InvalidOperationException>(() => e.Usar(s => s.GetRequiredService<ServicioPedidos>().Validar(id)));
    }

    [Fact]
    public async Task ValidarRecalculaElBorradorDesdeSusInstantaneas()
    {
        await using var e = new Escenario();
        await e.Iniciar();
        var id = await e.Borrador();
        await e.Usar(async s =>
        {
            var repo = s.GetRequiredService<IRepositorio>();
            (await repo.Pedido(id)).CambiarCantidad(0, 2);
            await repo.Guardar();
        });
        await e.Usar(s => s.GetRequiredService<ServicioPedidos>().Validar(id));
        Assert.Equal(113, (await e.Pedido(id)).Total);
    }
}
