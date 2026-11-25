namespace LogiMarket.Domain.Entregas;
// Abstract Factory este producto forma parte de una familia de entrega coherente
internal sealed record Etiqueta(string Prefijo) : IEtiquetaEntrega
{
    public string Crear(Guid pedido) => $"{Prefijo}-{pedido:N}";
}
