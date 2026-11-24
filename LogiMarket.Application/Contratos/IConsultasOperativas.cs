using LogiMarket.Domain.Operaciones;

namespace LogiMarket.Application.Contratos;
// Responsabilidad de IConsultasOperativas: colaborar desde LogiMarket.Application.Contratos sin mezclar reglas, persistencia y pantallas.
public interface IConsultasOperativas
{
    Task<List<MovimientoInventario>> Movimientos();
    Task<List<Auditoria>> Auditorias();
    Task<List<EventoOutbox>> Eventos();
    Task<List<Despacho>> Despachos();
    Task<List<Devolucion>> Devoluciones();
    Task<List<Aviso>> Avisos();
}
