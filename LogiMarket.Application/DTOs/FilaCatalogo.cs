namespace LogiMarket.Application.DTOs;
// Datos de entrada o salida del caso de uso no contienen consultas SQL ni controles de pantalla
public sealed record FilaCatalogo(Guid Id, string Sku, string Nombre, decimal Precio, decimal Peso, int Stock, bool EsKit);
