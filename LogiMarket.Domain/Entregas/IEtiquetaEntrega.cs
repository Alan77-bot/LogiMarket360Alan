namespace LogiMarket.Domain.Entregas;
// Responsabilidad de IEtiquetaEntrega: colaborar desde LogiMarket.Domain.Entregas sin mezclar reglas, persistencia y pantallas.
public interface IEtiquetaEntrega
{
    string Crear(Guid pedido);
}
