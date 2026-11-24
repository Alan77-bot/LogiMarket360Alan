namespace LogiMarket.Application.Validaciones;
// Responsabilidad de ResultadoValidacion: colaborar desde LogiMarket.Application.Validaciones sin mezclar reglas, persistencia y pantallas.
public sealed record ResultadoValidacion(bool Valido, string Codigo, string Mensaje)
{
    public static ResultadoValidacion Correcto => new(true, "OK", "Pedido valido.");
}
