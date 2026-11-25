using LogiMarket.WinForms.Compartido;
using Microsoft.Extensions.DependencyInjection;

namespace LogiMarket.WinForms.Formularios;
// Responsabilidad de DialogoCampos: colaborar desde LogiMarket.WinForms.Formularios sin mezclar reglas, persistencia y pantallas.
public sealed class DialogoCampos : Form
{
    private readonly Dictionary<string, Control> campos = [];
    private readonly TableLayoutPanel tabla = new()
    {
        Dock = DockStyle.Fill,
        AutoScroll = true,
        ColumnCount = 2,
        Padding = new Padding(18)
    };
    public DialogoCampos(string titulo)
    {
        Text = titulo;
        Width = 540;
        Height = 490;
        StartPosition = FormStartPosition.CenterParent;
        Font = new Font("Segoe UI", 10);
        tabla.ColumnStyles.Add(new(SizeType.Absolute, 160));
        tabla.ColumnStyles.Add(new(SizeType.Percent, 100));
        var pie = Ui.Barra();
        pie.Dock = DockStyle.Bottom;
        var aceptar = new Button
        {
            Text = "Aceptar",
            AutoSize = true
        };
        var errores = new ErrorProvider
        {
            ContainerControl = this,
            BlinkStyle = ErrorBlinkStyle.NeverBlink
        };
        aceptar.Click += (_, _) =>
        {
            var valido = true;
            foreach (var(nombre, control)in campos)
            {
                var error = control is TextBox texto && string.IsNullOrWhiteSpace(texto.Text) ? "Campo obligatorio." : control is ComboBox combo && combo.SelectedItem is null ? "Seleccione una opcion." : control is NumericUpDown numero && (nombre == "Precio" && numero.Value <= 0 || nombre.StartsWith("Cantidad") && numero.Value == 0) ? "Valor invalido." : "";
                errores.SetError(control, error);
                valido &= error.Length == 0;
            }

            if (valido)
                DialogResult = DialogResult.OK;
        };
        Disposed += (_, _) => errores.Dispose();
        pie.Controls.Add(aceptar);
        pie.Controls.Add(new Button { Text = "Cancelar", DialogResult = DialogResult.Cancel, AutoSize = true });
        Controls.Add(tabla);
        Controls.Add(pie);
        AcceptButton = aceptar;
        Shown += (_, _) => Height = Math.Clamp(140 + campos.Count * 48, 210, 650);
    }

    public DialogoCampos Texto(string nombre, string valor = "") => Agregar(nombre, new TextBox { Text = valor, Dock = DockStyle.Top });
    public DialogoCampos Numero(string nombre, decimal valor = 0, int decimales = 0, decimal minimo = 0) => Agregar(nombre, new NumericUpDown { Value = valor, Minimum = minimo, Maximum = 10000000, DecimalPlaces = decimales, Dock = DockStyle.Top });
    public DialogoCampos Lista(string nombre, object[] valores, object? seleccionado = null)
    {
        var combo = new ComboBox
        {
            DropDownStyle = ComboBoxStyle.DropDownList,
            Dock = DockStyle.Top
        };
        combo.Items.AddRange(valores);
        if (seleccionado is not null)
            combo.SelectedItem = seleccionado;
        if (combo.SelectedIndex < 0 && valores.Length > 0)
            combo.SelectedIndex = 0;
        return Agregar(nombre, combo);
    }

    public DialogoCampos Casilla(string nombre, bool valor) => Agregar(nombre, new CheckBox { Checked = valor });
    private DialogoCampos Agregar(string nombre, Control control)
    {
        var fila = tabla.RowCount++;
        tabla.RowStyles.Add(new(SizeType.AutoSize));
        tabla.Controls.Add(new Label { Text = nombre, AutoSize = true, Padding = new Padding(0, 7, 0, 10) }, 0, fila);
        tabla.Controls.Add(control, 1, fila);
        campos.Add(nombre, control);
        return this;
    }

    public string Texto(string nombre) => ((TextBox)campos[nombre]).Text;
    public decimal Numero(string nombre) => ((NumericUpDown)campos[nombre]).Value;
    public T Seleccion<T>(string nombre) => (T)((ComboBox)campos[nombre]).SelectedItem!;
    public bool Marcado(string nombre) => ((CheckBox)campos[nombre]).Checked;
}
