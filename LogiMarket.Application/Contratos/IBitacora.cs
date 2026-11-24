namespace LogiMarket.Application.Contratos;
// Responsabilidad de IBitacora: colaborar desde LogiMarket.Application.Contratos sin mezclar reglas, persistencia y pantallas.
public interface IBitacora
{
    Task Registrar(string usuario, string operacion, string resultado, string correlacion);
}
