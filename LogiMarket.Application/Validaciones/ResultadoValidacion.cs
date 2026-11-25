namespace LogiMarket.Application.Validaciones;

public sealed record ResultadoValidacion(bool Valido, string Codigo, string Mensaje)
{
    public static ResultadoValidacion Correcto => new(true, "OK", "Pedido valido.");
}
