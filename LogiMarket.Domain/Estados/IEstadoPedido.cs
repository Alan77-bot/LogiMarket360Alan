using LogiMarket.Domain.Enums;
namespace LogiMarket.Domain.Estados;
// State contrato del comportamiento que decide si se permite una transicion
public interface IEstadoPedido
{
    void Exigir(EstadoPedido destino);
}