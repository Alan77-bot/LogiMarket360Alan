using LogiMarket.Application.Catalogo;
using LogiMarket.Application.Contratos;
using LogiMarket.Application.DTOs;
using LogiMarket.Application.Seguridad;
using LogiMarket.Application.Validaciones;
using LogiMarket.Domain.Entregas;
using LogiMarket.Domain.Enums;
using LogiMarket.Domain.Pedidos;
using LogiMarket.Domain.Promociones;

namespace LogiMarket.Application.Pedidos;
// Responsabilidad de ServicioPedidos: colaborar desde LogiMarket.Application.Pedidos sin mezclar reglas, persistencia y pantallas.
public sealed class ServicioPedidos(IRepositorio repo, ITransacciones tx, Autorizador auth, SesionActual sesion, CalculadoraPrecios calculadora, IEnumerable<IFabricaEntrega> entregas, IReloj reloj)
{
    public async Task<Pedido> Preparar(SolicitudBorrador entrada)
    {
        await auth.Exigir(Rol.VENTAS);
        var cliente = (await repo.Clientes()).SingleOrDefault(c => c.Id == entrada.ClienteId) ?? throw new ArgumentException("Seleccione cliente.");
        var catalogo = CatalogoCompuesto.Construir(await repo.Productos(), await repo.Kits());
        var builder = new PedidoBuilder().Para(cliente).ConEntrega(entrada.Entrega);
        foreach (var linea in entrada.Lineas)
            builder.Agregar(catalogo.TryGetValue(linea.Sku, out var elemento) ? elemento : throw new ArgumentException("SKU inexistente."), linea.Cantidad);
        var pedido = builder.Construir();
        pedido.Valorar(calculadora, entregas.Single(e => e.Tipo == entrada.Entrega).CrearTarifa().Importe);
        return pedido;
    }

    public async Task<Guid> Guardar(SolicitudBorrador entrada, Guid? reemplazar = null)
    {
        await auth.Exigir(Rol.VENTAS);
        return await tx.Ejecutar(async () =>
        {
            var pedido = await Preparar(entrada);
            if (reemplazar is Guid id)
            {
                var anterior = await repo.Pedido(id);
                anterior.AplicarCambios(pedido);
                return anterior.Id;
            }

            repo.Agregar(pedido);
            return pedido.Id;
        });
    }

    public async Task<Guid> Clonar(Guid id)
    {
        await auth.Exigir(Rol.VENTAS);
        return await tx.Ejecutar(async () =>
        {
            var copia = (await repo.Pedido(id)).ClonarBorrador();
            repo.Agregar(copia);
            return copia.Id;
        });
    }

    public async Task Validar(Guid id)
    {
        await auth.Exigir(Rol.VENTAS);
        await tx.Ejecutar(async () =>
        {
            var pedido = await repo.Pedido(id);
            pedido.ExigirEditable();
            // Se usan precios historicos de las lineas, no el catalogo actual.
            pedido.Valorar(calculadora, entregas.Single(e => e.Tipo == pedido.Entrega).CrearTarifa().Importe);
            var resultado = CadenaConfirmacion.Validar(new(pedido, (await repo.Clientes()).SingleOrDefault(c => c.Id == pedido.ClienteId), await repo.Productos()), false);
            if (!resultado.Valido)
                throw new InvalidOperationException(resultado.Mensaje);
            pedido.Transicionar(EstadoPedido.VALIDADO, sesion.Nombre, reloj.Ahora);
            return true;
        });
    }

    public async Task Cancelar(Guid id)
    {
        await auth.Exigir(Rol.VENTAS);
        await tx.Ejecutar(async () =>
        {
            var pedido = await repo.Pedido(id);
            pedido.Transicionar(EstadoPedido.CANCELADO, sesion.Nombre, reloj.Ahora);
            return true;
        });
    }

    public async Task<List<Pedido>> Consultar()
    {
        await auth.Lectura();
        return (await repo.Pedidos()).OrderByDescending(p => p.Fecha).ToList();
    }
}
