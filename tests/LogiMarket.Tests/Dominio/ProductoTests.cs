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
// Escenarios de ProductoTests con datos controlados Comprobamos el efecto y los cambios que no deben ocurrir
public sealed class ProductoTests
{
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
