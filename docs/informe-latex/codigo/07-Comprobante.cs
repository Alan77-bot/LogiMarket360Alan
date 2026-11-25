using System.Globalization;

namespace LogiMarket.Application.Comprobantes;
// Patron Bridge el documento y su medio de salida cambian de forma independiente
// Bridge el comprobante y el medio de salida evolucionan por separado
public abstract class Comprobante(ISalidaDocumento salida)
{
    protected static string Dinero(decimal importe) => importe.ToString("F2", CultureInfo.InvariantCulture);
    public Task Exportar() => salida.Emitir(Nombre, Contenido());
    protected abstract string Nombre { get; }

    public abstract string Contenido();
}
