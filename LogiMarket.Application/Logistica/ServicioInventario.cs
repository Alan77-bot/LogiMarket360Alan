using LogiMarket.Application.Contratos;
using LogiMarket.Application.Seguridad;
using LogiMarket.Domain.Enums;
using LogiMarket.Domain.Operaciones;

namespace LogiMarket.Application.Logistica;

public sealed class ServicioInventario(IRepositorio repo, ITransacciones tx, Autorizador auth)
{
    public async Task Mover(Guid producto, int cantidad, string motivo, string referencia)
    {
        await auth.Exigir(Rol.BODEGA);
        ArgumentException.ThrowIfNullOrWhiteSpace(motivo);
        ArgumentException.ThrowIfNullOrWhiteSpace(referencia);
        await tx.Ejecutar(async () =>
        {
            (await repo.Productos()).Single(p => p.Id == producto).Mover(cantidad);
            repo.Agregar(new MovimientoInventario(producto, cantidad, motivo, referencia));
            return true;
        });
    }
}
