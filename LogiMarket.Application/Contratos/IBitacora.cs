namespace LogiMarket.Application.Contratos;
// Este puerto define lo que necesita Application Infrastructure proporciona la implementacion concreta
public interface IBitacora
{
    Task Registrar(string usuario, string operacion, string resultado, string correlacion);
}
