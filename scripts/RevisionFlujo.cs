using LogiMarket.Application.Catalogo;
using LogiMarket.Application.Comandos;
using LogiMarket.Application.Comprobantes;
using LogiMarket.Application.Contratos;
using LogiMarket.Application.DTOs;
using LogiMarket.Application.Eventos;
using LogiMarket.Application.Logistica;
using LogiMarket.Application.Pedidos;
using LogiMarket.Application.Seguridad;
using LogiMarket.Domain.Enums;
using LogiMarket.Domain.Operaciones;
using LogiMarket.Infrastructure.Documentos;
using LogiMarket.Infrastructure.InyeccionDependencias;
using LogiMarket.Infrastructure.Persistencia;
using LogiMarket.WinForms.Compartido;
using LogiMarket.WinForms.Formularios;
using Microsoft.Extensions.DependencyInjection;

// Ejecutor de evidencia: usa los servicios y formularios reales en una base temporal.
internal static class RevisionFlujo
{
    [STAThread]
    private static int Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        var salida = Path.GetFullPath(args[0]); Directory.CreateDirectory(salida);
        var rutaDb = Path.Combine(salida, "flujo.db");
        var servicios = new ServiceCollection().AgregarLogiMarket(rutaDb);
        servicios.AddSingleton<ISalidaDocumento>(new SalidaArchivoTxt(salida));
        using var proveedor = servicios.BuildServiceProvider();
        using (var scope = proveedor.CreateScope()) {
            scope.ServiceProvider.GetRequiredService<Inicializador>().Ejecutar().GetAwaiter().GetResult();
            scope.ServiceProvider.GetRequiredService<Autenticacion>().Entrar("admin", "Demo123!").GetAwaiter().GetResult();
        }
        var operaciones = new OperacionesUi(proveedor);
        var metricas = proveedor.GetRequiredService<MetricasSesion>();
        var registro = new List<string>(); var fallo = false; Guid id = default;
        using var form = new FormularioPrincipal(operaciones, proveedor.GetRequiredService<SesionActual>(), metricas, proveedor.GetRequiredService<OpcionesDemostracion>(), proveedor.GetRequiredService<ISalidaDocumento>());
        form.Shown += async (_, _) => {
            try {
                await Task.Delay(250);
                var clienteId = await operaciones.Consultar(async s => (await s.GetRequiredService<IRepositorio>().Clientes()).Single(c => c.Nombre == "Ana").Id);
                await operaciones.Ejecutar(async s => id = await s.GetRequiredService<ServicioPedidos>().Guardar(new(clienteId, TipoEntrega.ESTANDAR, [new("KIT_TELETRABAJO", 1)])));
                await operaciones.Ejecutar(s => s.GetRequiredService<ServicioPedidos>().Validar(id));
                proveedor.GetRequiredService<OpcionesDemostracion>().FallarNotificacion = true;
                await operaciones.Ejecutar(s => s.GetRequiredService<InvocadorComandos>().Ejecutar(new ConfirmarPedidoCommand(s.GetRequiredService<ConfirmacionPedidoFacade>(), id, "evidencia-ana")));
                var pedido = await operaciones.Consultar(s => s.GetRequiredService<IRepositorio>().Pedido(id));
                Exigir(pedido.Total == 59m && pedido.Estado == EstadoPedido.PAGADO, "Venta de Ana incorrecta");
                var productos = await operaciones.Consultar(s => s.GetRequiredService<IRepositorio>().Productos());
                Exigir(productos.Single(p => p.Sku == "P01").Stock == 7 && productos.Single(p => p.Sku == "P02").Stock == 4 && productos.Single(p => p.Sku == "P03").Stock == 19, "Stock incorrecto");
                registro.Add("VENTA: total 59.00; P01=7, P02=4, P03=19; estado PAGADO.");
                var eventos = await operaciones.Consultar(s => s.GetRequiredService<IConsultasOperativas>().Eventos());
                Exigir(eventos.Any(e => e.ProcesadoEn is null && e.Intentos > 0), "No se registro el fallo Outbox");
                proveedor.GetRequiredService<OpcionesDemostracion>().FallarNotificacion = false;
                await operaciones.Ejecutar(s => s.GetRequiredService<IProcesadorOutbox>().Procesar());
                await operaciones.Ejecutar(s => s.GetRequiredService<InvocadorComandos>().Ejecutar(new ConfirmarPedidoCommand(s.GetRequiredService<ConfirmacionPedidoFacade>(), id, "evidencia-ana")));
                Exigir(metricas.Pagos == 1, "Metrica duplicada");
                registro.Add("OUTBOX: fallo real y recuperacion; repetir clave conserva un pago y metrica Pagos=1.");
                await CapturarVista(form, "Cobro", Path.Combine(salida, "flujo-01-cobro.png"));
                await CapturarVista(form, "Inventario", Path.Combine(salida, "flujo-02-stock.png"));
                await operaciones.Ejecutar(s => s.GetRequiredService<ServicioDespachos>().Despachar(id));
                await operaciones.Ejecutar(s => s.GetRequiredService<ServicioDespachos>().Entregar(id));
                await CapturarVista(form, "Despacho y entrega", Path.Combine(salida, "flujo-03-entregado.png"));
                try { await operaciones.Ejecutar(s => s.GetRequiredService<ServicioPedidos>().Cancelar(id)); throw new Exception("Se permitio cancelar un entregado"); }
                catch (InvalidOperationException) { registro.Add("RECHAZO: cancelar ENTREGADO denegado; el pedido conserva su estado."); }
                Devolucion? devolucion = null;
                await operaciones.Ejecutar(async s => devolucion = await s.GetRequiredService<DevolucionDefecto>().Ejecutar(id, true));
                Exigir(devolucion!.Reembolso == 54m, "Reembolso incorrecto");
                productos = await operaciones.Consultar(s => s.GetRequiredService<IRepositorio>().Productos());
                Exigir(productos.Single(p => p.Sku == "P01").Stock == 8 && productos.Single(p => p.Sku == "P02").Stock == 5 && productos.Single(p => p.Sku == "P03").Stock == 20, "Stock no reintegrado");
                registro.Add("DEVOLUCION: 54.00 sin envio; P01=8, P02=5, P03=20.");
                await CapturarVista(form, "Inventario", Path.Combine(salida, "flujo-04-reintegro.png"));
                await CapturarVista(form, "Panel general", Path.Combine(salida, "flujo-05-metricas.png"));
                await new ComprobanteVenta(pedido, new SalidaArchivoTxt(salida)).Exportar();
                await new ComprobanteDevolucion(devolucion, new SalidaArchivoTxt(salida)).Exportar();
                await VerPreview(new ComprobanteVenta(pedido, new SalidaVistaPrevia(form)), "venta-" + pedido.Id.ToString("N"), Path.Combine(salida, "bridge-venta.png"));
                await VerPreview(new ComprobanteDevolucion(devolucion, new SalidaVistaPrevia(form)), "devolucion-" + devolucion.Id.ToString("N"), Path.Combine(salida, "bridge-devolucion.png"));
                Exigir(Directory.GetFiles(salida, "*.txt").Length >= 2, "Faltan TXT");
                registro.Add("BRIDGE: venta/devolucion en TXT y en ventanas reales de vista previa.");
                var fabrica = new CatalogoVisualFlyweightFactory();
                var antes = GC.GetAllocatedBytesForCurrentThread();
                var sin = Enumerable.Range(0, 1000).Select(i => new Fila(i, new MetadatosSku("P01", "Mouse USB", "Accesorios", "Demostracion"), 1, 20m)).ToArray();
                var bytesSin = GC.GetAllocatedBytesForCurrentThread() - antes;
                antes = GC.GetAllocatedBytesForCurrentThread();
                var con = Enumerable.Range(0, 1000).Select(i => new Fila(i, fabrica.Obtener("P01", "Mouse USB"), 1, 20m)).ToArray();
                var bytesCon = GC.GetAllocatedBytesForCurrentThread() - antes;
                Exigir(ReferenceEquals(con[0].Metadatos, con[999].Metadatos) && fabrica.Instancias == 1 && bytesCon < bytesSin, "Flyweight incorrecto");
                registro.Add($"FLYWEIGHT: 1000 filas, 1 instancia compartida, ReferenceEquals=True; sin compartir={bytesSin}, compartido={bytesCon} bytes asignados en el mismo hilo. No es memoria total del proceso.");
                GC.KeepAlive(sin); GC.KeepAlive(con);
            }
            catch (Exception ex) { fallo = true; registro.Add(ex.ToString()); }
            finally { form.Close(); }
        };
        Application.Run(form);
        if (!fallo) {
            using var otro = new ServiceCollection().AgregarLogiMarket(rutaDb).BuildServiceProvider();
            using var scope = otro.CreateScope();
            scope.ServiceProvider.GetRequiredService<Autenticacion>().Entrar("consulta", "Demo123!").GetAwaiter().GetResult();
            var conservado = scope.ServiceProvider.GetRequiredService<IRepositorio>().Pedido(id).GetAwaiter().GetResult();
            Exigir(conservado.Estado == EstadoPedido.ENTREGADO && conservado.Total == 59m, "No persiste la venta");
            registro.Add("PERSISTENCIA: un nuevo contenedor DI y DbContext recuperan ENTREGADO y total 59.00 desde el mismo SQLite.");
        }
        registro.Add("RESULTADO: " + (fallo ? "FALLO" : "APROBADO"));
        File.WriteAllLines(Path.Combine(salida, "flujo-ui.txt"), registro);
        return fallo ? 1 : 0;
    }
    private static async Task CapturarVista(Form form, string nombre, string ruta)
    {
        form.Controls.OfType<FlowLayoutPanel>().Single().Controls.OfType<Button>().Single(b => b.Text == nombre).PerformClick();
        var vista = form.Controls.OfType<Panel>().Single(p => p is not FlowLayoutPanel).Controls.OfType<UserControl>().Single();
        if (vista.GetType().GetMethod("Refrescar")?.Invoke(vista, null) is Task tarea) await tarea;
        form.PerformLayout(); Application.DoEvents();
        using var imagen = new Bitmap(form.Width, form.Height);
        form.DrawToBitmap(imagen, new Rectangle(Point.Empty, form.Size)); imagen.Save(ruta);
    }
    private static async Task VerPreview(Comprobante documento, string nombre, string ruta)
    {
        var capturada = false;
        using var timer = new System.Windows.Forms.Timer { Interval = 150 };
        timer.Tick += (_, _) => {
            var ventana = Application.OpenForms.Cast<Form>().FirstOrDefault(f => f.Text == nombre);
            if (ventana is null) return;
            using var imagen = new Bitmap(ventana.Width, ventana.Height);
            ventana.DrawToBitmap(imagen, new Rectangle(Point.Empty, ventana.Size)); imagen.Save(ruta);
            capturada = true; timer.Stop(); ventana.Close();
        };
        timer.Start(); await documento.Exportar();
        Exigir(capturada, "No se capturo la vista previa real");
    }
    private static void Exigir(bool condicion, string mensaje) { if (!condicion) throw new InvalidOperationException(mensaje); }
    private sealed record Fila(int Numero, MetadatosSku Metadatos, int Cantidad, decimal Precio);
}
