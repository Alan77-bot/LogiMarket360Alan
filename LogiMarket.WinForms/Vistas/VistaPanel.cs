using LogiMarket.Application.Contratos;
using LogiMarket.Application.Eventos;
using LogiMarket.Application.Pedidos;
using LogiMarket.Domain.Entidades;
using LogiMarket.Domain.Enums;
using LogiMarket.WinForms.Compartido;
using Microsoft.Extensions.DependencyInjection;

namespace LogiMarket.WinForms.Vistas;
// Consulta datos reales y metricas del Singleton; Mediator mantiene los indicadores actualizados.
// Esta vista presenta consultas; las reglas de venta siguen en Application y Domain.
public sealed class VistaPanel : UserControl
{
    private readonly OperacionesUi operaciones;
    private readonly MetricasSesion metricas;
    private readonly Label totalPedidos = Valor(), pendientes = Valor(), productos = Valor(), alertas = Valor();
    private readonly Label actualizado = Texto("Cargando datos...", 9);
    private readonly Label sesion = Texto("", 10);
    private readonly Label estadoInventario = Texto("", 10);
    private readonly DataGridView recientes = Ui.Grilla();
    private readonly DataGridView stock = Ui.Grilla();
    private readonly Dictionary<EstadoPedido, (Label Cantidad, ProgressBar Barra)> estados = new();
    public VistaPanel(OperacionesUi operaciones, FormularioPrincipalMediator mediator, MetricasSesion metricas)
    {
        this.operaciones = operaciones;
        this.metricas = metricas;
        Dock = DockStyle.Fill;
        BackColor = Color.FromArgb(243, 246, 250);
        AutoScroll = true;
        var tabla = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            AutoSize = true,
            ColumnCount = 1,
            Padding = new Padding(8)
        };
        tabla.ColumnStyles.Add(new(SizeType.Percent, 100));
        var cabecera = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Height = 66,
            ColumnCount = 2,
            Margin = new Padding(0, 0, 0, 12)
        };
        cabecera.ColumnStyles.Add(new(SizeType.Percent, 100));
        cabecera.ColumnStyles.Add(new(SizeType.Absolute, 130));
        var bienvenida = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false
        };
        bienvenida.Controls.Add(Texto("Resumen de la operacion", 17, true));
        bienvenida.Controls.Add(actualizado);
        cabecera.Controls.Add(bienvenida, 0, 0);
        var refrescar = Ui.Boton("Actualizar", Refrescar);
        refrescar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
        cabecera.Controls.Add(refrescar, 1, 0);
        tabla.Controls.Add(cabecera);
        var tarjetas = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Height = 130,
            ColumnCount = 4,
            Margin = new Padding(0, 0, 0, 14)
        };
        for (var i = 0; i < 4; i++)
            tarjetas.ColumnStyles.Add(new(SizeType.Percent, 25));
        tarjetas.Controls.Add(Tarjeta("PEDIDOS", totalPedidos, "En la base de datos", Ui.Azul), 0, 0);
        tarjetas.Controls.Add(Tarjeta("POR DESPACHAR", pendientes, "Pagados sin despacho", Color.FromArgb(35, 113, 185)), 1, 0);
        tarjetas.Controls.Add(Tarjeta("PRODUCTOS", productos, "En el catalogo", Color.FromArgb(27, 132, 111)), 2, 0);
        tarjetas.Controls.Add(Tarjeta("STOCK BAJO", alertas, "Hasta 3 unidades", Color.FromArgb(174, 94, 20)), 3, 0);
        tabla.Controls.Add(tarjetas);
        var centro = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Height = 282,
            ColumnCount = 2,
            Margin = new Padding(0, 0, 0, 14)
        };
        centro.ColumnStyles.Add(new(SizeType.Percent, 50));
        centro.ColumnStyles.Add(new(SizeType.Percent, 50));
        var flujo = Seccion("Pedidos por estado");
        var filas = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 3,
            RowCount = 6,
            Padding = new Padding(0, 4, 0, 0)
        };
        filas.ColumnStyles.Add(new(SizeType.Absolute, 110));
        filas.ColumnStyles.Add(new(SizeType.Percent, 100));
        filas.ColumnStyles.Add(new(SizeType.Absolute, 42));
        // Se usa el enum del dominio para mantener los estados visibles en un solo lugar.
        foreach (var estado in Enum.GetValues<EstadoPedido>())
        {
            var fila = estados.Count;
            filas.RowStyles.Add(new(SizeType.Percent, 100f / 6));
            var cantidad = Texto("0", 10, true);
            cantidad.Anchor = AnchorStyles.Right;
            var barra = new ProgressBar
            {
                Dock = DockStyle.Fill,
                Margin = new Padding(8, 10, 8, 10)
            };
            var nombre = Texto(estado.ToString(), 9);
            nombre.Anchor = AnchorStyles.Left;
            filas.Controls.Add(nombre, 0, fila);
            filas.Controls.Add(barra, 1, fila);
            filas.Controls.Add(cantidad, 2, fila);
            estados.Add(estado, (cantidad, barra));
        }

        flujo.Controls.Add(filas, 0, 1);
        var inventario = Seccion("Alertas de inventario");
        inventario.RowCount = 3;
        inventario.RowStyles.Add(new(SizeType.Absolute, 46));
        estadoInventario.Dock = DockStyle.Fill;
        estadoInventario.AutoSize = false;
        inventario.Controls.Add(stock, 0, 1);
        inventario.Controls.Add(estadoInventario, 0, 2);
        centro.Controls.Add(flujo, 0, 0);
        centro.Controls.Add(inventario, 1, 0);
        tabla.Controls.Add(centro);
        var ultimos = Seccion("Ultimos pedidos");
        ultimos.Height = 238;
        ultimos.Controls.Add(recientes, 0, 1);
        tabla.Controls.Add(ultimos);
        sesion.Margin = new Padding(4, 12, 4, 12);
        tabla.Controls.Add(sesion);
        Controls.Add(tabla);
        // Mediator conserva el refresco automatico al cambiar pedidos o inventario.
        mediator.Registrar(Refrescar);
    }

    public async Task Refrescar()
    {
        var datos = await operaciones.Consultar(async s => (Pedidos: await s.GetRequiredService<ServicioPedidos>().Consultar(), Productos: await s.GetRequiredService<IRepositorio>().Productos()));
        totalPedidos.Text = datos.Pedidos.Count.ToString();
        pendientes.Text = datos.Pedidos.Count(p => p.Estado == EstadoPedido.PAGADO).ToString();
        productos.Text = datos.Productos.Count.ToString();
        var bajos = datos.Productos.Where(p => p.Stock <= 3).OrderBy(p => p.Stock).ThenBy(p => p.Nombre).ToList();
        alertas.Text = bajos.Count.ToString();
        foreach (var(estado, controles)in estados)
        {
            var cantidad = datos.Pedidos.Count(p => p.Estado == estado);
            controles.Cantidad.Text = cantidad.ToString();
            controles.Barra.Value = datos.Pedidos.Count == 0 ? 0 : (int)(100L * cantidad / datos.Pedidos.Count);
        }

        Ui.Cargar(stock, bajos.Select(p => new { p.Sku, Producto = p.Nombre, Disponible = p.Stock }));
        estadoInventario.Text = bajos.Count == 0 ? "Sin alertas de stock. Inventario disponible." : "Revisa estas existencias antes de registrar nuevas ventas.";
        estadoInventario.ForeColor = bajos.Count == 0 ? Color.FromArgb(27, 132, 111) : Color.FromArgb(150, 79, 17);
        Ui.Cargar(recientes, datos.Pedidos.OrderByDescending(p => p.Fecha).Take(5).Select(p => new { p.Fecha, Cliente = p.ClienteNombre, p.Estado, p.Total }));
        sesion.Text = $"Actividad de esta sesion   |   Pagos: {metricas.Pagos}     Despachos: {metricas.Despachos}     Devoluciones: {metricas.Devoluciones}";
        actualizado.Text = $"Vista actualizada a las {DateTime.Now:HH:mm:ss}  |  Datos guardados en el sistema";
    }

    private static Label Texto(string texto, float tamano, bool negrita = false) => new()
    {
        Text = texto,
        AutoSize = true,
        ForeColor = Color.FromArgb(60, 76, 96),
        Font = new Font("Segoe UI", tamano, negrita ? FontStyle.Bold : FontStyle.Regular),
        Margin = new Padding(0, 0, 0, 6)
    };
    private static Label Valor() => Texto("0", 26, true);
    private static Panel Tarjeta(string titulo, Label valor, string detalle, Color color)
    {
        var tarjeta = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = Color.White,
            Margin = new Padding(0, 0, 10, 0),
            Padding = new Padding(14, 10, 10, 8)
        };
        var contenido = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3
        };
        contenido.ColumnStyles.Add(new(SizeType.Percent, 100));
        contenido.RowStyles.Add(new(SizeType.Absolute, 22));
        contenido.RowStyles.Add(new(SizeType.Absolute, 52));
        contenido.RowStyles.Add(new(SizeType.Percent, 100));
        var etiqueta = Texto(titulo, 9, true);
        etiqueta.ForeColor = color;
        valor.ForeColor = color;
        var descripcion = Texto(detalle, 8);
        descripcion.AutoSize = false;
        descripcion.Dock = DockStyle.Fill;
        contenido.Controls.Add(etiqueta, 0, 0);
        contenido.Controls.Add(valor, 0, 1);
        contenido.Controls.Add(descripcion, 0, 2);
        tarjeta.Controls.Add(contenido);
        tarjeta.Controls.Add(new Panel { Dock = DockStyle.Left, Width = 3, BackColor = color });
        return tarjeta;
    }

    private static TableLayoutPanel Seccion(string titulo)
    {
        var panel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            BackColor = Color.White,
            Padding = new Padding(14),
            Margin = new Padding(0, 0, 10, 0)
        };
        panel.ColumnStyles.Add(new(SizeType.Percent, 100));
        panel.RowStyles.Add(new(SizeType.Absolute, 34));
        panel.RowStyles.Add(new(SizeType.Percent, 100));
        var encabezado = Texto(titulo, 11, true);
        encabezado.AutoSize = false;
        encabezado.Dock = DockStyle.Fill;
        encabezado.Height = 34;
        // Las filas reservan espacio para el titulo y el contenido al redimensionar.
        panel.Controls.Add(encabezado, 0, 0);
        return panel;
    }
}
