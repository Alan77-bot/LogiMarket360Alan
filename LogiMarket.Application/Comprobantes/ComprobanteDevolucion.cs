using LogiMarket.Domain.Operaciones;
using System.Globalization;

namespace LogiMarket.Application.Comprobantes;
// Bridge, abstraccion concreta para devolucion. Nunca devuelve el costo del envio.
public sealed class ComprobanteDevolucion(Devolucion devolucion, ISalidaDocumento salida) : Comprobante(salida)
{
    protected override string Nombre => $"devolucion-{devolucion.Id:N}";

    public override string Contenido() => $"LOGIMARKET 360 - DEVOLUCION TOTAL\nPedido: {devolucion.PedidoId}\nMotivo: {devolucion.Motivo}\nReembolso simulado: USD {Dinero(devolucion.Reembolso)}\nReferencia: {devolucion.Referencia}\nNo incluye tarifa de envio.";
}
