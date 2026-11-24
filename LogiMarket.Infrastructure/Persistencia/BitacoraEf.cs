using LogiMarket.Application.Contratos;
using LogiMarket.Domain.Operaciones;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace LogiMarket.Infrastructure.Persistencia;
// Responsabilidad de BitacoraEf: colaborar desde LogiMarket.Infrastructure.Persistencia sin mezclar reglas, persistencia y pantallas.
public sealed class BitacoraEf(IDbContextFactory<LogiMarketDbContext> fabrica, LogiMarketDbContext actual) : IBitacora
{
    public async Task Registrar(string usuario, string operacion, string resultado, string correlacion)
    {
        if (actual.Database.CurrentTransaction is not null)
        {
            actual.Add(new Auditoria(usuario, operacion, resultado, correlacion));
            return;
        }

        // Fuera de la transaccion se guardan tambien intentos denegados.
        await using var db = await fabrica.CreateDbContextAsync();
        db.Add(new Auditoria(usuario, operacion, resultado, correlacion));
        await db.SaveChangesAsync();
    }
}
