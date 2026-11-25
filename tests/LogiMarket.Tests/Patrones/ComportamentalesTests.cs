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
// Escenarios de ComportamentalesTests con datos controlados. Comprobamos el efecto y los cambios que no deben ocurrir.
public sealed class ComportamentalesTests
{
    private static Pedido Crear(int cantidad = 1)
    {
        var pedido = new PedidoBuilder().Para(new Cliente("Ana", CategoriaCliente.VIP)).ConEntrega(TipoEntrega.ESTANDAR).Agregar(new ProductoSimple("P01", "Mouse", 20, .2m), cantidad).Construir();
        pedido.Valorar(new([new Mayorista15(), new Vip10(), new SinDescuento()]), 5);
        return pedido;
    }

    [Fact]
    public void P23_CadenaPrimeroClienteLuegoStock()
    {
        var p = Crear();
        var resultado = CadenaConfirmacion.Validar(new(p, null, []), false);
        Assert.Equal("CLIENTE", resultado.Codigo);
        resultado = CadenaConfirmacion.Validar(new(p, new Cliente("Ana", CategoriaCliente.VIP), []), false);
        Assert.Equal("STOCK", resultado.Codigo);
    }

    [Fact]
    public void P25_InterpreterSeleccionaPagado()
    {
        var p = Crear(3);
        p.Valorar(new([new SinDescuento()]), 5);
        p.Transicionar(EstadoPedido.VALIDADO, "test", DateTimeOffset.UtcNow);
        p.Transicionar(EstadoPedido.PAGADO, "test", DateTimeOffset.UtcNow);
        Assert.True(new InterpreterFiltro().Analizar("ESTADO = PAGADO AND TOTAL >= 50").Evaluar(p));
        Assert.False(new InterpreterFiltro().Analizar("ESTADO = BORRADOR AND TOTAL >= 50").Evaluar(p));
    }

    [Fact]
    public void P26_InterpreterPrecedenciaYErrores()
    {
        var p = Crear();
        var parser = new InterpreterFiltro();
        Assert.True(parser.Analizar("CLIENTE = Ana OR TOTAL >= 100 AND ESTADO = PAGADO").Evaluar(p));
        Assert.False(parser.Analizar("(CLIENTE = Ana OR TOTAL >= 100) AND ESTADO = PAGADO").Evaluar(p));
        Assert.Throws<FormatException>(() => parser.Analizar("TOTAL > 10"));
        Assert.Throws<FormatException>(() => parser.Analizar("(ESTADO = PAGADO"));
    }

    [Fact]
    public void P27_P34_IteratorYVisitorKits()
    {
        var oficina = new KitCompuesto("KIT_OFICINA");
        oficina.Agregar(new ProductoSimple("P01", "Mouse", 20, .2m));
        oficina.Agregar(new ProductoSimple("P02", "Teclado", 30, .7m));
        var tele = new KitCompuesto("KIT_TELETRABAJO");
        tele.Agregar(oficina);
        tele.Agregar(new ProductoSimple("P03", "Memoria", 10, .1m));
        using var iterador = new IteradorFisico(tele);
        var skus = new List<string>();
        while (iterador.MoveNext())
            skus.Add(iterador.Current.Sku);
        Assert.Equal(new[] { "P01", "P02", "P03" }, skus);
        Assert.False(iterador.MoveNext());
        var peso = new PesoVisitor();
        tele.Aceptar(peso);
        Assert.Equal(1m, peso.Peso);
        var desglose = new DesgloseVisitor();
        tele.Aceptar(desglose);
        Assert.Equal(3, desglose.Cantidades.Count);
    }

    [Fact]
    public void P29_MementoTresCambiosDosDeshacerUnRehacer()
    {
        var historial = new HistorialBorrador();
        historial.Capturar(Crear(1));
        historial.Capturar(Crear(2));
        historial.Capturar(Crear(3));
        Assert.Equal(2, historial.Deshacer().Detalles[0].Cantidad);
        Assert.Equal(1, historial.Deshacer().Detalles[0].Cantidad);
        Assert.Equal(2, historial.Rehacer().Detalles[0].Cantidad);
        historial.Bloquear();
        Assert.Throws<InvalidOperationException>(() => historial.Deshacer());
    }

    [Fact]
    public void MementoPermiteDeshacerEliminarLaUltimaLinea()
    {
        var historial = new HistorialBorrador();
        historial.Capturar(Crear());
        historial.CapturarVacio();
        Assert.Single(historial.DeshacerCompleto()!.Detalles);
        Assert.Null(historial.RehacerCompleto());
    }
}
