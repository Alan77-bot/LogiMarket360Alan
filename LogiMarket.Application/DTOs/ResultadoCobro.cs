namespace LogiMarket.Application.DTOs;
// Datos de entrada o salida del caso de uso no contienen consultas SQL ni controles de pantalla
public sealed record ResultadoCobro(bool Aceptado, string Referencia, string Mensaje);
