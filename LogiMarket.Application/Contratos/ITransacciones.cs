namespace LogiMarket.Application.Contratos;
// Este puerto define lo que necesita Application. Infrastructure proporciona la implementacion concreta.
public interface ITransacciones
{
    Task<T> Ejecutar<T>(Func<Task<T>> operacion);
}
