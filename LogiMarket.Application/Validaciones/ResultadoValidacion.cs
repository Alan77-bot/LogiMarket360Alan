namespace LogiMarket.Application.Validaciones;
// Coordina esta responsabilidad a traves de contratos; las reglas del negocio permanecen en su capa.
public sealed record ResultadoValidacion(bool Valido, string Codigo, string Mensaje)
{
    public static ResultadoValidacion Correcto => new(true, "OK", "Pedido valido.");
}
