namespace LogiMarket.Application.DTOs;
// Responsabilidad de ComponenteEntrada: colaborar desde LogiMarket.Application.DTOs sin mezclar reglas, persistencia y pantallas.
public sealed record ComponenteEntrada(Guid Id, bool EsKit, int Cantidad);
