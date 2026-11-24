using LogiMarket.Application.Comprobantes;
using System.Text;

namespace LogiMarket.Infrastructure.Documentos;
// Bridge, salida concreta: escribe un comprobante interno real en TXT, no una factura tributaria.
public sealed class SalidaArchivoTxt(string directorio) : ISalidaDocumento
{
    public async Task Emitir(string nombre, string contenido)
    {
        Directory.CreateDirectory(directorio);
        await File.WriteAllTextAsync(Path.Combine(directorio, Path.GetFileName(nombre) + ".txt"), contenido, Encoding.UTF8);
    }
}
