using LogiMarket.Application.Contratos;
using LogiMarket.Application.Eventos;
using LogiMarket.Domain.Operaciones;
using LogiMarket.Infrastructure.Persistencia;
using Microsoft.EntityFrameworkCore;

namespace LogiMarket.Infrastructure.Outbox;
// Procesa despues del commit Un fallo queda pendiente y se reintenta sin repetir pago ni suscriptores completados
public sealed class ProcesadorOutbox(LogiMarketDbContext db, PublicadorEventos publicador) : IProcesadorOutbox
{
    public async Task<int> Procesar()
    {
        var cantidad = 0;
        // Un bloqueo de escritura evita que dos procesadores publiquen a la vez
        var repo = new RepositorioEf(db);
        await repo.Ejecutar(async () =>
        {
            var eventos = await db.Set<LogiMarket.Domain.Operaciones.EventoOutbox>().Where(e => e.ProcesadoEn == null).ToListAsync();
            foreach (var evento in eventos.OrderBy(e => e.Fecha))
            {
                try
                {
                    await publicador.Publicar(evento);
                    evento.Procesado();
                    cantidad++;
                }
                catch (Exception ex)
                {
                    evento.Fallido(ex.Message);
                }

                await db.SaveChangesAsync();
            }

            return true;
        });
        return cantidad;
    }
}
