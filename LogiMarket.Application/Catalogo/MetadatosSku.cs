namespace LogiMarket.Application.Catalogo;
// Colabora con la composicion del catalogo manteniendo los datos de las filas separados de las reglas de venta
// Flyweight solo comparte datos inmutables cantidad precio y stock son externos
public sealed record MetadatosSku(string Sku, string Nombre, string Categoria, string Descripcion);
