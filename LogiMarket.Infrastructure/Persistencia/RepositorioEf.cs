using LogiMarket.Application.Contratos;
using LogiMarket.Application.Seguridad;
using LogiMarket.Domain.Entidades;
using LogiMarket.Domain.Operaciones;
using LogiMarket.Domain.Pedidos;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace LogiMarket.Infrastructure.Persistencia;
// Implementa los puertos sin filtrar EF a Application. BEGIN IMMEDIATE y versiones protegen frente a escritores concurrentes.
public sealed class RepositorioEf(LogiMarketDbContext db, SesionActual? sesion = null) : IRepositorio, ITransacciones, IConsultasOperativas, IRegistroRecepciones
{
    public Task<List<Producto>> Productos() => db.Productos.ToListAsync();
    public Task<List<Kit>> Kits() => db.Kits.Include(x => x.Componentes).ToListAsync();
    public Task<List<Cliente>> Clientes() => db.Clientes.ToListAsync();
    public Task<List<Pedido>> Pedidos() => db.Pedidos.Include(x => x.Detalles).ThenInclude(x => x.Componentes).Include(x => x.Historial).AsSplitQuery().ToListAsync();
    public async Task<Pedido> Pedido(Guid id) => await db.Pedidos.Include(x => x.Detalles).ThenInclude(x => x.Componentes).Include(x => x.Historial).AsSplitQuery().SingleOrDefaultAsync(x => x.Id == id) ?? throw new KeyNotFoundException("Pedido inexistente.");
    public Task<Usuario?> Usuario(string nombre) => db.Usuarios.SingleOrDefaultAsync(x => x.Nombre == nombre);
    public Task<Pago?> PagoPorClave(string clave) => db.Set<Pago>().SingleOrDefaultAsync(x => x.Clave == clave);
    public Task<Pago?> PagoPorPedido(Guid id) => db.Set<Pago>().SingleOrDefaultAsync(x => x.PedidoId == id);
    public Task<Devolucion?> DevolucionPorPedido(Guid id) => db.Set<Devolucion>().SingleOrDefaultAsync(x => x.PedidoId == id);
    public void Agregar<T>(T entidad)
        where T : class => db.Add(entidad);
    public void Eliminar<T>(T entidad)
        where T : class => db.Remove(entidad);
    public Task Guardar() => db.SaveChangesAsync();
    public Task<List<MovimientoInventario>> Movimientos() => db.Set<MovimientoInventario>().AsNoTracking().ToListAsync();
    public Task<List<Auditoria>> Auditorias() => db.Set<Auditoria>().AsNoTracking().ToListAsync();
    public Task<List<EventoOutbox>> Eventos() => db.Set<EventoOutbox>().ToListAsync();
    public Task<List<Despacho>> Despachos() => db.Set<Despacho>().AsNoTracking().ToListAsync();
    public Task<List<Devolucion>> Devoluciones() => db.Set<Devolucion>().AsNoTracking().ToListAsync();
    public Task<List<Aviso>> Avisos() => db.Set<Aviso>().AsNoTracking().ToListAsync();
    public Task<bool> Recibido(Guid evento, string suscriptor) => db.Set<RecepcionEvento>().AnyAsync(x => x.EventoId == evento && x.Suscriptor == suscriptor);
    public void Marcar(Guid evento, string suscriptor) => db.Add(new RecepcionEvento(evento, suscriptor));
    public async Task<T> Ejecutar<T>(Func<Task<T>> operacion)
    {
        // SQLite BEGIN IMMEDIATE toma el bloqueo de escritura antes de consultar stock.
        // Esto protege incluso dos procesos diferentes, no solo dos tareas de la interfaz.
        db.ChangeTracker.Clear();
        await db.Database.OpenConnectionAsync();
        var conexion = (SqliteConnection)db.Database.GetDbConnection();
        await using var transaccion = conexion.BeginTransaction(IsolationLevel.Serializable, deferred: false);
        await db.Database.UseTransactionAsync(transaccion);
        try
        {
            var resultado = await operacion();
            db.ChangeTracker.DetectChanges();
            var cambios = db.ChangeTracker.Entries().Where(e => e.State is EntityState.Added or EntityState.Modified or EntityState.Deleted).Where(e => e.Entity is not Auditoria).Select(e => $"{e.Metadata.ClrType.Name}:{e.State}").Distinct().ToArray();
            if (cambios.Length > 0)
                db.Add(new Auditoria(sesion?.Nombre ?? "SISTEMA", "TRANSACCION", string.Join(", ", cambios), Guid.NewGuid().ToString()));
            await db.SaveChangesAsync();
            await transaccion.CommitAsync();
            return resultado;
        }
        catch
        {
            var trazas = db.ChangeTracker.Entries<Auditoria>().Where(e => e.State == EntityState.Added).Select(e => e.Entity).ToList();
            await transaccion.RollbackAsync();
            await db.Database.UseTransactionAsync(null);
            db.ChangeTracker.Clear();
            // Las trazas de un intento real se conservan, aunque el negocio se revierta.
            if (trazas.Count > 0)
            {
                db.AddRange(trazas);
                await db.SaveChangesAsync();
            }

            throw;
        }
        finally
        {
            await db.Database.UseTransactionAsync(null);
            await db.Database.CloseConnectionAsync();
        }
    }
}
