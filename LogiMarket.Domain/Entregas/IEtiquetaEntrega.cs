namespace LogiMarket.Domain.Entregas;
// Abstract Factory este producto forma parte de una familia de entrega coherente
public interface IEtiquetaEntrega
{
    string Crear(Guid pedido);
}
