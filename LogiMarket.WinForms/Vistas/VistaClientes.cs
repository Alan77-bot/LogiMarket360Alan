using LogiMarket.Application.Clientes;
using LogiMarket.Domain.Entidades;
using LogiMarket.Domain.Enums;
using LogiMarket.WinForms.Compartido;
using LogiMarket.WinForms.Formularios;
using Microsoft.Extensions.DependencyInjection;

namespace LogiMarket.WinForms.Vistas;
// Esta pantalla presenta datos y delega los cambios en Application; no escribe tablas directamente.
public sealed class VistaClientes : UserControl
{
    private readonly DataGridView grid = Ui.Grilla();
    private readonly TextBox buscar = new()
    {
        Width = 220,
        PlaceholderText = "Nombre"
    };
    private readonly OperacionesUi operaciones;
    private readonly FormularioPrincipalMediator mediator;
    public VistaClientes(OperacionesUi operaciones, FormularioPrincipalMediator mediator, PermisosVista permisos)
    {
        this.operaciones = operaciones;
        this.mediator = mediator;
        Dock = DockStyle.Fill;
        var barra = Ui.Barra();
        barra.Controls.Add(buscar);
        barra.Controls.Add(Ui.Boton("Buscar", Refrescar));
        if (permisos.Vender)
        {
            barra.Controls.Add(Ui.Boton("Nuevo cliente", () => Editar(null)));
            barra.Controls.Add(Ui.Boton("Editar cliente", () => Editar(grid.CurrentRow?.DataBoundItem as Cliente ?? throw new InvalidOperationException("Seleccione cliente."))));
        }

        Controls.Add(grid);
        Controls.Add(barra);
        mediator.Registrar(Refrescar);
    }

    public async Task Refrescar() => Ui.Cargar(grid, await operaciones.Consultar(s => s.GetRequiredService<ServicioClientes>().Consultar(buscar.Text)));
    private async Task Editar(Cliente? cliente)
    {
        using var d = new DialogoCampos("Cliente").Texto("Nombre", cliente?.Nombre ?? "").Lista("Categoria", Enum.GetValues<CategoriaCliente>().Cast<object>().ToArray(), cliente?.Categoria).Casilla("Activo", cliente?.Activo ?? true);
        if (d.ShowDialog(this) != DialogResult.OK)
            return;
        await operaciones.Ejecutar(s => s.GetRequiredService<ServicioClientes>().Guardar(cliente?.Id, d.Texto("Nombre"), d.Seleccion<CategoriaCliente>("Categoria"), d.Marcado("Activo")));
        await mediator.Actualizar();
    }
}
