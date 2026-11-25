using LogiMarket.WinForms.Compartido;
namespace LogiMarket.WinForms.Vistas;
// Reutiliza la consulta y los Commands de VistaPedidos, sin duplicar reglas del negocio.
public sealed class VistaDevoluciones(OperacionesUi operaciones, FormularioPrincipalMediator mediator, PermisosVista permisos)
    : VistaPedidos(operaciones, mediator, "Devoluciones", permisos);