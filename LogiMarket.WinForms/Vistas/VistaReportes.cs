using LogiMarket.Application.Comprobantes;
using LogiMarket.Application.Contratos;
using LogiMarket.Application.Pedidos;
using LogiMarket.Application.Reportes;
using LogiMarket.Domain.Pedidos;
using LogiMarket.WinForms.Compartido;
using Microsoft.Extensions.DependencyInjection;

namespace LogiMarket.WinForms.Vistas;
// Presenta Interpreter para filtrar y Bridge para combinar documentos con vista previa o TXT
public sealed class VistaReportes : UserControl
{
    private readonly OperacionesUi operaciones;
    private readonly ISalidaDocumento archivo;
    private readonly DataGridView grid = Ui.Grilla();
    private readonly TextBox filtro = new()
    {
        Width = 400,
        PlaceholderText = "CLIENTE = Ana OR TOTAL >= 100"
    };
    public VistaReportes(OperacionesUi operaciones, FormularioPrincipalMediator mediator, ISalidaDocumento archivo)
    {
        this.operaciones = operaciones;
        this.archivo = archivo;
        Dock = DockStyle.Fill;
        var barra = Ui.Barra();
        barra.Controls.Add(filtro);
        barra.Controls.Add(Ui.Boton("Consultar", Refrescar));
        barra.Controls.Add(Ui.Boton("Venta vista previa", () => Exportar(false, false)));
        barra.Controls.Add(Ui.Boton("Venta TXT", () => Exportar(false, true)));
        barra.Controls.Add(Ui.Boton("Devolucion vista previa", () => Exportar(true, false)));
        barra.Controls.Add(Ui.Boton("Devolucion TXT", () => Exportar(true, true)));
        Controls.Add(grid);
        Controls.Add(barra);
        mediator.Registrar(Refrescar);
    }

    public async Task Refrescar()
    {
        var pedidos = await operaciones.Consultar(s => s.GetRequiredService<ServicioPedidos>().Consultar());
        Ui.Cargar(grid, string.IsNullOrWhiteSpace(filtro.Text) ? pedidos : pedidos.Where(new InterpreterFiltro().Analizar(filtro.Text).Evaluar));
        foreach (var n in new[]
        {
            "Detalles",
            "Historial"
        }

        )
            if (grid.Columns.Contains(n))
                grid.Columns[n].Visible = false;
    }

    private async Task Exportar(bool devolucion, bool txt)
    {
        var pedido = grid.CurrentRow?.DataBoundItem as Pedido ?? throw new InvalidOperationException("Seleccione pedido.");
        var salida = txt ? archivo : new SalidaVistaPrevia(this);
        Comprobante documento;
        if (devolucion)
        {
            var registro = await operaciones.Consultar(s => s.GetRequiredService<IRepositorio>().DevolucionPorPedido(pedido.Id));
            documento = new ComprobanteDevolucion(registro ?? throw new InvalidOperationException("Pedido sin devolucion."), salida);
        }
        else
            documento = new ComprobanteVenta(pedido, salida);
        await documento.Exportar();
        if (txt)
            MessageBox.Show("Archivo TXT generado en la carpeta de comprobantes indicada en README.");
    }
}
