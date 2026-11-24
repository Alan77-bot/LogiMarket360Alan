namespace LogiMarket.Application.Catalogo;
// Responsabilidad de MetadatosSku: colaborar desde LogiMarket.Application.Catalogo sin mezclar reglas, persistencia y pantallas.
// Flyweight: solo comparte datos inmutables; cantidad, precio y stock son externos.
public sealed record MetadatosSku(string Sku, string Nombre, string Categoria, string Descripcion);
