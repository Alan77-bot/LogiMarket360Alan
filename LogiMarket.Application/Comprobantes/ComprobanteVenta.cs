using LogiMarket.Domain.Entregas;
using LogiMarket.Domain.Pedidos;
using System.Globalization;

namespace LogiMarket.Application.Comprobantes;
// Bridge, abstraccion concreta para venta. La salida puede ser vista previa o archivo TXT.
public sealed class ComprobanteVenta(Pedido pedido, ISalidaDocumento salida) : Comprobante(salida)
{
    protected override string Nombre => $"venta-{pedido.Id:N}";

    public override string Contenido() => $"LOGIMARKET 360 - COMPROBANTE INTERNO\nPedido: {pedido.Id}\nCliente: {pedido.ClienteNombre}\n" + string.Join("\n", pedido.Detalles.Select(d => $"{d.Sku} x{d.Cantidad}")) + $"\nSubtotal: USD {Dinero(pedido.Subtotal)}\nDescuento: USD {Dinero(pedido.Descuento)}\nEnvio: USD {Dinero(pedido.Tarifa)}\nTotal: USD {Dinero(pedido.Total)}\nDocumento de simulacion sin validez tributaria.";
}
