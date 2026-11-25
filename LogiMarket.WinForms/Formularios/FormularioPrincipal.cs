using LogiMarket.Application.Comprobantes;
using LogiMarket.Application.Contratos;
using LogiMarket.Application.Eventos;
using LogiMarket.Application.Seguridad;
using LogiMarket.Domain.Enums;
using LogiMarket.WinForms.Compartido;
using LogiMarket.WinForms.Vistas;
using Microsoft.Extensions.DependencyInjection;

namespace LogiMarket.WinForms.Formularios;
// Construye solo las vistas del rol actual La navegacion no crea pantallas ocultas que luego se refresquen por Mediator
public sealed class FormularioPrincipal : Form
{
    private readonly Panel contenido = new()
    {
        Dock = DockStyle.Fill,
        Padding = new Padding(12)
    };
    private readonly Label titulo = new()
    {
        Dock = DockStyle.Top,
        Height = 76,
        Padding = new Padding(18),
        Font = new Font("Segoe UI", 19, FontStyle.Bold), ForeColor = Ui.Azul, BackColor = Color.White
    };
    private readonly VistaCarrito? carrito;
    public FormularioPrincipal(OperacionesUi operaciones, SesionActual sesion, MetricasSesion metricas, OpcionesDemostracion opciones, ISalidaDocumento archivo)
    {
        Text = "LogiMarket 360 Alan";
        Width = 1400;
        Height = 850;
        MinimumSize = new Size(1050, 650);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 10);
        BackColor = Ui.Fondo;
        AutoScaleMode = AutoScaleMode.Dpi;
        var mediator = new FormularioPrincipalMediator();
        var menu = new FlowLayoutPanel
        {
            Dock = DockStyle.Left,
            Width = 240,
            FlowDirection = FlowDirection.TopDown,
            WrapContents = false,
            AutoScroll = true,
            BackColor = Ui.Azul,
            Padding = new Padding(14, 20, 14, 14)
        };
        menu.Controls.Add(new Label { Text = $"LOGIMARKET 360\nALAN\n\n{sesion.Nombre} | {sesion.Rol}", ForeColor = Color.White, Height = 120, Width = 190, Font = new Font("Segoe UI", 11, FontStyle.Bold) });
        var permisos = new PermisosVista(sesion.Rol);
        if (!permisos.Consultar)
            throw new UnauthorizedAccessException("Inicie sesion con un rol valido.");
        if (permisos.Vender)
            carrito = new(operaciones, mediator);
        // Las fabricas evitan crear y registrar vistas ajenas al rol en el Mediator
        var definiciones = new (string Nombre, Func<UserControl> Crear)[]
        {
            ("Panel general", () => new VistaPanel(operaciones, mediator, metricas)),
            ("Catalogo y kits", () => new VistaCatalogo(operaciones, mediator, permisos)),
            ("Clientes", () => new VistaClientes(operaciones, mediator, permisos)),
            ("Carrito", () => carrito!),
            ("Pedidos", () => new VistaPedidos(operaciones, mediator, "Pedidos", permisos)),
            ("Cobro", () => new VistaCobro(operaciones, mediator, permisos)),
            ("Despacho y entrega", () => new VistaDespacho(operaciones, mediator, permisos)),
            ("Devoluciones", () => new VistaDevoluciones(operaciones, mediator, permisos)),
            ("Inventario", () => new VistaInventario(operaciones, mediator, permisos)),
            ("Reportes", () => new VistaReportes(operaciones, mediator, archivo)),
            ("Auditoria y demo", () => new VistaAuditoria(operaciones, mediator, opciones, sesion))
        };
        Button? seleccionMenu = null;
        var vistas = definiciones.Where(d => permisos.Ver(d.Nombre)).Select(d => (d.Nombre, Vista: d.Crear())).ToArray();
        foreach (var (nombre, vista) in vistas)
        {
            Ui.AplicarTema(vista);
            var boton = Ui.Boton(nombre, () =>
            {
                contenido.Controls.Clear();
                contenido.Controls.Add(vista);
                titulo.Text = nombre;
                return Task.CompletedTask;
            });
                        Ui.Menu(boton, nombre == "Panel general");
            if (nombre == "Panel general") seleccionMenu = boton;
            boton.Click += (_, _) => {
                if (seleccionMenu is not null) Ui.Menu(seleccionMenu, false);
                Ui.Menu(boton, true); seleccionMenu = boton;
            };
            menu.Controls.Add(boton);
        }

        menu.Controls.Add(Ui.Boton("Cerrar sesion", () =>
        {
            Close();
            return Task.CompletedTask;
        }));
        Controls.Add(contenido);
        Controls.Add(titulo);
        Controls.Add(menu);
        contenido.Controls.Add(vistas[0].Vista);
        titulo.Text = vistas[0].Nombre;
        Shown += async (_, _) =>
        {
            try
            {
                await mediator.Actualizar();
            }
            catch (Exception ex)
            {
                var directorio = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "LogiMarket360Alan");
                File.AppendAllText(Path.Combine(directorio, "ui-error.log"), DateTimeOffset.UtcNow + " " + ex + Environment.NewLine);
                MessageBox.Show(ex.Message + " " + ex.GetBaseException().Message);
            }
        };
        FormClosing += (_, e) =>
        {
            if (carrito?.TieneCambios == true && !Ui.Confirmar("Hay cambios sin guardar en el carrito. Cerrar y descartarlos?"))
                e.Cancel = true;
        };
        FormClosed += (_, _) =>
        {
            foreach (var(_, vista)in vistas)
                vista.Dispose();
        };
    }
}
