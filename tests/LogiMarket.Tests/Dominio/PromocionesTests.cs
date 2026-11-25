using LogiMarket.Domain.Catalogo;
using LogiMarket.Domain.Promociones;
using Xunit;
using LogiMarket.Application.Comprobantes;
using LogiMarket.Application.Contratos;
using LogiMarket.Application.DTOs;
using LogiMarket.Application.Eventos;
using LogiMarket.Application.Reportes;
using LogiMarket.Application.Validaciones;
using LogiMarket.Domain.Entidades;
using LogiMarket.Domain.Entregas;
using LogiMarket.Domain.Enums;
using LogiMarket.Domain.Operaciones;
using LogiMarket.Domain.Pedidos;
using LogiMarket.Infrastructure.Documentos;
using LogiMarket.Infrastructure.Pagos;
using LogiMarket.Tests.Soporte;
using Microsoft.Extensions.DependencyInjection;
using LogiMarket.Infrastructure.InyeccionDependencias;

namespace LogiMarket.Tests.Dominio;
// Escenarios de PromocionesTests con datos controlados. Comprobamos el efecto y los cambios que no deben ocurrir.
public sealed class PromocionesTests
{
    private sealed class PromocionNueva : IPoliticaDescuento
    {
        public int Prioridad => 100;
        public decimal Tasa => .20m;

        public bool Aplica(ContextoPromocion contexto) => true;
    }

    [Theory]
    [InlineData(true, 3, 60, 5, 59)]
    [InlineData(false, 4, 90, 12, 102)]
    [InlineData(false, 10, 100, 5, 90)]
    [InlineData(true, 10, 100, 5, 90)]
    public void PromocionesRespetanImportesYPrioridad(bool vip, int unidades, decimal subtotal, decimal tarifa, decimal esperado)
    {
        var calculadora = new CalculadoraPrecios([new Mayorista15(), new Vip10(), new SinDescuento()]);
        Assert.Equal(esperado, calculadora.Calcular(subtotal, tarifa, new(vip, unidades)).Total);
    }

    [Fact]
    public void P32_ExtenderStrategySinCambiarCalculadora()
    {
        var calculadora = new CalculadoraPrecios([new SinDescuento(), new PromocionNueva()]);
        Assert.Equal(20, calculadora.Calcular(100, 5, new(false, 1)).Descuento);
    }

    [Fact]
    public void RedondeoComercialAwayFromZero()
    {
        var c = new CalculadoraPrecios([new Vip10()]);
        Assert.Equal(0.01m, c.Calcular(.05m, 5, new(true, 1)).Descuento);
    }
}
