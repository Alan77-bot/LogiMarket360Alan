using LogiMarket.Domain.Catalogo;
using LogiMarket.Domain.Promociones;
using Xunit;
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
using LogiMarket.Infrastructure.InyeccionDependencias;

namespace LogiMarket.Tests.Dominio;
// Escenarios de KitsTests con datos controlados. Comprobamos el efecto y los cambios que no deben ocurrir.
public sealed class KitsTests
{
    [Fact]
    public void KitAnidadoExpandeCantidadesYRechazaCicloIndirecto()
    {
        var oficina = new KitCompuesto("KIT_OFICINA");
        oficina.Agregar(new ProductoSimple("P01", "Mouse USB", 20, 0.20m));
        oficina.Agregar(new ProductoSimple("P02", "Teclado", 30, 0.70m));
        var teletrabajo = new KitCompuesto("KIT_TELETRABAJO");
        teletrabajo.Agregar(oficina);
        teletrabajo.Agregar(new ProductoSimple("P03", "Memoria USB", 10, 0.10m));
        Assert.Equal(50, oficina.Precio);
        Assert.Equal(60, teletrabajo.Precio);
        Assert.Equal(new[] { "P01", "P02", "P03" }, teletrabajo.Expandir().Select(c => c.Sku));
        Assert.All(teletrabajo.Expandir(6), c => Assert.Equal(6, c.Cantidad));
        Assert.Throws<InvalidOperationException>(() => oficina.Agregar(teletrabajo));
    }

    [Fact]
    public async Task KitCircularPersistidoSeRevierte()
    {
        await using var e = new Escenario();
        await e.Iniciar();
        var kits = await e.Usar(s => s.GetRequiredService<IRepositorio>().Kits());
        var oficina = kits.Single(k => k.Sku == "KIT_OFICINA");
        var tele = kits.Single(k => k.Sku == "KIT_TELETRABAJO");
        await Assert.ThrowsAsync<InvalidOperationException>(() => e.Usar(s => s.GetRequiredService<ServicioCatalogo>().GuardarKit(oficina.Id, oficina.Sku, oficina.Nombre, [new(tele.Id, true, 1)])));
        Assert.Equal(2, (await e.Usar(s => s.GetRequiredService<IRepositorio>().Kits())).Single(k => k.Id == oficina.Id).Componentes.Count);
    }
}
