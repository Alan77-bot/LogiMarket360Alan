using LogiMarket.Application.Contratos;
using LogiMarket.Application.Seguridad;

namespace LogiMarket.Application.Comandos;
// Patron Command: ejecuta el mismo objeto desde UI o pruebas y registra el resultado de la invocacion.
public sealed class InvocadorComandos(IBitacora bitacora, SesionActual sesion)
{
    public async Task Ejecutar(IComando comando)
    {
        var correlacion = Guid.NewGuid().ToString();
        try
        {
            await comando.Ejecutar();
            await bitacora.Registrar(sesion.Nombre, comando.GetType().Name, "OK " + comando.Entrada, correlacion);
        }
        catch (Exception ex)
        {
            await bitacora.Registrar(sesion.Nombre, comando.GetType().Name, "ERROR " + comando.Entrada + " " + ex.Message, correlacion);
            throw;
        }
    }
}
