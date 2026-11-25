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

namespace LogiMarket.Tests.Dominio;
// Escenarios de PedidoTests con datos controlados. Comprobamos el efecto y los cambios que no deben ocurrir.
public sealed class PedidoTests
{
    private static async Task SinEfectos(Escenario e, Guid id) => await e.Usar(async s =>
    {
        var repo = s.GetRequiredService<IRepositorio>();
        Assert.Equal(8, (await repo.Productos()).Single(p => p.Sku == "P01").Stock);
        Assert.Null(await repo.PagoPorPedido(id));
        Assert.Empty(await s.GetRequiredService<IConsultasOperativas>().Eventos());
        Assert.Empty(await s.GetRequiredService<IConsultasOperativas>().Movimientos());
    });
    private sealed class ReembolsoConFallo : IReembolsosSimulados
    {
        public Task<string> ReembolsarAsync(Guid pedido, decimal importe) => throw new IOException("Reembolso indisponible.");
    }

    private sealed class PasarelaConFallo : IPasarelaPago
    {
        public Task<ResultadoCobro> CobrarAsync(decimal importe, string clave) => throw new IOException("Colaborador indisponible.");
    }

    [Fact]
    public async Task InstantaneaNoCambiaConCatalogo()
    {
        await using var e = new Escenario();
        await e.Iniciar();
        var id = await e.Pagar();
        await e.Usar(async s =>
        {
            var p = (await s.GetRequiredService<IRepositorio>().Productos()).Single(p => p.Sku == "P01");
            await s.GetRequiredService<ServicioCatalogo>().GuardarProducto(p.Id, p.Sku, p.Nombre, 99, p.Peso, 0);
        });
        Assert.Equal(59, (await e.Pedido(id)).Total);
        Assert.Equal(20, (await e.Pedido(id)).Detalles.SelectMany(d => d.Componentes).Single(c => c.Sku == "P01").Precio);
    }

    [Fact]
    public async Task EdicionConservaIdentidadYReemplazaLineas()
    {
        await using var e = new Escenario();
        await e.Iniciar();
        var id = await e.Borrador();
        var p = await e.Pedido(id);
        var mismo = await e.Usar(s => s.GetRequiredService<ServicioPedidos>().Guardar(new(p.ClienteId, TipoEntrega.EXPRES, [new("P04", 2)]), id));
        Assert.Equal(id, mismo);
        var editado = await e.Pedido(id);
        Assert.Single(editado.Detalles);
        Assert.Equal("P04", editado.Detalles[0].Sku);
        Assert.Equal(84, editado.Total);
    }

    [Fact]
    public async Task InstantaneaConservaArbolAnidadoYClon()
    {
        await using var e = new Escenario();
        await e.Iniciar();
        var id = await e.Borrador();
        var p = await e.Pedido(id);
        Assert.Contains("KIT_OFICINA", p.Detalles[0].ComposicionJson);
        var copia = p.ClonarBorrador();
        copia.CambiarCantidad(0, 2);
        Assert.Equal(1, p.Detalles[0].Cantidad);
        Assert.Equal(2, copia.Detalles[0].Cantidad);
        Assert.Equal(p.Detalles[0].ComposicionJson, copia.Detalles[0].ComposicionJson);
    }

    [Fact]
    public void CantidadEditadaExigeRevalorarAntesDeValidar()
    {
        var pedido = new PedidoBuilder().Para(new Cliente("Ana", CategoriaCliente.VIP)).Agregar(new ProductoSimple("P01", "Mouse", 20, .2m), 1).Construir();
        var calculadora = new CalculadoraPrecios([new Mayorista15(), new Vip10(), new SinDescuento()]);
        pedido.Valorar(calculadora, 5);
        pedido.CambiarCantidad(0, 3);
        Assert.Throws<InvalidOperationException>(() => pedido.Transicionar(EstadoPedido.VALIDADO, "test", DateTimeOffset.UtcNow));
        pedido.Valorar(calculadora, 5);
        pedido.Transicionar(EstadoPedido.VALIDADO, "test", DateTimeOffset.UtcNow);
        Assert.Equal(59, pedido.Total);
    }
}
