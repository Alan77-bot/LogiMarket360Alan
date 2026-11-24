namespace LogiMarket.Application.Contratos;
// Responsabilidad de ITransacciones: colaborar desde LogiMarket.Application.Contratos sin mezclar reglas, persistencia y pantallas.
public interface ITransacciones
{
    Task<T> Ejecutar<T>(Func<Task<T>> operacion);
}
