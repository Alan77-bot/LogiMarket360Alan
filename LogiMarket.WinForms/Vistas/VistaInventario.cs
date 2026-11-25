using LogiMarket.Application.Contratos;
using LogiMarket.Application.Logistica;
using LogiMarket.Domain.Entidades;
using LogiMarket.WinForms.Compartido;
using LogiMarket.WinForms.Formularios;
using Microsoft.Extensions.DependencyInjection;

namespace LogiMarket.WinForms.Vistas;
// Esta pantalla presenta datos y delega los cambios en Application no escribe tablas directamente
public sealed class VistaInventario : UserControl
{
    private readonly DataGridView stock = Ui.Grilla();
    private readonly DataGridView movimientos = Ui.Grilla();
    private readonly OperacionesUi operaciones;
    private readonly FormularioPrincipalMediator mediator;
    public VistaInventario(OperacionesUi operaciones, FormularioPrincipalMediator mediator, PermisosVista permisos)
    {
        this.operaciones = operaciones;
        this.mediator = mediator;
        Dock = DockStyle.Fill;
        var split = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Horizontal
        };
        split.Panel1.Controls.Add(stock);
        split.Panel2.Controls.Add(movimientos);
        var barra = Ui.Barra();
        if (permisos.OperarBodega)
            barra.Controls.Add(Ui.Boton("Registrar entrada / salida", Mover));
        barra.Controls.Add(Ui.Boton("Actualizar", Refrescar));
        Controls.Add(split);
        Controls.Add(barra);
        mediator.Registrar(Refrescar);
    }

    public async Task Refrescar()
    {
        var datos = await operaciones.Consultar(async s => (Productos: await s.GetRequiredService<IRepositorio>().Productos(), Movimientos: await s.GetRequiredService<IConsultasOperativas>().Movimientos()));
        Ui.Cargar(stock, datos.Productos.OrderBy(p => p.Sku));
        Ui.Cargar(movimientos, datos.Movimientos.OrderByDescending(m => m.Fecha).Select(m => new { Sku = datos.Productos.Single(p => p.Id == m.ProductoId).Sku, m.Cantidad, m.Motivo, m.Referencia, m.Fecha }));
    }

    private async Task Mover()
    {
        var producto = stock.CurrentRow?.DataBoundItem as Producto ?? throw new InvalidOperationException("Seleccione producto.");
        using var d = new DialogoCampos($"Movimiento {producto.Sku}").Numero("Cantidad con signo", 1, 0, -1000000).Texto("Motivo", "AJUSTE").Texto("Referencia", Guid.NewGuid().ToString());
        if (d.ShowDialog(this) != DialogResult.OK || !Ui.Confirmar("Registrar este movimiento de inventario?"))
            return;
        await operaciones.Ejecutar(s => s.GetRequiredService<ServicioInventario>().Mover(producto.Id, (int)d.Numero("Cantidad con signo"), d.Texto("Motivo"), d.Texto("Referencia")));
        await mediator.Actualizar();
    }
}
