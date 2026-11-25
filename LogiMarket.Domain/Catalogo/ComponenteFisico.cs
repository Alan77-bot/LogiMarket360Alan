namespace LogiMarket.Domain.Catalogo;
// Colabora con la composicion del catalogo manteniendo los datos de las filas separados de las reglas de venta.
public sealed record ComponenteFisico(string Sku, int Cantidad, decimal Precio, decimal Peso);
