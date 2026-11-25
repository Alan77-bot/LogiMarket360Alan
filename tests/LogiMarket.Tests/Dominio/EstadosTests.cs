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
using LogiMarket.Application.Comprobantes;
using LogiMarket.Application.Reportes;
using LogiMarket.Application.Validaciones;
using LogiMarket.Domain.Catalogo;
using LogiMarket.Domain.Entidades;
using LogiMarket.Domain.Entregas;
using LogiMarket.Domain.Promociones;
using LogiMarket.Infrastructure.Documentos;
using LogiMarket.Infrastructure.Pagos;

namespace LogiMarket.Tests.Dominio;
// Escenarios de EstadosTests con datos controlados. Comprobamos el efecto y los cambios que no deben ocurrir.
public sealed class EstadosTests
{
    private static async Task SinEfectos(Escenario e, Guid id) => await e.Usar(async s =>
    {
        var repo = s.GetRequiredService<IRepositorio>();
        Assert.Equal(8, (await repo.Productos()).Single(p => p.Sku == "P01").Stock);
        Assert.Null(await repo.PagoPorPedido(id));
        Assert.Empty(await s.GetRequiredService<IConsultasOperativas>().Eventos());
        Assert.Empty(await s.GetRequiredService<IConsultasOperativas>().Movimientos());
    });
    private static Pedido Crear(int cantidad = 1)
    {
        var pedido = new PedidoBuilder().Para(new Cliente("Ana", CategoriaCliente.VIP)).ConEntrega(TipoEntrega.ESTANDAR).Agregar(new ProductoSimple("P01", "Mouse", 20, .2m), cantidad).Construir();
        pedido.Valorar(new([new Mayorista15(), new Vip10(), new SinDescuento()]), 5);
        return pedido;
    }

    private sealed class SalidaCaptura : ISalidaDocumento
    {
        public string Texto { get; private set; } = "";

        public Task Emitir(string nombre, string contenido)
        {
            Texto = contenido;
            return Task.CompletedTask;
        }
    }

    private sealed class BitacoraCaptura : IBitacora
    {
        public List<string> Secuencia { get; } = [];

        public Task Registrar(string usuario, string operacion, string resultado, string correlacion)
        {
            Secuencia.Add(resultado);
            return Task.CompletedTask;
        }
    }

    private sealed class PasarelaCaptura(List<string> secuencia, bool acepta) : IPasarelaPago
    {
        public Task<ResultadoCobro> CobrarAsync(decimal importe, string clave)
        {
            secuencia.Add("PASARELA");
            return Task.FromResult(new ResultadoCobro(acepta, "REF", "Rechazo"));
        }
    }

    private sealed class PromocionNueva : IPoliticaDescuento
    {
        public int Prioridad => 100;
        public decimal Tasa => .20m;

        public bool Aplica(ContextoPromocion contexto) => true;
    }

    [Fact]
    public async Task P08_PagadoNoAdmiteCancelarNiEditar()
    {
        await using var e = new Escenario();
        await e.Iniciar();
        var id = await e.Pagar();
        var previo = await e.Pedido(id);
        await Assert.ThrowsAsync<InvalidOperationException>(() => e.Usar(s => s.GetRequiredService<ServicioPedidos>().Cancelar(id)));
        await Assert.ThrowsAsync<InvalidOperationException>(() => e.Usar(s => s.GetRequiredService<ServicioPedidos>().Guardar(new(previo.ClienteId, previo.Entrega, [new("P01", 1)]), id)));
        var posterior = await e.Pedido(id);
        Assert.Equal(previo.Total, posterior.Total);
        Assert.Equal(previo.Historial.Count, posterior.Historial.Count);
    }

    [Fact]
    public void P31_StateRechazaRetrocesoYRepeticion()
    {
        var p = Crear();
        Assert.Throws<InvalidOperationException>(() => p.Transicionar(EstadoPedido.DESPACHADO, "x", DateTimeOffset.UtcNow));
        foreach (var estado in new[]
        {
            EstadoPedido.VALIDADO,
            EstadoPedido.PAGADO,
            EstadoPedido.DESPACHADO,
            EstadoPedido.ENTREGADO
        }

        )
            p.Transicionar(estado, "test", DateTimeOffset.UtcNow);
        Assert.Throws<InvalidOperationException>(() => p.Transicionar(EstadoPedido.CANCELADO, "x", DateTimeOffset.UtcNow));
        Assert.Throws<InvalidOperationException>(() => p.Transicionar(EstadoPedido.ENTREGADO, "x", DateTimeOffset.UtcNow));
    }
}
