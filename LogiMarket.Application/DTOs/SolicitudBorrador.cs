using LogiMarket.Domain.Enums;

namespace LogiMarket.Application.DTOs;
// Datos de entrada o salida del caso de uso; no contienen consultas SQL ni controles de pantalla.
public sealed record SolicitudBorrador(Guid ClienteId, TipoEntrega Entrega, IReadOnlyList<LineaEntrada> Lineas);
