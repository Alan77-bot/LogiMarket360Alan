namespace LogiMarket.Application.Contratos;
// Responsabilidad de IProcesadorOutbox: colaborar desde LogiMarket.Application.Contratos sin mezclar reglas, persistencia y pantallas.
public interface IProcesadorOutbox
{
    Task<int> Procesar();
}
