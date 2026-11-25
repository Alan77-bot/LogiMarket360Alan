using LogiMarket.Domain.Operaciones;

namespace LogiMarket.Application.Contratos;
// Este puerto define lo que necesita Application. Infrastructure proporciona la implementacion concreta.
public interface IConsultasOperativas
{
    Task<List<MovimientoInventario>> Movimientos();
    Task<List<Auditoria>> Auditorias();
    Task<List<EventoOutbox>> Eventos();
    Task<List<Despacho>> Despachos();
    Task<List<Devolucion>> Devoluciones();
    Task<List<Aviso>> Avisos();
}
