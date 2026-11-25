using LogiMarket.Application.Catalogo;
using LogiMarket.Application.Clientes;
using LogiMarket.Application.Contratos;
using LogiMarket.Application.DTOs;
using LogiMarket.Application.Logistica;
using LogiMarket.Application.Pedidos;
using LogiMarket.Domain.Enums;
using LogiMarket.Domain.Pedidos;
using LogiMarket.Tests.Soporte;
using Microsoft.Extensions.DependencyInjection;
using Xunit;
using LogiMarket.Application.Comprobantes;
using LogiMarket.Application.Eventos;
using LogiMarket.Application.Reportes;
using LogiMarket.Application.Validaciones;
using LogiMarket.Domain.Catalogo;
using LogiMarket.Domain.Entidades;
using LogiMarket.Domain.Entregas;
using LogiMarket.Domain.Operaciones;
using LogiMarket.Domain.Promociones;
using LogiMarket.Infrastructure.Documentos;
using LogiMarket.Infrastructure.Pagos;
using LogiMarket.Infrastructure.InyeccionDependencias;

namespace LogiMarket.Tests.Dominio;
// Escenarios de ProductoTests con datos controlados. Comprobamos el efecto y los cambios que no deben ocurrir.
public sealed class ProductoTests
{
    private sealed class ReembolsoConFallo : IReembolsosSimulados
    {
        public Task<string> ReembolsarAsync(Guid pedido, decimal importe) => throw new IOException("Reembolso indisponible.");
    }

    private sealed class PasarelaConFallo : IPasarelaPago
    {
        public Task<ResultadoCobro> CobrarAsync(decimal importe, string clave) => throw new IOException("Colaborador indisponible.");
    }

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
    public async Task ProductoUsadoNoSeEliminaNiCambiaSku()
    {
        await using var e = new Escenario();
        await e.Iniciar();
        await e.Pagar();
        var producto = (await e.Usar(s => s.GetRequiredService<IRepositorio>().Productos())).Single(p => p.Sku == "P01");
        await Assert.ThrowsAsync<InvalidOperationException>(() => e.Usar(s => s.GetRequiredService<ServicioCatalogo>().Eliminar(producto.Id, false)));
        await Assert.ThrowsAsync<InvalidOperationException>(() => e.Usar(s => s.GetRequiredService<ServicioCatalogo>().GuardarProducto(producto.Id, "OTRO", producto.Nombre, producto.Precio, producto.Peso, 0)));
    }

    [Theory]
    [InlineData(0, 0)]
    [InlineData(-1, 0)]
    [InlineData(1, -1)]
    public void ProductoRechazaValoresInvalidos(decimal precio, decimal peso)
    {
        Assert.Throws<ArgumentException>(() => new ProductoSimple("P01", "Mouse", precio, peso));
    }
}
