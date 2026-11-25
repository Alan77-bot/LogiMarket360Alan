using LogiMarket.Application.Contratos;
using LogiMarket.Application.Seguridad;
using LogiMarket.Domain.Entidades;
using LogiMarket.Domain.Enums;

namespace LogiMarket.Application.Clientes;

public sealed class ServicioClientes(IRepositorio repo, ITransacciones tx, Autorizador auth)
{
    public async Task<List<Cliente>> Consultar(string filtro = "")
    {
        await auth.Lectura();
        return (await repo.Clientes()).Where(c => c.Nombre.Contains(filtro, StringComparison.OrdinalIgnoreCase)).OrderBy(c => c.Nombre).ToList();
    }

    public async Task Guardar(Guid? id, string nombre, CategoriaCliente categoria, bool activo)
    {
        await auth.Exigir(Rol.VENTAS);
        await tx.Ejecutar(async () =>
        {
            if (id is null)
            {
                var cliente = new Cliente(nombre, categoria);
                cliente.Editar(nombre, categoria, activo);
                repo.Agregar(cliente);
            }
            else
                (await repo.Clientes()).Single(c => c.Id == id).Editar(nombre, categoria, activo);
            return true;
        });
    }
}
