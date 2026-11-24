using System.Globalization;

namespace LogiMarket.Application.Comprobantes;
// Bridge, contrato de implementacion: el comprobante no necesita saber si se muestra o se escribe a disco.
public interface ISalidaDocumento
{
    Task Emitir(string nombre, string contenido);
}
