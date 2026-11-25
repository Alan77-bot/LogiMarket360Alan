using LogiMarket.Application.Contratos;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace LogiMarket.Infrastructure.Servicios;
// Implementacion de un colaborador externo al dominio registrada mediante inyeccion de dependencias
public sealed class RelojSistema : IReloj
{
    public DateTimeOffset Ahora => DateTimeOffset.UtcNow;
}
