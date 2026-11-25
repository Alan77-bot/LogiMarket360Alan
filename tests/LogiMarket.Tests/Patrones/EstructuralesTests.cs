using LogiMarket.Application.Catalogo;
using LogiMarket.Domain.Entidades;
using LogiMarket.Infrastructure.Persistencia;
using Xunit;
using LogiMarket.Application.Comprobantes;
using LogiMarket.Application.Contratos;
using LogiMarket.Application.DTOs;
using LogiMarket.Application.Eventos;
using LogiMarket.Application.Reportes;
using LogiMarket.Application.Validaciones;
using LogiMarket.Domain.Catalogo;
using LogiMarket.Domain.Entregas;
using LogiMarket.Domain.Enums;
using LogiMarket.Domain.Operaciones;
using LogiMarket.Domain.Pedidos;
using LogiMarket.Domain.Promociones;
using LogiMarket.Infrastructure.Documentos;
using LogiMarket.Infrastructure.Pagos;
using LogiMarket.Tests.Soporte;
using Microsoft.Extensions.DependencyInjection;
using LogiMarket.Infrastructure.InyeccionDependencias;

namespace LogiMarket.Tests.Patrones;
// Escenarios de EstructuralesTests con datos controlados. Comprobamos el efecto y los cambios que no deben ocurrir.
public sealed class EstructuralesTests
{
    private sealed record Fila(int Indice, MetadatosSku Metadatos, int Cantidad, decimal Precio);
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
    public void P21_MilFilasCompartenIdentidadConComparacionEquivalente()
    {
        var fabrica = new CatalogoVisualFlyweightFactory();
        var antes = GC.GetAllocatedBytesForCurrentThread();
        var sin = Enumerable.Range(0, 1000).Select(i => new Fila(i, new("P01", "Mouse USB", "Accesorios", "Catalogo de demostracion"), 1, 20)).ToArray();
        var bytesSin = GC.GetAllocatedBytesForCurrentThread() - antes;
        antes = GC.GetAllocatedBytesForCurrentThread();
        var con = Enumerable.Range(0, 1000).Select(i => new Fila(i, fabrica.Obtener("P01", "Mouse USB"), 1, 20)).ToArray();
        var bytesCon = GC.GetAllocatedBytesForCurrentThread() - antes;
        Assert.Same(con[0].Metadatos, con[999].Metadatos);
        Assert.NotSame(sin[0].Metadatos, sin[999].Metadatos);
        Assert.Single(con.Select(f => f.Metadatos).Distinct(ReferenceEqualityComparer.Instance));
        Assert.Equal(1, fabrica.Instancias);
        Assert.True(bytesCon < bytesSin);
        GC.KeepAlive(sin);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public async Task P16_P38_LspProveedores(bool alternativo)
    {
        var opciones = new OpcionesDemostracion();
        IPasarelaPago proveedor = alternativo ? new PasarelaAlternativaSimulada(opciones) : new PasarelaLegadaAdapter(new(), opciones);
        var resultado = await proveedor.CobrarAsync(59, "clave");
        var repetido = await proveedor.CobrarAsync(59, "clave");
        Assert.True(resultado.Aceptado);
        Assert.Equal(resultado.Referencia, repetido.Referencia);
        await Assert.ThrowsAsync<ArgumentException>(async () => await proveedor.CobrarAsync(0, "cero"));
        opciones.RechazarCobro = true;
        Assert.False((await proveedor.CobrarAsync(59, "fallo")).Aceptado);
    }

    [Fact]
    public async Task P17_BridgeCuatroCombinacionesContenidoYArchivo()
    {
        var directorio = Path.Combine(Path.GetTempPath(), "lm-txt-" + Guid.NewGuid().ToString("N"));
        try
        {
            var captura = new SalidaCaptura();
            var archivo = new SalidaArchivoTxt(directorio);
            var p = Crear();
            p.Valorar(new([new Mayorista15(), new Vip10(), new SinDescuento()]), 5);
            var d = new Devolucion(p.Id, MotivoDevolucion.DEFECTO, p.Subtotal - p.Descuento, "REF", DateTimeOffset.UtcNow);
            await new ComprobanteVenta(p, captura).Exportar();
            Assert.Contains("Total: USD 23.00", captura.Texto);
            await new ComprobanteVenta(p, archivo).Exportar();
            await new ComprobanteDevolucion(d, captura).Exportar();
            Assert.Contains("18.00", captura.Texto);
            await new ComprobanteDevolucion(d, archivo).Exportar();
            Assert.Equal(2, Directory.GetFiles(directorio).Length);
            Assert.All(Directory.GetFiles(directorio), f => Assert.Contains("LOGIMARKET", File.ReadAllText(f)));
        }
        finally
        {
            if (Directory.Exists(directorio))
                foreach (var f in Directory.GetFiles(directorio))
                    File.Delete(f);
            if (Directory.Exists(directorio))
                Directory.Delete(directorio);
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task P19_DecoradorRodeaEjecucion(bool acepta)
    {
        var bitacora = new BitacoraCaptura();
        var decorador = new AuditoriaPagoDecorator(new PasarelaCaptura(bitacora.Secuencia, acepta), bitacora, new());
        Assert.Equal(acepta, (await decorador.CobrarAsync(10, "clave")).Aceptado);
        Assert.Equal("INICIO", bitacora.Secuencia[0]);
        Assert.Equal("PASARELA", bitacora.Secuencia[1]);
        Assert.StartsWith(acepta ? "FIN" : "ERROR", bitacora.Secuencia[2]);
    }
}
