using LogiMarket.Domain.Enums;

namespace LogiMarket.Application.DTOs;
// Responsabilidad de SolicitudBorrador: colaborar desde LogiMarket.Application.DTOs sin mezclar reglas, persistencia y pantallas.
public sealed record SolicitudBorrador(Guid ClienteId, TipoEntrega Entrega, IReadOnlyList<LineaEntrada> Lineas);
