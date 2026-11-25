namespace LogiMarket.Application.DTOs;
// Datos de entrada o salida del caso de uso; no contienen consultas SQL ni controles de pantalla.
public sealed record ComponenteEntrada(Guid Id, bool EsKit, int Cantidad);
