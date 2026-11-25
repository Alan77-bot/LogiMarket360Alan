using LogiMarket.Application.Contratos;
using LogiMarket.Application.Comprobantes;
using LogiMarket.Application.Eventos;
using LogiMarket.Application.Seguridad;
using LogiMarket.Infrastructure.InyeccionDependencias;
using LogiMarket.Infrastructure.Documentos;
using LogiMarket.Infrastructure.Persistencia;
using LogiMarket.WinForms.Compartido;
using LogiMarket.WinForms.Formularios;
using Microsoft.Extensions.DependencyInjection;

// Ejecutado por verificar_roles.ps1 en un proyecto temporal, fuera de la solucion.
internal static class RevisionRoles
{
    [STAThread]
    private static int Main(string[] args)
    {
        ApplicationConfiguration.Initialize();
        var salida = Path.GetFullPath(args[0]);
        Directory.CreateDirectory(salida);
        var servicios = new ServiceCollection().AgregarLogiMarket(Path.Combine(salida, "roles-" + Guid.NewGuid() + ".db"));
        servicios.AddSingleton<ISalidaDocumento>(new SalidaArchivoTxt(salida));
        using var proveedor = servicios.BuildServiceProvider();
        using (var scope = proveedor.CreateScope()) scope.ServiceProvider.GetRequiredService<Inicializador>().Ejecutar().GetAwaiter().GetResult();
                // Captura del formulario implementado, sin sustituirlo por un diseno ilustrativo.
        using (var acceso = new FormularioAcceso(new OperacionesUi(proveedor))) {
            acceso.Show(); Application.DoEvents();
            using var imagen = new Bitmap(acceso.Width, acceso.Height);
            acceso.DrawToBitmap(imagen, new Rectangle(Point.Empty, acceso.Size));
            imagen.Save(Path.Combine(salida, "acceso.png")); acceso.Close();
        }
        var esperado = new Dictionary<string, string[]>
        {
            ["admin"] = ["Panel general", "Catalogo y kits", "Clientes", "Carrito", "Pedidos", "Cobro", "Despacho y entrega", "Devoluciones", "Inventario", "Reportes", "Auditoria y demo"],
            ["ventas"] = ["Panel general", "Catalogo y kits", "Clientes", "Carrito", "Pedidos", "Cobro", "Devoluciones", "Reportes"],
            ["bodega"] = ["Panel general", "Catalogo y kits", "Despacho y entrega", "Inventario"],
            ["consulta"] = ["Panel general", "Catalogo y kits", "Clientes", "Pedidos", "Despacho y entrega", "Devoluciones", "Inventario", "Reportes"]
        };
        var registro = new List<string>();
        var fallos = 0;
        foreach (var (usuario, apartados) in esperado)
        {
            using (var scope = proveedor.CreateScope()) scope.ServiceProvider.GetRequiredService<Autenticacion>().Entrar(usuario, "Demo123!").GetAwaiter().GetResult();
            using var form = new FormularioPrincipal(new OperacionesUi(proveedor), proveedor.GetRequiredService<SesionActual>(), proveedor.GetRequiredService<MetricasSesion>(), proveedor.GetRequiredService<OpcionesDemostracion>(), proveedor.GetRequiredService<ISalidaDocumento>());
            form.Shown += async (_, _) =>
            {
                try
                {
                    var menu = form.Controls.OfType<FlowLayoutPanel>().Single();
                    var botones = menu.Controls.OfType<Button>().Where(b => b.Text != "Cerrar sesion").ToArray();
                    Exigir(botones.Select(b => b.Text).SequenceEqual(apartados), "Menu incorrecto: " + usuario);
                    Exigir(menu.Controls.OfType<Button>().Any(b => b.Text == "Cerrar sesion"), "Falta cerrar sesion");
                    registro.Add(usuario + " MENU APROBADO: " + string.Join(", ", apartados));
                    var contenido = form.Controls.OfType<Panel>().Single(p => p is not FlowLayoutPanel);
                    foreach (var boton in botones)
                    {
                        boton.PerformClick();
                        var vista = contenido.Controls.OfType<UserControl>().Single();
                        if (vista.GetType().GetMethod("Refrescar")?.Invoke(vista, null) is Task tarea) await tarea;
                        Exigir(!Descendientes(vista).Any(c => c is PictureBox || c.BackgroundImage is not null), "La interfaz no debe usar imagenes");
                        var acciones = Descendientes(vista).OfType<Button>().Select(b => b.Text).ToArray();
                        var ventas = usuario is "admin" or "ventas";
                        var bodega = usuario is "admin" or "bodega";
                        if (boton.Text == "Catalogo y kits")
                            Exigir(acciones.SequenceEqual(usuario == "admin" ? ["Buscar", "Nuevo producto", "Editar", "Nuevo kit", "Eliminar"] : ["Buscar"]), "Acciones de catalogo");
                        if (boton.Text == "Clientes")
                            Exigir(acciones.SequenceEqual(ventas ? ["Buscar", "Nuevo cliente", "Editar cliente"] : ["Buscar"]), "Acciones de clientes");
                        if (boton.Text == "Pedidos")
                            Exigir(acciones.SequenceEqual(ventas ? ["Filtrar", "Detalle e historial", "Editar borrador", "Clonar borrador", "Validar", "Cancelar pedido"] : ["Filtrar", "Detalle e historial"]), "Acciones de pedidos");
                        if (boton.Text == "Despacho y entrega")
                            Exigir(acciones.SequenceEqual(bodega ? ["Filtrar", "Detalle e historial", "Despachar", "Marcar entregado", "Seguimiento"] : ["Filtrar", "Detalle e historial", "Seguimiento"]), "Acciones de despacho");
                        if (boton.Text == "Devoluciones")
                            Exigir(acciones.SequenceEqual(ventas ? ["Filtrar", "Detalle e historial", "Devolucion total"] : ["Filtrar", "Detalle e historial"]), "Acciones de devolucion");
                        if (boton.Text == "Inventario")
                            Exigir(acciones.SequenceEqual(bodega ? ["Registrar entrada / salida", "Actualizar"] : ["Actualizar"]), "Acciones de inventario");
                        Exigir(Descendientes(vista).OfType<DataGridView>().All(g => g.ReadOnly), "Tabla editable directamente");
                                                if (usuario == "admin") {
                            form.PerformLayout(); Application.DoEvents();
                            using var imagenVista = new Bitmap(form.Width, form.Height);
                            form.DrawToBitmap(imagenVista, new Rectangle(Point.Empty, form.Size));
                            imagenVista.Save(Path.Combine(salida, "vista-" + boton.Text.Replace(" ", "-") + ".png"));
                        }
                        registro.Add(usuario + " / " + boton.Text + " APROBADO: " + string.Join(", ", acciones));
                    }
                    botones[0].PerformClick();
                    Application.DoEvents();
                    using var captura = new Bitmap(form.Width, form.Height);
                    form.DrawToBitmap(captura, new Rectangle(Point.Empty, form.Size));
                    captura.Save(Path.Combine(salida, usuario + ".png"));
                }
                catch (Exception ex) { fallos++; registro.Add(usuario + " FALLO: " + ex); }
                finally { form.Close(); }
            };
            Application.Run(form);
            proveedor.GetRequiredService<SesionActual>().Cerrar();
        }
        registro.Add("RESULTADO: " + (fallos == 0 ? "APROBADO" : "FALLO"));
        File.WriteAllLines(Path.Combine(salida, "roles-ui.txt"), registro);
        return fallos == 0 ? 0 : 1;
    }
    private static void Exigir(bool condicion, string mensaje) { if (!condicion) throw new InvalidOperationException(mensaje); }
    private static IEnumerable<Control> Descendientes(Control padre) => padre.Controls.Cast<Control>().SelectMany(c => new[] { c }.Concat(Descendientes(c)));
}
