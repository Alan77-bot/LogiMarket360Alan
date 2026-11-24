using LogiMarket.Application.Contratos;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace LogiMarket.Infrastructure.Servicios;
// Responsabilidad de RelojSistema: colaborar desde LogiMarket.Infrastructure.Servicios sin mezclar reglas, persistencia y pantallas.
public sealed class RelojSistema : IReloj
{
    public DateTimeOffset Ahora => DateTimeOffset.UtcNow;
}
