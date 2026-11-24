namespace LogiMarket.Application.DTOs;
// Responsabilidad de ResultadoCobro: colaborar desde LogiMarket.Application.DTOs sin mezclar reglas, persistencia y pantallas.
public sealed record ResultadoCobro(bool Aceptado, string Referencia, string Mensaje);
