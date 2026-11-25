using LogiMarket.Application.Comandos;
using LogiMarket.Application.Contratos;
using LogiMarket.Application.Logistica;
using LogiMarket.Application.Pedidos;
using LogiMarket.Application.Reportes;
using LogiMarket.Domain.Entidades;
using LogiMarket.Domain.Enums;
using LogiMarket.Domain.Pedidos;
using LogiMarket.WinForms.Compartido;
using LogiMarket.WinForms.Formularios;
using Microsoft.Extensions.DependencyInjection;

namespace LogiMarket.WinForms.Vistas;
// La UI invoca Commands y servicios State y los permisos se verifican de nuevo fuera de los botones
public class VistaPedidos : UserControl
{
    private readonly OperacionesUi operaciones;
    private readonly FormularioPrincipalMediator mediator;
    private readonly DataGridView grid = Ui.Grilla();
    private readonly TextBox filtro = new()
    {
        Width = 390,
        PlaceholderText = "ESTADO = PAGADO AND TOTAL >= 50"
    };
    private readonly string modo;
    public VistaPedidos(OperacionesUi operaciones, FormularioPrincipalMediator mediator, string modo, PermisosVista permisos)
    {
        this.operaciones = operaciones;
        this.mediator = mediator;
        this.modo = modo;
        Dock = DockStyle.Fill;
        var barra = Ui.Barra();
        barra.Controls.Add(filtro);
        barra.Controls.Add(Ui.Boton("Filtrar", Refrescar));
        barra.Controls.Add(Ui.Boton("Detalle e historial", Detalle));
        if (modo == "Pedidos" && permisos.Vender)
        {
            barra.Controls.Add(Ui.Boton("Editar borrador", () => mediator.EditarBorrador(Seleccion())));
            barra.Controls.Add(Ui.Boton("Clonar borrador", async () => await Accion(s => s.GetRequiredService<ServicioPedidos>().Clonar(Seleccion().Id))));
            barra.Controls.Add(Ui.Boton("Validar", async () => await Accion(s => s.GetRequiredService<ServicioPedidos>().Validar(Seleccion().Id))));
            barra.Controls.Add(Ui.Boton("Cancelar pedido", async () =>
            {
                var id = Seleccion().Id;
                if (Ui.Confirmar("Cancelar pedido?"))
                    await Accion(s => s.GetRequiredService<InvocadorComandos>().Ejecutar(new CancelarPedidoCommand(s.GetRequiredService<ServicioPedidos>(), id)));
            }));
        }

        if (modo == "Cobro" && permisos.Vender)
            barra.Controls.Add(Ui.Boton("Confirmar cobro simulado", Cobrar));
        if (modo == "Despacho" && permisos.OperarBodega)
        {
            barra.Controls.Add(Ui.Boton("Despachar", async () =>
            {
                var id = Seleccion().Id;
                if (Ui.Confirmar("Registrar despacho?"))
                    await Accion(s => s.GetRequiredService<InvocadorComandos>().Ejecutar(new DespacharPedidoCommand(s.GetRequiredService<ServicioDespachos>(), id)));
            }));
            barra.Controls.Add(Ui.Boton("Marcar entregado", async () =>
            {
                var id = Seleccion().Id;
                if (Ui.Confirmar("Registrar entrega?"))
                    await Accion(s => s.GetRequiredService<ServicioDespachos>().Entregar(id));
            }));
        }

        if (modo == "Despacho")
            barra.Controls.Add(Ui.Boton("Seguimiento", Seguimiento));
        if (modo == "Devoluciones" && permisos.Vender)
            barra.Controls.Add(Ui.Boton("Devolucion total", Devolver));
        Controls.Add(grid);
        Controls.Add(barra);
        mediator.Registrar(Refrescar);
    }

    private Pedido Seleccion() => grid.CurrentRow?.DataBoundItem as Pedido ?? throw new InvalidOperationException("Seleccione pedido.");
    public async Task Refrescar()
    {
        var pedidos = await operaciones.Consultar(s => s.GetRequiredService<ServicioPedidos>().Consultar());
        IEnumerable<Pedido> filas = pedidos;
        if (!string.IsNullOrWhiteSpace(filtro.Text))
        {
            var expresion = new InterpreterFiltro().Analizar(filtro.Text);
            filas = filas.Where(expresion.Evaluar);
        }

        if (modo == "Cobro")
            filas = filas.Where(p => p.Estado is EstadoPedido.VALIDADO or EstadoPedido.PAGADO);
        if (modo == "Despacho")
            filas = filas.Where(p => p.Estado is EstadoPedido.PAGADO or EstadoPedido.DESPACHADO or EstadoPedido.ENTREGADO);
        if (modo == "Devoluciones")
            filas = filas.Where(p => p.Estado == EstadoPedido.ENTREGADO);
        Ui.Cargar(grid, filas);
        foreach (var nombre in new[]
        {
            "Detalles",
            "Historial"
        }

        )
            if (grid.Columns.Contains(nombre))
                grid.Columns[nombre].Visible = false;
    }

    private async Task Accion(Func<IServiceProvider, Task> accion)
    {
        await operaciones.Ejecutar(accion);
        await mediator.Actualizar();
    }

    private Task Detalle()
    {
        var p = Seleccion();
        var texto = string.Join("\n", p.Detalles.SelectMany(d => d.Componentes).Select(c => $"{c.Sku} x{c.Cantidad} @ {c.Precio:N2}"));
        texto += "\n\n" + string.Join("\n", p.Historial.OrderBy(h => h.Fecha).Select(h => $"{h.Fecha:g} {h.Estado} {h.Usuario}"));
        MessageBox.Show(texto, $"{p.ClienteNombre} - Total USD {p.Total:N2}");
        return Task.CompletedTask;
    }

    private async Task Cobrar()
    {
        var pedido = Seleccion();
        using var d = new DialogoCampos($"Cobro USD {pedido.Total:N2}").Texto("Clave idempotencia", pedido.Id.ToString());
        if (d.ShowDialog(this) != DialogResult.OK || !Ui.Confirmar($"Confirmar cobro simulado de USD {pedido.Total:N2}?"))
            return;
        await Accion(s => s.GetRequiredService<InvocadorComandos>().Ejecutar(new ConfirmarPedidoCommand(s.GetRequiredService<ConfirmacionPedidoFacade>(), pedido.Id, d.Texto("Clave idempotencia"))));
    }

    private async Task Devolver()
    {
        var pedido = Seleccion();
        using var d = new DialogoCampos("Devolucion total").Lista("Motivo", Enum.GetValues<MotivoDevolucion>().Cast<object>().ToArray()).Casilla("Sin uso", true);
        if (d.ShowDialog(this) != DialogResult.OK || !Ui.Confirmar($"Solicitar reembolso USD {pedido.Subtotal - pedido.Descuento:N2}, sin envio?"))
            return;
        await Accion(s => s.GetRequiredService<InvocadorComandos>().Ejecutar(new ProcesarDevolucionCommand(d.Seleccion<MotivoDevolucion>("Motivo") == MotivoDevolucion.DEFECTO ? s.GetRequiredService<DevolucionDefecto>() : s.GetRequiredService<DevolucionArrepentimiento>(), pedido.Id, d.Marcado("Sin uso"))));
    }

    private async Task Seguimiento()
    {
        var id = Seleccion().Id;
        var registros = await operaciones.Consultar(s => s.GetRequiredService<IConsultasOperativas>().Despachos());
        var envio = registros.SingleOrDefault(d => d.PedidoId == id);
        MessageBox.Show(envio is null ? "Sin despacho" : $"Guia: {envio.Guia}\nSalida: {envio.Fecha:g}\nEstimada: {envio.Estimada:g}");
    }
}
