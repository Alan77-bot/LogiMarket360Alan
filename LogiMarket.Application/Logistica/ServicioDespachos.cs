using LogiMarket.Application.Contratos;
using LogiMarket.Application.Seguridad;
using LogiMarket.Domain.Entregas;
using LogiMarket.Domain.Enums;
using LogiMarket.Domain.Operaciones;
using LogiMarket.Domain.Pedidos;

namespace LogiMarket.Application.Logistica;
// Coordina esta responsabilidad a traves de contratos; las reglas del negocio permanecen en su capa.
public sealed class ServicioDespachos(IRepositorio repo, ITransacciones tx, Autorizador auth, SesionActual sesion, IEnumerable<IFabricaEntrega> familias, IReloj reloj)
{
    public async Task Despachar(Guid id)
    {
        await auth.Exigir(Rol.BODEGA);
        await tx.Ejecutar(async () =>
        {
            var pedido = await repo.Pedido(id);
            var familia = familias.Single(f => f.Tipo == pedido.Entrega);
            pedido.Transicionar(EstadoPedido.DESPACHADO, sesion.Nombre, reloj.Ahora);
            repo.Agregar(new Despacho(id, familia.CrearEtiqueta().Crear(id), reloj.Ahora, reloj.Ahora.AddDays(familia.CrearPlazo().Dias)));
            repo.Agregar(new EventoOutbox(id, "DespachoRegistrado"));
            return true;
        });
    }

    public async Task Entregar(Guid id)
    {
        await auth.Exigir(Rol.BODEGA);
        await tx.Ejecutar(async () =>
        {
            (await repo.Pedido(id)).Transicionar(EstadoPedido.ENTREGADO, sesion.Nombre, reloj.Ahora);
            return true;
        });
    }
}
