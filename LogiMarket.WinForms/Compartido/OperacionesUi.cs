using LogiMarket.Application.Contratos;
using LogiMarket.Application.Seguridad;
using Microsoft.Extensions.DependencyInjection;

namespace LogiMarket.WinForms.Compartido;
// Responsabilidad de OperacionesUi: colaborar desde LogiMarket.WinForms.Compartido sin mezclar reglas, persistencia y pantallas.
// El ambito vive solo durante una operacion; no se comparte DbContext entre pantallas.
public sealed class OperacionesUi(IServiceProvider proveedor)
{
    public async Task<T> Consultar<T>(Func<IServiceProvider, Task<T>> accion)
    {
        using var ambito = proveedor.CreateScope();
        await ambito.ServiceProvider.GetRequiredService<Autorizador>().Lectura();
        return await accion(ambito.ServiceProvider);
    }

    public async Task Ejecutar(Func<IServiceProvider, Task> accion)
    {
        using var ambito = proveedor.CreateScope();
        await accion(ambito.ServiceProvider);
        await ambito.ServiceProvider.GetRequiredService<IProcesadorOutbox>().Procesar();
    }
}
