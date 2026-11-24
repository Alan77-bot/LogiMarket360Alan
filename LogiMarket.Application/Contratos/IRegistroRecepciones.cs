namespace LogiMarket.Application.Contratos;
// Responsabilidad de IRegistroRecepciones: colaborar desde LogiMarket.Application.Contratos sin mezclar reglas, persistencia y pantallas.
public interface IRegistroRecepciones
{
    Task<bool> Recibido(Guid evento, string suscriptor);
    void Marcar(Guid evento, string suscriptor);
}
