namespace LogiMarket.Domain.Entregas;
// Responsabilidad de Etiqueta: colaborar desde LogiMarket.Domain.Entregas sin mezclar reglas, persistencia y pantallas.
internal sealed record Etiqueta(string Prefijo) : IEtiquetaEntrega
{
    public string Crear(Guid pedido) => $"{Prefijo}-{pedido:N}";
}
