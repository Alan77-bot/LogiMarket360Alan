using Microsoft.Extensions.DependencyInjection;

namespace LogiMarket.WinForms.Compartido;
// Reunimos estilos y controles reutilizables los formularios conservan su foco en capturar y mostrar datos
public static class Ui
{
    public static readonly Color Fondo = Color.FromArgb(244, 247, 251);
    public static readonly Color Azul = Color.FromArgb(22, 39, 67);
    public static DataGridView Grilla()
    {
        var grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            ReadOnly = true,
            AutoGenerateColumns = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.AllCells,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            BackgroundColor = Color.White,
            BorderStyle = BorderStyle.None,
            RowHeadersVisible = false,
            AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells, EnableHeadersVisualStyles = false, CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal, GridColor = Color.FromArgb(232, 237, 244), ColumnHeadersHeight = 38, ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing
        };

        grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle { BackColor = Azul, ForeColor = Color.White, SelectionBackColor = Azul, SelectionForeColor = Color.White, Font = new Font("Segoe UI", 9, FontStyle.Bold), Padding = new Padding(8) };
        grid.DefaultCellStyle = new DataGridViewCellStyle { BackColor = Color.White, ForeColor = Color.FromArgb(40, 54, 73), SelectionBackColor = Color.FromArgb(221, 235, 252), SelectionForeColor = Azul, Padding = new Padding(8, 5, 8, 5), Font = new Font("Segoe UI", 10) };
        grid.AlternatingRowsDefaultCellStyle.BackColor = Color.FromArgb(247, 249, 252);
        grid.RowTemplate.Height = 34;
        grid.CellFormatting += (_, e) =>
        {
            if (e.Value is DateTimeOffset fecha)
            {
                e.Value = TimeZoneInfo.ConvertTime(fecha, TimeZoneInfo.FindSystemTimeZoneById("SA Pacific Standard Time")).ToString("yyyy-MM-dd HH:mm");
                e.FormattingApplied = true;
            }
        };
        return grid;
    }

    public static Button Boton(string texto, Func<Task> accion)
    {
        var boton = new Button
        {
            Text = texto,
            AutoSize = true,
            Height = 40, MinimumSize = new Size(0, 40), Cursor = Cursors.Hand, AccessibleName = texto,
            FlatStyle = FlatStyle.Flat,
            Padding = new Padding(12, 8, 12, 8),
            BackColor = Color.White
        };
                var principal = new[] { "Guardar", "Confirmar", "Iniciar", "Validar" }.Any(texto.StartsWith);
        var peligro = texto.StartsWith("Eliminar") || texto.StartsWith("Cancelar pedido");
        boton.BackColor = principal ? Color.FromArgb(35, 105, 191) : Color.White;
        boton.ForeColor = principal ? Color.White : peligro ? Color.FromArgb(170, 45, 55) : Azul;
        boton.FlatAppearance.BorderColor = principal ? boton.BackColor : Color.FromArgb(211, 220, 232);
        boton.FlatAppearance.MouseOverBackColor = principal ? Color.FromArgb(26, 83, 153) : Color.FromArgb(233, 241, 251);
        boton.Click += async (_, _) =>
        {
            boton.Enabled = false;
            try
            {
                await accion();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.GetBaseException().Message, "LogiMarket 360 Alan", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                boton.Enabled = true;
            }
        };
        return boton;
    }

    public static FlowLayoutPanel Barra() => new()
    {
        Dock = DockStyle.Top,
        AutoSize = true,
        Padding = new Padding(12, 8, 12, 8),
        WrapContents = true
    };
    public static bool Confirmar(string mensaje) => MessageBox.Show(mensaje, "Confirmar operacion", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;
    public static void Cargar<T>(DataGridView grid, IEnumerable<T> datos)
    {
        var seleccion = grid.CurrentRow?.Cells[0].Value;
        grid.DataSource = datos.ToList();
        foreach (DataGridViewColumn c in grid.Columns)
        {
            if (c.ValueType == typeof(decimal))
                { c.DefaultCellStyle.Format = "N2"; c.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight; }
            if (c.ValueType == typeof(Guid) || c.ValueType == typeof(Guid? ) || c.Name is "Version" or "Hash")
                c.Visible = false;
            c.HeaderText = c.Name switch
            {
                "ClienteNombre" => "Cliente",
                "ClienteVip" => "VIP",
                "EsKit" => "Kit",
                "EntregadoEn" => "Entrega registrada",
                _ => c.Name
            };
        // AllCells evita redistribuir columnas de vistas que aun no tienen ventana
        }

        if (seleccion is not null)
            foreach (DataGridViewRow fila in grid.Rows)
                if (Equals(fila.Cells[0].Value, seleccion))
                {
                    grid.CurrentCell = fila.Cells.Cast<DataGridViewCell>().First(c => c.Visible);
                    break;
                }
    }

    public static void Menu(Button boton, bool seleccionado)
    {
        boton.AutoSize = false; boton.Size = new Size(206, 43);
        boton.TextAlign = ContentAlignment.MiddleLeft; boton.Padding = new Padding(14, 0, 8, 0);
        boton.BackColor = seleccionado ? Color.FromArgb(43, 91, 153) : Azul;
        boton.ForeColor = Color.White; boton.FlatAppearance.BorderSize = 0;
        boton.FlatAppearance.MouseOverBackColor = Color.FromArgb(38, 65, 102);
        boton.Margin = new Padding(0, 3, 0, 3);
    }

    public static void AplicarTema(Control raiz)
    {
        if (raiz is UserControl) raiz.BackColor = Fondo;
        if (raiz is FlowLayoutPanel) raiz.BackColor = Color.White;
        if (raiz is TabControl) raiz.Font = new Font("Segoe UI", 10);
        if (raiz is TextBoxBase) raiz.Margin = new Padding(5, 10, 5, 8);
        if (string.IsNullOrWhiteSpace(raiz.AccessibleName) && raiz is Button or TextBoxBase or ComboBox or NumericUpDown or DataGridView)
            raiz.AccessibleName = string.IsNullOrWhiteSpace(raiz.Text) ? raiz.GetType().Name : raiz.Text;
        foreach (Control hijo in raiz.Controls) AplicarTema(hijo);
    }}
