using LogiMarket.Application.Catalogo;
using LogiMarket.Application.Contratos;
using LogiMarket.Application.DTOs;
using LogiMarket.WinForms.Compartido;
using LogiMarket.WinForms.Formularios;
using Microsoft.Extensions.DependencyInjection;

namespace LogiMarket.WinForms.Vistas;
// Esta pantalla presenta datos y delega los cambios en Application no escribe tablas directamente
public sealed class VistaCatalogo : UserControl
{
    private readonly OperacionesUi operaciones;
    private readonly FormularioPrincipalMediator mediator;
    private readonly DataGridView grid = Ui.Grilla();
    private readonly TextBox buscar = new()
    {
        Width = 220,
        PlaceholderText = "SKU o nombre"
    };
    public VistaCatalogo(OperacionesUi operaciones, FormularioPrincipalMediator mediator, PermisosVista permisos)
    {
        this.operaciones = operaciones;
        this.mediator = mediator;
        Dock = DockStyle.Fill;
        var barra = Ui.Barra();
        barra.Controls.Add(buscar);
        barra.Controls.Add(Ui.Boton("Buscar", Refrescar));
        if (permisos.Administrar)
        {
            barra.Controls.Add(Ui.Boton("Nuevo producto", () => EditarProducto(null)));
            barra.Controls.Add(Ui.Boton("Editar", Editar));
            barra.Controls.Add(Ui.Boton("Nuevo kit", () => EditarKit(null)));
            barra.Controls.Add(Ui.Boton("Eliminar", Eliminar));
        }

        Controls.Add(grid);
        Controls.Add(barra);
        mediator.Registrar(Refrescar);
    }

    public async Task Refrescar() => Ui.Cargar(grid, await operaciones.Consultar(s => s.GetRequiredService<ServicioCatalogo>().Consultar(buscar.Text)));
    private FilaCatalogo Seleccion() => grid.CurrentRow?.DataBoundItem as FilaCatalogo ?? throw new InvalidOperationException("Seleccione una fila.");
    private Task Editar()
    {
        var fila = Seleccion();
        return fila.EsKit ? EditarKit(fila) : EditarProducto(fila);
    }

    private async Task EditarProducto(FilaCatalogo? fila)
    {
        using var d = new DialogoCampos("Producto").Texto("SKU", fila?.Sku ?? "").Texto("Nombre", fila?.Nombre ?? "").Numero("Precio", fila?.Precio ?? 1, 2).Numero("Peso", fila?.Peso ?? 0, 2).Numero("Stock inicial", fila?.Stock ?? 0);
        if (d.ShowDialog(this) != DialogResult.OK)
            return;
        await operaciones.Ejecutar(s => s.GetRequiredService<ServicioCatalogo>().GuardarProducto(fila?.Id, d.Texto("SKU"), d.Texto("Nombre"), d.Numero("Precio"), d.Numero("Peso"), (int)d.Numero("Stock inicial")));
        await mediator.Actualizar();
    }

    private async Task EditarKit(FilaCatalogo? fila)
    {
        var datos = await operaciones.Consultar(async s => (Filas: await s.GetRequiredService<ServicioCatalogo>().Consultar(), Kits: await s.GetRequiredService<IRepositorio>().Kits()));
        var actual = datos.Kits.SingleOrDefault(k => k.Id == fila?.Id);
        var texto = actual is null ? "P01:1,P02:1" : string.Join(",", actual.Componentes.Select(c => $"{datos.Filas.Single(f => f.Id == (c.ProductoId ?? c.KitHijoId)).Sku}:{c.Cantidad}"));
        using var d = new DialogoCampos("Composicion del kit").Texto("SKU", fila?.Sku ?? "").Texto("Nombre", fila?.Nombre ?? "").Texto("SKU:cantidad", texto);
        if (d.ShowDialog(this) != DialogResult.OK)
            return;
        var componentes = d.Texto("SKU:cantidad").Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t =>
        {
            var partes = t.Split(':');
            if (partes.Length != 2 || !int.TryParse(partes[1], out var cantidad))
                throw new ArgumentException("Use P01:1,P02:2.");
            var elegido = datos.Filas.Single(f => f.Sku.Equals(partes[0].Trim(), StringComparison.OrdinalIgnoreCase));
            return new ComponenteEntrada(elegido.Id, elegido.EsKit, cantidad);
        }).ToArray();
        await operaciones.Ejecutar(s => s.GetRequiredService<ServicioCatalogo>().GuardarKit(fila?.Id, d.Texto("SKU"), d.Texto("Nombre"), componentes));
        await mediator.Actualizar();
    }

    private async Task Eliminar()
    {
        var fila = Seleccion();
        if (Ui.Confirmar($"Eliminar {fila.Sku}? Las referencias existentes pueden impedirlo."))
        {
            await operaciones.Ejecutar(s => s.GetRequiredService<ServicioCatalogo>().Eliminar(fila.Id, fila.EsKit));
            await mediator.Actualizar();
        }
    }
}
