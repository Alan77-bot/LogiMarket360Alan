namespace LogiMarket.Application.DTOs;
// Responsabilidad de LineaEntrada: colaborar desde LogiMarket.Application.DTOs sin mezclar reglas, persistencia y pantallas.
public sealed record LineaEntrada(string Sku, int Cantidad);
