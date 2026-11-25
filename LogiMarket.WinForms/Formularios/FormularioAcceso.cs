using LogiMarket.Application.Seguridad;
using LogiMarket.WinForms.Compartido;
using Microsoft.Extensions.DependencyInjection;

namespace LogiMarket.WinForms.Formularios;

// La ventana captura datos; Autenticacion y Autorizador mantienen las reglas fuera de la UI.
public sealed class FormularioAcceso : Form
{
    public FormularioAcceso(OperacionesUi operaciones)
    {
        Text = "Acceso | LogiMarket 360 Alan";
        ClientSize = new Size(860, 540);
        MinimumSize = new Size(820, 560);
        StartPosition = FormStartPosition.CenterScreen;
        Font = new Font("Segoe UI", 10);
        BackColor = Color.White;
        AutoScaleMode = AutoScaleMode.Dpi;
        var distribucion = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1 };
        distribucion.ColumnStyles.Add(new(SizeType.Percent, 39));
        distribucion.ColumnStyles.Add(new(SizeType.Percent, 61));
        var marca = new TableLayoutPanel { Dock = DockStyle.Fill, BackColor = Ui.Azul, Padding = new Padding(28, 42, 20, 24), RowCount = 4, ColumnCount = 1 };
        marca.ColumnStyles.Add(new(SizeType.Percent, 100));
        marca.RowStyles.Add(new(SizeType.Absolute, 110));
        marca.RowStyles.Add(new(SizeType.Absolute, 110));
        marca.RowStyles.Add(new(SizeType.Percent, 100));
        marca.RowStyles.Add(new(SizeType.Absolute, 50));
        marca.Controls.Add(new Label { Text = "LOGIMARKET\n360 ALAN", Dock = DockStyle.Fill, ForeColor = Color.White, Font = new Font("Segoe UI", 24, FontStyle.Bold) }, 0, 0);
        marca.Controls.Add(new Label { Text = "Ventas, inventario y entregas\nen un solo lugar.", Dock = DockStyle.Fill, ForeColor = Color.FromArgb(196, 214, 239), Font = new Font("Segoe UI", 13) }, 0, 1);
        marca.Controls.Add(new Label { Text = "ORDEN EN CADA OPERACION", Dock = DockStyle.Fill, ForeColor = Color.FromArgb(152, 183, 220), Font = new Font("Segoe UI", 8, FontStyle.Bold) }, 0, 3);
        var campos = new TableLayoutPanel { Dock = DockStyle.Fill, Padding = new Padding(38, 32, 38, 20), ColumnCount = 1, RowCount = 11 };
        campos.ColumnStyles.Add(new(SizeType.Percent, 100));
        foreach (var alto in new[] { 28, 46, 40, 27, 42, 27, 42, 56, 50, 48 }) campos.RowStyles.Add(new(SizeType.Absolute, alto));
        campos.RowStyles.Add(new(SizeType.Percent, 100));
        var usuario = new TextBox { Dock = DockStyle.Fill, PlaceholderText = "Escribe tu usuario", AccessibleName = "Usuario", Font = new Font("Segoe UI", 12), TabIndex = 0 };
        var clave = new TextBox { Dock = DockStyle.Fill, PlaceholderText = "Escribe tu contrasena", UseSystemPasswordChar = true, AccessibleName = "Contrasena", Font = new Font("Segoe UI", 12), TabIndex = 1 };
        var error = new Label { Dock = DockStyle.Fill, ForeColor = Color.FromArgb(175, 45, 55), AccessibleName = "Resultado del acceso" };
        campos.Controls.Add(new Label { Text = "TU ESPACIO DE TRABAJO", Dock = DockStyle.Fill, ForeColor = Color.FromArgb(35, 105, 191), Font = new Font("Segoe UI", 9, FontStyle.Bold) }, 0, 0);
        campos.Controls.Add(new Label { Text = "Bienvenido de nuevo", Dock = DockStyle.Fill, ForeColor = Ui.Azul, Font = new Font("Segoe UI", 21, FontStyle.Bold) }, 0, 1);
        campos.Controls.Add(new Label { Text = "Ingresa con tu cuenta para continuar.", Dock = DockStyle.Fill, ForeColor = Color.SlateGray }, 0, 2);
        campos.Controls.Add(new Label { Text = "Usuario", Dock = DockStyle.Fill, ForeColor = Ui.Azul }, 0, 3);
        campos.Controls.Add(usuario, 0, 4);
        campos.Controls.Add(new Label { Text = "Contrasena", Dock = DockStyle.Fill, ForeColor = Ui.Azul }, 0, 5);
        campos.Controls.Add(clave, 0, 6);
        campos.Controls.Add(error, 0, 7);
        var entrar = Ui.Boton("Iniciar sesion", async () => {
            error.Text = "";
            try {
                await operaciones.Ejecutar(s => s.GetRequiredService<Autenticacion>().Entrar(usuario.Text, clave.Text));
                DialogResult = DialogResult.OK; Close();
            }
            catch (Exception ex) { error.Text = ex.GetBaseException().Message; clave.SelectAll(); clave.Focus(); }
        });
        entrar.Dock = DockStyle.Fill; entrar.TabIndex = 2;
        campos.Controls.Add(entrar, 0, 8);
        campos.Controls.Add(new Label { Text = "Las opciones disponibles dependen de tu perfil.", Dock = DockStyle.Fill, ForeColor = Color.SlateGray, Padding = new Padding(0, 12, 0, 0), Font = new Font("Segoe UI", 9) }, 0, 9);
        distribucion.Controls.Add(marca, 0, 0); distribucion.Controls.Add(campos, 1, 0);
        Controls.Add(distribucion); AcceptButton = entrar;
        Shown += (_, _) => usuario.Focus();
    }
}