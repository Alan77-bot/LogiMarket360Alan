namespace LogiMarket.Application.DTOs;
// Responsabilidad de FilaCatalogo: colaborar desde LogiMarket.Application.DTOs sin mezclar reglas, persistencia y pantallas.
public sealed record FilaCatalogo(Guid Id, string Sku, string Nombre, decimal Precio, decimal Peso, int Stock, bool EsKit);
