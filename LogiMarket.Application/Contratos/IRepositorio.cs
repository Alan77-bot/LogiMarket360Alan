using LogiMarket.Domain.Entidades;
using LogiMarket.Domain.Operaciones;
using LogiMarket.Domain.Pedidos;

namespace LogiMarket.Application.Contratos;
// Este puerto define lo que necesita Application Infrastructure proporciona la implementacion concreta
public interface IRepositorio
{
    Task<List<Producto>> Productos();
    Task<List<Kit>> Kits();
    Task<List<Cliente>> Clientes();
    Task<List<Pedido>> Pedidos();
    Task<Pedido> Pedido(Guid id);
    Task<Usuario?> Usuario(string nombre);
    Task<Pago?> PagoPorClave(string clave);
    Task<Pago?> PagoPorPedido(Guid pedido);
    Task<Devolucion?> DevolucionPorPedido(Guid pedido);
    void Agregar<T>(T entidad)
        where T : class;
    void Eliminar<T>(T entidad)
        where T : class;
    Task Guardar();
}
