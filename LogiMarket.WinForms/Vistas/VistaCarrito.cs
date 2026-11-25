using LogiMarket.Application.Catalogo;
using LogiMarket.Application.Clientes;
using LogiMarket.Application.DTOs;
using LogiMarket.Application.Pedidos;
using LogiMarket.Domain.Entregas;
using LogiMarket.Domain.Enums;
using LogiMarket.Domain.Pedidos;
using LogiMarket.WinForms.Compartido;
using Microsoft.Extensions.DependencyInjection;

namespace LogiMarket.WinForms.Vistas;
// Presenta Builder y Memento prepara el pedido y permite deshacer/rehacer sin calcular reglas dentro del formulario
public sealed class VistaCarrito : UserControl
{
    private sealed record OpcionCliente(Guid Id, string Nombre)
    {
        public override string ToString() => Nombre;
    }

    private readonly OperacionesUi operaciones;
    private readonly FormularioPrincipalMediator mediator;
    private readonly ComboBox cliente = new()
    {
        Width = 180,
        DropDownStyle = ComboBoxStyle.DropDownList
    };
    private readonly ComboBox entrega = new()
    {
        Width = 130,
        DropDownStyle = ComboBoxStyle.DropDownList
    };
    private readonly ComboBox sku = new()
    {
        Width = 180,
        DropDownStyle = ComboBoxStyle.DropDownList
    };
    private readonly NumericUpDown cantidad = new()
    {
        Minimum = 1,
        Maximum = 10000,
        Value = 1,
        Width = 70
    };
    private readonly DataGridView grid = Ui.Grilla();
    private readonly Label totales = new()
    {
        Dock = DockStyle.Bottom,
        Height = 62,
        Font = new Font("Segoe UI", 12),
        Padding = new Padding(12)
    };
    private List<LineaEntrada> lineas = [];
    private HistorialBorrador historial = new();
    private Guid? idEdicion;
    public bool TieneCambios { get; private set; }

    public VistaCarrito(OperacionesUi operaciones, FormularioPrincipalMediator mediator)
    {
        this.operaciones = operaciones;
        this.mediator = mediator;
        Dock = DockStyle.Fill;
        entrega.Items.AddRange(Enum.GetValues<TipoEntrega>().Cast<object>().ToArray());
        entrega.SelectedIndex = 0;
        var barra = Ui.Barra();
        barra.Controls.AddRange([new Label { Text = "Cliente", AutoSize = true }, cliente, entrega, sku, cantidad]);
        barra.Controls.Add(Ui.Boton("Agregar / cambiar cantidad", Agregar));
        barra.Controls.Add(Ui.Boton("Quitar linea", Quitar));
        barra.Controls.Add(Ui.Boton("Recalcular", Cambiar));
        barra.Controls.Add(Ui.Boton("Deshacer", async () => await Restaurar(historial.DeshacerCompleto())));
        barra.Controls.Add(Ui.Boton("Rehacer", async () => await Restaurar(historial.RehacerCompleto())));
        barra.Controls.Add(Ui.Boton("Guardar borrador", Guardar));
        barra.Controls.Add(Ui.Boton("Nuevo", Nuevo));
        Controls.Add(grid);
        Controls.Add(totales);
        Controls.Add(barra);
        mediator.Registrar(Refrescar);
        mediator.BorradorSolicitado += Cargar;
        grid.SelectionChanged += (_, _) =>
        {
            if (grid.CurrentRow?.DataBoundItem is LineaEntrada linea)
            {
                sku.SelectedItem = linea.Sku;
                cantidad.Value = linea.Cantidad;
            }
        };
    }

    public async Task Refrescar()
    {
        var datos = await operaciones.Consultar(async s => (Clientes: await s.GetRequiredService<ServicioClientes>().Consultar(), Catalogo: await s.GetRequiredService<ServicioCatalogo>().Consultar()));
        var id = (cliente.SelectedItem as OpcionCliente)?.Id;
        cliente.Items.Clear();
        cliente.Items.AddRange(datos.Clientes.Where(c => c.Activo || c.Id == id).Select(c => new OpcionCliente(c.Id, c.Nombre)).Cast<object>().ToArray());
        cliente.SelectedItem = cliente.Items.Cast<OpcionCliente>().FirstOrDefault(c => c.Id == id);
        if (cliente.SelectedIndex < 0 && cliente.Items.Count > 0)
            cliente.SelectedIndex = 0;
        var seleccion = sku.SelectedItem;
        sku.Items.Clear();
        sku.Items.AddRange(datos.Catalogo.Select(c => (object)c.Sku).ToArray());
        sku.SelectedItem = seleccion;
        if (sku.SelectedIndex < 0 && sku.Items.Count > 0)
            sku.SelectedIndex = 0;
        if (idEdicion is Guid pedidoId)
        {
            var guardado = await operaciones.Consultar(async s => (await s.GetRequiredService<ServicioPedidos>().Consultar()).SingleOrDefault(p => p.Id == pedidoId));
            if (guardado is not null && guardado.Estado != EstadoPedido.BORRADOR)
            {
                historial.Bloquear();
                TieneCambios = false;
                totales.Text = $"Pedido {guardado.Estado}. Use Nuevo para otro carrito. Total USD {guardado.Total:N2}";
            }
        }
    }

    private SolicitudBorrador Entrada() => new((cliente.SelectedItem as OpcionCliente)?.Id ?? throw new ArgumentException("Seleccione cliente."), (TipoEntrega)entrega.SelectedItem!, lineas.ToArray());
    private async Task Agregar()
    {
        var seleccionado = sku.SelectedItem as string ?? throw new ArgumentException("Seleccione SKU.");
        var anteriores = lineas.ToList();
        lineas.RemoveAll(l => l.Sku == seleccionado);
        lineas.Add(new(seleccionado, (int)cantidad.Value));
        try
        {
            await Cambiar();
        }
        catch
        {
            lineas = anteriores;
            throw;
        }
    }

    private async Task Quitar()
    {
        if (grid.CurrentRow?.DataBoundItem is LineaEntrada linea)
        {
            lineas.Remove(linea);
            if (lineas.Count == 0)
            {
                historial.CapturarVacio();
                Ui.Cargar(grid, lineas);
                totales.Text = "Sin lineas";
                TieneCambios = true;
            }
            else
                await Cambiar();
        }
    }

    private async Task Cambiar()
    {
        var pedido = await operaciones.Consultar(s => s.GetRequiredService<ServicioPedidos>().Preparar(Entrada()));
        historial.Capturar(pedido);
        Mostrar(pedido);
        TieneCambios = true;
    }

    private void Mostrar(Pedido pedido)
    {
        Ui.Cargar(grid, lineas);
        totales.Text = $"Subtotal USD {pedido.Subtotal:N2}   Descuento USD {pedido.Descuento:N2}   Envio USD {pedido.Tarifa:N2}   Total USD {pedido.Total:N2}";
    }

    private Task Restaurar(Pedido? pedido)
    {
        if (pedido is null)
        {
            lineas.Clear();
            Ui.Cargar(grid, lineas);
            totales.Text = "Sin lineas";
            TieneCambios = true;
            return Task.CompletedTask;
        }

        lineas = pedido.Detalles.Select(d => new LineaEntrada(d.Sku, d.Cantidad)).ToList();
        cliente.SelectedItem = cliente.Items.Cast<OpcionCliente>().FirstOrDefault(c => c.Id == pedido.ClienteId);
        entrega.SelectedItem = pedido.Entrega;
        Mostrar(pedido);
        TieneCambios = true;
        return Task.CompletedTask;
    }

    private async Task Cargar(Pedido pedido)
    {
        pedido.ExigirEditable();
        if (TieneCambios && !Ui.Confirmar("Reemplazar los cambios del carrito?"))
            return;
        idEdicion = pedido.Id;
        historial = new();
        historial.Capturar(pedido);
        await Restaurar(pedido);
        TieneCambios = false;
        MessageBox.Show("Borrador cargado. Abra Carrito para editarlo.");
    }

    private async Task Guardar()
    {
        Guid id = default;
        await operaciones.Ejecutar(async s => id = await s.GetRequiredService<ServicioPedidos>().Guardar(Entrada(), idEdicion));
        idEdicion = id;
        TieneCambios = false;
        await mediator.Actualizar();
        MessageBox.Show($"Borrador guardado: {id}");
    }

    private Task Nuevo()
    {
        if (TieneCambios && !Ui.Confirmar("Descartar cambios sin guardar?"))
            return Task.CompletedTask;
        lineas.Clear();
        historial = new();
        idEdicion = null;
        TieneCambios = false;
        Ui.Cargar(grid, lineas);
        totales.Text = "Nuevo borrador";
        return Task.CompletedTask;
    }
}
