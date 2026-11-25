using LogiMarket.Infrastructure.InyeccionDependencias;
using LogiMarket.Application.Contratos;
using LogiMarket.Application.DTOs;
using LogiMarket.Application.Pedidos;
using LogiMarket.Application.Seguridad;
using LogiMarket.Domain.Enums;
using LogiMarket.Domain.Pedidos;
using LogiMarket.Infrastructure.Persistencia;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace LogiMarket.Tests.Soporte;
// Responsabilidad de Escenario: colaborar desde LogiMarket.Tests.Soporte sin mezclar reglas, persistencia y pantallas.
public sealed class Escenario : IAsyncDisposable
{
    public string Ruta { get; } = Path.Combine(Path.GetTempPath(), $"logimarket-test-{Guid.NewGuid():N}.db");
    public ServiceProvider Proveedor { get; }
    public RelojPrueba Reloj { get; } = new();

    public Escenario(Action<IServiceCollection>? configurar = null)
    {
        var servicios = new ServiceCollection().AgregarLogiMarket(Ruta);
        servicios.AddSingleton<IReloj>(Reloj);
        configurar?.Invoke(servicios);
        Proveedor = servicios.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true, ValidateOnBuild = true });
    }

    public async Task Iniciar()
    {
        using var scope = Proveedor.CreateScope();
        await scope.ServiceProvider.GetRequiredService<Inicializador>().Ejecutar();
        await scope.ServiceProvider.GetRequiredService<Autenticacion>().Entrar("admin", "Demo123!");
    }

    public async Task<T> Usar<T>(Func<IServiceProvider, Task<T>> operacion)
    {
        using var scope = Proveedor.CreateScope();
        return await operacion(scope.ServiceProvider);
    }

    public async Task Usar(Func<IServiceProvider, Task> operacion)
    {
        using var scope = Proveedor.CreateScope();
        await operacion(scope.ServiceProvider);
    }

    public async Task Entrar(string usuario) => await Usar(s => s.GetRequiredService<Autenticacion>().Entrar(usuario, "Demo123!"));
    public async Task<Guid> Borrador(string cliente = "Ana", TipoEntrega entrega = TipoEntrega.ESTANDAR, params LineaEntrada[] lineas)
    {
        return await Usar(async s =>
        {
            var id = (await s.GetRequiredService<IRepositorio>().Clientes()).Single(c => c.Nombre == cliente).Id;
            return await s.GetRequiredService<ServicioPedidos>().Guardar(new(id, entrega, lineas.Length == 0 ? [new("KIT_TELETRABAJO", 1)] : lineas));
        });
    }

    public async Task<Guid> Pagar()
    {
        var id = await Borrador();
        await Usar(s => s.GetRequiredService<ServicioPedidos>().Validar(id));
        await Usar(async s => await s.GetRequiredService<ConfirmacionPedidoFacade>().Confirmar(id, id.ToString()));
        return id;
    }

    public Task<Pedido> Pedido(Guid id) => Usar(s => s.GetRequiredService<IRepositorio>().Pedido(id));
    public async ValueTask DisposeAsync()
    {
        await Proveedor.DisposeAsync();
        Microsoft.Data.Sqlite.SqliteConnection.ClearAllPools();
        // Solo se elimina el archivo temporal creado por este escenario.
        foreach (var archivo in new[]
        {
            Ruta,
            Ruta + "-wal",
            Ruta + "-shm"
        }

        )
            if (File.Exists(archivo))
                File.Delete(archivo);
    }
}
