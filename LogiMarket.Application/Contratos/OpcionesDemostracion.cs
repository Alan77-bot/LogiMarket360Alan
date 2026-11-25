namespace LogiMarket.Application.Contratos;
// Este puerto define lo que necesita Application. Infrastructure proporciona la implementacion concreta.
public sealed class OpcionesDemostracion
{
    public bool RechazarCobro { get; set; }
    public bool FallarNotificacion { get; set; }
    public bool ProveedorAlternativo { get; set; }
    public string Canal { get; set; } = "EMAIL";
}
