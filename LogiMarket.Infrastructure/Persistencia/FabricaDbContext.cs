using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace LogiMarket.Infrastructure.Persistencia;
// Permite generar migraciones sin abrir WinForms La base de diseno esta separada de la base del usuario
// Permite generar migraciones con dotnet ef sin iniciar la interfaz
public sealed class FabricaDbContext : IDesignTimeDbContextFactory<LogiMarketDbContext>
{
    public LogiMarketDbContext CreateDbContext(string[] args) => new(new DbContextOptionsBuilder<LogiMarketDbContext>().UseSqlite("Data Source=logimarket-design.db").Options);
}
