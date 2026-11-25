using LogiMarket.Application.Comprobantes;
using Microsoft.Extensions.DependencyInjection;

namespace LogiMarket.WinForms.Compartido;
// Bridge, salida concreta: muestra el documento completo en una ventana de lectura.
public sealed class SalidaVistaPrevia(IWin32Window propietario) : ISalidaDocumento
{
    public Task Emitir(string nombre, string contenido)
    {
        using var vista = new Form
        {
            Text = nombre,
            Width = 760,
            Height = 600,
            StartPosition = FormStartPosition.CenterParent
        };
        vista.Controls.Add(new TextBox { Dock = DockStyle.Fill, Multiline = true, ReadOnly = true, ScrollBars = ScrollBars.Both, Text = contenido.Replace("\n", Environment.NewLine), Font = new Font("Consolas", 11) });
        vista.Shown += (_, _) => ((TextBox)vista.Controls[0]).Select(0, 0);
        vista.ShowDialog(propietario);
        return Task.CompletedTask;
    }
}
