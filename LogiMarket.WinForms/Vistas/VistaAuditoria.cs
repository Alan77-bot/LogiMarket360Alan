using LogiMarket.Application.Catalogo;
using LogiMarket.Application.Contratos;
using LogiMarket.Application.Seguridad;
using LogiMarket.WinForms.Compartido;
using LogiMarket.WinForms.Formularios;
using Microsoft.Extensions.DependencyInjection;

namespace LogiMarket.WinForms.Vistas;
// Demuestra Factory Method, Adapter, Observer y Flyweight con colaboradores reales y escenarios de fallo controlados.
public sealed class VistaAuditoria : UserControl
{
    private readonly OperacionesUi operaciones;
    private readonly OpcionesDemostracion opciones;
    private readonly SesionActual sesion;
    private readonly DataGridView logs = Ui.Grilla();
    private readonly DataGridView eventos = Ui.Grilla();
    private readonly DataGridView avisos = Ui.Grilla();
    private readonly DataGridView flyweight = Ui.Grilla();
    public VistaAuditoria(OperacionesUi operaciones, FormularioPrincipalMediator mediator, OpcionesDemostracion opciones, SesionActual sesion)
    {
        this.operaciones = operaciones;
        this.opciones = opciones;
        this.sesion = sesion;
        Dock = DockStyle.Fill;
        var tabs = new TabControl
        {
            Dock = DockStyle.Fill
        };
        foreach (var(nombre, grilla)in new[]
        {
            ("Auditoria", logs),
            ("Outbox", eventos),
            ("Avisos simulados", avisos),
            ("Flyweight", flyweight)
        }

        )
        {
            var tab = new TabPage(nombre);
            tab.Controls.Add(grilla);
            tabs.TabPages.Add(tab);
        }

        var barra = Ui.Barra();
        barra.Controls.Add(Ui.Boton("Opciones de demostracion", Configurar));
        barra.Controls.Add(Ui.Boton("Reintentar Outbox", async () =>
        {
            await operaciones.Ejecutar(async s =>
            {
                await s.GetRequiredService<Autorizador>().Exigir();
                await s.GetRequiredService<IProcesadorOutbox>().Procesar();
            });
            await mediator.Actualizar();
        }));
        barra.Controls.Add(Ui.Boton("Demostrar 1.000 filas", DemostrarFlyweight));
        Controls.Add(tabs);
        Controls.Add(barra);
        mediator.Registrar(Refrescar);
    }

    public async Task Refrescar()
    {
        var datos = await operaciones.Consultar(async s => (Logs: await s.GetRequiredService<IConsultasOperativas>().Auditorias(), Eventos: await s.GetRequiredService<IConsultasOperativas>().Eventos(), Avisos: await s.GetRequiredService<IConsultasOperativas>().Avisos()));
        Ui.Cargar(logs, datos.Logs.OrderByDescending(l => l.Fecha));
        Ui.Cargar(eventos, datos.Eventos);
        Ui.Cargar(avisos, datos.Avisos);
    }

    private async Task Configurar()
    {
        await operaciones.Consultar(async s =>
        {
            await s.GetRequiredService<Autorizador>().Exigir();
            return true;
        });
        using var d = new DialogoCampos("Demostracion local").Casilla("Proveedor alternativo", opciones.ProveedorAlternativo).Casilla("Rechazar cobro", opciones.RechazarCobro).Casilla("Fallar notificacion", opciones.FallarNotificacion).Lista("Canal", ["EMAIL", "SMS"], opciones.Canal);
        if (d.ShowDialog(this) != DialogResult.OK)
            return;
        opciones.ProveedorAlternativo = d.Marcado("Proveedor alternativo");
        opciones.RechazarCobro = d.Marcado("Rechazar cobro");
        opciones.FallarNotificacion = d.Marcado("Fallar notificacion");
        opciones.Canal = d.Seleccion<string>("Canal");
        await operaciones.Ejecutar(s => s.GetRequiredService<IBitacora>().Registrar(sesion.Nombre, "DEMOSTRACION", $"Alternativo={opciones.ProveedorAlternativo}; Rechazo={opciones.RechazarCobro}; FalloOutbox={opciones.FallarNotificacion}", Guid.NewGuid().ToString()));
    }

    private Task DemostrarFlyweight()
    {
        var fabrica = new CatalogoVisualFlyweightFactory();
        // La medicion compara estructuras externas equivalentes en el mismo hilo.
        var antes = GC.GetAllocatedBytesForCurrentThread();
        var sin = Enumerable.Range(0, 1000).Select(i => new FilaVisual(i, new MetadatosSku("P01", "Mouse USB", "Accesorios", "Catalogo de demostracion"), 1, 20m)).ToArray();
        var bytesSin = GC.GetAllocatedBytesForCurrentThread() - antes;
        antes = GC.GetAllocatedBytesForCurrentThread();
        var con = Enumerable.Range(0, 1000).Select(i => new FilaVisual(i, fabrica.Obtener("P01", "Mouse USB"), 1, 20m)).ToArray();
        var bytesCon = GC.GetAllocatedBytesForCurrentThread() - antes;
        Ui.Cargar(flyweight, con);
        GC.KeepAlive(sin);
        if (flyweight.Parent is TabPage pagina && pagina.Parent is TabControl pestanas)
            pestanas.SelectedTab = pagina;
        MessageBox.Show($"Filas: 1000\nInstancias: {fabrica.Instancias}\nReferenceEquals: {ReferenceEquals(con[0].Metadatos, con[999].Metadatos)}\nSin compartir: {bytesSin} bytes asignados\nCompartido: {bytesCon} bytes asignados\nNo representa memoria total del proceso.");
        return Task.CompletedTask;
    }

    private sealed record FilaVisual(int Fila, MetadatosSku Metadatos, int Cantidad, decimal Precio);
}
