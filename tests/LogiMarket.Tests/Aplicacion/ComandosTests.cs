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

namespace LogiMarket.Tests.Aplicacion;
// Escenarios de ComandosTests con datos controlados. Comprobamos el efecto y los cambios que no deben ocurrir.
public sealed class ComandosTests
{
    [Fact]
    public async Task P24_ComandoInvocadoRegistraResultado()
    {
        await using var e = new Escenario();
        await e.Iniciar();
        var id = await e.Borrador();
        await e.Usar(s => s.GetRequiredService<ServicioPedidos>().Validar(id));
        await e.Usar(s => s.GetRequiredService<InvocadorComandos>().Ejecutar(new ConfirmarPedidoCommand(s.GetRequiredService<ConfirmacionPedidoFacade>(), id, "command")));
        await e.Usar(async s => Assert.Contains(await s.GetRequiredService<IConsultasOperativas>().Auditorias(), a => a.Operacion == nameof(ConfirmarPedidoCommand) && a.Resultado.StartsWith("OK")));
    }
}
