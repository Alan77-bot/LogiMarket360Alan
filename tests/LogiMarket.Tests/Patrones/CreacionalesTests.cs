using LogiMarket.Application.Comprobantes;
using LogiMarket.Application.Contratos;
using LogiMarket.Application.DTOs;
using LogiMarket.Application.Eventos;
using LogiMarket.Application.Reportes;
using LogiMarket.Application.Validaciones;
using LogiMarket.Domain.Catalogo;
using LogiMarket.Domain.Entidades;
using LogiMarket.Domain.Entregas;
using LogiMarket.Domain.Enums;
using LogiMarket.Domain.Operaciones;
using LogiMarket.Domain.Pedidos;
using LogiMarket.Domain.Promociones;
using LogiMarket.Infrastructure.Documentos;
using LogiMarket.Infrastructure.Pagos;
using LogiMarket.Tests.Soporte;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using LogiMarket.Infrastructure.InyeccionDependencias;

namespace LogiMarket.Tests.Patrones;
// Escenarios de CreacionalesTests con datos controlados. Comprobamos el efecto y los cambios que no deben ocurrir.
public sealed class CreacionalesTests
{
    private static Pedido Crear(int cantidad = 1)
    {
        var pedido = new PedidoBuilder().Para(new Cliente("Ana", CategoriaCliente.VIP)).ConEntrega(TipoEntrega.ESTANDAR).Agregar(new ProductoSimple("P01", "Mouse", 20, .2m), cantidad).Construir();
        pedido.Valorar(new([new Mayorista15(), new Vip10(), new SinDescuento()]), 5);
        return pedido;
    }

    [Fact]
    public void P12_FamiliasCompatibles()
    {
        IFabricaEntrega estandar = new EntregaEstandarFactory(), expres = new EntregaExpresFactory();
        Assert.Equal(5, estandar.CrearTarifa().Importe);
        Assert.Equal(3, estandar.CrearPlazo().Dias);
        Assert.StartsWith("STD", estandar.CrearEtiqueta().Crear(Guid.NewGuid()));
        Assert.Equal(12, expres.CrearTarifa().Importe);
        Assert.Equal(1, expres.CrearPlazo().Dias);
        Assert.StartsWith("EXP", expres.CrearEtiqueta().Crear(Guid.NewGuid()));
    }

    [Fact]
    public void P13_BuilderRechazaIncompleto()
    {
        Assert.Throws<InvalidOperationException>(() => new PedidoBuilder().Construir());
        Assert.Throws<ArgumentException>(() => new PedidoBuilder().Para(new Cliente("Ana", CategoriaCliente.VIP)).Construir());
        Assert.Single(Crear().Detalles);
    }

    [Fact]
    public void P14_PrototypeProfundo()
    {
        var original = Crear();
        var copia = original.ClonarBorrador();
        Assert.NotEqual(original.Id, copia.Id);
        Assert.NotSame(original.Detalles, copia.Detalles);
        Assert.NotSame(original.Detalles[0].Componentes, copia.Detalles[0].Componentes);
        copia.CambiarCantidad(0, 3);
        Assert.Equal(1, original.Detalles[0].Cantidad);
        Assert.Equal(3, copia.Detalles[0].Cantidad);
        Assert.Equal(EstadoPedido.BORRADOR, copia.Estado);
    }

    [Fact]
    public async Task P15_SingletonIdentidadYConcurrencia()
    {
        await using var e = new Escenario();
        using var a = e.Proveedor.CreateScope();
        using var b = e.Proveedor.CreateScope();
        var metricas = a.ServiceProvider.GetRequiredService<MetricasSesion>();
        Assert.Same(metricas, b.ServiceProvider.GetRequiredService<MetricasSesion>());
        var evento = new EventoOutbox(Guid.NewGuid(), "PedidoPagado");
        Parallel.For(0, 1000, _ => metricas.Registrar(evento));
        Assert.Equal(1, metricas.Pagos);
    }
}
