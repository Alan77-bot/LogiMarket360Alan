namespace LogiMarket.Application.Contratos;
// Responsabilidad de OpcionesDemostracion: colaborar desde LogiMarket.Application.Contratos sin mezclar reglas, persistencia y pantallas.
public sealed class OpcionesDemostracion
{
    public bool RechazarCobro { get; set; }
    public bool FallarNotificacion { get; set; }
    public bool ProveedorAlternativo { get; set; }
    public string Canal { get; set; } = "EMAIL";
}
